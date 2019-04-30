using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public static class PharmacistScheduleDb
    {
        public static DataTable InsertEmployeeSchedule(DateTime start, DateTime end,PharmacistSchedule schedule)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "START", start);
            DataBaseFunction.AddSqlParameter(parameterList, "END", end);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertEmployeeSchedule]", parameterList);
        }

        public static DataTable GetEmployeeSchedule(DateTime start, DateTime end)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeScheduleWithPrescriptionCount]", parameterList);
        }

        public static void InsertSchedule(DateTime start, DateTime end, PharmacistSchedule schedule)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", end);
            MainWindow.ServerConnection.ExecuteProc("", parameterList);
        }
    }
}
