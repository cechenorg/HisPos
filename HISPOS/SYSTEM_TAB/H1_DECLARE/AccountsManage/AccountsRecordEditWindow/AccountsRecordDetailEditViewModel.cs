using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage.AccountsRecordEditWindow
{
    public class AccountsRecordDetailEditViewModel : ViewModelBase
    {
        #region Properties
        private DataTable accounts;

        public DataTable Accounts
        {
            get => accounts;
            set
            {
                Set(() => Accounts, ref accounts, value);
            }
        }
        private List<AccountsAccount> CashFlowAccountsSource = new List<AccountsAccount>();

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

        private string originContent;

        public string OriginContent
        {
            get => originContent;
            set { Set(() => OriginContent, ref originContent, value); }
        }

        private bool expensesCheck;

        public bool ExpensesCheck
        {
            get => expensesCheck;
            set
            {
                if (value)
                {
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Expenses).ToList();
                    SelectedCashFlowAccount = CashFlowAccounts[0];
                }
                Set(() => ExpensesCheck, ref expensesCheck, value);
            }
        }

        private bool incomeCheck;

        public bool IncomeCheck
        {
            get => incomeCheck;
            set
            {
                if (value)
                {
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Income).ToList();
                    SelectedCashFlowAccount = CashFlowAccounts[0];
                }
                Set(() => IncomeCheck, ref incomeCheck, value);
            }
        }

        private AccountsRecordDetail editedCashFlowRecord;

        public AccountsRecordDetail EditedCashFlowRecord
        {
            get => editedCashFlowRecord;
            set
            {
                Set(() => EditedCashFlowRecord, ref editedCashFlowRecord, value);
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

        #endregion Properties

        #region Commands

        public RelayCommand Cancel { get; set; }
        public RelayCommand Submit { get; set; }

        #endregion Commands

        public AccountsRecordDetailEditViewModel(AccountsRecordDetail selectedDetail)
        {
            InitVariables(selectedDetail);
            InitCommands();
        }

        private void InitVariables(AccountsRecordDetail selectedDetail)
        {
            MainWindow.ServerConnection.OpenConnection();
            Accounts = MainWindow.ServerConnection.ExecuteProc("[Get].[Account]");
            MainWindow.ServerConnection.CloseConnection();
            foreach (DataRow dr in Accounts.Rows)
            {
                string accountID = Convert.ToString(dr["Accounts_ID"]);
                string accountName = Convert.ToString(dr["Accounts_Name"]);
                CashFlowType cashFlowType = Convert.ToInt32(dr["CashFlowType"]) == 0 ? CashFlowType.Income : CashFlowType.Expenses;
                AccountsAccount account = new AccountsAccount(cashFlowType, accountName, accountID);
                CashFlowAccountsSource.Add(account);
            }
            EditedCashFlowRecord = selectedDetail.DeepCloneViaJson();
            DataRow[] currentRow = Accounts.Select(string.Format("Accounts_ID = '{0}'", selectedDetail.SubjectID));
            ExpensesCheck = Convert.ToBoolean(currentRow[0]["CashFlowType"]);
            IncomeCheck = !ExpensesCheck;
            var type = IncomeCheck ? "借方" : "貸方";
            OriginContent = string.Format("類別 : {0}     金額 : {1}    科目 :{2} \n登錄時間 : {3}    立帳日期 : {4}  \n立帳人 : {5}  備註 : {6}",
                type,
                Math.Abs(selectedDetail.CashFlowValue),
                selectedDetail.Name,
                DateTimeExtensions.ConvertToTaiwanCalenderWithTime(selectedDetail.InsertDate, true),
                DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(selectedDetail.Date),
                selectedDetail.EmpName,
                selectedDetail.Note
                );
            SelectedCashFlowAccount = CashFlowAccounts.SingleOrDefault(acc => acc.ID == selectedDetail.SubjectID);
            if (SelectedCashFlowAccount == null)
                SelectedCashFlowAccount = CashFlowAccounts.FirstOrDefault();

            CashFlowValue = Math.Abs(EditedCashFlowRecord.CashFlowValue);
        }

        private void InitCommands()
        {
            Cancel = new RelayCommand(CancelAction);
            Submit = new RelayCommand(SubmitAction);
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CashFlowRecordDetailEditCancel"));
        }

        private void SubmitAction()
        {
            EditedCashFlowRecord.Name = SelectedCashFlowAccount.AccountName;
            EditedCashFlowRecord.CashFlowValue = CashFlowValue;
            MainWindow.ServerConnection.OpenConnection();
            AccountsDb.UpdateCashFlowRecordDetail(SelectedCashFlowAccount, EditedCashFlowRecord);
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CashFlowRecordDetailEditSubmit"));
        }
    }
}