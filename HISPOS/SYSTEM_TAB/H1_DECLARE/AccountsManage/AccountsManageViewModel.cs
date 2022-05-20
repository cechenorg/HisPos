using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using His_Pos.NewClass.Report.Accounts.AccountsRecords;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage
{
    public class AccountsManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private DataTable left;

        public DataTable Left
        {
            get => left;
            set
            {
                Set(() => Left, ref left, value);
            }
        }

        private DataTable right;

        public DataTable Right
        {
            get => right;
            set
            {
                Set(() => Right, ref right, value);
            }
        }

        private List<AccountsAccount> CashFlowAccountsSource;

        private List<AccountsAccount> cashFlowAccounts;

        public List<AccountsAccount> CashFlowAccounts
        {
            get => cashFlowAccounts;
            set
            {
                Set(() => CashFlowAccounts, ref cashFlowAccounts, value);
            }
        }

        private AccountsAccount selectedCashFlowAccount;

        public AccountsAccount SelectedCashFlowAccount
        {
            get => selectedCashFlowAccount;
            set
            {
                Set(() => SelectedCashFlowAccount, ref selectedCashFlowAccount, value);
            }
        }

        private AccountsRecords cashFlowRecords;

        public AccountsRecords CashFlowRecords
        {
            get => cashFlowRecords;
            set
            {
                Set(() => CashFlowRecords, ref cashFlowRecords, value);
            }
        }

        private AccountsRecord selectedCashFlowRecord;

        public AccountsRecord SelectedCashFlowRecord
        {
            get => selectedCashFlowRecord;
            set
            {
                Set(() => SelectedCashFlowRecord, ref selectedCashFlowRecord, value);
            }
        }

        private DateTime? startDate;

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? endDate;

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private DateTime? recordDate = DateTime.Today;

        public DateTime? RecordDate
        {
            get => recordDate;
            set
            {
                Set(() => RecordDate, ref recordDate, value);
            }
        }

        private bool payCheck;

        public bool PayCheck
        {
            get => payCheck;
            set
            {
                if (value)
                {
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Expenses).ToList();
                    int count = CashFlowAccounts.Count();
                    if (count == 0)
                    {
                    }
                    else
                    {
                        SelectedCashFlowAccount = CashFlowAccounts[0];
                    }
                }
                Set(() => PayCheck, ref payCheck, value);
            }
        }

        private bool gainCheck = true;

        public bool GainCheck
        {
            get => gainCheck;
            set
            {
                if (value)
                {
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Income).ToList();
                    int count = CashFlowAccounts.Count();
                    if (count == 0)
                    {
                    }
                    else
                    {
                        SelectedCashFlowAccount = CashFlowAccounts[0];
                    }
                }
                Set(() => GainCheck, ref gainCheck, value);
            }
        }

        private string typeName;

        public string TypeName
        {
            get => typeName;
            set { Set(() => TypeName, ref typeName, value); }
        }

        private string cashFlowNote;

        public string CashFlowNote
        {
            get => cashFlowNote;
            set
            {
                Set(() => CashFlowNote, ref cashFlowNote, value);
            }
        }

        private string keyWords;

        public string KeyWords
        {
            get => keyWords;
            set
            {
                Set(() => KeyWords, ref keyWords, value);
            }
        }

        private int cashFlowValue;

        public int CashFlowValue
        {
            get => cashFlowValue;
            set
            {
                Set(() => CashFlowValue, ref cashFlowValue, value);
            }
        }

        #region Commands

        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }
        public RelayCommand Search { get; set; }
        public RelayCommand EditCashFlowRecord { get; set; }
        public RelayCommand DeleteCashFlowRecord { get; set; }

        #endregion Commands

        public AccountsManageViewModel()
        {
            InitCommand();
            InitAccounts();
            CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Income).ToList();
            if (CashFlowAccounts.Count > 0)
            {
                SelectedCashFlowAccount = CashFlowAccounts[0];
            }
            CashFlowRecords = new AccountsRecords();
            StartDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            EndDate = DateTime.Today;
            SearchAction();
        }

        private void InitAccounts()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            Right = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsRight]");
            Left = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsLeft]");
            MainWindow.ServerConnection.CloseConnection();
            CashFlowAccountsSource = new List<AccountsAccount>();
            foreach (DataRow R in Right.Rows)
            {
                CashFlowAccountsSource.Add(new AccountsAccount(CashFlowType.Expenses, R["Accounts_Name"].ToString(), R["Accounts_ID"].ToString()));
            }

            foreach (DataRow L in Left.Rows)
            {
                CashFlowAccountsSource.Add(new AccountsAccount(CashFlowType.Income, L["Accounts_Name"].ToString(), L["Accounts_ID"].ToString()));
            }
        }

        private void InitCommand()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            Search = new RelayCommand(SearchAction);
            EditCashFlowRecord = new RelayCommand(EditCashFlowRecordAction);
            DeleteCashFlowRecord = new RelayCommand(DeleteCashFlowRecordAction);
        }

        private void SubmitAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行輸入科目金額", "確認");
            if (!(bool)cw.DialogResult) { return; }
            if ((DateTime)RecordDate == null)
            {
                MessageWindow.ShowMessage("未填寫立帳日期", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            AccountsDb.InsertCashFlowRecordDetail(SelectedCashFlowAccount, CashFlowNote, CashFlowValue, (DateTime)RecordDate);
            MainWindow.ServerConnection.CloseConnection();
            CashFlowValue = 0;
            CashFlowNote = "";
            EndDate = DateTime.Today;
            SearchAction();
        }

        private void DateMouseDoubleClickAction(MaskedTextBox sender)
        {
            switch (sender.Name)
            {
                case "StartDate":
                    StartDate = DateTime.Today;
                    break;

                case "EndDate":
                    EndDate = DateTime.Today;
                    break;
            }
        }

        private void SearchAction()
        {
            if (StartDate is null)
            {
                MessageWindow.ShowMessage("請填寫起始日期", MessageType.ERROR);
                return;
            }
            if (EndDate is null)
            {
                MessageWindow.ShowMessage("請填寫結束日期", MessageType.ERROR);
                return;
            }
            CashFlowRecords.Clear();
            GetCashFlowRecordsByDate();
        }

        private void EditCashFlowRecordAction()
        {
            var selectedId = SelectedCashFlowRecord.SelectedDetail.ID;
            var editWindow = new AccountsRecordEditWindow.AccountsRecordEditWindow(SelectedCashFlowRecord.SelectedDetail);
            editWindow.ShowDialog();
            var result = editWindow.EditResult;
            if (!result)
                return;
            SearchAction();
            foreach (var rec in CashFlowRecords)
            {
                if (rec.Details.SingleOrDefault(det => det.ID.Equals(selectedId)) is null)
                    continue;
                SelectedCashFlowRecord = rec;
                SelectedCashFlowRecord.SelectedDetail = SelectedCashFlowRecord.Details.Single(det => det.ID.Equals(selectedId));
                break;
            }
        }

        private void DeleteCashFlowRecordAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            ConfirmWindow cw = new ConfirmWindow("是否刪除選擇項目", "確認");
            if (!(bool)cw.DialogResult) { return; }
            DataTable strikeData = AccountsDb.GetStrikeDataById(SelectedCashFlowRecord.SelectedDetail);
            if (strikeData != null && strikeData.Rows.Count > 0)
            {
                MessageWindow.ShowMessage("已沖帳，請先刪除沖帳紀錄", MessageType.WARNING);
                return;
            }
            AccountsDb.DeleteCashFlow(SelectedCashFlowRecord.SelectedDetail);
            MainWindow.ServerConnection.CloseConnection();
            SearchAction();
        }

        private void GetCashFlowRecordsByDate()
        {
            MainWindow.ServerConnection.OpenConnection();
            var table = AccountsDb.GetDataByDate((DateTime)startDate, (DateTime)endDate, keyWords);
            var tempDetails = new AccountsRecordDetails();
            foreach (DataRow r in table.Rows)
            {
                tempDetails.Add(new AccountsRecordDetail(r));
            }
            MainWindow.ServerConnection.CloseConnection();
            GroupCashFlowByDate(tempDetails);
        }

        private void GroupCashFlowByDate(AccountsRecordDetails tempDetails)
        {
            var result = tempDetails.GroupBy(x => x.Date.Date);
            foreach (var group in result)
            {
                var rec = new AccountsRecord { Date = @group.Key };
                foreach (var det in group)
                {
                    rec.Details.Add(det);
                }
                rec.CountTotalValue();
                CashFlowRecords.Add(rec);
            }
        }
    }
}