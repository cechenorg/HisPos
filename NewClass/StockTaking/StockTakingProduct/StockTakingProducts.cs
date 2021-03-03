using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct
{
    public class StockTakingProducts : ObservableCollection<StockTakingProduct>
    {
        public StockTakingProducts()
        {
        }

        public StockTakingProducts(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingProduct(r));
            }
        }

        public StockTakingProducts GetStockTakingProductsByID(string ID)
        {
            return new StockTakingProducts(StockTakingDB.GetStockTakingProductsByID(ID));
        }

        public static StockTakingProducts GetStockTakingPlanProducts(StockTakingPlanProducts stockTakingPlanProducts, string warID)
        {
            return new StockTakingProducts(StockTakingDB.GetStockTakingProductsInventory(stockTakingPlanProducts, warID));
        }
    }
}