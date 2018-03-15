using System.Collections.Generic;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.AdjustCase
{
    public static class AdjustCaseDb
    {
        public static readonly List<AdjustCase> AdjustCaseList = new List<AdjustCase>();

        static AdjustCaseDb()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[ADJUSTCASE]", dbConnection);
            foreach (DataRow adjustcase in divisionTable.Rows)
            {
                var d = new AdjustCase(adjustcase["ADJUSTCASE_ID"].ToString(), adjustcase["ADJUSTCASE_NAME"].ToString());
                AdjustCaseList.Add(d);
            }
        }
        
    }
}
