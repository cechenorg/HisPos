using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
   public static class DeclareTradeDb
    {
        internal static DeclareTrade GetDeclarTradeByMasId(string Id)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MASID", Id));
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[GetDeclarTradeByMasId]", parameters);
            DeclareTrade declareTrade = new DeclareTrade(
             table.Rows[0]["CUS_ID"].ToString(),
             table.Rows[0]["EMP_ID"].ToString(),
             table.Rows[0]["PAYSELF"].ToString(),
             table.Rows[0]["DEPOSIT"].ToString(),
             table.Rows[0]["RECEIVE_MONEY"].ToString(),
             table.Rows[0]["COPAYMENT"].ToString(),
             table.Rows[0]["PAYMONEY"].ToString(),
             table.Rows[0]["CHANGE"].ToString(),
             table.Rows[0]["PAYWAY"].ToString()
                );
            return declareTrade;
        }

    }
}
