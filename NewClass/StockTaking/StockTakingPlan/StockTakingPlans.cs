using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
