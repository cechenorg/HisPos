using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingPlanProduct
{
    public class StockTakingPlanProducts : ObservableCollection<StockTakingPlanProduct>
    {
        public StockTakingPlanProducts()
        {
        }

        public StockTakingPlanProducts(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingPlanProduct(r));
            }
        }

        public StockTakingPlanProducts GetProductByWarID(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.StockTakingPlanProductByWarID(this, warID));
        }

        public StockTakingPlanProducts GetControlMedincines(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("ControlMedicines", warID));
        }

        public StockTakingPlanProducts GetStockLessProducts(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("StockLess", warID));
        }

        public StockTakingPlanProducts GetMonthMedicines(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("MonthMedUse", warID));
        }

        public StockTakingPlanProducts GetOnTheFrameMedicines(string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByType("OnTheFrame", warID));
        }

        public StockTakingPlanProducts GetStockTakingPlanProductByProName(string name, string warID)
        {
            return new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductByProName(name, warID));
        }
    }
}