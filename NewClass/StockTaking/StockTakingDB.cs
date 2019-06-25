using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Database;
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
        internal static void NewStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "name", stockTakingPlan.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", stockTakingPlan.WareHouse.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "note", stockTakingPlan.Note);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStockTakingPlan]", parameterList); 
        }
        internal static void UpdateStockTakingPlan() {

        }
        internal static void DeleteStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", stockTakingPlan.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStockTakingPlan]", parameterList);
        }
        internal static DataTable GetStockTakingProductByType(string type,string warID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "type", type);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID); 
           return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingProductByType]", parameterList);
        }
    }
}
