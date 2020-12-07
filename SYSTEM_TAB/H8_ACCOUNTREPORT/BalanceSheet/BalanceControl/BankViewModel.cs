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
    public class BankViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string transferValue;
        private string strikeValue;
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
        public string StrikeValue
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
        #endregion
        public BankViewModel(string ID)
        {
            AccData = new AccountsReport();
            IDClone = ID;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
        }
        public BankViewModel()
        {
            AccData = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
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
        private void StrikeAction(RelayCommand command)
        {
            if (!TransferValueIsValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", double.Parse(StrikeValue)));
            parameters.Add(new SqlParameter("TYPE", "0"));
            parameters.Add(new SqlParameter("NOTE", "現金"));
            parameters.Add(new SqlParameter("TARGET", "001001"));
            parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            DataTable dataTable = MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByBank]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("轉帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("轉帳失敗", MessageType.ERROR);
            }
            TransferValue = "";
            command.Execute(null);
        }
        private bool TransferValueIsValid()
        {
            double temp;
            if (Selected == null) {
                MessageWindow.ShowMessage("請選擇項目", MessageType.ERROR);
                return false;
            }
            MaxValue = (double)Selected.Value;

         
            if (double.TryParse(StrikeValue, out temp))
            {
                if (temp <= 0)
                {
                    MessageWindow.ShowMessage("轉帳金額不可小於等於0", MessageType.ERROR);
                    return false;
                }

                if (temp > MaxValue)
                {
                    MessageWindow.ShowMessage("轉帳金額超過餘額", MessageType.ERROR);
                    return false;
                }
            }
            else
            {
                MessageWindow.ShowMessage("輸入金額非數字", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}
