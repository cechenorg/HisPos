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

        public static IEnumerable<JournalAccount> GetJournalAccount(string tabName)
        {
            IEnumerable<JournalAccount> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalAccount>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[AccountsCommonItems]",
                    param: new
                    {
                        Item = tabName//"立帳作業"
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public static IEnumerable<JournalMaster> GetJournalData(DateTime sdate, DateTime edate, string jouMas_ID, string acct1ID, string acct2ID, string acct3ID, string keyword, int source)
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
                        keyword,
                        source
                    },
                    commandType: CommandType.StoredProcedure);
            });
            return result;
        }
        public static IEnumerable<JournalMaster> GetJournalMasterData(string jouMas_ID)
        {
            IEnumerable<JournalMaster> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalMaster>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[JournalMasterData]",
                    param: new
                    {
                        ID = jouMas_ID
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }
        public static IEnumerable<JournalDetail> GetJournalData(string jouMas_ID)
        {
            IEnumerable<JournalDetail> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<JournalDetail>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[JournalDetailData]",
                    param: new
                    {
                        ID = jouMas_ID
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public static string InsertTempJournal()
        {
            int emp = ViewModelMainWindow.CurrentUser.ID;
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("Emp", emp)
            };
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertNewJournal]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            string newID = (table != null && table.Rows.Count > 0) ? Convert.ToString(table.Rows[0]["NewJournalID"]) : string.Empty;
            return newID;
        }
        public static void UpdateJournalData(string undo, JournalMaster master)
        {
            int emp = ViewModelMainWindow.CurrentUser.ID;
            DataTable tbMaster = CreateJournalMaster(master);
            DataTable tbDetail = CreateJournalDetail(master);
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[UpdateJournalData]",
                    param: new
                    {
                        undo = undo,
                        Emp = emp,
                        master = tbMaster,
                        detail = tbDetail
                    }, commandType: CommandType.StoredProcedure);
            });
        }
        private static DataTable CreateJournalMaster(JournalMaster master)
        {
            DataTable table = new DataTable();
            DataColumn dc1 = new DataColumn("JouMas_ID", typeof(string));
            DataColumn dc2 = new DataColumn("JouMas_Date", typeof(string));
            DataColumn dc3 = new DataColumn("JouMas_Status", typeof(string));
            DataColumn dc4 = new DataColumn("JouMas_IsEnable", typeof(int));
            DataColumn dc5 = new DataColumn("JouMas_Memo", typeof(string));
            DataColumn dc6 = new DataColumn("JouMas_VoidReason", typeof(string));
            DataColumn dc7 = new DataColumn("JouMas_Source", typeof(int));
            table.Columns.Add(dc1);
            table.Columns.Add(dc2);
            table.Columns.Add(dc3);
            table.Columns.Add(dc4);
            table.Columns.Add(dc5);
            table.Columns.Add(dc6);
            table.Columns.Add(dc7);
            DataRow newRow = table.NewRow();
            newRow["JouMas_ID"] = master.JouMas_ID;
            newRow["JouMas_Date"] = Convert.ToDateTime(master.JouMas_Date).ToString("yyyy-MM-dd");
            newRow["JouMas_Status"] = master.JouMas_Status;
            newRow["JouMas_IsEnable"] = master.JouMas_IsEnable;
            newRow["JouMas_Memo"] = master.JouMas_Memo;
            newRow["JouMas_VoidReason"] = master.JouMas_VoidReason;
            newRow["JouMas_Source"] = master.JouMas_Source;
            table.Rows.Add(newRow);
            return table;
        }
        private static DataTable CreateJournalDetail(JournalMaster master)
        {
            DataTable table = new DataTable();
            DataColumn dc1 = new DataColumn("JouDet_ID", typeof(string));
            DataColumn dc2 = new DataColumn("JouDet_Type", typeof(string));
            DataColumn dc3 = new DataColumn("JouDet_Number", typeof(int));
            DataColumn dc4 = new DataColumn("JouDet_AcctLvl1", typeof(string));
            DataColumn dc5 = new DataColumn("JouDet_AcctLvl2", typeof(string));
            DataColumn dc6 = new DataColumn("JouDet_AcctLvl3", typeof(string));
            DataColumn dc7 = new DataColumn("JouDet_Amount", typeof(float));
            DataColumn dc8 = new DataColumn("JouDet_Memo", typeof(string));
            DataColumn dc9 = new DataColumn("JouDet_Source", typeof(string));
            DataColumn dc10 = new DataColumn("JouDet_SourceID", typeof(string));
            DataColumn dc11 = new DataColumn("JouDet_WriteOffID", typeof(string));
            DataColumn dc12 = new DataColumn("JouDet_WriteOffNumber", typeof(int));
            table.Columns.Add(dc1);
            table.Columns.Add(dc2);
            table.Columns.Add(dc3);
            table.Columns.Add(dc4);
            table.Columns.Add(dc5);
            table.Columns.Add(dc6);
            table.Columns.Add(dc7);
            table.Columns.Add(dc8);
            table.Columns.Add(dc9);
            table.Columns.Add(dc10);
            table.Columns.Add(dc11);
            table.Columns.Add(dc12);
            JournalDetails[] details = new JournalDetails[] { master.DebitDetails, master.CreditDetails };
            foreach (JournalDetails items in details)
            {
                foreach (JournalDetail item in items)
                {
                    DataRow newRow = table.NewRow();
                    if (!string.IsNullOrEmpty(item.JouDet_ID))
                    {
                        newRow["JouDet_ID"] = string.IsNullOrEmpty(item.JouDet_ID) ? string.Empty : item.JouDet_ID;
                        newRow["JouDet_Type"] = string.IsNullOrEmpty(item.JouDet_Type) ? string.Empty : item.JouDet_Type;
                        newRow["JouDet_Number"] = item.JouDet_Number;
                        if (item.Account != null)
                        {
                            newRow["JouDet_AcctLvl1"] = string.IsNullOrEmpty(Convert.ToString(item.Account.acctLevel1)) ? string.Empty : Convert.ToString(item.Account.acctLevel1);
                            newRow["JouDet_AcctLvl2"] = string.IsNullOrEmpty(Convert.ToString(item.Account.acctLevel2)) ? string.Empty : Convert.ToString(item.Account.acctLevel2).PadLeft(4, '0');
                            newRow["JouDet_AcctLvl3"] = string.IsNullOrEmpty(Convert.ToString(item.Account.acctLevel3)) ? string.Empty : Convert.ToString(item.Account.acctLevel3).PadLeft(4, '0');
                        }
                        else
                        {
                            newRow["JouDet_AcctLvl1"] = string.Empty;
                            newRow["JouDet_AcctLvl2"] = string.Empty;
                            newRow["JouDet_AcctLvl3"] = string.Empty;
                        }
                        newRow["JouDet_Amount"] = item.JouDet_Amount;
                        newRow["JouDet_Memo"] = string.IsNullOrEmpty(item.JouDet_Memo) ? string.Empty : item.JouDet_Memo;
                        newRow["JouDet_Source"] = string.IsNullOrEmpty(item.JouDet_Source) ? string.Empty : item.JouDet_Source;
                        newRow["JouDet_SourceID"] = string.IsNullOrEmpty(item.JouDet_SourceID) ? string.Empty : item.JouDet_SourceID;
                        newRow["JouDet_WriteOffID"] = string.IsNullOrEmpty(item.JouDet_WriteOffID) ? string.Empty : item.JouDet_WriteOffID;
                        newRow["JouDet_WriteOffNumber"] = item.JouDet_WriteOffNumber;
                        table.Rows.Add(newRow);
                    }
                }
            }
            return table;
        }
        public static DataTable GetSourceData(JournalDetail detail)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DetType", detail.JouDet_Type));
            parameters.Add(new SqlParameter("DetAcctLvl1", Convert.ToString(detail.Account.acctLevel1).PadLeft(1, '0')));
            parameters.Add(new SqlParameter("DetAcctLvl2", Convert.ToString(detail.Account.acctLevel2).PadLeft(4, '0')));
            parameters.Add(new SqlParameter("DetAcctLvl3", Convert.ToString(detail.Account.acctLevel3).PadLeft(4, '0')));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[JournalWriteOff]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
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