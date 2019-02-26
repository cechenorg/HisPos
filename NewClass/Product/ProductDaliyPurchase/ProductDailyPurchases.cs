using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductDaliyPurchase {
    public class ProductDailyPurchases : ObservableCollection<ProductDailyPurchase> {
        public ProductDailyPurchases() {

        }
        public void GetDailyPurchaseData() {
            var table = ProductDailyPurchaseDb.GetDailyPurchaseData();
            foreach (DataRow r in table.Rows) {
                Add(new ProductDailyPurchase(r));
            } 
        }
    }
}
