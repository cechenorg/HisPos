using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public RelayCommand<TextBox> AddProductByInputCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StoreOrder currentStoreOrder;
        private StoreOrders storeOrderCollection;
        private bool isBusy;
        private string busyContent;

        public bool IsBusy
        {
            get => isBusy;
            set {Set(() => IsBusy, ref isBusy, value);}
        }
        public string BusyContent
        {
            get => busyContent;
            set {Set(() => BusyContent, ref busyContent, value);}
        }
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
            TabName = MainWindow.HisFeatures[1].Functions[1];
            Icon = MainWindow.HisFeatures[1].Icon;
            RegisterCommend();
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
            {
                StoreOrderCollection.Remove(CurrentStoreOrder);
                CurrentStoreOrder = StoreOrderCollection.FirstOrDefault();
            }
        }
        private void ToNextStatusAction()
        {
            if (CurrentStoreOrder.CheckOrder())
            {
                CurrentStoreOrder.MoveToNextStatus();
                StoreOrderCollection.ReloadCollection();
            }
        }
        private void ReloadAction()
        {
            InitVariables();
        }
        private void AddProductByInputAction(TextBox textBox)
        {
            string searchString = textBox.Text;

            if (CurrentStoreOrder.SelectedItem != null && CurrentStoreOrder.SelectedItem.ID.Equals(searchString)) return;

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
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString);
                productPurchaseReturnAddProductWindow.ShowDialog();
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString);
                Messenger.Default.Unregister(this);
            }
            else
            {
                MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
            }

            textBox.Text = "";
        }
        private void AddProductAction()
        {
            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow("");
            productPurchaseReturnAddProductWindow.ShowDialog();
            Messenger.Default.Unregister(this);
        }
        private void DeleteProductAction()
        {
            CurrentStoreOrder.DeleteSelectedProduct();
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            IsBusy = true;

            MainWindow.ServerConnection.OpenConnection();
            BusyContent = "取得訂單資料...";
            StoreOrderCollection = StoreOrders.GetOrdersNotDone();
            if (StoreOrderCollection.Count > 0)
                CurrentStoreOrder = StoreOrderCollection[0];
            MainWindow.ServerConnection.CloseConnection();
            IsBusy = false;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                //MainWindow.ServerConnection.OpenConnection();
                BusyContent = "取得訂單資料...";
                //StoreOrderCollection = StoreOrders.GetOrdersNotDone();

                //List<StoreOrder> storeOrders = StoreOrderCollection.Where(s => s.OrderStatus == OrderStatusEnum.WAITING).OrderBy(s => s.ID).ToList();
                //string dateTime = DateTime.Now.ToShortDateString();

                //if (storeOrders.Count > 0)
                //    dateTime = storeOrders[0].ID.Substring(1, 8);

                //MainWindow.SingdeConnection.OpenConnection();

                //BusyContent = "取得杏德訂單最新狀態...";
                //DataTable dataTable = StoreOrderDB.GetSingdeOrderNewStatus(dateTime);
                //StoreOrderCollection.UpdateSingdeOrderStatus(dataTable);

                //MainWindow.SingdeConnection.CloseConnection();
                //MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (StoreOrderCollection.Count > 0)
                    CurrentStoreOrder = StoreOrderCollection[0];

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }

        private void RegisterCommend()
        {
            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);
            ReloadCommand = new RelayCommand(ReloadAction);
            AddProductByInputCommand = new RelayCommand<TextBox>(AddProductByInputAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            AddProductCommand = new RelayCommand(AddProductAction);
        }
        
        #region ----- Messenger Functions -----
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
