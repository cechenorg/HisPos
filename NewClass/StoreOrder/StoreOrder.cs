using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.NewClass.StoreOrder
{
    public abstract class StoreOrder: ObservableObject
    {
        public StoreOrder() { }
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
            OrderWarehouse = new WareHouse.WareHouse(row);
            OrderEmployeeName = row.Field<string>("Emp_Name");
            Note = row.Field<string>("StoOrd_Note");
            TotalPrice = (double)row.Field<decimal>("Total");

            initProductCount = row.Field<int>("ProductCount");
        }

        #region ----- Define Variables -----
        private Product.Product selectedItem;

        protected int initProductCount = 0;

        public Product.Product SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if(selectedItem != null)
                    ((IDeletableProduct) selectedItem).IsSelected = false;

                Set(() => SelectedItem, ref selectedItem, value);

                if (selectedItem != null)
                    ((IDeletableProduct)selectedItem).IsSelected = true;
            }
        }
        public OrderTypeEnum OrderType { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public string ID { get; set; }
        public Manufactory.Manufactory OrderManufactory { get; set; }
        public WareHouse.WareHouse OrderWarehouse { get; set; }
        public string OrderEmployeeName { get; set; }
        public string Note { get; set; }
        public double TotalPrice { get; set; }
        #endregion

        #region ----- Define Functions -----

        public abstract void GetOrderProducts();
        public abstract void SaveOrder();
        public abstract void AddProductByID(string iD);
        public abstract void DeleteSelectedProduct();

        #region ----- Status Function -----
        public void MoveToNextStatus()
        {
            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                    ToNormalProcessingStatus();
                    break;
                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    ToWaitingStatus();
                    break;
                case OrderStatusEnum.WAITING:
                    ToSingdeProcessingStatus();
                    break;
                case OrderStatusEnum.NORMAL_PROCESSING:
                case OrderStatusEnum.SINGDE_PROCESSING:
                    ToDoneStatus();
                    break;
                default:
                    MessageWindow.ShowMessage("轉單錯誤!", MessageType.ERROR);
                    break;
            }

            SaveOrder();
        }
        private void ToWaitingStatus()
        {
            bool isSuccess = SendOrderToSingde();

            if (isSuccess)
            {
                MainWindow.ServerConnection.OpenConnection();

                SaveOrder();
                OrderStatus = OrderStatusEnum.WAITING;
                StoreOrderDB.StoreOrderToWaiting(ID);

                MainWindow.ServerConnection.CloseConnection();
            }
            else
                MessageWindow.ShowMessage("傳送杏德失敗 請稍後在嘗試", MessageType.ERROR);
        }
        private void ToNormalProcessingStatus()
        {
            OrderStatus = OrderStatusEnum.NORMAL_PROCESSING;
        }
        private void ToSingdeProcessingStatus()
        {
            OrderStatus = OrderStatusEnum.SINGDE_PROCESSING;
        }
        private void ToDoneStatus()
        {
            OrderStatus = OrderStatusEnum.DONE;
        }
        #endregion

        #region ----- Check Function -----
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
                    return CheckSingdeProcessingOrder();
                default:
                    return false;
            }
        }
        protected abstract bool CheckUnProcessingOrder();
        protected abstract bool CheckNormalProcessingOrder();
        protected abstract bool CheckSingdeProcessingOrder();
        #endregion

        private bool SendOrderToSingde()
        {
            MainWindow.SingdeConnection.OpenConnection();
            bool isSuccess = StoreOrderDB.SendStoreOrderToSingde(this);
            MainWindow.SingdeConnection.CloseConnection();

            return isSuccess;
        }

        public bool DeleteOrder()
        {
            DataTable dataTable = StoreOrderDB.RemoveStoreOrderByID(ID);
            return dataTable.Rows[0].Field<bool>("");
        }
        
        public static StoreOrder AddNewStoreOrder(OrderTypeEnum orderType, Manufactory.Manufactory manufactory, int employeeID)
        {
            DataTable dataTable = StoreOrderDB.AddNewStoreOrder(orderType, manufactory, employeeID);

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

        #endregion
    }
}
