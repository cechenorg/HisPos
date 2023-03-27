using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.BalanceSheet;
using System.Linq;
using System.Windows;
using His_Pos.NewClass.Accounts;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class NormalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }

        public RelayCommand DetailChangeCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeFinalCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string transferValue;
        private int strikeValue;
        private string target;
        public string IDClone;
        public DateTime EndDate;
        public DataTable acountSettingDB;

        public string SelectClone;
        public string SelectDetailClone;
        public bool IsFirst = true;
        public double MaxValue { get; set; } = 0;
        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                RaisePropertyChanged(nameof(Target));
            }
        }

        public string TransferValue
        {
            get { return transferValue; }
            set
            {
                transferValue = value;
                RaisePropertyChanged(nameof(TransferValue));
            }
        }

        public int StrikeValue
        {
            get { return strikeValue; }
            set
            {
                strikeValue = value;
                RaisePropertyChanged(nameof(StrikeValue));
            }
        }
        public class AccountsLevel
        {
            public AccountsLevel(string acct1, string acct2, string acct3, string acctName, int acctValue)
            {
                AcctLvl1 = acct1;
                AcctLvl2 = acct2;
                AcctLvl3 = acct3;
                AcctName = acctName;
                AcctValue = acctValue;
            }
            public string AcctLvl1 { get; set; }
            public string AcctLvl2 { get; set; }
            public string AcctLvl3 { get; set; }
            public string AcctName { get; set; }
            public int AcctValue { get; set; }
        }


        private AccountsReport accData;

        public AccountsReport AccData
        {
            get => accData;
            set
            {
                Set(() => AccData, ref accData, value);
            }
        }
        private List<AccountsLevel> accLvlData;

        public List<AccountsLevel> AccLvlData
        {
            get => accLvlData;
            set
            {
                Set(() => AccLvlData, ref accLvlData, value);
            }
        }

        private AccountsDetailReport accDataDetail;

        public AccountsDetailReport AccDataDetail
        {
            get => accDataDetail;
            set
            {
                Set(() => AccDataDetail, ref accDataDetail, value);
            }
        }

        private AccountsDetailReports selectedDetail;

        public AccountsDetailReports SelectedDetail
        {
            get => selectedDetail;
            set
            {
                Set(() => SelectedDetail, ref selectedDetail, value);
            }
        }

        private string selectedDetailCopy;

        public string SelectedDetailCopy
        {
            get => selectedDetailCopy;
            set
            {
                Set(() => SelectedDetailCopy, ref selectedDetailCopy, value);
            }
        }

        private AccountsReports selected;

        public AccountsReports Selected
        {
            get => selected;
            set
            {
                Set(() => Selected, ref selected, value);
            }
        }

        private AccountsLevel selectLvlData;
        public AccountsLevel SelectLvlData
        {
            get => selectLvlData;
            set
            {
                Set(() => SelectLvlData, ref selectLvlData, value);
                if (value != null)
                {
                    GetNoStrikeData(value.AcctLvl1, value.AcctLvl2, value.AcctLvl3);
                }
            }
        }

        private string selectedCopy;

        public string SelectedCopy
        {
            get => selectedCopy;
            set
            {
                Set(() => SelectedCopy, ref selectedCopy, value);
            }
        }

        private List<AccountsReports> selectedType;

        public List<AccountsReports> SelectedType
        {
            get => selectedType;
            set
            {
                Set(() => SelectedType, ref selectedType, value);
            }
        }

        private AccountsReports selectedBank;

        public AccountsReports SelectedBank
        {
            get => selectedBank;
            set
            {
                Set(() => SelectedBank, ref selectedBank, value);
            }
        }

        private int selectedindex;

        public int Selectedindex
        {
            get => selectedindex;
            set
            {
                Set(() => Selectedindex, ref selectedindex, value);
            }
        }

        private int selectedDetailindex;

        public int SelectedDetailindex
        {
            get => selectedDetailindex;
            set
            {
                Set(() => SelectedDetailindex, ref selectedDetailindex, value);
            }
        }

        #endregion ----- Define Variables -----
        public NormalViewModel(DataTable table, DataTable acountSetting, string id, DateTime endDate)
        {
            acountSettingDB = acountSetting;
            EndDate = endDate;
            AccLvlData = new List<AccountsLevel>();
            foreach (DataRow dr in table.Rows)
            {
                string acctLevel2 = Convert.ToString(dr["acctLevel2"]);
                if (acctLevel2.Equals(id))
                {
                    string acctLvl1 = Convert.ToString(dr["acctLevel1"]);
                    string acctLvl2 = Convert.ToString(dr["acctLevel2"]);
                    string acctLvl3 = Convert.ToString(dr["acctLevel3"]);
                    string acctName = Convert.ToString(dr["acctName3"]);
                    int acctValue = Convert.ToInt32(dr["acctValue"]);
                    if (acctValue == 0)
                        continue;

                    if (!string.IsNullOrEmpty(acctName))
                    {
                        if (!AccLvlData.Contains(new AccountsLevel(acctLvl1, acctLvl2, acctLvl3, acctName, acctValue)))
                        {
                            AccLvlData.Add(new AccountsLevel(acctLvl1, acctLvl2, acctLvl3, acctName, acctValue));
                        }
                    }
                    else
                    {
                        string acctName2 = Convert.ToString(dr["acctName2"]);
                        if (!AccLvlData.Contains(new AccountsLevel(acctLvl1, acctLvl2, acctLvl3, acctName2, acctValue)))
                        {
                            AccLvlData.Add(new AccountsLevel(acctLvl1, acctLvl2, acctLvl3, acctName2, acctValue));
                        }
                    }
                }
            }
        }
        
        public NormalViewModel(string ID, DateTime endDate)
        {
            AccDataDetail = new AccountsDetailReport();
            SelectedType = new List<AccountsReports>();
            DataTable table = AccountsDb.GetBankByAccountsID();
            SelectedType = new List<AccountsReports>();
            if (ID != "001001")
            {
                SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            }
            foreach (DataRow dr in table.Rows)
            {
                SelectedType.Add(new AccountsReports(dr["Name"].ToString(), 0, dr["ID"].ToString()));
            }
            AccData = new AccountsReport();
            IDClone = ID;
            EndDate = endDate;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);

            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            DetailChangeCommand = new RelayCommand(DetailChangeAction);
            StrikeFinalCommand = new RelayCommand<RelayCommand>(StrikeFinalAction);
        }
        

        private void StrikeFinalAction(RelayCommand command)
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail == null)
            {
                MessageWindow.ShowMessage("請選擇細目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.Name == "20000101" || SelectedDetail.ID == "0")
            {
                MessageWindow.ShowMessage("不得結案該項目", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行結案?", "確認");
            if (!(bool)cw.DialogResult) { return; }


            if (SelectedDetail.StrikeValue != 0)
            {
                PREStrikeAction();
            }


            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Detail", SelectedDetail.ID));
            parameters.Add(new SqlParameter("ID", Selected.ID));
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareClosed]", parameters);
            MessageWindow.ShowMessage("結案成功", MessageType.SUCCESS);
            MainWindow.ServerConnection.CloseConnection();
            StrikeValue = 0;
            DetailChangeAction();
            Init();
            command.Execute(null);
        }

        private void DetailChangeAction()
        {
            if (Selected == null) { return; }

            AccDataDetail = new AccountsDetailReport();
            DataTable table = AccountsDb.GetAccountsDetailDetailReport(Selected.ID);
            if(Selected.ID == "203999")
            {
                table = GetProfit(table);
            }
            
            int index = 0;
            int nowindex = 0;
            foreach (DataRow r in table.Rows)
            {
                if (r["ID"].ToString() == SelectDetailClone)
                {
                    nowindex = index;
                }
                AccDataDetail.Add(new AccountsDetailReports(r));
                index++;
            }
            //SelectedDetailCopy = SelectDetailClone;
            SelectedDetailindex = nowindex;
        }
        
        public NormalViewModel()
        {
            DataTable table = AccountsDb.GetBankByAccountsID();
            SelectedType = new List<AccountsReports>();
            SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            foreach (DataRow dr in table.Rows)
            {
                SelectedType.Add(new AccountsReports(dr["Name"].ToString(), 0, dr["ID"].ToString()));
            }
            AccData = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
        }
        public NormalViewModel(bool undo)
        {

        }
        public void Init()
        {
            AccData = new AccountsReport();
            Selectedindex = -1;
            DataTable Data = AccountsDb.GetAccountsDetail(IDClone, EndDate);
            if(IDClone.Equals("203"))
            {
                Data = GetProfit(Data, false);
            }
            int index = 0;
            int nowindex = 0;
            if (Data.Rows.Count > 0)
            {
                foreach (DataRow r in Data.Rows)
                {
                    AccData.Add(new AccountsReports(r));

                    if (r["ID"].ToString() == SelectClone)
                    {
                        nowindex = index;
                    }
                    index++;
                }
                Selectedindex = nowindex;
                if (IsFirst == true)
                {
                    IsFirst = false;
                    Selected = AccData[0];
                    DetailChangeAction();
                }
            }
        }
        
        public void DeleteAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("錯誤", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", Selected.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[AccountsDetailDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
            Selected = null;
            Init();
        }

        public void InsertAction()
        {
            if (TransferValue == "" || TransferValue == null)
            {
                MessageWindow.ShowMessage("請輸入名稱", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行新增科目?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            parameters.Add(new SqlParameter("NAME", TransferValue));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccounts]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("新增成功", MessageType.SUCCESS);
            TransferValue = "";
            Init();
        }

        public void StrikeAction(RelayCommand command)
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.StrikeValue == 0 || SelectedDetail == null)
            {
                MessageWindow.ShowMessage("不得為零", MessageType.ERROR);
                return;
            }
            if (SelectedBank == null)
            {
                MessageWindow.ShowMessage("請選擇沖帳目標", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行科目沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            //List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            //parameters.Add(new SqlParameter("VALUE", SelectedDetail.StrikeValue));
            //if (SelectedDetail.TransferID != null && SelectedDetail.TransferID != string.Empty)
            //{
            //    parameters.Add(new SqlParameter("TYPE", SelectedDetail.TransferID));
            //}
            //else
            //{
            //    parameters.Add(new SqlParameter("TYPE", SelectedDetail.ID));
            //}
            //parameters.Add(new SqlParameter("NOTE", Selected.Name));
            //parameters.Add(new SqlParameter("TARGET", SelectedBank.ID));
            //parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            //MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByAccount]", parameters);

            string type = (SelectedDetail.TransferID != null && SelectedDetail.TransferID != string.Empty) ? SelectedDetail.TransferID : SelectedDetail.ID;
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(SelectedBank.ID, type, (double)SelectedDetail.StrikeValue, Selected.ID, Selected.Name);

            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            if (SelectedDetail.StrikeValue == SelectedDetail.Value)
            {
            }
            else
            {
                SelectDetailClone = SelectedDetail.ID;
                SelectClone = Selected.ID;
            }
            StrikeValue = 0;
            DetailChangeAction();
            Init();

            command.Execute(null);
        }

        public void PREStrikeAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.StrikeValue == 0 || SelectedDetail == null)
            {
                MessageWindow.ShowMessage("不得為零", MessageType.ERROR);
                return;
            }
            if (SelectedBank == null)
            {
                MessageWindow.ShowMessage("請選擇沖帳目標", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行科目沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            //List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            //parameters.Add(new SqlParameter("VALUE", SelectedDetail.StrikeValue));
            ////parameters.Add(new SqlParameter("TYPE", SelectedDetail.ID));
            //if (SelectedDetail.TransferID != null && SelectedDetail.TransferID != string.Empty)
            //{
            //    parameters.Add(new SqlParameter("TYPE", SelectedDetail.TransferID));
            //}
            //else
            //{
            //    parameters.Add(new SqlParameter("TYPE", SelectedDetail.ID));
            //}
            //parameters.Add(new SqlParameter("NOTE", Selected.Name));
            //parameters.Add(new SqlParameter("TARGET", SelectedBank.ID));
            //parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            //MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByAccount]", parameters);

            string type = (SelectedDetail.TransferID != null && SelectedDetail.TransferID != string.Empty) ? SelectedDetail.TransferID : SelectedDetail.ID;
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(SelectedBank.ID, type, (double)SelectedDetail.StrikeValue, Selected.ID, Selected.Name);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            if (SelectedDetail.StrikeValue == SelectedDetail.Value)
            {
            }
            else
            {
                SelectDetailClone = SelectedDetail.ID;
                SelectClone = Selected.ID;
            }
            StrikeValue = 0;
            DetailChangeAction();
            Init();

        }

        public DataTable tableClone()
        {
            DataTable table = new DataTable();
            DataColumn dcID = new DataColumn("ID", typeof(string));
            table.Columns.Add(dcID);
            DataColumn dcName = new DataColumn("Header", typeof(string));
            table.Columns.Add(dcName);
            DataColumn dcType = new DataColumn("Type", typeof(string));
            table.Columns.Add(dcType);
            DataColumn dcValue = new DataColumn("Value", typeof(decimal));
            table.Columns.Add(dcValue);
            return table;
        }
        /// <summary>
        /// </summary>
        /// <param name="table">沖帳紀錄</param>
        /// <param name="isPeriod">是否本期損益</param>
        /// <returns></returns>
        public DataTable GetProfit(DataTable table, bool isPeriod)
        {
            decimal total = 0;
            int count = 13;
            DateTime dt = new DateTime();
            if(EndDate == dt)
            {
                EndDate = DateTime.Today;
            }
            if (isPeriod)
            {
                DataSet dataSet = AccountsDb.GetIncomeData(EndDate.Year);
                DataRow[] totalIncome = dataSet.Tables[1].Select("MONTH = '總計'");
                DataRow[] totalExpanse = dataSet.Tables[2].Select("MONTH = '總計'");
                DataRow[] totalClosed = dataSet.Tables[3].Select("MONTH = '總計'");
                total = (totalIncome != null && totalIncome.Length > 0 ? Convert.ToDecimal(totalIncome[0][EndDate.Month]) : 0) +
                    (totalExpanse != null && totalExpanse.Length > 0 ? Convert.ToDecimal(totalExpanse[0][EndDate.Month]) : 0) +
                    (totalClosed != null && totalClosed.Length > 0 ? Convert.ToDecimal(totalClosed[0][EndDate.Month]) : 0);
            }
            else
            {
                for (int year = 2021; year <= EndDate.Year; year++)
                {
                    DataSet dataSet = AccountsDb.GetIncomeData(year);
                    DataRow[] totalIncome = dataSet.Tables[1].Select("MONTH = '總計'");
                    DataRow[] totalExpanse = dataSet.Tables[2].Select("MONTH = '總計'");
                    DataRow[] totalClosed = dataSet.Tables[3].Select("MONTH = '總計'");
                    if (year == EndDate.Year)
                        count = EndDate.Month;

                    for (int j = 1; j < count; j++)
                    {
                        total += (totalIncome != null && totalIncome.Length > 0 ? Convert.ToDecimal(totalIncome[0][j]) : 0) +
                            (totalExpanse != null && totalExpanse.Length > 0 ? Convert.ToDecimal(totalExpanse[0][j]) : 0) +
                            (totalClosed != null && totalClosed.Length > 0 ? Convert.ToDecimal(totalClosed[0][j]) : 0);
                    }
                }
            }
            if (isPeriod)
            {
                DataRow row = table.NewRow();
                row["Value"] = total;
                table.Rows.Add(row);
            }
            else if (!isPeriod && table != null && table.Select("ID = '203999'").Length > 0)
            {
                DataRow dr = table.Select("ID = '203999'")[0];
                dr["Value"] = Convert.ToDecimal(dr["Value"]) + total;
            }
            else
            {
                DataRow row = table.NewRow();
                row["ID"] = "203999";
                row["Name"] = "股東權益";
                row["Value"] = total;
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// 203999沖帳明細
        /// </summary>
        /// <param name="tbStrike">沖帳紀錄</param>
        /// <returns></returns>
        public DataTable GetProfit(DataTable tbStrike)
        {
            DataTable table = SetTableStruct();
            int count = 13;
            //List<string> colMonth = new List<string>() {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "AUG", "SEP", "OCT", "NOV", "DEC" };
            for(int year = 2021; year <= DateTime.Today.Year; year++)
            {
                DataSet dataSet = AccountsDb.GetIncomeData(year);//同月份總計相加
                DataRow[] totalIncome = dataSet.Tables[1].Select("MONTH = '總計'");
                DataRow[] totalExpanse = dataSet.Tables[2].Select("MONTH = '總計'");
                DataRow[] totalClosed = dataSet.Tables[3].Select("MONTH = '總計'");
                if(year == DateTime.Today.Year)
                {
                    count = DateTime.Today.Month + 1;
                }
                for(int month = 1; month < count; month++)
                {
                    decimal monTotal = (totalIncome != null && totalIncome.Length > 0 ? Convert.ToDecimal(totalIncome[0][month]) : 0) +
                                       (totalExpanse != null && totalExpanse.Length > 0 ? Convert.ToDecimal(totalExpanse[0][month]) : 0) +
                                       (totalClosed != null && totalClosed.Length > 0 ? Convert.ToDecimal(totalClosed[0][month]) : 0);
                    string sdate = string.Format("{0}-{1}-01", year - 1911, month.ToString().PadLeft(2, '0'));
                    DataRow[] drs = tbStrike.Select(string.Format("Date = '{0}'", sdate));
                    if(drs != null && drs.Length > 0)
                    {
                        foreach(DataRow dr in drs)
                        {
                            decimal strikeValue = Convert.ToDecimal(dr["Value"]);
                            monTotal = monTotal + strikeValue;
                        }
                    }
                    if(monTotal != 0)
                    {
                        DataRow newRow = table.NewRow();
                        newRow["Record_ID"] = sdate;
                        newRow["Date"] = sdate;
                        newRow["Value"] = monTotal;
                        newRow["ID"] = sdate;
                        newRow["StrikeValue"] = 0.00;
                        table.Rows.Add(newRow);
                    }
                }
            }
            return table;
        }
        public DataTable SetTableStruct()
        {
            DataTable table = new DataTable();
            DataColumn dcRecID = new DataColumn("Record_ID", typeof(string));
            DataColumn dcDate = new DataColumn("Date", typeof(string));
            DataColumn dcValue = new DataColumn("Value", typeof(decimal));
            DataColumn dcID = new DataColumn("ID", typeof(string));
            DataColumn dcStrikeValue = new DataColumn("StrikeValue", typeof(decimal));
            table.Columns.Add(dcRecID);
            table.Columns.Add(dcDate);
            table.Columns.Add(dcValue);
            table.Columns.Add(dcID);
            table.Columns.Add(dcStrikeValue);
            return table;
        }
        private void GetNoStrikeData(string acct1, string acct2, string acct3)
        {
            AccDataDetail = new AccountsDetailReport();
            DataRow[] setCollapsed = acountSettingDB.Select("acct_BSDisplayMode = 0");
            //var setCollapsed = acountSettingDB.Select("acct_BSDisplayMode = 0").ToList();
            DataRow[] setMerge = acountSettingDB.Select("acct_BSDisplayMode = 2");

            foreach (DataRow dr in setCollapsed)
            {
                string act1 = Convert.ToString(dr["acct1"]);
                string act2 = Convert.ToString(dr["acct2"]);
                string act3 = Convert.ToString(dr["acct3"]);
                if (act1.Equals(acct1) && act2.Equals(acct2) && act3.Equals(acct3))
                    return;
            }

            List<string> posNum = new List<string>() { "1", "5", "6", "8" };
            string type = posNum.Contains(acct1) ? "C" : "D";
            DataTable firstData = AccountsDb.GetAccountBalFirst(acct1, acct2, acct3, EndDate, type);
            int first = 0;
            DateTime maxDate = new DateTime();
            DataRow drs = firstData.NewRow();
            if (firstData != null && firstData.Rows.Count > 0)
            {
                if (firstData.Rows.Count == 1)
                {
                    maxDate = Convert.ToDateTime(firstData.Rows[0]["AccBal_Date"]);
                    first = Convert.ToInt32(firstData.Rows[0]["AccBal_Amount"]);
                }
                else
                { 
                    foreach (DataRow item in firstData.Rows)
                    {
                        string jouMas_Date = Convert.ToString(item["AccBal_Date"]);
                        int jouDet_Amount = Convert.ToInt32(item["AccBal_Amount"]);
                        if (jouDet_Amount != 0)
                        {
                            AccDataDetail.Add(new AccountsDetailReports(jouMas_Date, jouDet_Amount, string.Empty));
                        }
                    }                
                }
            }
            else
            {
                first = 0;
            }

            DataTable table = AccountsDb.GetSourceDataInLocal(type, acct1, acct2, acct3, EndDate);//可沖帳

            bool isMerge = false;
            foreach (DataRow dr in setMerge)
            {
                string act1 = Convert.ToString(dr["acct1"]);
                string act2 = Convert.ToString(dr["acct2"]);
                string act3 = Convert.ToString(dr["acct3"]);
                if (act1.Equals(acct1) && act2.Equals(acct2) && act3.Equals(acct3))
                {
                    isMerge = true;
                    continue;
                }
            }
            if (isMerge)
            {
                if (first != 0)
                {
                    AccDataDetail.Add(new AccountsDetailReports(maxDate.ToString("yyyy/MM/dd"), first, "期初"));
                }
                foreach (DataRow item in table.Rows)
                {
                    string ym = Convert.ToDateTime(item["JouMas_Date"]).ToString("yyyy/MM");
                    int jouDet_Amount = Convert.ToInt32(item["JouDet_Amount"]);
                    string jouMas_ID = Convert.ToString(item["JouDet_ID"]);
                    if (AccDataDetail.Where(w=>w.Name.Equals(ym)).Count() > 0)
                    {
                        AccDataDetail.Where(w => w.Name.Equals(ym)).First().Value += Convert.ToDecimal(item["JouDet_Amount"]);
                    }
                    else
                    {
                        AccDataDetail.Add(new AccountsDetailReports(ym, jouDet_Amount, string.Empty));
                    }
                }
            }
            else
            {
                if (first != 0)
                {
                    AccDataDetail.Add(new AccountsDetailReports(maxDate.ToString("yyyy/MM/dd"), first, "期初"));
                }
                foreach (DataRow item in table.Rows)
                {
                    string jouMas_Date = Convert.ToDateTime(item["JouMas_Date"]).ToString("yyyy/MM/dd");
                    int jouDet_Amount = Convert.ToInt32(item["JouDet_Amount"]);
                    string jouMas_ID = Convert.ToString(item["JouDet_ID"]);
                    if (jouDet_Amount != 0)
                    {
                        AccDataDetail.Add(new AccountsDetailReports(jouMas_Date, jouDet_Amount, jouMas_ID));
                    }
                }
            }
        }
    }
}