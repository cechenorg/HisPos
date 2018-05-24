using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
