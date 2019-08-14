using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Trade;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDetailControl.FunctionWindows;
using MaterialDesignThemes.Wpf;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    public class ProductTransactionCustomerViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ReturnMoneyCommand { get; set; }
        public RelayCommand GetCustomerCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        public RelayCommand<int> ChangeTabCommand { get; set; }
        public RelayCommand TakeLeavingProductCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private TradeCustomer customer;
        private TradeTabEnum tabEnum = TradeTabEnum.TradeRecord;

        public string SearchString { get; set; }
        public TradeTabEnum TabEnum
        {
            get => tabEnum;
            set { Set(() => TabEnum, ref tabEnum, value); }
        }
        public TradeCustomer Customer
        {
            get => customer;
            set
            {
                Set(() => Customer, ref customer, value);
                RaisePropertyChanged(nameof(HasCustomer));
            }
        }

        public bool HasCustomer => !(Customer is null);
        #endregion

        public ProductTransactionCustomerViewModel()
        {
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ReturnMoneyAction()
        {
            ReturnMoneyWindow returnMoneyWindow = new ReturnMoneyWindow();
            returnMoneyWindow.ShowDialog();
        }
        private void GetCustomerAction()
        {
            GetCustomerWindow getCustomerWindow = new GetCustomerWindow(SearchString);
            getCustomerWindow.ShowDialog();
        }
        private void AddCustomerAction()
        {
            AddNewCustomerWindow addNewCustomerWindow = new AddNewCustomerWindow();
            addNewCustomerWindow.ShowDialog();
        }
        private void ChangeTabAction(int tab)
        {
            TabEnum = (TradeTabEnum)tab;
        }
        private void TakeLeavingProductAction()
        {
            TakeLeavingProductWindow takeLeavingProductWindow = new TakeLeavingProductWindow();
            takeLeavingProductWindow.ShowDialog();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ReturnMoneyCommand = new RelayCommand(ReturnMoneyAction);
            GetCustomerCommand = new RelayCommand(GetCustomerAction);
            AddCustomerCommand = new RelayCommand(AddCustomerAction);
            ChangeTabCommand = new RelayCommand<int>(ChangeTabAction);
            TakeLeavingProductCommand = new RelayCommand(TakeLeavingProductAction);
        }
        internal void Clear()
        {
            Customer = null;
        }

        #region ///// Messenger Functions /////

        #endregion
        
        #endregion
    }
}
