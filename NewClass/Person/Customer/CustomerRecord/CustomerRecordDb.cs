using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public static class CustomerRecordDb
    {
        public static DataTable GetData(int id)
        { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusID", id);
           return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerRecordByCusID]", parameterList);
        }
    }
}
