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
        public double Total
        {
            get
            {
                return PurchasePrice - ReturnPrice;
            }
        }
        #endregion

        public SingdeTotalOrder(DataRow dataRow)
        {
            Date = dataRow.Field<string>("DATE");
            PurchaseCount = dataRow.Field<int>("P_COUNT");
            ReturnCount = dataRow.Field<int>("R_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("P_TOTAL");
            ReturnPrice = (double)dataRow.Field<decimal>("R_TOTAL");
        }

        #region ----- Define Functions -----
        internal void GetProcessingOrders()
        {
            StoreOrders = ProcessingStoreOrders.GetProcessingStoreOrdersByDate(Date);
        }
        #endregion
    }
}
