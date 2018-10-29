using His_Pos.H5_ATTEND.WorkScheduleManage;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.Class.WorkSchedule
{
    public class WorkScheduleDb
    {
        internal static ObservableCollection<WorkSchedule> GetWorkSchedules(string year, string month)
        {
            ObservableCollection<WorkSchedule> collection = new ObservableCollection<WorkSchedule>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("YEAR", year));
            parameters.Add(new SqlParameter("MONTH", month));
            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetWorkSchedule]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new WorkSchedule(row));
            }

            return collection;
        }

        internal static ObservableCollection<UserIconData> GetTodayUsers()
        {
            ObservableCollection<UserIconData> collection = new ObservableCollection<UserIconData>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ClockInView].[GetTodayUserData]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new UserIconData(row));
            }

            return collection;
        }

        internal static ObservableCollection<UserIconData> GetUserIconDatas()
        {
            ObservableCollection<UserIconData> collection = new ObservableCollection<UserIconData>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetUserIconData]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new UserIconData(row));
            }

            return collection;
        }

        internal static string UserClockIn(string id, string password, string inout)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("EMP_ID", id));
            parameters.Add(new SqlParameter("EMP_PASSWORD", password));
            parameters.Add(new SqlParameter("INOUT", inout));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ClockInView].[ClockIn]", parameters);
            return table.Rows[0]["RESULT"].ToString();
        }

        internal static void InsertWorkSchedules(ObservableCollection<WorkSchedule> workSchedules, string year, string month)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            DataTable workSchedule = new DataTable();
            workSchedule.Columns.Add("EMP_ID", typeof(string));
            workSchedule.Columns.Add("EMPSCH_DATE", typeof(DateTime));
            workSchedule.Columns.Add("EMPSCH_PERIOD", typeof(string));

            int selectedYear = Int32.Parse(year);
            int selectedMonth = Int32.Parse(month);

            foreach (var ws in workSchedules)
            {
                var newRow = workSchedule.NewRow();
                newRow["EMP_ID"] = ws.Id;
                newRow["EMPSCH_DATE"] = new DateTime(selectedYear, selectedMonth, Int32.Parse(ws.Day));
                newRow["EMPSCH_PERIOD"] = ws.Period;
                workSchedule.Rows.Add(newRow);
            }
            parameters.Add(new SqlParameter("WORKSCHEDULE", workSchedule));
            parameters.Add(new SqlParameter("YEAR", year));
            parameters.Add(new SqlParameter("MONTH", month));

            dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[InsertWorkSchedules]", parameters);
        }

        internal static void SaveCalendarRemark(DateTime thisDay, string importantMessage)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("DATE", thisDay));
            parameters.Add(new SqlParameter("REMARK", importantMessage));

            dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[SaveCalendarRemark]", parameters);
        }

        internal static Collection<WorkScheduleManageView.SpecialData> GetSpecialData(int year, int month)
        {
            Collection<WorkScheduleManageView.SpecialData> collection = new Collection<WorkScheduleManageView.SpecialData>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("YEAR", year));
            parameters.Add(new SqlParameter("MONTH", month));

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetSpecialDate]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new WorkScheduleManageView.SpecialData(row));
            }

            return collection;
        }
    }
}
