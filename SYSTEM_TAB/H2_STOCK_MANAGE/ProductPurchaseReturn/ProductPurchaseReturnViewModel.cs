using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
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
        public RelayCommand<string> AddProductByInputCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand CalculateTotalPriceCommand { get; set; }
        public RelayCommand<string> FilterOrderStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StoreOrder currentStoreOrder;
        private bool isBusy;
        private string busyContent;
        private StoreOrders storeOrderCollection;
        private ICollectionView storeOrderCollectionView;
        private OrderFilterStatusEnum filterStatus = OrderFilterStatusEnum.ALL;

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
        public ICollectionView StoreOrderCollectionView
        {
            get => storeOrderCollectionView;
            set
            {
                Set(() => StoreOrderCollectionView, ref storeOrderCollectionView, value);
            }
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
            RegisterCommand();
        }

        #region ----- Define Actions -----
        private void CalculateTotalPriceAction()
        {
            CurrentStoreOrder.CalculateTotalPrice();
        }
        private void AddOrderAction()
        {
            AddNewOrderWindow.AddNewOrderWindow addNewOrderWindow = new AddNewOrderWindow.AddNewOrderWindow();
            addNewOrderWindow.ShowDialog();

            AddNewOrderWindowViewModel viewModel = addNewOrderWindow.DataContext as AddNewOrderWindowViewModel;

            if (viewModel.NewStoreOrder != null)
            {
                storeOrderCollection.Insert(0, viewModel.NewStoreOrder);

                CurrentStoreOrder = storeOrderCollection[0];
            }
        }
        private void DeleteOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            bool isSuccess = CurrentStoreOrder.DeleteOrder();
            MainWindow.SingdeConnection.CloseConnection();
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
            {
                storeOrderCollection.Remove(CurrentStoreOrder);
                CurrentStoreOrder = storeOrderCollection.FirstOrDefault();
            }
        }
        private void ToNextStatusAction()
        {
            if (CurrentStoreOrder.CheckOrder())
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                CurrentStoreOrder.MoveToNextStatus();
                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
                storeOrderCollection.ReloadCollection();
            }
        }
        private void ReloadAction()
        {
            InitVariables();
        }
        private void AddProductByInputAction(string searchString)
        {
            if (CurrentStoreOrder.SelectedItem != null && CurrentStoreOrder.SelectedItem.ID.Equals(searchString)) return;

            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            AddProductEnum addProductEnum = CurrentStoreOrder.OrderType == OrderTypeEnum.PURCHASE ? AddProductEnum.ProductPurchase : AddProductEnum.ProductReturn;

            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(searchString, addProductEnum);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum);
                productPurchaseReturnAddProductWindow.ShowDialog();
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum);
                Messenger.Default.Unregister(this);
            }
            else
            {
                MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
            }
        }
        private void AddProductAction()
        {
            AddProductEnum addProductEnum = CurrentStoreOrder.OrderType == OrderTypeEnum.PURCHASE ? AddProductEnum.ProductPurchase : AddProductEnum.ProductReturn;

            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow("", addProductEnum);
            productPurchaseReturnAddProductWindow.ShowDialog();
            Messenger.Default.Unregister(this);
        }
        private void DeleteProductAction()
        {
            CurrentStoreOrder.DeleteSelectedProduct();
        }
        private void FilterOrderStatusAction(string filterCondition)
        {
            filterStatus = (OrderFilterStatusEnum)int.Parse(filterCondition);
            StoreOrderCollectionView.Filter += OrderStatusFilter;
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            IsBusy = true;
            
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();

                DataTable dataTable;

                if (MainWindow.SingdeConnection.ConnectionStatus() == ConnectionState.Open)
                {
                    BusyContent = "取得杏德新訂單...";
                    dataTable = StoreOrderDB.GetNewSingdeOrders();
                    if (dataTable.Rows.Count > 0)
                        StoreOrders.AddNewOrdersFromSingde(dataTable);

                    dataTable = StoreOrderDB.GetNewSingdePrescriptionOrders();
                    if (dataTable.Rows.Count > 0)
                        StoreOrders.AddNewPrescriptionOrdersFromSingde(dataTable);
                }

                BusyContent = "取得訂單資料...";
                storeOrderCollection = StoreOrders.GetOrdersNotDone();

                if (MainWindow.SingdeConnection.ConnectionStatus() == ConnectionState.Open)
                {
                    List<StoreOrder> storeOrders = storeOrderCollection.Where(s => s.OrderStatus == OrderStatusEnum.WAITING || s.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING).OrderBy(s => s.CreateDateTime).ToList();
                    string dateTime = DateTime.Now.ToString("yyyyMMdd");

                    if (storeOrders.Count > 0)
                        dateTime = storeOrders[0].CreateDateTime.ToString("yyyyMMdd");

                    BusyContent = "取得杏德訂單最新狀態...";
                    dataTable = StoreOrderDB.GetSingdeOrderNewStatus(dateTime);
                    if (dataTable.Rows.Count > 0)
                    {
                        storeOrderCollection.UpdateSingdeOrderStatus(dataTable);
                        storeOrderCollection = new StoreOrders(storeOrderCollection.Where(s => s.OrderStatus != OrderStatusEnum.SCRAP).ToList());
                    }
                }
                
                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                StoreOrderCollectionView = CollectionViewSource.GetDefaultView(storeOrderCollection);
                StoreOrderCollectionView.Filter += OrderStatusFilter;

                if (!StoreOrderCollectionView.IsEmpty)
                {
                    StoreOrderCollectionView.MoveCurrentToFirst();
                    CurrentStoreOrder = StoreOrderCollectionView.CurrentItem as StoreOrder;
                }

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }
        private void RegisterCommand()
        {
            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);
            ReloadCommand = new RelayCommand(ReloadAction);
            AddProductByInputCommand = new RelayCommand<string>(AddProductByInputAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            CalculateTotalPriceCommand = new RelayCommand(CalculateTotalPriceAction);
            FilterOrderStatusCommand = new RelayCommand<string>(FilterOrderStatusAction);
        }
        
        #region ///// Messenger Functions /////
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

        #region ///// Filter Functions /////
        private bool OrderStatusFilter(object order)
        {
            StoreOrder tempOrder = order as StoreOrder;

            switch (filterStatus)
            {
                case OrderFilterStatusEnum.ALL:
                    return true;
                case OrderFilterStatusEnum.UNPROCESSING:
                    if (tempOrder.OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
                        return true;
                    else
                        return false;
                case OrderFilterStatusEnum.WAITING:
                    if (tempOrder.OrderStatus == OrderStatusEnum.WAITING)
                        return true;
                    else
                        return false;
                case OrderFilterStatusEnum.PROCESSING:
                    if (tempOrder.OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING)
                        return true;
                    else
                        return false;
            }

            return false;
        }
        #endregion

        #endregion
    }
}
