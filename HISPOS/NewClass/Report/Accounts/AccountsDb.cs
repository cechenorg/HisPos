using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

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

        public static IEnumerable<JournalAccount> GetJournalAccount()
        {
            IEnumerable<JournalAccount> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalAccount>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[AccountsCommonItems]",
                    param: new
                    {
                        Item = "立帳作業"
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public static IEnumerable<JournalMaster> GetJournalData(DateTime sdate, DateTime edate, string jouMas_ID, string acct1ID, string acct2ID, string acct3ID, string keyword)
        {
            IEnumerable<JournalMaster> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalMaster>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[JournalRecord]",
                    param: new
                    {
                        sdate,
                        edate,
                        jouMas_ID,
                        acct1ID,
                        acct2ID,
                        acct3ID,
                        keyword
                    },
                    commandType: CommandType.StoredProcedure);
            });
            return result;
        }
        public static IEnumerable<JournalDetail> GetJournalData(string jouMas_ID)
        {
            string sql = string.Format("Select JouDet_ID, JouDet_Type,JouDet_Number,JouDet_AcctLvl1,JouDet_AcctLvl2,JouDet_AcctLvl3,Cast(JouDet_Amount As int) As JouDet_Amount,JouDet_Memo From {0}.dbo.JournalDetail Where JouDet_ID = '{1}'", Properties.Settings.Default.SystemSerialNumber, jouMas_ID);
            IEnumerable<JournalDetail> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalDetail>(sql,
                    commandType: CommandType.Text);
            });
            return result;
        }

        public static string InsertTempJournal()
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            int emp = ViewModelMainWindow.CurrentUser.ID;
            #region 新單號
            string cntSQL = string.Format("Select Count(JouMas_ID)+1 From [{0}].[dbo].[JournalMaster] Where Cast(JouMas_InsertTime As Date) = Cast(GetDate() As Date)", Properties.Settings.Default.SystemSerialNumber);
            string cnt = "1";
            IEnumerable<string> cntQuery = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                cntQuery = conn.Query<string>(cntSQL,
                    commandType: CommandType.Text);
            });
            foreach (var item in cntQuery)
            {
                cnt = item;
            }
            string newID = DateTime.Today.ToString("yyyyMMdd") + "-" + cnt.PadLeft(3, '0');
            #endregion
            #region 新增暫存
            string sql = string.Format(@"Insert Into [{0}].[dbo].[JournalMaster] Values('{1}', '{2}', 'T', 1, null, null, GetDate(), {3}, null, null)",
                                        Properties.Settings.Default.SystemSerialNumber, newID, date, emp);
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>(sql, commandType: CommandType.Text);
            });
            #endregion
            return newID;
        }
        public static void UpdateJournalData(string undo, JournalMaster master)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                int emp = ViewModelMainWindow.CurrentUser.ID;
                string sql = string.Empty;
                string sqlDetail = string.Empty;
                int i = 1, j = 1;
                if (undo.Equals("修改"))
                {
                    sql = string.Format(@"Update [{0}].[dbo].[JournalMaster] Set JouMas_Memo = '{1}', JouMas_ModifyTime = GETDATE(), JouMas_ModifyEmpID = {2}, JouMas_Date = '{3}' Where JouMas_ID = '{4}'", Properties.Settings.Default.SystemSerialNumber, master.JouMas_Memo, emp, master.JouMas_Date.Value.ToString("yyyy-MM-dd"), master.JouMas_ID);
                    sqlDetail = string.Format("Delete [{0}].[dbo].[JournalDetail] Where JouDet_ID = '{1}'", Properties.Settings.Default.SystemSerialNumber, master.JouMas_ID);
                    sql = sql + "\r\n" + sqlDetail;
                }
                else if(undo.Equals("新增"))
                {
                    sql = string.Format(@"Update [{0}].[dbo].[JournalMaster] Set JouMas_Status = 'F', JouMas_Memo = '{1}', JouMas_ModifyTime = GETDATE(), JouMas_ModifyEmpID = {2}, JouMas_Date = '{3}' Where JouMas_ID = '{4}'", Properties.Settings.Default.SystemSerialNumber, master.JouMas_Memo, emp, master.JouMas_Date.Value.ToString("yyyy-MM-dd"), master.JouMas_ID);
                }
                else//保存
                {
                    sql = string.Format(@"Update [{0}].[dbo].[JournalMaster] Set JouMas_Memo = '{1}', JouMas_Date = '{2}' Where JouMas_ID = '{3}'", Properties.Settings.Default.SystemSerialNumber, master.JouMas_Memo, master.JouMas_Date.Value.ToString("yyyy-MM-dd"), master.JouMas_ID);
                    sqlDetail = string.Format("Delete [{0}].[dbo].[JournalDetail] Where JouDet_ID = '{1}'", Properties.Settings.Default.SystemSerialNumber, master.JouMas_ID);
                    sql = sql + "\r\n" + sqlDetail;
                }

                foreach (JournalDetail item in master.DebitDetails)
                {
                    sqlDetail = string.Format(@"Insert Into [{0}].[dbo].[JournalDetail] (JouDet_ID, JouDet_Type, JouDet_Number, JouDet_AcctLvl1, JouDet_AcctLvl2, JouDet_AcctLvl3, JouDet_Amount, JouDet_Memo) Values('{1}', 'D', {2}, '{3}', '{4}','{5}', {6}, '{7}')", Properties.Settings.Default.SystemSerialNumber,
                        master.JouMas_ID,
                        i,
                        item.Account.acctLevel1,
                        item.Account.acctLevel2,
                        item.Account.acctLevel3,
                        item.JouDet_Amount,
                        item.JouDet_Memo);
                    sql = sql + "\r\n" + sqlDetail;
                    i++;
                }

                foreach (JournalDetail item in master.CreditDetails)
                {
                    sqlDetail = string.Format(@"Insert Into [{0}].[dbo].[JournalDetail] (JouDet_ID, JouDet_Type, JouDet_Number, JouDet_AcctLvl1, JouDet_AcctLvl2, JouDet_AcctLvl3, JouDet_Amount, JouDet_Memo) Values('{1}', 'C', {2}, '{3}', '{4}', '{5}', {6}, '{7}')", Properties.Settings.Default.SystemSerialNumber,
                        master.JouMas_ID,
                        j,
                        item.Account.acctLevel1,
                        item.Account.acctLevel2,
                        item.Account.acctLevel3,
                        item.JouDet_Amount,
                        item.JouDet_Memo);
                    sql = sql + "\r\n" + sqlDetail;
                    j++;
                }

                SQLServerConnection.DapperQuery((conn) =>
                {
                    _ = conn.Query<int>(sql, commandType: CommandType.Text);
                });
                scope.Complete();
            }
        }
        public static void InvalidJournalData(string jouMas_ID)
        {
            int emp = ViewModelMainWindow.CurrentUser.ID;
            string sql = string.Format(@"Update [{0}].[dbo].[JournalMaster] Set JouMas_IsEnable = 0, JouMas_ModifyTime = GETDATE(), JouMas_ModifyEmpID = {1} Where JouMas_ID = '{2}'", Properties.Settings.Default.SystemSerialNumber, emp, jouMas_ID);
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>(sql, commandType: CommandType.Text);
            });
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