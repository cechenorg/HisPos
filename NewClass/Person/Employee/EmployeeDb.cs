using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person
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
            var table = MainWindow.ServerConnection.ExecuteProc("[HISPOS_Develop].[Get].[EmployeeLogin]",parameterList);  
            return table;
        } 
    }
}
