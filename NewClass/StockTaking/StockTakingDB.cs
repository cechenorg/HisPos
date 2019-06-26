using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Database;
using His_Pos.NewClass.Product.StockTaking;
using His_Pos.NewClass.StockTaking.StockTakingProduct;

namespace His_Pos.NewClass.StockTaking
{
    public class StockTakingDB
    {
        internal static DataTable GetStockTakingPlans()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlans]");
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
        internal static DataTable GetStockTakingPlanProductsByID(int planID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PLAN_ID", planID); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductsByID]", parameterList);
        }
         
        internal static void UpdateStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", stockTakingPlan.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Name", stockTakingPlan.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Note", stockTakingPlan.Note);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductIDs", SetStockTakingPlanProducts(stockTakingPlan.StockTakingProductCollection)); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStockTakingPlan]", parameterList);
             
        }
         
        #region TableSet
        public static DataTable ProductListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("MedicineID", typeof(string));
            return masterTable;
        }
        public static DataTable SetStockTakingPlanProducts(StockTakingPlanProducts productIds)
        { 
            DataTable productListTable = ProductListTable();
            foreach (var m in productIds)
            {
                DataRow newRow = productListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m.ID);
                productListTable.Rows.Add(newRow);
            }
            return productListTable;
        }
        #endregion
    }
}
