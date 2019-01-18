using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Employee
{ 
    public static class EmployeeDb
    {
        public static  DataTable GetData()
        {
            var table = new DataTable();
            return table;
        }
        public static void DeleteEmployee() {

        }
        public static DataTable EmployeeLogin(string account,string password) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Account", account));
            parameterList.Add(new SqlParameter("Password", password));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeLogin]",parameterList);  
            return table;
        }
        public static DataTable GetTabAuth(int AuthValue) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("AuthValue", AuthValue)); 
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[TabAuth]", parameterList);
            return table;
        }
    }
}
