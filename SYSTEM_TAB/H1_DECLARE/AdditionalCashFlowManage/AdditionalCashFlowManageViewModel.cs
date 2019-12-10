using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using His_Pos.Class;
using His_Pos.NewClass.Report.CashFlow;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecords;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage
{
    public class AdditionalCashFlowManageViewModel : TabBase {
        public override TabBase getTab() {
            return this;
        }
        public List<CashFlowAccount> CashFlowAccountsSource => new List<CashFlowAccount> {new CashFlowAccount(CashFlowType.Expenses, "雜支"),new CashFlowAccount(CashFlowType.Income, "額外收入") };

        private List<CashFlowAccount> cashFlowAccounts;
        public List<CashFlowAccount> CashFlowAccounts
        {
            get => cashFlowAccounts;
            set
            {
                Set(() => CashFlowAccounts, ref cashFlowAccounts, value);
            }
        }
        private CashFlowAccount selectedCashFlowAccounts;
        public CashFlowAccount SelectedCashFlowAccounts
        {
            get => selectedCashFlowAccounts;
            set
            {
                Set(() => SelectedCashFlowAccounts, ref selectedCashFlowAccounts, value);
            }
        }
        private CashFlowRecords cashFlowRecords;
        public CashFlowRecords CashFlowRecords
        {
            get => cashFlowRecords;
            set
            {
                Set(() => CashFlowRecords, ref cashFlowRecords, value);
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
        private bool payCheck = false;
        public bool PayCheck
        {
            get => payCheck;
            set
            {
                if (value)
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Expenses).ToList();
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
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Income).ToList();
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
        public string CashFlowNote {
            get => cashFlowNote;
            set { Set(() => CashFlowNote,ref cashFlowNote,value); }
        }
        private double cashFlowValue;
        public double CashFlowValue
        {
            get => cashFlowValue;
            set { Set(() => CashFlowValue, ref cashFlowValue, value); }
        }

        #region Commands
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }
        public RelayCommand Search { get; set; }

        #endregion

        public AdditionalCashFlowManageViewModel()
        {
            InitCommand();
        }

        private void InitCommand()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            Search = new RelayCommand(SearchAction);
            
        }

        private void SubmitAction() {
            CashFlowDb.InsertCashFlowRecordDetail(SelectedCashFlowAccounts, CashFlowNote, CashFlowValue);
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
                MessageWindow.ShowMessage("請填寫起始日期",MessageType.ERROR);
                return;
            }
            if (EndDate is null)
            {
                MessageWindow.ShowMessage("請填寫結束日期",MessageType.ERROR);
                return;
            }
            GetCashFlowRecordsByDate();
        }

        public void GetCashFlowRecordsByDate()
        {
            var table = CashFlowDb.GetDataByDate((DateTime)startDate, (DateTime)endDate);
            var tempDetails = new CashFlowRecordDetails();
            foreach (DataRow r in table.Rows)
            {
                tempDetails.Add(new CashFlowRecordDetail(r));
            }
        }
    }
}
