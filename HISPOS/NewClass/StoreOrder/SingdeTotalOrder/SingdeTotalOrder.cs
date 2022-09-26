using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Data;
using System.Linq;
using DomainModel.Class.StoreOrder;
using DomainModel.Enum;
using His_Pos.NewClass.Product.PurchaseReturn;
using System;
using System.Windows;
using System.Collections.Generic;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrder : ObservableObject
    {
        #region ----- Define Variables -----

        public string Date { get; set; }
        public int PurchaseCount { get; set; }
        public int ReturnCount { get; set; }
        public double PurchasePrice { get; set; }
        public double ReturnPrice { get; set; }
        public ProcessingStoreOrders StoreOrders { get; set; }

        public bool IsAllDone
        {
            get
            {
                if (StoreOrders is null) return false;
                return StoreOrders.Count(s => s.Status == OrderStatusEnum.SINGDE_PROCESSING) == 0;
            }
        }

        public double Total
        {
            get
            {
                return PurchasePrice - ReturnPrice;
            }
        }

        public double OTCTotal
        {
            get
            {
                return (double)StoreOrders.Where(s => s.IsType == "OTC").Sum(d => d.Total);
            }
        }

        public double DrugTotal
        {
            get
            {
                return (double)StoreOrders.Where(s => s.IsType == "藥品").Sum(d => d.Total);
            }
        }



        #endregion ----- Define Variables -----

        //public SingdeTotalOrder() // for Dapper
        //{

        //}

        public SingdeTotalOrder(DataRow dataRow)
        {
            Date = dataRow.Field<string>("DATE");
            PurchaseCount = dataRow.Field<int>("P_COUNT");
            ReturnCount = dataRow.Field<int>("R_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("P_TOTAL");
            ReturnPrice = (double)dataRow.Field<decimal>("R_TOTAL");
        }

        public SingdeTotalOrder(dSingdeTotalOrder data)
        {
            Date = data.Date;
            PurchaseCount = data.PurchaseCount;
            ReturnCount = data.ReturnCount;
            PurchasePrice = data.PurchasePrice;
            ReturnPrice = data.ReturnPrice;
        }
        
        #region ----- Define Functions -----

        internal void GetProcessingOrders()
        {
            StoreOrders = ProcessingStoreOrders.GetProcessingStoreOrdersByDate(Date);
        }

        internal void OrderToDone(string id)
        {
            foreach (var order in StoreOrders)
            {
                if (order.ID == id)
                {
                    bool isReturnOrderToDone = CheckOrderToDone(id, order.Total);
                    if(!isReturnOrderToDone)
                    {
                        return;
                    }

                    bool IsSusses = true;
                    if (order.Type != OrderTypeEnum.RETURN)
                    {
                        IsSusses = InsertLowerOrder(order);
                    }
                    if (!IsSusses)
                        break;

                    DataTable result = new DataTable();

                    switch (order.Type)
                    {
                        case OrderTypeEnum.PURCHASE:
                            result = StoreOrderDB.PurchaseStoreOrderToDone(id, false);
                            break;

                        case OrderTypeEnum.RETURN:
                            result = StoreOrderDB.ReturnStoreOrderToDone(id);
                            break;
                    }
                    if(result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                        order.Status = OrderStatusEnum.DONE;
                    if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                        MessageWindow.ShowMessage((order.Type == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單未完成\r\n請重新整理後重試", MessageType.ERROR);
                    break;
                }
            }

            RaisePropertyChanged(nameof(IsAllDone));
        }

        internal void AllOrderToDone()
        {
            List<string> returnOrderNoDone = new List<string>();//退貨單為零，使用者選擇是否完成退貨
            List<string> listOrder = new List<string>();//紀錄不足量訂單
            foreach (var order in StoreOrders)//先檢查有無不足量訂單
            {
                OrderStatusEnum orderStatus = order.Status;
                if(orderStatus != OrderStatusEnum.DONE)
                {
                    DataTable OrderTable = PurchaseReturnProductDB.GetProductsByStoreOrderID(order.ID);
                    PurchaseProducts orderProducts = new PurchaseProducts(OrderTable, orderStatus);
                    DataRow[] drs = OrderTable.Select("StoOrdDet_OrderAmount > StoOrdDet_RealAmount");//預訂量 > 實際量
                    if (drs != null && drs.Length > 0 && order.Type != OrderTypeEnum.RETURN)
                    {
                        listOrder.Add(order.ID);
                    }
                    if(order.Type == OrderTypeEnum.RETURN)
                    {
                        bool isReturnOrderToDone = CheckOrderToDone(order.ID, order.Total);
                        if (!isReturnOrderToDone)
                        {
                            returnOrderNoDone.Add(order.ID);
                        }
                    }
                }
            }
            if (listOrder.Count > 0)//如果有不足量訂單
            {
                List<string> succesOrder = new List<string>();
                foreach (var order in StoreOrders)
                {
                    if (listOrder.Contains(order.ID) && order.Type != OrderTypeEnum.RETURN)
                    {
                        DataTable OrderTable = PurchaseReturnProductDB.GetProductsByStoreOrderID(order.ID);
                        OrderStatusEnum orderStatus = order.Status;
                        PurchaseProducts orderProducts = new PurchaseProducts(OrderTable, orderStatus);
                        string ReceiveID = order.RecID;//杏德出貨單
                        string ManID = Convert.ToString(OrderTable.Rows[0]["StoOrd_ManufactoryID"]); ;//供應商
                        string wareID = Convert.ToString(OrderTable.Rows[0]["StoOrd_WarehouseID"]);//出貨倉庫
                        DataTable dataTable = StoreOrderDB.AddStoreOrderLowerThenOrderAmount(ReceiveID, ManID, wareID, orderProducts);//新增不足量訂單
                        if (dataTable != null && dataTable.Rows.Count > 0 && order.Type != OrderTypeEnum.RETURN)
                        {
                            succesOrder.Add(dataTable.Rows[0].Field<string>("NEW_ID"));
                        }
                    }
                }
                if (succesOrder.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageWindow.ShowMessage(string.Format("已新增收貨單 {0}{1}", "\r\n", String.Join("\r\n", succesOrder.ToArray())), MessageType.SUCCESS);
                    });
                }
            }

            foreach (var order in StoreOrders)
            {
                if (order.Status == OrderStatusEnum.SINGDE_PROCESSING)
                {
                    DataTable result = new DataTable();
                    switch (order.Type)
                    {
                        case OrderTypeEnum.PURCHASE:
                            result = StoreOrderDB.PurchaseStoreOrderToDone(order.ID, false);
                            break;

                        case OrderTypeEnum.RETURN:
                            if(!returnOrderNoDone.Contains(order.ID))
                            {
                                result = StoreOrderDB.ReturnStoreOrderToDone(order.ID);
                            }
                            break;
                    }

                    if (result != null && result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("FAIL") && !returnOrderNoDone.Contains(order.ID))
                    {
                        Application.Current.Dispatcher.Invoke(() => {
                            MessageWindow.ShowMessage((order.Type == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單未完成\r\n請重新整理後重試", MessageType.ERROR);
                        });

                        break;
                    }
                    else if (returnOrderNoDone.Contains(order.ID))
                    {
                        break;
                    }
                    else
                    {
                        order.Status = OrderStatusEnum.DONE;
                    }
                }
            }

            RaisePropertyChanged(nameof(IsAllDone));
        }

        private bool CheckOrderToDone(string orderID, double amt)
        {
            bool confirm = amt > 0 ? true : false;
            Application.Current.Dispatcher.Invoke(() => {
                string msg = string.Format("{0}退貨金額為零\n是否確認完成退貨單?", orderID);
                if(amt == 0)
                {
                    ConfirmWindow confirmWindow = new ConfirmWindow(msg, "", false);
                    confirm = (bool)confirmWindow.DialogResult;
                }
            });
            return confirm;
        }
        private bool InsertLowerOrder(ProcessingStoreOrder order)
        {
            DataTable OrderTable = PurchaseReturnProductDB.GetProductsByStoreOrderID(order.ID);
            OrderStatusEnum orderStatus = order.Status;
            PurchaseProducts orderProducts = new PurchaseProducts(OrderTable, orderStatus);
            DataRow[] drs = OrderTable.Select("StoOrdDet_OrderAmount > StoOrdDet_RealAmount");//預訂量 > 實際量 
            if (drs != null && drs.Length > 0)
            {
                string ReceiveID = order.RecID;//杏德出貨單
                string ManID = Convert.ToString(drs[0]["StoOrd_ManufactoryID"]); ;//供應商
                string wareID = Convert.ToString(drs[0]["StoOrd_WarehouseID"]);//出貨倉庫
                DataTable dataTable = StoreOrderDB.AddStoreOrderLowerThenOrderAmount(ReceiveID, ManID, wareID, orderProducts);//新增不足量訂單
                if (dataTable.Rows.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageWindow.ShowMessage($"已新增收貨單 {dataTable.Rows[0].Field<string>("NEW_ID")} !", MessageType.SUCCESS);
                    });
                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageWindow.ShowMessage($"新增失敗 請稍後再試!", MessageType.ERROR);
                    });
                    return false;
                }
            }
            return true;
        }

        #endregion ----- Define Functions -----
    }
}