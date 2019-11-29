using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrder
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
        #endregion

        public SingdeTotalOrder(DataRow dataRow)
        {
            Date = dataRow.Field<string>("");
            PurchaseCount = dataRow.Field<int>("");
            ReturnCount = dataRow.Field<int>("");
            PurchasePrice = dataRow.Field<double>("");
            ReturnPrice = dataRow.Field<double>("");
        }

        #region ----- Define Functions -----
        internal void GetProcessingOrders()
        {
            StoreOrders = ProcessingStoreOrders.GetProcessingStoreOrdersByDate(Date);
        }
        #endregion
    }
}
