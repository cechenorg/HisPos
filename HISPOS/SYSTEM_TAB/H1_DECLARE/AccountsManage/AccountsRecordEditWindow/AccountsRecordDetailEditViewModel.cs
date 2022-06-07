using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage.AccountsRecordEditWindow
{
    public class AccountsRecordDetailEditViewModel : ViewModelBase
    {
        #region Properties

        private List<AccountsAccount> CashFlowAccountsSource => new List<AccountsAccount> { new AccountsAccount(CashFlowType.Expenses, "雜支", "1"), new AccountsAccount(CashFlowType.Income, "額外收入", "2") };

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
            EditedCashFlowRecord = selectedDetail.DeepCloneViaJson();
            ExpensesCheck = selectedDetail.CashFlowValue < 0;
            IncomeCheck = !ExpensesCheck;
            var type = selectedDetail.CashFlowValue >= 0 ? "收入" : "支出";
            OriginContent = string.Format("類別 : {0}     金額 : {1}    科目 :{2} \n登錄時間 : {3}    立帳日期 : {4}  \n立帳人 : {5}  備註 : {6}", 
                type,
                Math.Abs(selectedDetail.CashFlowValue),
                selectedDetail.Name,
                DateTimeExtensions.ConvertToTaiwanCalenderWithTime(selectedDetail.InsertDate, true),
                DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(selectedDetail.Date),
                selectedDetail.EmpName,
                selectedDetail.Note
                );
            SelectedCashFlowAccount = CashFlowAccounts.SingleOrDefault(acc => acc.AccountName == selectedDetail.Name);
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