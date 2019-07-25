using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Trade;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    public class ProductTransactionCustomerViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ReturnMoneyCommand { get; set; }
        public RelayCommand GetCustomerCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        public RelayCommand ChangeTabCommand { get; set; }
        public RelayCommand TakeLeavingProductCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public TradeCustomer Customer { get; set; }
        public TradeTabEnum TabEnum { get; set; }

        public bool HasCustomer => !(Customer is null);
        #endregion

        public ProductTransactionCustomerViewModel()
        {
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ReturnMoneyAction()
        {

        }
        private void GetCustomerAction()
        {

        }
        private void AddCustomerAction()
        {

        }
        private void ChangeTabAction()
        {

        }
        private void TakeLeavingProductAction()
        {

        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ReturnMoneyCommand = new RelayCommand(ReturnMoneyAction);
            GetCustomerCommand = new RelayCommand(GetCustomerAction);
            AddCustomerCommand = new RelayCommand(AddCustomerAction);
            ChangeTabCommand = new RelayCommand(ChangeTabAction);
            TakeLeavingProductCommand = new RelayCommand(TakeLeavingProductAction);
        }
        internal void Clear()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
