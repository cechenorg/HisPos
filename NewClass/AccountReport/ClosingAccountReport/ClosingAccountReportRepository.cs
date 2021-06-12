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
        private string groupServerConnectionString  = $@"Data Source={ViewModelMainWindow.CurrentPharmacy.GroupServerName};Persist Security Info=True;User ID=singde;Password=city1234";
        
    
        public IEnumerable<DailyClosingAccount> GetGroupClosingAccountRecord()
        { 
          
            SqlConnection connection = new SqlConnection(groupServerConnectionString);
            connection.Open();
             
            var sql = "exec [GetPharmacyInfoByVerifyKey] @Phamas_VerifyKey";
            var values = new { Phamas_VerifyKey = "2017.1.1" };
            var results = connection.Query<DailyClosingAccount>(sql, values).ToList();
             

            connection.Close();
            return results;
        }

        public void InsertDailyClosingAccountRecord(DailyClosingAccount data)
        {
            SqlConnection connection = new SqlConnection(groupServerConnectionString);
           
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Pharmacy_VerifyKey", Properties.Settings.Default.SystemSerialNumber));
            parameterList.Add(new SqlParameter("ClosingDate", data.ClosingDate));
            parameterList.Add(new SqlParameter("OTCSaleProfit", data.OTCSaleProfit));
            parameterList.Add(new SqlParameter("DailyAdjustAmount", data.DailyAdjustAmount));
            parameterList.Add(new SqlParameter("CooperativeClinicProfit", data.CooperativeClinicProfit));
            parameterList.Add(new SqlParameter("PrescribeProfit", data.PrescribeProfit));
            parameterList.Add(new SqlParameter("ChronicAndOtherProfit", data.ChronicAndOtherProfit));
            parameterList.Add(new SqlParameter("SelfProfit", data.SelfProfit));
            parameterList.Add(new SqlParameter("TotalProfit", data.TotalProfit));

            MainWindow.ServerConnection.ExecuteProcBySchema(
                ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[InsertClosingAccountRecord]", parameterList);
           
        }

    }
}
