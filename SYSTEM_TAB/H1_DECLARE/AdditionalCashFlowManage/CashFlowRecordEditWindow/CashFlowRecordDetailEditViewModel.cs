﻿using System;
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

        private List<CashFlowAccount> CashFlowAccountsSource => new List<CashFlowAccount> { new CashFlowAccount(CashFlowType.Expenses, "雜支",0), new CashFlowAccount(CashFlowType.Income, "額外收入",1) };

        private List<CashFlowAccount> cashFlowAccounts;
        public List<CashFlowAccount> CashFlowAccounts
        {
            get => cashFlowAccounts;
            set
            {
                Set(() => CashFlowAccounts, ref cashFlowAccounts, value);
            }
        }
        private CashFlowAccount selectedCashFlowAccount;
        public CashFlowAccount SelectedCashFlowAccount
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
            ExpensesCheck = selectedDetail.CashFlowValue < 0;
            IncomeCheck = !ExpensesCheck;
            var type = selectedDetail.CashFlowValue >= 0 ? "收入" : "支出";
            OriginContent =
                $"類別 : {type}  科目 : {selectedDetail.Name}  金額 : {Math.Abs(selectedDetail.CashFlowValue)} \n備註 : {selectedDetail.Note}  登錄時間 : {DateTimeExtensions.ConvertToTaiwanCalenderWithTime(selectedDetail.Date,true)} 登錄人 : {selectedDetail.EmpName}";
            SelectedCashFlowAccount = CashFlowAccounts.Single(acc => acc.AccountName.Equals(selectedDetail.Name));
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
            CashFlowDb.UpdateCashFlowRecordDetail(SelectedCashFlowAccount, EditedCashFlowRecord);
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CashFlowRecordDetailEditSubmit"));
        }
    }
}
