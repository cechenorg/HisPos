using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
    public class IndexReserveDetail : Product.Product
    {
        public IndexReserveDetail(DataRow r) : base(r)
        {
            Stock = r.Field<string>("Inventory");
            InvID = r.Field<int>("Inv_ID").ToString();
            OntheWay = r.Field<string>("OntheWay");
            Amount = r.Field<double>("TotalAmount");
            IsFrozen = r.Field<bool>("Med_IsFrozen");
            IsControl = r.Field<byte?>("Med_Control");
            SendAmount = r.Field<double>("SendAmount");
        }

        public string StoOrdID { get; set; }
        public string Stock { get; set; }
        public string OntheWay { get; set; }
        public bool IsFrozen { get; set; }
        public byte? IsControl { get; set; }
        public double Amount { get; set; }
        public string InvID { get; set; }
        private double frameAmount;

        public double FrameAmount
        {
            get => frameAmount;
            set
            {
                Set(() => FrameAmount, ref frameAmount, value);
            }
        }

        private double sendAmount;

        public double SendAmount
        {
            get => sendAmount;
            set
            {
                Set(() => SendAmount, ref sendAmount, value);
                if (SendAmount > Convert.ToInt32(Amount))
                    SendAmount = Convert.ToInt32(Amount);
            }
        }
    }
}