using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class TransferViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand<RelayCommand> StrikeCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string transferValue;
        private string target;

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

        private List<AccountsReports> bank;

        public List<AccountsReports> Bank
        {
            get { return bank; }
            set
            {
                bank = value;
                RaisePropertyChanged(nameof(Bank));
            }
        }

        private AccountsReports selectBank;

        public AccountsReports SelectBank
        {
            get { return selectBank; }
            set
            {
                selectBank = value;
                RaisePropertyChanged(nameof(SelectBank));
            }
        }

        private CollectionViewSource bankCollectionViewSource;

        private CollectionViewSource BankCollectionViewSource
        {
            get => bankCollectionViewSource;
            set { Set(() => BankCollectionViewSource, ref bankCollectionViewSource, value); }
        }

        private ICollectionView bankCollectionView;

        public ICollectionView BankCollectionView
        {
            get => bankCollectionView;
            private set { Set(() => BankCollectionView, ref bankCollectionView, value); }
        }

        #endregion ----- Define Variables -----

        public TransferViewModel()
        {
            SelectBank = new AccountsReports();
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);

            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            Bank = new List<AccountsReports>();
            foreach (DataRow c in result.Rows)
            {
                Bank.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
            BankCollectionViewSource = new CollectionViewSource { Source = Bank };
            BankCollectionView = BankCollectionViewSource.View;
        }

        #region ----- Define Actions -----

        private void StrikeAction(RelayCommand command)
        {
            if (!TransferValueIsValid()) return;
            ConfirmWindow cw = new ConfirmWindow("是否進行沖帳", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", TransferValue));
            parameters.Add(new SqlParameter("TYPE", "0"));
            parameters.Add(new SqlParameter("NOTE", SelectBank.Name));
            parameters.Add(new SqlParameter("TARGET", SelectBank.ID));
            parameters.Add(new SqlParameter("SOURCE_ID", "001001"));
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

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private bool TransferValueIsValid()
        {
            double temp;
            if (double.TryParse(TransferValue, out temp))
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

        #endregion ----- Define Functions -----
    }
}