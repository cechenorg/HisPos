using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.BalanceSheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.CashReport
{
    public static class CashReportDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportByDate]", parameterList);
        }

        internal static DataTable GetClosingHistories(DateTime beginDate, DateTime endDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", beginDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkHistory]", parameterList);
        }

        internal static DataSet GetYearIncomeStatementForExport(int year)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "YEAR", year);
            return MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[YearIncomeStatementForExport]", parameterList);
        }

        public static DataTable GetPerDayDataByDate(DateTime sDate, DateTime eDate, string insID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "InsID", insID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportPerDayByID]", parameterList);
        }

        internal static DataSet GetBalanceSheet()
        {
            return MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[BalanceSheet]");
        }

        internal static DataTable StrikeBalanceSheet(string strikeType, string sheetType, double value, string sourceID, string note = "")//BalanceSheetTypeEnum
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", value));
            parameters.Add(new SqlParameter("TYPE", sheetType.ToString()));
            parameters.Add(new SqlParameter("NOTE", note));
            parameters.Add(new SqlParameter("TARGET", strikeType));
            parameters.Add(new SqlParameter("SOURCE_ID", sourceID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheet]", parameters);
        }

        internal static DataTable SetDeclareDoneMonth(DateTime dateTime)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE", dateTime));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareDoneMonth]", parameters);
        }

        internal static DataTable GetStrikeHistories()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StrikeHistoriesBySource]");
        }
        internal static DataTable GetSelectStrikeHistories(string type, DateTime sdate, DateTime edate)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            if(!string.IsNullOrEmpty(type))
                parameters.Add(new SqlParameter("type", type));
            parameters.Add(new SqlParameter("sdate", sdate));
            parameters.Add(new SqlParameter("edate", edate));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StrikeHistoriesBySourceByDate]", parameters);
        }


        public static void DeleteStrikeHistory(StrikeHistory selectedHistory)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Values", selectedHistory.StrikeValue));
            parameters.Add(new SqlParameter("StrikeSource", selectedHistory.StrikeSource));
            parameters.Add(new SqlParameter("StrikeID", selectedHistory.StrikeID));
            parameters.Add(new SqlParameter("Source", selectedHistory.StrikeWay));
            parameters.Add(new SqlParameter("StrikeSourceID", selectedHistory.StrikeSourceID));
            parameters.Add(new SqlParameter("StrikeNote", selectedHistory.StrikeNote));
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStrikeHistory]", parameters);
        }

        public static DataTable GetInventoryDifferenceByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDATE", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDATE", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[InventoryDifferanceByDate]", parameterList);
        }
    }
}