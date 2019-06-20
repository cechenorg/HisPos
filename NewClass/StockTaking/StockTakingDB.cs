using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.StockTaking;

namespace His_Pos.NewClass.StockTaking
{
    public class StockTakingDB
    {
        internal static DataTable GetStockTakingPlans()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlans]");
        }

        internal static DataTable GetStockTakingPlanProductsByID(int planID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PLAN_ID", planID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductsByID]", parameters);
        }
    }
}
