using His_Pos.Properties;
using System.Data;
using His_Pos.Service;

namespace His_Pos.Class.StockTakingOrder
{
    public static class StockTakingOrderDb
    {
        internal static DataTable GetStockTakingRecord()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[StockTakingRecord].[GetStockTakingRecord]");
        }
    }
}
