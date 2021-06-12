using Dapper;
using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
   public class ClosingAccountReportRepository
    {
        public IEnumerable<DailyClosingAccount> GetGroupClosingAccountRecordByKey(string verifyKey)
        {
            var groupID = ViewModelMainWindow.CurrentPharmacy.GroupServerName;
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.SQL_global);
            connection.Open();

            var sql = "exec [GetPharmacyInfoByVerifyKey] @Phamas_VerifyKey";
            var values = new { Phamas_VerifyKey = "2017.1.1" };
            var results = connection.Query<DailyClosingAccount>(sql, values).ToList();
             

            connection.Close();
            return results;
        }

        public void InsertDailyClosingAccountRecord(DailyClosingAccount data)
        {

        }

    }
}
