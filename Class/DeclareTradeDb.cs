using His_Pos.Properties;
using His_Pos.Service;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace His_Pos.Class
{
   public static class DeclareTradeDb
    {
        internal static DeclareTrade GetDeclarTradeByMasId(string Id)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MASID", Id));
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[GetDeclarTradeByMasId]", parameters);
            DeclareTrade declareTrade = null;
            if (table.Rows.Count != 0) 
                 declareTrade = new DeclareTrade(table.Rows[0]["EMP_ID"].ToString(), table.Rows[0]["PAYSELF"].ToString(),
            table.Rows[0]["DEPOSIT"].ToString(), table.Rows[0]["RECEIVE_MONEY"].ToString(),  table.Rows[0]["COPAYMENT"].ToString(), table.Rows[0]["PAYMONEY"].ToString(),
            table.Rows[0]["CHANGE"].ToString(), table.Rows[0]["PAYWAY"].ToString(), table.Rows[0]["CUS_ID"].ToString()); 
            else
                declareTrade = new DeclareTrade("0", "0", "0", "0","0", "0","0","0", "0");

            return declareTrade;
        }

    }
}
