using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingPlanProduct {
    public class StockTakingPlanProduct : Product.Product {
        public StockTakingPlanProduct(DataRow r):base(r) {
            IsFrozen = r.Field<bool>("Med_IsFrozen");
            IsControl = r.Field<byte?>("Med_Control");
        }
        public bool IsFrozen { get; set; }
        public byte? IsControl { get; set; }
        private bool isSelected;
        public bool IsSelected {
            get { return isSelected; }
            set
            {
                Set(() => IsSelected, ref isSelected, value); 
            }
        }
    }
}
