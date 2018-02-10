using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.CustomerHistory
{
    public static class CustomerHistoryDb
    {
        public static void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CUS_ID", "1"));
            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORY]", parameters);

            foreach (DataRow d in table.Rows)
            {
                
            }
        }
    }
}
