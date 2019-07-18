using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingPlanProduct {
    public class StockTakingPlanProduct : Product.Product {
        public StockTakingPlanProduct(DataRow r):base(r) {

        }
        
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
