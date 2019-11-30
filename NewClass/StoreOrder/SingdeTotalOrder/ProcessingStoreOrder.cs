using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class ProcessingStoreOrder
    {
        #region ----- Define Variables -----
        public string ID { get; set; }
        public OrderTypeEnum Type { get; set; }
        public OrderStatusEnum Status { get; set; }
        public double Total { get; set; }
        public string Note { get; set; }
        #endregion

        public ProcessingStoreOrder(DataRow row)
        {
            ID = row.Field<string>("StoOrd_ReceiveID");
            Type = row.Field<string>("StoOrd_Type").Equals("P")? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN ;
            Status = row.Field<string>("StoOrd_Status").Equals("D")? OrderStatusEnum.DONE : OrderStatusEnum.SINGDE_PROCESSING;
            Total = (double)row.Field<decimal>("TOTAL");
            Note = row.Field<string>("StoOrd_Note");
        }
    }
}
