using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Product.PurchaseReturn;
using System;
using System.Data;
using System.Windows;

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
                if (selectedItem != null)
                    ((IDeletableProduct)selectedItem).IsSelected = false;

                Set(() => SelectedItem, ref selectedItem, value);

                if (selectedItem != null)
                    ((IDeletableProduct)selectedItem).IsSelected = true;

                if (selectedItem == null)
                {

                }
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

        public OrderTypeEnum OrderType { get; set; }
        public string ID { get; set; }
        public string ReceiveID { get; set; }
        public Manufactory.Manufactory OrderManufactory { get; set; }
        public WareHouse.WareHouse OrderWarehouse { get; set; }
        public string OrderEmployeeName { get; set; }
        public string ReceiveEmployeeName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? DoneDateTime { get; set; }
        public string Note { get; set; }

        public string TargetPreOrderCustomer { get; set; }
        public DateTime Day { get; set; }

        public int IsOTC { get; set; }

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
            ReceiveID = String.IsNullOrEmpty(row.Field<string>("StoOrd_ReceiveID")) ? row.Field<string>("StoOrd_ID") : row.Field<string>("StoOrd_ReceiveID");
            OrderWarehouse = new WareHouse.WareHouse(row);
            OrderEmployeeName = row.Field<string>("OrderEmp_Name");
            ReceiveEmployeeName = row.Field<string>("RecEmp_Name");
            Note = row.Field<string>("StoOrd_Note");
            TotalPrice = (double)row.Field<decimal>("Total");
            CreateDateTime = row.Field<DateTime>("StoOrd_CreateTime");
            DoneDateTime = row.Field<DateTime?>("StoOrd_ReceiveTime");

            initProductCount = row.Field<int>("ProductCount");
            OrderTypeIsOTC= row.Field<string>("StoOrd_IsOTCType");
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

        #endregion ///// Abstract Function /////

        #region ///// Status Function /////

        public void MoveToNextStatus()
        {
            SaveOrder();

            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                    SaveOrderCus();
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    ToWaitingStatus();
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.NORMAL_PROCESSING:
                case OrderStatusEnum.SINGDE_PROCESSING:
                    ToDoneStatus();
                    break;

                default:
                    MessageWindow.ShowMessage("轉單錯誤!", MessageType.ERROR);
                    break;
            }
        }

        public void MoveToNextStatusNoSingde()
        {
            SaveOrder();

            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                    SaveOrderCus();
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    ToNormalProcessingStatus();
                    break;

                case OrderStatusEnum.NORMAL_PROCESSING:
                case OrderStatusEnum.SINGDE_PROCESSING:
                    ToDoneStatus();
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

        private void ToSingdeProcessingStatus()
        {
            OrderStatus = OrderStatusEnum.SINGDE_PROCESSING;
        }

        private void ToScrapStatus()
        {
            OrderStatus = OrderStatusEnum.SCRAP;
            StoreOrderDB.StoreOrderToScrap(ID);
        }

        private void ToDoneStatus()
        {
            OrderStatus = OrderStatusEnum.DONE;

            DataTable result = new DataTable();

            switch (OrderType)
            {
                case OrderTypeEnum.PURCHASE:
                    result = StoreOrderDB.PurchaseStoreOrderToDone(ID);
                    break;

                case OrderTypeEnum.RETURN:
                    result = StoreOrderDB.ReturnStoreOrderToDone(ID);
                    break;
            }

            if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                MessageWindow.ShowMessage((OrderType == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單已完成\r\n請至進退貨記錄查詢", MessageType.ERROR);
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
                DataTable dataTable = StoreOrderDB.SendStoreOrderToSingde(this);
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
            }
        }

        public void UpdateOrderDataFromSingde(DataRow dataRow)
        {
            long orderFlag = dataRow.Field<long>("FLAG");
            bool isShipment = dataRow.Field<long>("IS_SHIPMENT").Equals(1);
            string prescriptionReceiveID = dataRow.Field<string>("PRESCRIPTION_RECEIVEID");

            /*if (orderFlag == 2)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow.ShowMessage("訂單 " + ID + " 已被杏德作廢\r\n紀錄可至進退或記錄查詢!", MessageType.ERROR);
                });

                ToScrapStatus();
            }
            else*/ if (isShipment)
            {
                ReceiveID = prescriptionReceiveID;

                bool isSuccess = UpdateOrderProductsFromSingde();

                if (isSuccess)
                    ToSingdeProcessingStatus();
            }
        }

        private bool UpdateOrderProductsFromSingde()
        {
            bool isSuccess = PurchaseProducts.UpdateSingdeProductsByStoreOrderID(ID, ReceiveID);

            if (isSuccess)
                GetOrderProducts();

            return isSuccess;
        }

        #endregion ///// Singde Function /////

        public bool DeleteOrder()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認要作廢?", "作廢", true);

            if (!(bool)confirmWindow.DialogResult)
                return false;

            DataTable dataTable;

            if (OrderManufactory.ID.Equals("0") && OrderStatus == OrderStatusEnum.WAITING)
            {
                bool isSuccess = StoreOrderDB.RemoveSingdeStoreOrderByID(ID).Rows[0].Field<string>("RESULT").Equals("SUCCESS");

                if (!isSuccess)
                {
                    MessageWindow.ShowMessage("杏德已處理準備出貨 無法作廢", MessageType.ERROR);
                    return false;
                }

                dataTable = StoreOrderDB.RemoveStoreOrderToSingdeByID(ID);
            }
            else
            {
                dataTable = StoreOrderDB.RemoveStoreOrderByID(ID);
            }

            return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        public static StoreOrder AddNewStoreOrder(OrderTypeEnum orderType, Manufactory.Manufactory manufactory, int employeeID, int wareHouseID,string type)
        {
            DataTable dataTable = StoreOrderDB.AddNewStoreOrder(orderType, manufactory, employeeID, wareHouseID,type);

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