using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting.SplitProduct {
    public class SplitProducts : ObservableCollection<SplitProduct> {
        public SplitProducts() { }

        public void GetDataByProID(string proID) {
            var table = SplitProductDb.GetProductInventoryByProID(proID);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new SplitProduct(r));
            }
        }
        public void SplitProductInventory(string proID) {
           SplitProductDb.SplitProductInventory(proID,this);
          
        }
        
    }
}
