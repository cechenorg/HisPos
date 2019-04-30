using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.Class.CustomerHistory
{
    public static class CustomerHistoryDb
    {
        public static DataTable GetData(int id)
        { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusID", id);
           return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerHistoryByCusID]", parameterList);
        }
    }
}
