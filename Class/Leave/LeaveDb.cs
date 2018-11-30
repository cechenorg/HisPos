using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Leave
{
    public static class LeaveDb
    {
        internal static ObservableCollection<Leave> GetLeaveType()
        {
            ObservableCollection<Leave> collection = new ObservableCollection<Leave>();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetLeaveType]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add( new Leave(row));
            }

            return collection;
        }

        internal static Collection<LeaveRecord> GetLeaveRecord(string year, string month)
        {
            Collection<LeaveRecord> collection = new Collection<LeaveRecord>();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("YEAR", year));
            parameters.Add(new SqlParameter("MONTH", month));

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetLeaveRecord]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new LeaveRecord(row));
            }

            return collection;
        }
        
        internal static string AddNewLeave(string id, string leaveType, DateTime startDate, DateTime endDate, string note)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", id));
            parameters.Add(new SqlParameter("LEAVETYPE", leaveType));
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));
            parameters.Add(new SqlParameter("NOTE", note));

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[AddNewLeave]", parameters);

            return table.Rows[0]["RESULT"].ToString();
        }
    }
}
