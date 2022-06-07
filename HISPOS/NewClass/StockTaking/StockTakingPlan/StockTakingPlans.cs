using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingPlan
{
    public class StockTakingPlans : Collection<StockTakingPlan>
    {
        private StockTakingPlans(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new StockTakingPlan(row));
            }
        }

        internal static StockTakingPlans GetStockTakingPlans()
        {
            return new StockTakingPlans(StockTakingDB.GetStockTakingPlans());
        }
    }
}