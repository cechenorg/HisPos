using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.StockValue
{
    public static class StockValueDb
    {
        public static void UpdateDailyStockValue()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDailyStockValue]");
        }

        public static DataTable GetDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockChangeReport]", parameterList);
        }

        public static DataTable GetOTCDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[OTCStockChangeReport]", parameterList);
        }
    }
}