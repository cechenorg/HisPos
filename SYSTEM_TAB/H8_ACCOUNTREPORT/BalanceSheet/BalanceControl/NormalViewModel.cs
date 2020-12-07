using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class NormalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand StrikeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string transferValue;
        private int strikeValue;
        private string target;
        public string IDClone;
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

        private AccountsReport accData;
        public AccountsReport AccData
        {
            get => accData;
            set
            {
                Set(() => AccData, ref accData, value);
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
        #endregion
        public NormalViewModel(string ID)
        {
            SelectedType = new List<AccountsReports>();
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            SelectedType = new List<AccountsReports>();
            SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            foreach (DataRow c in result.Rows)
            {
                SelectedType.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
               AccData = new AccountsReport();
            IDClone = ID;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            StrikeCommand = new RelayCommand(StrikeAction);
        }
        public NormalViewModel()
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
            AccData = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
        }
        public void Init()
        {
            AccData = new AccountsReport();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            DataTable Data = new DataTable();
            Data = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameters);
            foreach (DataRow r in Data.Rows)
            {
                AccData.Add(new AccountsReports(r));
            }
            MainWindow.ServerConnection.CloseConnection();
        }
        public void DeleteAction()
        {
            if ( Selected == null) {
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
        public void StrikeAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("錯誤", MessageType.ERROR);
                return;
            }
            if (StrikeValue == 0)
            {
                MessageWindow.ShowMessage("不得為零", MessageType.ERROR);
                return;
            }
            if (SelectedBank == null)
            {
                MessageWindow.ShowMessage("請選擇沖帳目標", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", StrikeValue));
            parameters.Add(new SqlParameter("TYPE", "0"));
            parameters.Add(new SqlParameter("NOTE", Selected.Name)) ;
            parameters.Add(new SqlParameter("TARGET", SelectedBank.ID));
            parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
             MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByAccount]", parameters);
            MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            MainWindow.ServerConnection.CloseConnection();
            StrikeValue = 0;
            Selected = null;
            Init();
        }

    }
}
