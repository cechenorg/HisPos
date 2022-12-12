using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.Accounts
{
    public class AccountsDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate, string keyWord)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "keyword", keyWord);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsRecordDetailsByDate]", parameterList);
        }

        public static void InsertCashFlowRecordDetail(AccountsAccount account, string note, double value, DateTime recorddate)
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
            DataBaseFunction.AddSqlParameter(parameterList, "RecordDate", recorddate);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccountsRecordDetail]", parameterList);
        }

        public static void UpdateCashFlowRecordDetail(AccountsAccount account, AccountsRecordDetail editedDetail)
        {
            //decimal cashFlowValue;
            //if (account.Type == CashFlowType.Expenses && editedDetail.CashFlowValue > 0)
            //    cashFlowValue = editedDetail.CashFlowValue * -1;
            //else
            //    cashFlowValue = editedDetail.CashFlowValue;
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowId", editedDetail.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "SubjectID", account.ID.Trim());
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowValue", editedDetail.CashFlowValue);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowNote", editedDetail.Note);
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowEmpId", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateAccountsRecord]", parameterList);
        }

        public static void DeleteCashFlow(AccountsRecordDetail selectedDetail)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CashFlowId", selectedDetail.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "EMP", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteAccountsRecord]", parameterList);
        }
        public static DataTable GetStrikeDataById(AccountsRecordDetail selectedDetail)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", selectedDetail.ID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StrikeHistoriesById]", parameterList);
        }

        public static DataTable GetAccountsDetail(string id, DateTime edate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", id));
            parameters.Add(new SqlParameter("edate", edate));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static DataTable GetAccountsDetailDetailReport(string id)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", id));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetailDetailReport]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static DataTable GetBankByAccountsID()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static DataSet GetIncomeData(int year)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> paraCount = new List<SqlParameter>();
            List<SqlParameter> paraIncome = new List<SqlParameter>();
            List<SqlParameter> paraExpanse = new List<SqlParameter>();
            List<SqlParameter> paraClosed = new List<SqlParameter>();
            paraCount.Add(new SqlParameter("YEAR", year));
            paraIncome.Add(new SqlParameter("YEAR", year));
            paraExpanse.Add(new SqlParameter("YEAR", year));
            paraClosed.Add(new SqlParameter("YEAR", year));
            DataTable count = MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCountByYear]", paraCount);//處方張數
            DataTable income = MainWindow.ServerConnection.ExecuteProc("[Get].[IncomeByYear]", paraIncome);//毛利
            DataTable expanse = MainWindow.ServerConnection.ExecuteProc("[Get].[ExpanseByYear]", paraExpanse);//費用
            DataTable closed = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsClosedByYear]", paraClosed);//結案差額
            MainWindow.ServerConnection.CloseConnection();
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(income);
            ds.Tables.Add(expanse);
            ds.Tables.Add(closed);
            return ds;
        }

        public static DataTable GetBalanceSheet(DateTime edate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("eDate", edate));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[BalanceSheet]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
    }
}