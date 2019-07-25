using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using His_Pos.NewClass.Trade;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    public class ProductTransactionViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----
        public ProductTransactionCustomerViewModel CustomerViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand ClearTradeCommand { get; set; }
        public RelayCommand TradeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public TradeEmployee SelectedEmployee { get; set; }
        public Transaction NewTransaction { get; set; }
        #endregion

        public ProductTransactionViewModel()
        {
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ClearTradeAction()
        {
            ClearAll();
        }
        private void TradeAction()
        {
            bool isSuccess = TradeService.Trade(NewTransaction, CustomerViewModel.Customer, SelectedEmployee);

            if (isSuccess)
            {
                MessageWindow.ShowMessage("交易成功!", MessageType.SUCCESS);
            }
            else
                MessageWindow.ShowMessage("交易失敗 請稍後再試!", MessageType.ERROR);
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ClearTradeCommand = new RelayCommand(ClearTradeAction);
            TradeCommand = new RelayCommand(TradeAction);
        }
        private void ClearAll()
        {
            CustomerViewModel.Clear();
            NewTransaction.Clear();
        }
        #endregion
    }
}
