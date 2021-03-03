using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public static class PharmacistScheduleDb
    {
        public static DataTable GetEmployeeScheduleWithCount(DateTime start, DateTime end)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeScheduleWithPrescriptionCount]", parameterList);
        }

        public static DataTable GetEmployeeSchedule(DateTime start, DateTime end)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeSchedule]", parameterList);
        }

        public static void InsertSchedule(DateTime start, DateTime end, PharmacistSchedule schedule)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", end);
            DataBaseFunction.AddSqlParameter(parameterList, "DateList", SetScheduleTable(schedule));
            DataBaseFunction.AddSqlParameter(parameterList, "User_ID", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateScheduleTemp]", parameterList);
        }

        #region TableSet

        public static DataTable ScheduleTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("Emp_ID", typeof(int));
            masterTable.Columns.Add("Sch_Date", typeof(DateTime));
            masterTable.Columns.Add("Register_Time", typeof(DateTime));
            return masterTable;
        }

        public static DataTable SetScheduleTable(PharmacistSchedule schedule)
        {
            DataTable scheduleTable = ScheduleTable();

            foreach (var s in schedule)
            {
                DataRow newRow = scheduleTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "Emp_ID", s.MedicalPersonnel.ID);
                DataBaseFunction.AddColumnValue(newRow, "Sch_Date", s.Date);
                DataBaseFunction.AddColumnValue(newRow, "Register_Time", s.RegisterTime);
                scheduleTable.Rows.Add(newRow);
            }
            return scheduleTable;
        }

        #endregion TableSet
    }
}