using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using His_Pos.NewClass.Product;
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
        public RelayCommand<string> AddProductByInputCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand CalculateTotalPriceCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public TradeEmployee SelectedEmployee { get; set; }
        public Transaction NewTransaction { get; set; }
        #endregion

        public ProductTransactionViewModel()
        {
            CustomerViewModel = new ProductTransactionCustomerViewModel();
            RegisterCommands();
            NewTransaction = new Transaction();
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
        private void AddProductByInputAction(string searchString)
        {
            if (NewTransaction.SelectedItem != null && NewTransaction.SelectedItem.ID.Equals(searchString)) return;

            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            var productCount = 1;
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                
                Messenger.Default.Unregister(this);
            }
            else
            {
                MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
            }
        }
        private void DeleteProductAction()
        {
            NewTransaction.DeleteSelectedProduct();
        }
        private void CalculateTotalPriceAction()
        {
            NewTransaction.CalculateTotalPrice();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ClearTradeCommand = new RelayCommand(ClearTradeAction);
            TradeCommand = new RelayCommand(TradeAction);
            AddProductByInputCommand = new RelayCommand<string>(AddProductByInputAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            CalculateTotalPriceCommand = new RelayCommand(CalculateTotalPriceAction);
        }
        private void ClearAll()
        {
            CustomerViewModel.Clear();
            NewTransaction = new Transaction();
        }

        #region ///// Messenger Functions /////
        private void GetSelectedProduct(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductTransactionViewModel))
            {
                MainWindow.ServerConnection.OpenConnection();
                NewTransaction.AddProductByID(notificationMessage.Content.ID);
                MainWindow.ServerConnection.CloseConnection();
            }
        }
        #endregion

        #endregion
    }
}
