using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrder: ObservableObject
    {
        public StoreOrder(DataRow row)
        {

        }

        #region ----- Define Variables -----
        public OrderTypeEnum OrderType { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public string ID { get; set; }
        //public Manufactory OrderManufactory { get; set; }
        //public WareHouse OrderWarehouse { get; set; }
        public string OrderEmployeeName { get; set; }
        public string Note { get; set; }
        public Products OrderProducts { get; set; }
        public int TotalPrice { get; set; }
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

        public bool CheckOrder()
        {
            return true;
        }

        public void DeleteOrder()
        {

        }

        public void SaveOrder()
        {

        }
        #endregion
    }
}
