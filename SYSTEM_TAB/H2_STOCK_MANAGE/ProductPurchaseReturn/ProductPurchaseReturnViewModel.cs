using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Command -----
        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand<string> AddProductCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StoreOrder currentStoreOrder;
        private StoreOrders storeOrderCollection;

        public StoreOrders StoreOrderCollection
        {
            get { return storeOrderCollection; }
            set { Set(() => StoreOrderCollection, ref storeOrderCollection, value); }
        }
        public StoreOrder CurrentStoreOrder
        {
            get { return currentStoreOrder; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                currentStoreOrder?.SaveOrder();
                value?.GetOrderProducts();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentStoreOrder, ref currentStoreOrder, value);
            }
        }
        #endregion

        public ProductPurchaseReturnViewModel()
        {
            InitVariables();
            RegisterCommend();
            RegisterMessenger();
        }

        ~ProductPurchaseReturnViewModel()
        {
            UnRegisterMessenger();
        }

        #region ----- Define Actions -----
        private void AddOrderAction()
        {
            AddNewOrderWindow.AddNewOrderWindow addNewOrderWindow = new AddNewOrderWindow.AddNewOrderWindow();
            addNewOrderWindow.ShowDialog();

            AddNewOrderWindowViewModel viewModel = addNewOrderWindow.DataContext as AddNewOrderWindowViewModel;

            if (viewModel.NewStoreOrder != null)
            {
                StoreOrderCollection.Insert(0, viewModel.NewStoreOrder);

                CurrentStoreOrder = StoreOrderCollection[0];
            }
        }
        private void DeleteOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            bool isSuccess = CurrentStoreOrder.DeleteOrder();
            MainWindow.ServerConnection.CloseConnection();
            
            if (isSuccess)
                StoreOrderCollection.Remove(CurrentStoreOrder);
        }
        private void ToNextStatusAction()
        {
            if (CurrentStoreOrder.CheckOrder())
            {
                CurrentStoreOrder.MoveToNextStatus();
                StoreOrderCollection.ClearDoneStoreOrder();
            }

        }
        private void ReloadAction()
        {
            InitVariables();
        }
        private void AddProductAction(string searchString)
        {
            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructsBySearchString(searchString).Count;
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString);
                productPurchaseReturnAddProductWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString);
            }
            else
            {
                MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
            }
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            MainWindow.ServerConnection.OpenConnection();
            StoreOrderCollection = StoreOrders.GetOrdersNotDone();
            MainWindow.ServerConnection.CloseConnection();

            if (StoreOrderCollection.Count > 0)
                CurrentStoreOrder = StoreOrderCollection[0];
        }

        private void RegisterCommend()
        {
            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);
            ReloadCommand = new RelayCommand(ReloadAction);
            AddProductCommand = new RelayCommand<string>(AddProductAction);
        }
        
        #region ----- Messenger Functions -----
        private void RegisterMessenger()
        {
            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
        }
        private void UnRegisterMessenger()
        {
            Messenger.Default.Unregister(this);
        }
        private void GetSelectedProduct(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductPurchaseReturnViewModel))
            {
                MainWindow.ServerConnection.OpenConnection();
                CurrentStoreOrder.AddProductByID(notificationMessage.Content.ID);
                MainWindow.ServerConnection.CloseConnection();
            }
        }
        #endregion

        #endregion
    }
}
