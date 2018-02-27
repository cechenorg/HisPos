using System.Collections.Generic;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Copayment
{
    public static class CopaymentDb
    {
        public static List<Copayment> CopaymentList { get;} = new List<Copayment>();

        public static void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[COPAYMENT]", dbConnection);
            foreach (DataRow copayment in divisionTable.Rows)
            {
                var c = new Copayment(copayment["HISCOP_ID"].ToString(), copayment["HISCOP_NAME"].ToString());
                CopaymentList.Add(c);
            }
        }
        /*
         *回傳對應部分負擔之id + name string
         */
        public static string GetCopayment(string tag)
        {
            string result = string.Empty;
            GetData();
            foreach (var copayment in CopaymentList)
            {
                if (copayment.Id == tag)
                {
                    result = copayment.Id + ". " + copayment.Name;
                }
            }
            return result;
        }
    }
}
