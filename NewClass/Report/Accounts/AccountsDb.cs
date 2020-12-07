﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;

namespace His_Pos.NewClass.Report.Accounts
{
    public class AccountsDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsRecordDetailsByDate]", parameterList);
        }

        public static void InsertCashFlowRecordDetail(AccountsAccount account, string note, double value)
        {
            double cashFlowValue;
            if (account.Type == CashFlowType.Expenses)
                cashFlowValue = value * 1;
            else
                cashFlowValue = value;
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", account.AccountName);
            DataBaseFunction.AddSqlParameter(parameterList, "Value", cashFlowValue);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceId", account.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "CurrentUserId", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Note", note);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccountsRecordDetail]", parameterList);
        }

        public static void UpdateCashFlowRecordDetail(AccountsAccount account, AccountsRecordDetail editedDetail)
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
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateAccountsRecord]", parameterList);
        }

        public static void DeleteCashFlow(AccountsRecordDetail selectedDetail)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowId", selectedDetail.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteAccountsRecord]", parameterList);
        }
    }
}
