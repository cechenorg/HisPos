using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.StockValue.StockEntry
{
    public static class StockEntryDb
    {
        public static DataTable GetDataByDate(DateTime date)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Date", date);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockEntryDetail]", parameterList);
        }
    }
}