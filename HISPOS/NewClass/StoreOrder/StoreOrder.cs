﻿using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.PurchaseReturn;
using System;
using System.Data;
using DomainModel.Enum;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System.Globalization;
using His_Pos.ChromeTabViewModel;
using System.Collections.Generic;

namespace His_Pos.NewClass.StoreOrder
{
    public abstract class StoreOrder : ObservableObject, ICloneable
    {
        #region ----- Define Variables -----

        private Product.Product selectedItem;
        private OrderStatusEnum orderStatus;
        private double totalPrice;
        private StoreOrderHistorys storeOrderHistory;

        protected int initProductCount;

        public Product.Product SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            { 
                Set(() => SelectedItem, ref selectedItem, value); 
            }
        }

        public StoreOrderHistorys StoreOrderHistory
        {
            get { return storeOrderHistory; }
            set { Set(() => StoreOrderHistory, ref storeOrderHistory, value); }
        }

        public OrderStatusEnum OrderStatus
        {
            get { return orderStatus; }
            set { Set(() => OrderStatus, ref orderStatus, value); }
        }

        private string orderTypeIsOTC;

        public string OrderTypeIsOTC
        {
            get { return orderTypeIsOTC; }
            set { Set(() => OrderTypeIsOTC, ref orderTypeIsOTC, value); }
        }

        private string orderIsPayCash;

        public string OrderIsPayCash
        {
            get { return orderIsPayCash; }
            set { Set(() => OrderIsPayCash, ref orderIsPayCash, value); }
        }

        private bool isPayCash = false;

        public bool IsPayCash
        {
            get { return isPayCash; }
            set { Set(() => IsPayCash, ref isPayCash, value); }
        }

        public OrderTypeEnum OrderType { get; set; }
        public string ID { get; set; }
        public string ReceiveID { get; set; }
        public string SourceID { get; set; }//來源單
        public string CheckCode { get; set; }
        public Manufactory.Manufactory OrderManufactory { get; set; }
        public WareHouse.WareHouse OrderWarehouse { get; set; }
        public string OrderEmployeeName { get; set; }
        public string ReceiveEmployeeName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? DoneDateTime { get; set; }
        public string Note { get; set; }
        public string ModifyUser { get; set; }
        public DateTime ModifyTime { get; set; }
        public string VoidReason { get; set; }
        public string TargetPreOrderCustomer { get; set; }
        public DateTime Day { get; set; }
        public int IsOTC { get; set; }
        public bool IsEnable { get; set; }
        public string IsEnableVoid { get; set; }
        public bool IsScrap { get; set; }
        public bool IsCanDelete { get; set; }
        public bool IsCancel { get; set; }
        public string Visibility { get; set; }
        public int IsWaitOrder = 0;
        public double TotalPrice
        {
            get { return totalPrice; }
            set { Set(() => TotalPrice, ref totalPrice, value); }
        }

        #endregion ----- Define Variables -----

        protected StoreOrder()
        {
        }

        public StoreOrder(DataRow row)
        {
            OrderManufactory = new Manufactory.Manufactory(row);

            switch (row.Field<string>("StoOrd_Status"))
            {
                case "U":
                    OrderStatus = OrderManufactory.ID.Equals("0")
                        ? OrderStatusEnum.SINGDE_UNPROCESSING
                        : OrderStatusEnum.NORMAL_UNPROCESSING;
                    break;

                case "W":
                    OrderStatus = OrderStatusEnum.WAITING;
                    break;

                case "P":
                    OrderStatus = OrderManufactory.ID.Equals("0")
                        ? OrderStatusEnum.SINGDE_PROCESSING
                        : OrderStatusEnum.NORMAL_PROCESSING;
                    break;

                case "S":
                    OrderStatus = OrderStatusEnum.SCRAP;
                    break;

                case "D":
                    OrderStatus = OrderStatusEnum.DONE;
                    break;

                default:
                    OrderStatus = OrderStatusEnum.ERROR;
                    break;
            }

            ID = row.Field<string>("StoOrd_ID");
            ReceiveID = string.IsNullOrEmpty(row.Field<string>("StoOrd_ReceiveID")) ? row.Field<string>("StoOrd_ID") : row.Field<string>("StoOrd_ReceiveID");
            SourceID = row.Field<string>("StoOrd_SourceID");
            CheckCode = row.Field<string>("StoOrd_CheckCode");
            var auth = ViewModelMainWindow.CurrentUser.Authority;
            //if ((auth == Authority.Admin) || (string.IsNullOrEmpty(CheckCode)))
                IsCanDelete = true;
            //else
            //    IsCanDelete = false;
            List<Authority> IsScrapAuthority = new List<Authority>() {Authority.Admin, Authority.AccountingStaff};
            if (OrderStatus != OrderStatusEnum.SCRAP && IsScrapAuthority.Contains(auth))
                IsScrap = true;
            else
                IsScrap = false;
            OrderWarehouse = new WareHouse.WareHouse(row);
            OrderEmployeeName = row.Field<string>("OrderEmp_Name");
            ReceiveEmployeeName = row.Field<string>("RecEmp_Name");
            Note = row.Field<string>("StoOrd_Note");
            TotalPrice = (double)Math.Round(row.Field<decimal>("Total"),0);
            CreateDateTime = row.Field<DateTime>("StoOrd_CreateTime");
            DoneDateTime = row.Field<DateTime?>("StoOrd_ReceiveTime");
            initProductCount = row.Field<int>("ProductCount");
            OrderTypeIsOTC = row.Field<string>("StoOrd_IsOTCType");
            OrderIsPayCash = row.Field<bool>("StoOrd_IsPayCash") ? "下貨付現" : "一般收貨";
            if(row.Table.Columns.Contains("StoOrd_ModifyUser"))
            {
                if(!DBNull.Value.Equals(row["StoOrd_ModifyUser"]))
                    ModifyUser = row.Field<string>("StoOrd_ModifyUser");
            }
            if (row.Table.Columns.Contains("StoOrd_ModifyTime"))
            {
                if (!DBNull.Value.Equals(row["StoOrd_ModifyTime"]))
                    ModifyTime = Convert.ToDateTime(row.Field<DateTime>("StoOrd_ModifyTime"));
            }
            if (row.Table.Columns.Contains("StoOrd_VoidReason"))
            {
                if(!DBNull.Value.Equals(row["StoOrd_VoidReason"]))
                    VoidReason = row.Field<string>("StoOrd_VoidReason");
            }
            
            if (row.Table.Columns.Contains("StoOrd_IsEnable"))
            {
                IsEnable = row.Field<bool>("StoOrd_IsEnable");
                if (!IsEnable)
                {
                    OrderStatus = OrderStatusEnum.SCRAP;
                    IsCanDelete = false;
                    Visibility = "Hidden"; 
                }
                else
                {
                    IsEnableVoid = "Hidden";
                }
            }
        }

        #region ----- Define Functions -----

        #region ///// Abstract Function /////

        public abstract void GetOrderProducts();

        public abstract void SaveOrder();

        public abstract void SaveOrderCus();

        public abstract void AddProductByID(string iD, bool isFromAddButton);

        public abstract void DeleteSelectedProduct();

        public abstract void CalculateTotalPrice();

        public abstract void SetProductToProcessingStatus();

        public abstract object Clone();

        public abstract int GetOrderProductsIsOTC();

        public abstract void SetRealAmount(string id);

        public abstract bool ChkPrice();

        public abstract bool ChkPurchase();

        #endregion ///// Abstract Function /////

        #region ///// Status Function /////

        /// <summary>
        /// 傳送採購 & 確認收貨
        /// </summary>
        public void MoveToNextStatus()
        {
            SaveOrder();
            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                    ToNormalProcessingStatus();
                    break;
                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    ToWaitingStatus();//傳送訂單至杏德
                    ToNormalProcessingStatus();
                    // 直接結案
                    if (OrderType == OrderTypeEnum.RETURN)
                    {
                        ToDoneStatus(0);
                    }
                    break;
                case OrderStatusEnum.NORMAL_PROCESSING:
                case OrderStatusEnum.SINGDE_PROCESSING:
                    ToDoneStatus(1);
                    break;
                default:
                    MessageWindow.ShowMessage("轉單錯誤!", MessageType.ERROR);
                    break;
            }
        }
        /// <summary>
        /// 手動入庫,不傳送杏德
        /// </summary>
        public void MoveToNextStatusNoSingde()
        {
            SaveOrder();

            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.NORMAL_PROCESSING:
                case OrderStatusEnum.SINGDE_PROCESSING:
                    ToDoneStatus(0);
                    break;

                default:
                    MessageWindow.ShowMessage("轉單錯誤!", MessageType.ERROR);
                    break;
            }
        }

        private void ToWaitingStatus()
        {
            bool isSuccess = SendOrderToSingde();

            if (isSuccess)
            {
                OrderStatus = OrderStatusEnum.WAITING;

                if (OrderType == OrderTypeEnum.RETURN)
                {
                    DataTable dataTable = StoreOrderDB.ReturnOrderToProccessing(this as ReturnOrder);

                    if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                    {
                        MessageWindow.ShowMessage("退貨失敗 請稍後再試", MessageType.ERROR);
                        return;
                    }
                }

                StoreOrderDB.StoreOrderToWaiting(ID);
            }
            else
                MessageWindow.ShowMessage("傳送杏德失敗 請稍後再試", MessageType.ERROR);
        }

        private void ToNormalProcessingStatus()
        {
            if (OrderType == OrderTypeEnum.RETURN && !OrderManufactory.ID.Equals("0"))
            {
                if(OrderType == OrderTypeEnum.PURCHASE)
                { }
                DataTable dataTable = StoreOrderDB.ReturnOrderToProccessing(this as ReturnOrder);
                if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                {
                    MessageWindow.ShowMessage("退貨失敗 請稍後再試", MessageType.ERROR);
                    return;
                }
            }

            if (OrderManufactory.ID.Equals("0"))
            {
                OrderStatus = OrderStatusEnum.SINGDE_PROCESSING;
            }
            else { OrderStatus = OrderStatusEnum.NORMAL_PROCESSING; }

            if (OrderType == OrderTypeEnum.RETURN && OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
            {
                OrderStatus = OrderStatusEnum.NORMAL_PROCESSING;
            }

            ReceiveID = ID;
            SetProductToProcessingStatus();

            StoreOrderDB.StoreOrderToNormalProcessing(ID);
        }

        //private void ToSingdeProcessingStatus()
        //{
        //    OrderStatus = OrderStatusEnum.SINGDE_PROCESSING;
        //}

        private void ToScrapStatus()
        {
            OrderStatus = OrderStatusEnum.SCRAP;
            StoreOrderDB.StoreOrderToScrap(ID);
        }
        public bool IsDoneOrder { get; set; }
        public string LowOrderID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0:手動入庫 1:一般入庫</param>
        /// <returns></returns>
        private bool ToDoneStatus(int type)
        {
            DataTable result = new DataTable();

            switch (OrderType)
            {
                case OrderTypeEnum.WAITPREPARE:
                case OrderTypeEnum.PREPARE:
                case OrderTypeEnum.PURCHASE:
                    string pay = IsPayCash ? "下貨付現" : "";
                    ConfirmWindow confirmWindow = new ConfirmWindow("訂單金額: " + TotalPrice + "\n" + pay + "\n是否確認收貨?", "確認收貨");
                    if (!(bool)confirmWindow.DialogResult)
                    {
                        IsDoneOrder = false;
                        IsCancel = true;
                        return false;
                    }
                    else
                    {
                        result = StoreOrderDB.PurchaseStoreOrderToDone(ID, IsPayCash);
                        if(type == 1)
                            CheckStoreOrderLower();
                    }
                    if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                    {
                        IsDoneOrder = false;
                        MessageWindow.ShowMessage(((OrderType == OrderTypeEnum.PURCHASE || OrderType == OrderTypeEnum.PREPARE) ? "進" : "退") + "貨錯誤，判斷為異常操作", MessageType.ERROR);
                        return false;
                    }
                    else
                    {
                        string id = LowOrderID;
                        IsDoneOrder = true;
                        OrderStatus = OrderStatusEnum.DONE;
                        string msg = string.Empty;
                        if (!string.IsNullOrEmpty(id))
                            msg = string.Format("收貨成功! \n 不足訂購單之品項已轉新進貨單({0})", id);
                        else
                            msg = "收貨成功!";
                        MessageWindow.ShowMessage(msg, MessageType.SUCCESS);
                        return true;
                    }
                    break;

                case OrderTypeEnum.RETURN:
                    result = StoreOrderDB.ReturnStoreOrderToDone(ID);
                    if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                    {
                        IsDoneOrder = false;
                        MessageWindow.ShowMessage((OrderType == OrderTypeEnum.RETURN ? "退" : "進") + "貨錯誤，判斷為異常操作", MessageType.ERROR);
                        return false;
                    }
                    else
                    {
                        IsDoneOrder = true;
                        return true;
                    }
                    break;
            }
            return false;
        }

        #endregion ///// Status Function /////

        #region ///// Check Function /////

        public bool CheckOrder()
        {
            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    return CheckUnProcessingOrder();

                case OrderStatusEnum.NORMAL_PROCESSING:
                    return CheckNormalProcessingOrder();

                case OrderStatusEnum.SINGDE_PROCESSING:
                    return CheckNormalProcessingOrder();

                default:
                    return false;
            }
        }

        protected abstract bool CheckUnProcessingOrder();

        protected abstract bool CheckNormalProcessingOrder();
        protected abstract bool CheckStoreOrderLower();

        protected abstract bool CheckSingdeProcessingOrder();

        #endregion ///// Check Function /////

        #region ///// Singde Function /////

        private bool SendOrderToSingde()
        {
            if (GetOrderProductsIsOTC() == 2)
            {
                DataTable dataTable = StoreOrderDB.SendOTCStoreOrderToSingde(this);
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
            }
            else
            {
                DataTable dataTable;
                ReturnOrder order = new ReturnOrder();
                if (this is ReturnOrder)
                {
                    order = new ReturnOrder();
                    order = (ReturnOrder)this;
                    ReturnProducts products = new ReturnProducts();
                    foreach (ReturnProduct product in ((ReturnOrder)this).ReturnProducts)
                    {
                        if(product.IsChecked)
                        {
                            products.Add(product);
                        }
                    }
                    order.ReturnProducts = products;
                    dataTable = StoreOrderDB.SendStoreOrderToSingde(order);
                    return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                }

                dataTable = StoreOrderDB.SendStoreOrderToSingde(this);
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
            }
        }

        public bool UpdateOrderDataFromSingde(DataRow dataRow)
        {
            string orderID = dataRow.Field<string>("ORDER_ID");
            long orderFlag = dataRow.Field<long>("FLAG");
            bool isShipment = dataRow.Field<long>("IS_SHIPMENT").Equals(1);
            string prescriptionReceiveID = dataRow.Field<string>("PRESCRIPTION_RECEIVEID");
            string checkCode = dataRow.Field<string>("CHECK_CODE");

            /*if (orderFlag == 2)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow.ShowMessage("訂單 " + ID + " 已被杏德作廢\r\n紀錄可至進退或記錄查詢!", MessageType.ERROR);
                });

                ToScrapStatus();
            }
            else*/
            bool isSuccess = false;
            if (orderFlag != 2)
            {
                SourceID = orderID;
                ReceiveID = prescriptionReceiveID;
                CheckCode = checkCode;

                isSuccess = UpdateOrderProductsFromSingde();

                if (isSuccess)
                    OrderStatus = OrderStatusEnum.SINGDE_PROCESSING;
            }
            return isSuccess;
        }

        private bool UpdateOrderProductsFromSingde()
        {
            bool isSuccess = PurchaseProducts.UpdateSingdeProductsByStoreOrderID(ID, ReceiveID, CheckCode, SourceID);

            if (isSuccess)
                GetOrderProducts();

            return isSuccess;
        }

        #endregion ///// Singde Function /////

        public bool DeleteOrder()
        {
            DataTable dataTable;
            bool isCanModify = false;
            string VoidReason = string.Empty;
            if (OrderStatus == OrderStatusEnum.WAITING || OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.SINGDE_PROCESSING)
            {
                if (OrderManufactory.ID.Equals("0") && Note != "手動入庫")
                {
                    //杏德訂單需先檢查是否杏德資料是否可以刪除                
                    if (CheckCode != string.Empty)
                    {
                        isCanModify = false;
                    }
                    else
                    {
                        string dateTime = DateTime.Now.ToString("yyyyMMdd");
                        dateTime = CreateDateTime.ToString("yyyy/MM/dd");
                        if (SourceID != null && SourceID != "")
                        {
                            dateTime = CreateDateTime.AddMonths(-1).ToString("yyyy/MM/dd");
                            dataTable = StoreOrderDB.GetSingdeOrderCanModify(dateTime, SourceID);
                        }
                        else 
                        {
                            dataTable = StoreOrderDB.GetSingdeOrderCanModify(dateTime, ID);
                        }                            
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            isCanModify = Convert.ToBoolean(dataTable.Rows[0]["Result"]);
                        }
                    }
                    if (isCanModify == false)
                    {
                        //var auth = ViewModelMainWindow.CurrentUser.Authority;
                        //if (auth == Authority.Admin)
                        //{
                            ConfirmWindow confirmWindow = new ConfirmWindow("訂單已備貨，如需刪除需再通知杏德，是否確認刪除?", "確認");
                            if (!(bool)confirmWindow.DialogResult)
                                return false;
                        //}
                        //else
                        //{
                        //    MessageWindow.ShowMessage("訂單已備貨，不可刪除！", MessageType.ERROR);
                        //    return false;
                        //}
                    }
                }
                //作廢原因                
                ScrapOrderWindow ScrapOrderWindow = new ScrapOrderWindow();
                ScrapOrderWindowViewModel ScrapOrder = (ScrapOrderWindowViewModel)ScrapOrderWindow.DataContext;
            
                if (!(bool)ScrapOrderWindow.DialogResult)
                    return false;
            
                VoidReason = ScrapOrder.Content + ScrapOrder.Other;
                DateTime dt;
                string update = DateTime.Now.ToString("yyyy/MM/dd");
                string uptime = DateTime.Now.ToString("HHmmss");
                dt = DateTime.Parse(update);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                update = dt.ToString("yyyMMdd", culture);
            
                if (isCanModify == true)
                {
                    dataTable = StoreOrderDB.UpdateOrderToScrap(ID, update, uptime, VoidReason);//更新杏德訂單資料
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        bool isSucces = Convert.ToBoolean(dataTable.Rows[0]["Result"]);//FALSE未更新 TRUE已更新
                        if (!isSucces)
                        {
                            MessageWindow.ShowMessage("杏德訂單更新失敗，取消作廢", MessageType.ERROR);
                            return false;
                        }
                    }
                }
            }
            dataTable = StoreOrderDB.RemoveStoreOrderByID(ID, VoidReason);          
            return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        public bool ReductOrder()
        {
            bool isSuccess = false;
            if (!IsEnable)
            {
                DataTable table = StoreOrderDB.UpdateStoreOrderToOriginal(ID);
                if(table != null && table.Rows.Count > 0)
                {
                    isSuccess = Convert.ToBoolean(table.Rows[0]["RESULT"]);
                }
            }
            return isSuccess;
        }
        public static StoreOrder AddNewStoreOrder(OrderTypeEnum orderType, Manufactory.Manufactory manufactory, int employeeID, int wareHouseID, string type)
        {
            DataTable dataTable = StoreOrderDB.AddNewStoreOrder(orderType, manufactory, employeeID, wareHouseID, type);

            switch (orderType)
            {
                case OrderTypeEnum.PURCHASE:
                    return new PurchaseOrder(dataTable.Rows[0]);

                case OrderTypeEnum.RETURN:
                    return new ReturnOrder(dataTable.Rows[0]);

                default:
                    return null;
            }
        }

        protected void CloneBaseData(StoreOrder storeOrder)
        {
            ID = storeOrder.ID;
            ReceiveID = storeOrder.ReceiveID;
            OrderStatus = storeOrder.OrderStatus;
            OrderType = storeOrder.OrderType;
            OrderManufactory = storeOrder.OrderManufactory.Clone() as Manufactory.Manufactory;
            OrderWarehouse = storeOrder.OrderWarehouse.Clone() as WareHouse.WareHouse;
            OrderEmployeeName = storeOrder.OrderEmployeeName;
            ReceiveEmployeeName = storeOrder.ReceiveEmployeeName;
            CreateDateTime = storeOrder.CreateDateTime;
            DoneDateTime = storeOrder.DoneDateTime;
            Day = storeOrder.Day;
            TargetPreOrderCustomer = storeOrder.TargetPreOrderCustomer;
            Note = storeOrder.Note;
            TotalPrice = storeOrder.TotalPrice;

            initProductCount = storeOrder.initProductCount;
        }

        #endregion ----- Define Functions -----
    }
}