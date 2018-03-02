using System.Collections.Generic;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Division
{
    public static class DivisionDb
    {
        public static List<Division> DivisionsList { get; } = new List<Division>();

        public static void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[DIVISION]", dbConnection);
            foreach (DataRow division in divisionTable.Rows)
            {
                var d = new Division(division["HISDIV_ID"].ToString(), division["HISDIV_NAME"].ToString());
                DivisionsList.Add(d);
            }
        }
    }
}
