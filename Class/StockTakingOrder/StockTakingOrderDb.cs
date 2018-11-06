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
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[StockTakingRecord].[GetStockTakingRecord]");
        }
        internal static void StockCheckById(string proId,string stockCheckValue) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("StockCheckValue", stockCheckValue));
            dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[StockCheckById]", parameters);
        } 
    } 
}
