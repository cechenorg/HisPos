using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class ClosingAccountReportRepository
    {
        public List<PharmacyInfo> GetPharmacyInfosByGroupServerName(string groupServerName)
        {
            List<PharmacyInfo> result = new List<PharmacyInfo>();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("groupServerName", groupServerName));
            DataTable table = MainWindow.ServerConnection.ExecuteProcBySchema(
                  ViewModelMainWindow.CurrentPharmacy.HISPOS_ServerName, "[Get].[PharmacyListByGroupServerName]", parameterList);

            foreach (DataRow r in table.Rows)
            {
                result.Add(new PharmacyInfo(r));
            }
            return result;
        }

        public List<DailyClosingAccount> GetGroupClosingAccountRecord()
        {
            DataTable table = MainWindow.ServerConnection.ExecuteProcBySchema(
                  ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Get].[GroupClosingAccountRecord]");

            List<DailyClosingAccount> result = new List<DailyClosingAccount>();

            foreach (DataRow r in table.Rows)
            {
                DailyClosingAccount data = new DailyClosingAccount(r);
                result.Add(data);
            }

            return result;
        }

        public void InsertDailyClosingAccountRecord(DailyClosingAccount data)
        {
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

            if (ViewModelMainWindow.CurrentPharmacy.GroupServerName != string.Empty)
                MainWindow.ServerConnection.ExecuteProcBySchema(
                    ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[InsertClosingAccountRecord]", parameterList);
            else
            {
                MainWindow.ServerConnection.ExecuteProcBySchema(
                    ViewModelMainWindow.CurrentPharmacy.HISPOS_ServerName, "[Set].[InsertClosingAccountRecord]", parameterList);
            }
        }

        public List<MonthlyAccountTarget> GetMonthTargetByGroupServerName(string groupServerName)
        {
            List<MonthlyAccountTarget> result = new List<MonthlyAccountTarget>();

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("groupServerName", groupServerName));
            DataTable table = MainWindow.ServerConnection.ExecuteProcBySchema(
                ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Get].[MonthTargetByGroupServerName]", parameterList);

            foreach (DataRow r in table.Rows)
            {
                result.Add(new MonthlyAccountTarget(r));
            }
            return result;
        }

        public void UpdateClosingAccountTarget(MonthlyAccountTarget data)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("verifykey", data.VerifyKey));
            parameterList.Add(new SqlParameter("TargetMonth", data.Month));
            parameterList.Add(new SqlParameter("TargetValue", data.MonthlyTarget));
            MainWindow.ServerConnection.ExecuteProcBySchema(
               ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[InsertUpdateAccountTarget]", parameterList);
        }

        public void UpdateWorkingDaySetting(DateTime date, int count)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@date", date));
            parameterList.Add(new SqlParameter("@count", count));
            MainWindow.ServerConnection.ExecuteProcBySchema(
                ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[UpdateWorkingDaySetting]", parameterList);
        }

        public List<WorkingDaySetting> GetWorkingDaySetting()
        {
            List<WorkingDaySetting> result = new List<WorkingDaySetting>();
            DataTable table = MainWindow.ServerConnection.ExecuteProcBySchema(
                 ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Get].[WorkingDayRecord]");

            foreach (DataRow row in table.Rows)
            {
                WorkingDaySetting data = new WorkingDaySetting(row);
                result.Add(data);
            }
            return result;
        }

        public class WorkingDaySetting
        {
            public WorkingDaySetting(DataRow r)
            {
                Date = r.Field<DateTime>("WorkingDate");
                DayCount = Convert.ToInt32(r["WorkingDayCount"].ToString());
            }

            public DateTime Date { get; set; }
            public int DayCount { get; set; }
        }

        public class PharmacyInfo
        {
            public PharmacyInfo(DataRow r)
            {
                VerifyKey = r.Field<string>("PHAMAS_VerifyKey");
                Name = r.Field<string>("PHAMAS_NAME");
            }

            public string VerifyKey { get; set; }

            public string Name { get; set; }
        }
    }
}