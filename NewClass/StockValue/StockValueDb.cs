using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.StockValue {
    public static class StockValueDb {
        public static void UpdateDailyStockValue() { 
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDailyStockValue]");
        }
        public static DataTable GetDataByDate(DateTime startDate,DateTime endDate) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", endDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[DailyStockValueByDate]", parameterList);
        } 
    }
}
