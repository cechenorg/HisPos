using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.WorkSchedule
{
    public static class WorkScheduleDb
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

        internal static ObservableCollection<UserIconData> GetUserIconDatas()
        {
            ObservableCollection<UserIconData> collection = new ObservableCollection<UserIconData>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetUserIconData]");

            string[] hexs = { "#4CFF0000", "#4CFF8B00", "#4CE8FF00", "#4C5DFF00", "#4C00FF97", "#4C00A2FF", "#4C0000FF", "#4C8B00FF", "#4CFF00FF" };

            int index = 0;

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new UserIconData(row, hexs[index]));

                index = (index + 1) % 9;
            }

            return collection;
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
    }
}
