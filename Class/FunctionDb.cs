
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using static His_Pos.Function;

namespace His_Pos.Class
{
    public static class FunctionDb
    {
        internal static bool CheckYearlyHoliday() {
            var dd = new DbConnection(Settings.Default.SQL_local);
          var table =  dd.ExecuteProc("[HIS_POS_DB].[FunctionView].[CheckYearlyHoliday]");
            if (table.Rows[0][0].ToString() == "0")
                return true;
            else
                return false;
        }
        internal static void UpdateLastYearlyHoliday(Holiday holiday)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE", holiday.date));
            parameters.Add(new SqlParameter("NAME", holiday.name));
            string today = holiday.date.DayOfWeek.ToString("d");
            switch (today) {
                case "0":
                    parameters.Add(new SqlParameter("CATEGORY", "禮拜日"));
                    break;
                case "6":
                    parameters.Add(new SqlParameter("CATEGORY","禮拜六"));
                    break;
                default:
                    parameters.Add(new SqlParameter("CATEGORY", holiday.holidayCategory));
                    break;
            }
            parameters.Add(new SqlParameter("DESCRIPTION", holiday.description));
            dd.ExecuteProc("[HIS_POS_DB].[FunctionView].[UpdateLastYearlyHoliday]", parameters);
        }

        internal static string GetSystemVersionId()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[GetSystemVersion]");

            return table.Rows[0]["VERSION"].ToString();
        }

        internal static void UpdateSystemVersionId()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("VER", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            dd.ExecuteProc("[HIS_POS_DB].[dbo].[UpdateSystemVersion]", parameters);
        }

        internal static DateTime GetLastSyncDate()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[GetSyncDate]");

            return DateTime.Parse(table.Rows[0]["SINGDE_SYNCTIME"].ToString());
        }
    }
  
}