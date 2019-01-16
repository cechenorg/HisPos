using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.NewClass.StoreOrder
{
    public abstract class StoreOrder: ObservableObject
    {
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
        protected int initProductCount = 0;

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

        #region ----- Status Function -----
        public void MoveToNextStatus()
        {

        }
        private void ToWaitingStatus()
        {

        }
        private void ToNormalProcessingStatus()
        {

        }
        private void ToSingdeProcessingStatus()
        {

        }
        private void ToDoneStatus()
        {

        }
        #endregion

        #region ----- Check Function -----
        public bool CheckOrder()
        {
            return false;
        }
        public virtual bool CheckUnProcessingOrder()
        {
            return false;
        }
        public virtual bool CheckNormalProcessingOrder()
        {
            return false;
        }
        public virtual bool CheckSingdeProcessingOrder()
        {
            return false;
        }
        #endregion

        public bool DeleteOrder()
        {
            DataTable dataTable = StoreOrderDB.RemoveStoreOrderByID(ID);
            return dataTable.Rows[0].Field<bool>("Usa_PrintName");
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
