using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.StockTaking;

namespace His_Pos.NewClass.Product.StockTaking
{
    public class StockTakingProducts : Collection<StockTakingProduct>
    {
        public StockTakingProducts() {

        }
        private StockTakingProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new StockTakingProduct(row));
            }
        }
        internal static StockTakingProducts GetStockTakingPlanProductsByID(int planID)
        {
            return new StockTakingProducts(StockTakingDB.GetStockTakingPlanProductsByID(planID));
        }
    }
}
