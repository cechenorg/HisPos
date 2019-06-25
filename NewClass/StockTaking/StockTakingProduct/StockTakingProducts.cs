using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct {
   public class StockTakingProducts : ObservableCollection<StockTakingProduct> {
        public StockTakingProducts() { }
        public StockTakingProducts(DataTable table) {
            foreach (DataRow r in table.Rows) {
                Add(new StockTakingProduct(r));
            }
        }

        public StockTakingProducts GetControlMedincines(string warID) {
          return new StockTakingProducts( StockTakingDB.GetStockTakingProductByType("ControlMedicines", warID));
        }
        public StockTakingProducts GetStockLessProducts(string warID) {
            return new StockTakingProducts(StockTakingDB.GetStockTakingProductByType("StockLess", warID)); 
        }
    }
}
