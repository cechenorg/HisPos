using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.CashFlow;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecords;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage
{
    public class AdditionalCashFlowManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private List<CashFlowAccount> CashFlowAccountsSource;

        private List<CashFlowAccount> cashFlowAccounts;

        public List<CashFlowAccount> CashFlowAccounts
        {
            get => cashFlowAccounts;
            set
            {
                Set(() => CashFlowAccounts, ref cashFlowAccounts, value);
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

        private CashFlowAccount selectedCashFlowAccount;

        public CashFlowAccount SelectedCashFlowAccount
        {
            get => selectedCashFlowAccount;
            set
            {
                Set(() => SelectedCashFlowAccount, ref selectedCashFlowAccount, value);
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

        private CashFlowRecords cashFlowRecords;

        public CashFlowRecords CashFlowRecords
        {
            get => cashFlowRecords;
            set
            {
                Set(() => CashFlowRecords, ref cashFlowRecords, value);
            }
        }

        private CashFlowRecord selectedCashFlowRecord;

        public CashFlowRecord SelectedCashFlowRecord
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

        private bool payCheck;

        public bool PayCheck
        {
            get => payCheck;
            set
            {
                if (value)
                {
                    CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Expenses).ToList();
                    SelectedCashFlowAccount = CashFlowAccounts[0];
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
                    SelectedCashFlowAccount = CashFlowAccounts[0];
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
        

        public RelayCommand ToExcel { get; set; }

        #endregion Commands




        public AdditionalCashFlowManageViewModel()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            SelectedType = new List<AccountsReports>();
            SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            foreach (DataRow c in result.Rows)
            {
                SelectedType.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
            InitCommand();

            MainWindow.ServerConnection.OpenConnection();
            DataTable Accu = MainWindow.ServerConnection.ExecuteProc("[Get].[ExpanseByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            CashFlowAccountsSource = new List<CashFlowAccount>();
            foreach (DataRow c in Accu.Rows)
            {
                CashFlowAccountsSource.Add(new CashFlowAccount(CashFlowType.Expenses, c["Name"].ToString(), (int)c["ID"]));
            }
            CashFlowAccountsSource.Add(new CashFlowAccount(CashFlowType.Income, "額外收入", 0));
            InitCommand();

            CashFlowAccounts = CashFlowAccountsSource.Where(acc => acc.Type == CashFlowType.Income).ToList();
            SelectedCashFlowAccount = CashFlowAccounts[0];
            SelectedBank = SelectedType[0];
            CashFlowRecords = new CashFlowRecords();
            StartDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            EndDate = DateTime.Today;
            SearchAction();
        }

        private void InitCommand()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            Search = new RelayCommand(SearchAction);
            EditCashFlowRecord = new RelayCommand(EditCashFlowRecordAction);
            DeleteCashFlowRecord = new RelayCommand(DeleteCashFlowRecordAction);
            ToExcel = new RelayCommand(ToExcelAct);
        }

        private void ToExcelAct()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", EndDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[ExtraMoneyByDateExcel]", parameters);
            MainWindow.ServerConnection.CloseConnection();


            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "額外收支存檔";
            fdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fdlg.Filter = "報表格式|*.csv";
            fdlg.FileName = $"額外收支_{((DateTime)StartDate).ToString("yyyyMMdd")}-{((DateTime)EndDate).ToString("yyyyMMdd")}";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("時間,科目,金額,來源,登錄人,備註");
                        foreach (DataRow order in result.Rows)
                        {
                            file.WriteLine($"{order["CashFlow_Time"].ToString()},{order["CashFlow_Name"].ToString()},{order["CashFlow_Value"].ToString()},{order["Accounts_Name"].ToString()},{order["Emp_Name"].ToString()},{order["CashFlow_Note"].ToString()}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            
        }
        }

        private void SubmitAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行輸入額外收支", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            CashFlowDb.InsertCashFlowRecordDetail(SelectedCashFlowAccount, CashFlowNote, CashFlowValue, SelectedBank.ID);
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
            var editWindow = new CashFlowRecordEditWindow.CashFlowRecordEditWindow(SelectedCashFlowRecord.SelectedDetail);
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
            ConfirmWindow cw = new ConfirmWindow("是否刪除選擇項目", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            CashFlowDb.DeleteCashFlow(SelectedCashFlowRecord.SelectedDetail);
            MainWindow.ServerConnection.CloseConnection();
            SearchAction();
        }

        private void GetCashFlowRecordsByDate()
        {
            MainWindow.ServerConnection.OpenConnection();
            var table = CashFlowDb.GetDataByDate((DateTime)startDate, (DateTime)endDate);
            var tempDetails = new CashFlowRecordDetails();
            foreach (DataRow r in table.Rows)
            {
                tempDetails.Add(new CashFlowRecordDetail(r));
            }
            MainWindow.ServerConnection.CloseConnection();
            GroupCashFlowByDate(tempDetails);
        }

        private void GroupCashFlowByDate(CashFlowRecordDetails tempDetails)
        {
            var result = tempDetails.GroupBy(x => x.Date.Date);
            foreach (var group in result)
            {
                var rec = new CashFlowRecord { Date = @group.Key };
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