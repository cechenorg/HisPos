using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using Newtonsoft.Json;
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
            parameters.Add(new SqlParameter("DetAcctLvl3", string.IsNullOrEmpty(Convert.ToString(detail.Account.acctLevel3)) ? null : Convert.ToString(detail.Account.acctLevel3).PadLeft(4, '0')));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[JournalWriteOff]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static DataTable GetSourceDataInLocal(DateTime endDate)
        {
            DataTable table = new DataTable();
            string sql = string.Format(@"
                Declare @date date = '{1}'

                SELECT CASE WHEN ISNULL(a3.acct_ID,'') = '' THEN a1.acct_Name + '(' + a1.acct_ID + ')-' + a2.acct_Name + '(' + a2.acct_ID + ')'
                ELSE a1.acct_Name + a2.acct_Name + a3.acct_Name + '(' + a1.acct_ID + '-' + a2.acct_ID + '-' + a3.acct_ID + ')' END AS acctFullName
                , j.*
                FROM (
                    SELECT d.JouDet_AcctLvl1, d.JouDet_AcctLvl2, d.JouDet_AcctLvl3
                    , m.JouMas_Date, d.JouDet_ID, d.JouDet_Number, d.JouDet_SourceID, (d.JouDet_Amount - ISNULL(w.JouDet_Amount,0)) JouDet_Amount
                    FROM [{0}].[dbo].[JournalMaster] m
                    INNER JOIN [{0}].[dbo].[JournalDetail] d on m.JouMas_ID = d.JouDet_ID
                    LEFT JOIN(SELECT JouDet_WriteOffID, JouDet_WriteOffNumber, JouDet_SourceID, SUM(JouDet_Amount) JouDet_Amount
                    FROM [{0}].[dbo].[JournalMaster] wm
                    INNER JOIN [{0}].[dbo].[JournalDetail] wd on wm.JouMas_ID = wd.JouDet_ID
                    WHERE wm.JouMas_Status = 'F'
                    AND wm.JouMas_IsEnable = 1
                    AND wd.JouDet_Type = 'D'
                    AND JouDet_AcctLvl1 IN ('2','3','4','7')
                    AND ISNULL(wd.JouDet_WriteOffID,'') <> '' AND ISNULL(wd.JouDet_WriteOffNumber,0) > 0
                    AND Cast(wm.JouMas_InsertTime as date) <= @date
                GROUP BY JouDet_WriteOffID, JouDet_WriteOffNumber, JouDet_SourceID
                ) w ON d.JouDet_ID = w.JouDet_WriteOffID AND d.JouDet_Number = w.JouDet_WriteOffNumber AND d.JouDet_SourceID = w.JouDet_SourceID
                WHERE JouMas_Status = 'F'
                AND JouMas_IsEnable = 1
                AND JouDet_Type = 'C'
                AND JouDet_AcctLvl1 IN('2','3','4','7')
                AND(d.JouDet_Amount - ISNULL(w.JouDet_Amount,0)) <> 0
                AND Cast(m.JouMas_InsertTime as date) <= @date
                UNION ALL
                SELECT d.JouDet_AcctLvl1,d.JouDet_AcctLvl2,d.JouDet_AcctLvl3
                ,m.JouMas_Date,d.JouDet_ID,d.JouDet_Number,d.JouDet_SourceID,(d.JouDet_Amount - ISNULL(w.JouDet_Amount,0)) JouDet_Amount
                FROM [{0}].[dbo].[JournalMaster] m
                INNER JOIN [{0}].[dbo].[JournalDetail] d on m.JouMas_ID = d.JouDet_ID
                LEFT JOIN (SELECT JouDet_WriteOffID, JouDet_WriteOffNumber, JouDet_SourceID, SUM(JouDet_Amount) JouDet_Amount
                FROM [{0}].[dbo].[JournalMaster] wm
                INNER JOIN [{0}].[dbo].[JournalDetail] wd on wm.JouMas_ID = wd.JouDet_ID
                WHERE wm.JouMas_Status = 'F'
                AND wm.JouMas_IsEnable = 1
                AND wd.JouDet_Type = 'C'
                AND JouDet_AcctLvl1 IN ('1','5','6','8')
                AND ISNULL(wd.JouDet_WriteOffID,'') <> '' AND ISNULL(wd.JouDet_WriteOffNumber,0) > 0
                AND Cast(wm.JouMas_InsertTime as date) <= @date
                GROUP BY JouDet_WriteOffID, JouDet_WriteOffNumber, JouDet_SourceID
                ) w ON d.JouDet_ID = w.JouDet_WriteOffID AND d.JouDet_Number = w.JouDet_WriteOffNumber AND d.JouDet_SourceID = w.JouDet_SourceID
                WHERE JouMas_Status = 'F'
                AND JouMas_IsEnable = 1
                AND JouDet_Type = 'D'
                AND JouDet_AcctLvl1 IN('1','5','6','8')
                AND (d.JouDet_Amount - ISNULL(w.JouDet_Amount,0)) <> 0
                AND Cast(m.JouMas_InsertTime as date) <= @date
                ) j
                LEFT JOIN (SELECT* FROM [HIS_POS_Server].[dbo].[Accounts] WHERE acct_Level = '1') a1 ON j.JouDet_AcctLvl1 = a1.acct_ID
                LEFT JOIN (SELECT* FROM [HIS_POS_Server].[dbo].[Accounts] WHERE acct_Level = '2') a2 ON j.JouDet_AcctLvl1 = a2.acct_PreLevel AND j.JouDet_AcctLvl2 = a2.acct_ID
                LEFT JOIN (SELECT* FROM [HIS_POS_Server].[dbo].[Accounts] WHERE acct_Level = '3') a3 ON j.JouDet_AcctLvl2 = a3.acct_PreLevel AND j.JouDet_AcctLvl3 = a3.acct_ID
                ORDER BY 1,2,3,4", Properties.Settings.Default.SystemSerialNumber, endDate.ToString("yyyy-MM-dd"));
            
            SQLServerConnection.DapperQuery((conn) =>
            {
                var dapper = conn.Query(sql, commandType: CommandType.Text);
                string json = JsonConvert.SerializeObject(dapper);//序列化成JSON
                table = JsonConvert.DeserializeObject<DataTable>(json);//反序列化成DataTable
            });
            return table;
        }

        public static DataTable GetSourceData()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[JournalSourceWriteOff]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static DataTable GetBalanceSheet(DateTime edate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("eDate", edate));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[BalanceSheetNew]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }

        public static DataTable GetJournalToExcel(string jouID)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("jouID", jouID));
            DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[JournalToExcel]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }

        public static IEnumerable<LedgerDetail> GetLedgerData(int ledgerType, DateTime sDate, DateTime eDate, string acct1, string acct2, string acct3, string keyWord)
        {
            IEnumerable<LedgerDetail> result = null;
            string acctID = string.Empty;
            acctID += string.IsNullOrEmpty(acct1) ? string.Empty : acct1;
            acctID += string.IsNullOrEmpty(acct2) ? string.Empty : "-" + acct2;
            acctID += string.IsNullOrEmpty(acct3) ? string.Empty : "-" + acct3;

            keyWord = string.Format("%{0}%", keyWord);
            string sql = string.Format(@"
                DECLARE @CurPha_Name nvarchar(30) = (Select TOP 1 CurPha_Name From [{0}].[SystemInfo].[CurrentPharmacy]);

	            SELECT m.JouMas_Date
	                ,CASE m.JouMas_Source WHEN '1' THEN '傳票作業' WHEN '2' THEN '關班轉入' 
		            WHEN '3' THEN '進退貨轉入' ELSE m.JouMas_Source END AS JouMas_Source
		            ,m.JouMas_ID
	                ,CASE d.JouDet_Type WHEN 'D' THEN '借' WHEN 'C' THEN '貸' 
		            ELSE d.JouDet_Type END AS JouDet_Type
		            ,d.JouDet_Number,d.JouDet_AcctLvl1,d.JouDet_AcctLvl2,d.JouDet_AcctLvl3
		            ,CASE WHEN ISNULL(d.JouDet_AcctLvl3,'') = '' THEN d.JouDet_AcctLvl1 + '-' + d.JouDet_AcctLvl2
		            ELSE d.JouDet_AcctLvl1 + '-' + d.JouDet_AcctLvl2 + '-' + d.JouDet_AcctLvl3 END AS AcctID
		            ,CASE WHEN ISNULL(d.JouDet_AcctLvl3,'') = '' THEN a2.acct_Name
		            ELSE a2.acct_Name + '-' + a3.acct_Name END AS JouDet_AcctName
		            ,d.JouDet_Amount,d.JouDet_Memo,d.JouDet_SourceID,m.JouMas_Memo
		            ,m.JouMas_InsertTime,ei.Emp_Name InsertEmpName,m.JouMas_ModifyTime,em.Emp_Name ModifyEmpName
	            INTO #tempJournal
	            FROM [{0}].[dbo].[JournalMaster] m 
	            INNER JOIN [{0}].[dbo].[JournalDetail] d ON m.JouMas_ID = d.JouDet_ID
	            LEFT JOIN (SELECT * FROM [{0}].[dbo].[Accounts] WHERE acct_Level ='1') a1 ON d.JouDet_AcctLvl1 = a1.acct_ID
	            LEFT JOIN (SELECT * FROM [{0}].[dbo].[Accounts] WHERE acct_Level ='2') a2 ON d.JouDet_AcctLvl1 = a2.acct_PreLevel AND d.JouDet_AcctLvl2 = a2.acct_ID
	            LEFT JOIN (SELECT * FROM [{0}].[dbo].[Accounts] WHERE acct_Level ='3') a3 ON d.JouDet_AcctLvl2 = a3.acct_PreLevel AND d.JouDet_AcctLvl3 = a3.acct_ID
	            LEFT JOIN [{0}].[Employee].[Master] ei ON m.JouMas_InsertEmpID = ei.Emp_ID
	            LEFT JOIN [{0}].[Employee].[Master] em ON m.JouMas_ModifyEmpID = em.Emp_ID
	            WHERE m.JouMas_Date BETWEEN '{1}' AND '{2}'
	                AND m.JouMas_Status = 'F'
	                AND m.JouMas_IsEnable = 1

                ", Properties.Settings.Default.SystemSerialNumber, sDate.ToString("yyyy-MM-dd"), eDate.ToString("yyyy-MM-dd"));

            if (ledgerType == 0)
            {
                sql += string.Format(@"
                    SELECT @CurPha_Name as CurPha_Name,* 
		            FROM #tempJournal
		            WHERE (ISNULL('{0}','') = '' OR AcctID = '{0}')
		                AND (ISNULL('{1}','') = '' 
		                OR JouDet_AcctName LIKE '{1}'
			            OR JouDet_AcctLvl1 LIKE '{1}'
			            OR JouDet_AcctLvl2 LIKE '{1}'
			            OR JouDet_AcctLvl3 LIKE '{1}'
			            OR JouDet_Memo LIKE '{1}'
			            OR JouDet_SourceID LIKE '{1}'
			            OR JouMas_Memo LIKE '{1}'
			            OR InsertEmpName LIKE '{1}'
			            OR ModifyEmpName LIKE '{1}')
		                ORDER BY JouMas_Date,JouMas_ID,JouDet_Type,JouDet_Number

                ", acctID, keyWord);
            }
            if (ledgerType == 1)
            {
                sql += string.Format(@"

                    SELECT @CurPha_Name as CurPha_Name, * FROM 
		            (
			            SELECT JouDet_AcctLvl1,JouDet_AcctLvl2,JouDet_AcctLvl3,JouDet_AcctName
				            ,JouMas_Date,JouMas_Source,JouMas_ID,JouDet_Number,JouDet_Amount AS DAmount,0 AS CAmount,0 AS Balance
				            ,JouDet_Memo,JouDet_SourceID,JouMas_Memo,JouMas_InsertTime,InsertEmpName,JouMas_ModifyTime,ModifyEmpName
			            FROM #tempJournal
			            WHERE JouDet_Type = '借'
			            UNION ALL
			            SELECT JouDet_AcctLvl1,JouDet_AcctLvl2,JouDet_AcctLvl3,JouDet_AcctName
				            ,JouMas_Date,JouMas_Source,JouMas_ID,JouDet_Number,0 AS DAmount,JouDet_Amount AS CAmount,0 AS Balance
				            ,JouDet_Memo,JouDet_SourceID,JouMas_Memo,JouMas_InsertTime,InsertEmpName,JouMas_ModifyTime,ModifyEmpName
			            FROM #tempJournal
			            WHERE JouDet_Type = '貸'
		            ) T
		            WHERE JouDet_AcctLvl1 = '{0}' AND JouDet_AcctLvl2 = '{1}' AND JouDet_AcctLvl3 = '{2}'
		            ORDER BY JouDet_AcctLvl1,JouDet_AcctLvl2,JouDet_AcctLvl3,JouMas_Date,JouMas_ID		 

                ", acct1, acct2, acct3);
            }

            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<LedgerDetail>(sql,
                    commandType: CommandType.Text);
            });
            return result;
        }
        public static int GetLedgerFirst(DateTime sDate, string acct1, string acct2, string acct3)
        {
            int result = 0;
            string sql = string.Format(@"

                DECLARE @sDate date
                DECLARE @AccBalDate date
                DECLARE @AccBalAmount money
                DECLARE @AcctLvl1 varchar(1)
                DECLARE @AcctLvl2 varchar(4)
                DECLARE @AcctLvl3 varchar(4)
                set @sDate = '{1}'
                set @AcctLvl1 = '{2}'
                set @AcctLvl2 = '{3}'
                set @AcctLvl3 = '{4}'
	
                --已結轉會科的金額
                SELECT TOP 1 @AccBalDate=AccBal_Date,@AccBalAmount = AccBal_Amount
                FROM [{0}].[dbo].[AccountsBalance]
                WHERE AccBal_AcctLvl1 = @AcctLvl1
                AND AccBal_AcctLvl2 = @AcctLvl2
                AND AccBal_AcctLvl3 = @AcctLvl3
                AND AccBal_Date <= @sDate
                ORDER BY AccBal_Date DESC

                IF @AccBalDate IS NULL
                BEGIN
	                SELECT TOP 1 @AccBalDate=AccBal_Date,@AccBalAmount = AccBal_Amount
		            FROM [{0}].[dbo].[AccountsBalance]
	                WHERE AccBal_AcctLvl1 = @AcctLvl1
	                AND AccBal_AcctLvl2 = @AcctLvl2
	                AND AccBal_AcctLvl3 = @AcctLvl3
	                ORDER BY AccBal_Date

	                IF @AccBalDate >= @sDate
	                BEGIN
	                SET @AccBalDate = dateadd(d,-1,@sDate)
	                END
                END
                IF @AccBalAmount IS NULL
                BEGIN
	                SET @AccBalAmount = 0
                END

                --結轉後的傳票資料
                /* JouDet_AcctLvl1 為 1.5.6.8 = 期初 + 借 - 貸
                   JouDet_AcctLvl1 為 2.3.4.7 = 期初 + 貸 - 借 */

                SELECT @AccBalAmount = @AccBalAmount + ISNULL(SUM(JouDet_Amount),0)
                FROM (	SELECT JouDet_Type,JouDet_AcctLvl1,JouDet_AcctLvl2,JouDet_AcctLvl3,
                              CASE WHEN (JouDet_AcctLvl1 IN ('1','5','6','8') AND JouDet_Type = 'D')
                                OR (JouDet_AcctLvl1 IN ('2','3','4','7') AND JouDet_Type = 'C') 
					THEN JouDet_Amount ELSE JouDet_Amount*-1 END AS JouDet_Amount
		        FROM [{0}].[dbo].[JournalMaster] m
		        INNER JOIN [{0}].[dbo].[JournalDetail] d ON m.JouMas_ID = d.JouDet_ID
		        WHERE m.JouMas_Date >= @AccBalDate 
		        AND m.JouMas_Date < @sDate --不含查詢當天
		        AND JouMas_Status = 'F'
		        AND JouMas_IsEnable = 1	
		        AND JouDet_AcctLvl1 = @AcctLvl1
		        AND JouDet_AcctLvl2 = @AcctLvl2
		        AND JouDet_AcctLvl3 = @AcctLvl3
                ) j

                SELECT @AccBalAmount

                ", Properties.Settings.Default.SystemSerialNumber, sDate.ToString("yyyy-MM-dd"), acct1, acct2, acct3);

            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.QueryFirst<int>(sql, commandType: CommandType.Text);
            });
            return result;
        }

        public static DataTable GetAccountFirst(string acct1, string acct2, string acct3)
        {
            DataTable table = new DataTable();
            string sql = string.Format(@"
                Select * From [{0}].[dbo].[AccountsBalance]
                Where AccBal_AcctLvl1 = '{1}' and AccBal_AcctLvl2 = '{2}' and AccBal_AcctLvl3 = '{3}'", Properties.Settings.Default.SystemSerialNumber, acct1, acct2, acct3);
            SQLServerConnection.DapperQuery((conn) =>
            {
                var dapper = conn.Query(sql, commandType: CommandType.Text);
                string json = JsonConvert.SerializeObject(dapper);//序列化成JSON
                table = JsonConvert.DeserializeObject<DataTable>(json);//反序列化成DataTable
            });
            return table;
        }
    }
}