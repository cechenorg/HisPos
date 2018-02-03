using System.Collections.Generic;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Copayment
{
    public class Copayments
    {
        public List<Copayment> CopaymentList { get;} = new List<Copayment>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[COPAYMENT]", dbConnection);
            foreach (DataRow copayment in divisionTable.Rows)
            {
                var c = new Copayment(copayment["HISCOP_ID"].ToString(), copayment["HISCOP_NAME"].ToString());
                CopaymentList.Add(c);
            }
        }
    }
}
