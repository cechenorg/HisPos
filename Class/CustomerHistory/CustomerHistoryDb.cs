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
        public static CustomerHistory GetDataByCUS_ID(string CUS_ID)
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var masterParameters = new List<SqlParameter>();
            masterParameters.Add(new SqlParameter("CUS_ID", CUS_ID));

            var detailParameters = new List<SqlParameter>();
            detailParameters.Add(new SqlParameter("CUS_ID", CUS_ID));

            CustomerHistory customerHistory  = new CustomerHistory(dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORY]", masterParameters),
                                                                   dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORYDETAIL]", detailParameters));
            
            return customerHistory;
        }
    }
}
