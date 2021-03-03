using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class BankViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region ----- Define Commands -----

        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        public RelayCommand DetailChangeCommand { get; set; }

        #endregion ----- Define Commands -----

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

        private ObservableCollection<AccountsReports> accData;

        public ObservableCollection<AccountsReports> AccData
        {
            get { return accData; }
            set
            {
                if (Equals(value, accData)) return;
                accData = value;
                OnPropertyChanged();
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

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (Equals(value, selectedIndex)) return;
                selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AccountsReports> bank;

        public ObservableCollection<AccountsReports> Bank
        {
            get { return bank; }
            set
            {
                if (Equals(value, bank)) return;
                bank = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private AccountsReports selectBank;

        public AccountsReports SelectBank
        {
            get => selectBank;
            set
            {
                Set(() => SelectBank, ref selectBank, value);
            }
        }

        #endregion ----- Define Variables -----

        public BankViewModel(string ID)
        {
            AccData = new AccountsReport();
            IDClone = ID;
            Init();
            SelectedIndex = -1;
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            DetailChangeCommand = new RelayCommand(DetailChangeAction);

            SelectedIndex = 0;
            if (Selected != null)
            {
                Selected = AccData[0];
            }
            DetailChangeAction();
        }

        private void DetailChangeAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            Bank = new ObservableCollection<AccountsReports>();
            Bank.Add(new AccountsReports("現金", 0, "001001"));
            foreach (DataRow c in result.Rows)
            {
                if (Selected != null && Selected.ID != c["ID"].ToString())
                {
                    Bank.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
                }
            }
        }

        public BankViewModel()
        {
            AccData = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            DetailChangeCommand = new RelayCommand(DetailChangeAction);
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
            SelectedIndex = -1;
            SelectedIndex = 0;
            if (AccData.Count > 1)
            {
                Selected = AccData[0];
            }
            SelectedIndex = 0;
            DetailChangeAction();
            MainWindow.ServerConnection.CloseConnection();
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

            ConfirmWindow cw = new ConfirmWindow("是否新增會計科目?", "確認");
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

        private void StrikeAction(RelayCommand command)
        {
            if (!TransferValueIsValid()) return;

            ConfirmWindow cw = new ConfirmWindow("是否進行科目沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", double.Parse(StrikeValue)));
            parameters.Add(new SqlParameter("TYPE", "0"));
            parameters.Add(new SqlParameter("NOTE", SelectBank.Name));
            parameters.Add(new SqlParameter("TARGET", SelectBank.ID));
            parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            DataTable dataTable = MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByBank]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("轉帳成功", MessageType.SUCCESS);
                Init();
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
            if (Selected == null)
            {
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
            if (SelectBank == null)
            {
                MessageWindow.ShowMessage("請選擇轉帳目標", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}