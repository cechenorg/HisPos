using System.Data;
using His_Pos.Database;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Employee.ClockIn
{
    public static class ClockInDb
    {

        public static DataTable EmployeeClockInLog(string year, string month, string day, string account, int type)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", year));
            parameterList.Add(new SqlParameter("WMonth", month));
            parameterList.Add(new SqlParameter("WDay", day));
            parameterList.Add(new SqlParameter("EmpNo", account));
            parameterList.Add(new SqlParameter("Type", type));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLog]", parameterList);
            return table;
        }
        public static DataTable ClockInLogByDate(string year, string month, string account)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", year));
            parameterList.Add(new SqlParameter("WMonth", month));
            parameterList.Add(new SqlParameter("EmpId", account));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLogByDate]", parameterList);
            return table;
        }
    }
}