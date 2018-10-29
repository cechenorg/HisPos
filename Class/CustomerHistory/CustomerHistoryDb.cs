using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.CustomerHistory
{
    public static class CustomerHistoryDb
    {
        public static CustomerHistoryMaster GetDataByCUS_ID(string CUS_ID)
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var masterParameters = new List<SqlParameter>();
            masterParameters.Add(new SqlParameter("CUS_ID", CUS_ID));

            var detailParameters = new List<SqlParameter>();
            detailParameters.Add(new SqlParameter("CUS_ID", CUS_ID));

            CustomerHistoryMaster customerHistory  = new CustomerHistoryMaster(dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORY]", masterParameters),
                                                                   dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORYDETAIL]", detailParameters));
            
            return customerHistory;
        }
    }
}
