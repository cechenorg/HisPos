using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.StockTakingOTCReport
{
    public static class StockTakingOTCReportDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingOTCReportByDate]", parameterList);
        }

        public static DataTable GetPrescriptionPointEditRecordByDates(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionPointEditRecordByDates]", parameterList);
        }

        public static DataTable GetPrescriptionPointEditRecordsByDates(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionPointEditRecordsByDates]", parameterList);
        }
    }
}