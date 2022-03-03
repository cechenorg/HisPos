using GalaSoft.MvvmLight;
using System.Data;
using DomainModel.Enum;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class ProcessingStoreOrder : ObservableObject
    {
        #region ----- Define Variables -----

        private OrderStatusEnum status;

        public string ID { get; set; }
        public string RecID { get; set; }
        public OrderTypeEnum Type { get; set; }

        public OrderStatusEnum Status
        {
            get { return status; }
            set { Set(() => Status, ref status, value); }
        }

        public double Total { get; set; }
        public string Note { get; set; }

        #endregion ----- Define Variables -----

        public ProcessingStoreOrder(DataRow row)
        {
            ID = row.Field<string>("StoOrd_ID");
            RecID = row.Field<string>("StoOrd_ReceiveID");
            Type = row.Field<string>("StoOrd_Type").Equals("P") ? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN;
            Status = row.Field<string>("StoOrd_Status").Equals("D") ? OrderStatusEnum.DONE : OrderStatusEnum.SINGDE_PROCESSING;
            Total = (double)row.Field<decimal>("TOTAL");
            Note = row.Field<string>("StoOrd_Note");
        }
    }
}