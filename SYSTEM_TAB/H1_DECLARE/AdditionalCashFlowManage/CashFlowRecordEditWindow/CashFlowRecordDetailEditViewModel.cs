using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Report.CashFlow;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage.CashFlowRecordEditWindow
{
    public class CashFlowRecordDetailEditViewModel : ViewModelBase
    {
        #region Properties

        private List<string> CashFlowAccountsExpenses => new List<string> { "雜支", "額外收入" };
        private List<string> CashFlowAccountsIncome => new List<string> {"額外收入" };

        private List<string> cashFlowAccounts;
        public List<string> CashFlowAccounts
        {
            get => cashFlowAccounts;
            set
            {
                Set(() => CashFlowAccounts, ref cashFlowAccounts, value);
            }
        }
        private string selectedCashFlowAccount;
        public string SelectedCashFlowAccount
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
                    CashFlowAccounts = CashFlowAccountsExpenses;
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
                    CashFlowAccounts = CashFlowAccountsIncome;
                    SelectedCashFlowAccount = CashFlowAccounts[0];
                }
                Set(() => IncomeCheck, ref incomeCheck, value);
            }
        }

        private CashFlowRecordDetail editedCashFlowRecord;
        public CashFlowRecordDetail EditedCashFlowRecord
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

        #endregion
        #region Commands
        public RelayCommand Cancel { get; set; }
        public RelayCommand Submit { get; set; }

        #endregion
        public CashFlowRecordDetailEditViewModel(CashFlowRecordDetail selectedDetail)
        {
            InitVariables(selectedDetail);
            InitCommands();
        }

        private void InitVariables(CashFlowRecordDetail selectedDetail)
        {
            EditedCashFlowRecord = selectedDetail.DeepCloneViaJson();
            ExpensesCheck = selectedDetail.Value < 0;
            IncomeCheck = !ExpensesCheck;
            var type = selectedDetail.Value >= 0 ? "收入" : "支出";
            OriginContent =
                $"類別 : {type}  科目 : {selectedDetail.Name}  金額 : {Math.Abs(selectedDetail.Value)} \n備註 : {selectedDetail.Note}  登錄時間 : {DateTimeExtensions.ConvertToTaiwanCalenderWithTime(selectedDetail.Date,true)} 登錄人 : {selectedDetail.EmpName}";
            SelectedCashFlowAccount = CashFlowAccounts.Single(acc => acc.Equals(selectedDetail.Name));
            CashFlowValue = decimal.ToInt32(Math.Abs(EditedCashFlowRecord.Value));
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
            var type = IncomeCheck ? CashFlowType.Income : CashFlowType.Expenses;
            var selectedAccount = new CashFlowAccount(type,SelectedCashFlowAccount);
            EditedCashFlowRecord.Value = CashFlowValue;
            EditedCashFlowRecord.Name = SelectedCashFlowAccount;
            MainWindow.ServerConnection.OpenConnection();
            CashFlowDb.UpdateCashFlowRecordDetail(selectedAccount,EditedCashFlowRecord);
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CashFlowRecordDetailEditSubmit"));
        }
    }
}
