using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct {
   public class StockTakingPlanProducts : ObservableCollection<StockTakingPlanProduct> {
        public StockTakingPlanProducts() { }
        public StockTakingPlanProducts(DataTable table) {
            foreach (DataRow r in table.Rows) {
                Add(new StockTakingPlanProduct(r));
            }
        }

        public StockTakingPlanProducts GetControlMedincines(string warID) {
          return new StockTakingPlanProducts( StockTakingDB.GetStockTakingPlanProductByType("ControlMedicines", warID));
        }
        public StockTakingPlanProducts GetStockLessProducts(string warID) {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("StockLess", warID)); 
        }
        public StockTakingPlanProducts GetMonthMedicines(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("MonthMedUse", warID));
        }
        
        public StockTakingPlanProducts GetStockTakingPlanProductByProName(string name)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByProName(name));
        }
        
    }
}
