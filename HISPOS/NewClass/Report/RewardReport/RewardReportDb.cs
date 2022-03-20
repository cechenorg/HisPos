using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.RewardReport
{
    public static class RewardReportDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[POS].[RewardReportByDate]", parameterList);
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