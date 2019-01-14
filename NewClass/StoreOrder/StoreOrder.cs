using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrder: ObservableObject
    {
        public StoreOrder(DataRow row)
        {
            OrderManufactory = new Manufactory.Manufactory(row);

            switch (row["StoOrd_Type"].ToString())
            {
                case "P":
                    OrderType = OrderTypeEnum.PURCHASE;
                    break;
                case "R":
                    OrderType = OrderTypeEnum.RETURN;
                    break;
            }

            switch (row["StoOrd_Status"].ToString())
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

            ID = row["StoOrd_ID"].ToString();
            OrderWarehouse = new WareHouse.WareHouse(row);
            OrderEmployeeName = row["Emp_Name"].ToString();
            Note = row["StoOrd_Note"].ToString();
            PatientName = row["Cus_Name"].ToString();
            TotalPrice = double.Parse(row["Total"].ToString());

            initProductCount = int.Parse(row["ProductCount"].ToString());
        }
        
        #region ----- Define Variables -----
        private int initProductCount = 0;

        public OrderTypeEnum OrderType { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public string ID { get; set; }
        public Manufactory.Manufactory OrderManufactory { get; set; }
        public WareHouse.WareHouse OrderWarehouse { get; set; }
        public string OrderEmployeeName { get; set; }
        public string Note { get; set; }
        public string PatientName { get; set; }
        public Products OrderProducts { get; set; }
        public double TotalPrice { get; set; }
        public int ProductCount
        {
            get
            {
                if (OrderProducts is null) return initProductCount;
                else return OrderProducts.Count;
            }
        }
        #endregion

        #region ----- Define Functions -----

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

        public void DeleteOrder()
        {

        }

        public void SaveOrder()
        {

        }
        #endregion
    }
}
