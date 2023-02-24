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
using His_Pos.NewClass.StockValue;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class ProductViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string IDClone;
        public DateTime EndDate;
        public ProductViewModel(string ID, DateTime endDate)
        {
            DetailChangeCommand = new RelayCommand(DetailChangeAction);
            AccData = new AccountsReport();
            IDClone = ID;
            EndDate = endDate;
            //Init();
            if (ID.Equals("006"))
            {
                DataTable table = StockValueDb.GetStockVale(endDate.AddDays(-7), endDate);
                foreach (DataRow dr in table.Rows)
                {
                    AccData.Add(new AccountsReports(dr));
                }
            }
            else
            {
                DataTable table = AccountsDb.GetAccountsDetail(IDClone, EndDate);
                foreach (DataRow dr in table.Rows)
                {
                    AccData.Add(new AccountsReports(dr));
                }
            }
            
            
            SelectedIndex = 0;
            if (Selected != null)
            {
                Selected = AccData[0];
            }
            //DetailChangeAction();
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
            DataTable table = AccountsDb.GetAccountsDetail(IDClone, EndDate);
            Product = new ObservableCollection<AccountsReports>();
            foreach (DataRow dr in table.Rows)
            {
                if (Selected != null && Selected.ID != dr["ID"].ToString())
                {
                    Product.Add(new AccountsReports(dr["Name"].ToString(), 0, dr["ID"].ToString()));
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
