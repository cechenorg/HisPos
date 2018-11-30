using His_Pos.Properties;
using System.Data;
using His_Pos.Service;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace His_Pos.Class.StockTakingOrder
{
    public static class StockTakingOrderDb
    {
        internal static DataTable GetStockTakingRecord()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            return dd.ExecuteProc("[HIS_POS_DB].[StockTakingRecord].[GetStockTakingRecord]");
        }
        internal static string StockCheckById(string proId,string stockCheckValue) {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("EMP_ID", MainWindow.CurrentUser.Id));
            parameters.Add(new SqlParameter("StockCheckValue", stockCheckValue));
            var table = dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[StockCheckById]", parameters);

            return table.Rows[0]["TOTAL"].ToString();
        } 
    } 
}
