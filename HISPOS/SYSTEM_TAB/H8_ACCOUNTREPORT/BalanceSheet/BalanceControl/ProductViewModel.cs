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
using System;
using His_Pos.Database;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class ProductViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string IDClone;
        public DateTime EndDate;
        public ProductViewModel(string ID, DateTime endDate)
        {
            AccData = new AccountsReport();
            IDClone = ID;
            EndDate = endDate;
            Init();
            DetailChangeCommand = new RelayCommand(DetailChangeAction);

            SelectedIndex = 0;
            if (Selected != null)
            {
                Selected = AccData[0];
            }
            DetailChangeAction();
        }
        public void Init()
        {
            AccData = new AccountsReport();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            parameters.Add(new SqlParameter("edate", EndDate));
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
        public ProductViewModel()
        {
            AccData = new AccountsReport();
            DetailChangeCommand = new RelayCommand(DetailChangeAction);
        }
        public RelayCommand DetailChangeCommand { get; set; }
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
        private AccountsReports selected;
        public AccountsReports Selected
        {
            get => selected;
            set
            {
                Set(() => Selected, ref selected, value);
            }
        }
        private void DetailChangeAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", IDClone);
            DataBaseFunction.AddSqlParameter(parameterList, "edate", EndDate);
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameterList);
            MainWindow.ServerConnection.CloseConnection();
            Product = new ObservableCollection<AccountsReports>();
            foreach (DataRow c in result.Rows)
            {
                if (Selected != null && Selected.ID != c["ID"].ToString())
                {
                    Product.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<AccountsReports> _product;
        public ObservableCollection<AccountsReports> Product
        {
            get { return _product; }
            set
            {
                if (Equals(value, _product)) return;
                _product = value;
                OnPropertyChanged();
            }
        }
    }
}
