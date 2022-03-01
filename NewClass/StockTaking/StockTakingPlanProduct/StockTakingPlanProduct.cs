using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingPlanProduct
{
    public class StockTakingPlanProduct : Product.Product
    {
        public StockTakingPlanProduct(DataRow r) : base(r)
        {
            IsFrozen = r.Field<bool>("Med_IsFrozen");
            IsControl = r.Field<byte?>("Med_Control");
            Inv_ID = r.Field<int>("Inv_ID");
            Inventory = r.Field<double>("Inv_Inventory");
            MedBagAmount = r.Field<double>("Inv_MedBagAmount");
            OnTheFrame = r.Field<double>("InvOnTheFrame");
            TotalPrice = r.Field<double>("TotalPrice");
            AveragePrice = Inventory == 0 ? 0 : TotalPrice / Inventory;
            IsError = MedBagAmount > Inventory;
            Type = r.Field<int>("Type");
        }

        public bool IsFrozen { get; set; }
        public byte? IsControl { get; set; }
        public int Inv_ID { get; set; }
        public int Type { get; set; }
        public double Inventory { get; set; }
        public double MedBagAmount { get; set; }
        public double OnTheFrame { get; set; }
        public double TotalPrice { get; set; }
        public double AveragePrice { get; set; }
        public bool IsError { get; set; }
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                Set(() => IsSelected, ref isSelected, value);
            }
        }
    }
}