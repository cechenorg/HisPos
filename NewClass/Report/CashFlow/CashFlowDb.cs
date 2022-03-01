using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.CashFlow
{
    public class CashFlowDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashFlowRecordDetailsByDate]", parameterList);
        }

        public static void InsertCashFlowRecordDetail(CashFlowAccount account, string note, double value, string ID)
        {
            double cashFlowValue;
            if (account.Type == CashFlowType.Expenses)
                cashFlowValue = value * -1;
            else
                cashFlowValue = value;
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", account.AccountName);
            DataBaseFunction.AddSqlParameter(parameterList, "Value", cashFlowValue);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceId", account.AccountID);
            DataBaseFunction.AddSqlParameter(parameterList, "CurrentUserId", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Note", note);
            DataBaseFunction.AddSqlParameter(parameterList, "Bank", ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCashFlowRecordDetail]", parameterList);
        }

        public static void UpdateCashFlowRecordDetail(CashFlowAccount account, CashFlowRecordDetail editedDetail)
        {
            decimal cashFlowValue;
            if (account.Type == CashFlowType.Expenses && editedDetail.CashFlowValue > 0)
                cashFlowValue = editedDetail.CashFlowValue * -1;
            else
                cashFlowValue = editedDetail.CashFlowValue;
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowId", editedDetail.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowName", account.AccountName);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowValue", cashFlowValue);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowNote", editedDetail.Note);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowEmpId", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCashFlowRecord]", parameterList);
        }

        public static void DeleteCashFlow(CashFlowRecordDetail selectedDetail)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowId", selectedDetail.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteCashFlowRecord]", parameterList);
        }
    }
}