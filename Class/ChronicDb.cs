using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public static class ChronicDb
    {
        internal static void CaculateChironic() { //假設病人1-3沒領  要幫他算出2-1~2-3
            var dd = new DbConnection(Settings.Default.SQL_global);
            dd.ExecuteProc("[HIS_POS_DB].[Index].[CaculateChironic]");
        }
    }
}
