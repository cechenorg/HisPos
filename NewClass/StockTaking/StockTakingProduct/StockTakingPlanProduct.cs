using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct {
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
