using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.StoreOrder.ExportOrderRecord;
using His_Pos.Service.ExportService;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.InsertProductWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DomainModel.Enum;
using System.Threading;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class NormalViewModel : ViewModelBase
    {
        #region ----- Define Command -----

        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand DeleteWaitingOrderCommand { get; set; }
        public RelayCommand<string> AddProductByInputCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand CalculateTotalPriceCommand { get; set; }
        public RelayCommand<string> FilterOrderCommand { get; set; }
        public RelayCommand<string> SplitBatchCommand { get; set; }
        public RelayCommand<PurchaseProduct> MergeBatchCommand { get; set; }
        public RelayCommand CloseTabCommand { get; set; }
        public RelayCommand ReturnOrderCalculateReturnAmountCommand { get; set; }
        public RelayCommand ReturnOrderRePurchaseCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand NoSingdeCommand { get; set; }
        public RelayCommand ExportOrderDataCommand { get; set; }
        public RelayCommand<string> RealAmountMouseDoubleClickCommand { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        private bool isBusy;
        private string busyContent;
        private StoreOrder currentStoreOrder;
        public StoreOrders storeOrderCollection;
        private ICollectionView storeOrderCollectionView;
        private string searchString;
        private OrderFilterStatusEnum filterStatus = OrderFilterStatusEnum.ALL;
        private BackgroundWorker initBackgroundWorker;

        private bool isCanDelete;
        public bool IsCanDelete
        {
            get => isCanDelete;
            set { Set(() => IsCanDelete, ref isCanDelete, value); }
        }
        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }

        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
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
            get 
            {
                if (currentStoreOrder != null)
                    IsCanDelete = currentStoreOrder.IsCanDelete;
                return currentStoreOrder; 
            }
            set
            {
                if (CurrentStoreOrder != null && (currentStoreOrder.OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || currentStoreOrder.OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING))
                {
                    MainWindow.ServerConnection.OpenConnection();
                    currentStoreOrder?.SaveOrder();
                    value?.GetOrderProducts();
                    MainWindow.ServerConnection.CloseConnection();
                }
                else
                {
                    value?.GetOrderProducts();
                }

                Set(() => CurrentStoreOrder, ref currentStoreOrder, value);

                if (CurrentStoreOrder != null)
                {
                    CurrentStoreOrder.StoreOrderHistory = new StoreOrderHistorys();
                    CurrentStoreOrder.StoreOrderHistory.getData(CurrentStoreOrder.ID);
                }
            }
        }

        public string SearchString
        {
            get => searchString;
            set { Set(() => SearchString, ref searchString, value); }
        }

        #endregion ----- Define Variables -----

        public NormalViewModel()
        {
            RegisterCommand();
            RegisterMessengers();
        }

        #region ----- Define Actions -----

        private void ReturnOrderRePurchaseAction()
        {
            if (CurrentStoreOrder.CheckOrder())
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                (CurrentStoreOrder as ReturnOrder).ReturnOrderRePurchase();
                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
                storeOrderCollection.ReloadCollection();
                AddOrderByMinus();
            }
        }

        private void ReturnOrderCalculateReturnAmountAction()
        {
            (CurrentStoreOrder as ReturnOrder).CalculateReturnAmount();
        }

        private void DoubleClickRealAmount(string id)
        {
            CurrentStoreOrder.SetRealAmount(id);
        }

        private void CalculateTotalPriceAction()
        {
            CurrentStoreOrder.CalculateTotalPrice();
        }

        public void AddOrderByMinus()
        {
            StoreOrderCollectionView = CollectionViewSource.GetDefaultView(StoreOrders.GetOrdersNotDone());

            CurrentStoreOrder = StoreOrderCollectionView.CurrentItem as StoreOrder;
        }

        private void AddOrderAction()
        {
            AddNewOrderWindow.AddNewOrderWindow addNewOrderWindow = new AddNewOrderWindow.AddNewOrderWindow();
            addNewOrderWindow.ShowDialog();

            AddNewOrderWindowViewModel viewModel = addNewOrderWindow.DataContext as AddNewOrderWindowViewModel;

            if (viewModel.NewStoreOrder != null)
            {
                //storeOrderCollection.Insert(0, viewModel.NewStoreOrder);
                AddOrderByMinus();
                storeOrderCollection = (StoreOrders)StoreOrderCollectionView.SourceCollection;
                string tempId = Convert.ToString(viewModel.NewStoreOrder.ID);
                int count = -1;
                if(!string.IsNullOrEmpty(tempId))
                {
                    foreach (StoreOrder item in storeOrderCollection)
                    {
                        string orderId = Convert.ToString(item.ID);
                        if (!string.IsNullOrEmpty(orderId) && tempId == orderId)
                        {
                            count = storeOrderCollection.IndexOf(item);
                            break;
                        }
                    }
                }
                if(count >= 0 && count < storeOrderCollection.Count)
                {
                    CurrentStoreOrder = storeOrderCollection[count];
                }
                else
                {
                    CurrentStoreOrder = storeOrderCollection[0];
                }
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
                MessageWindow.ShowMessage("已作廢刪除，請通知杏德！", MessageType.SUCCESS);
                //storeOrderCollection.Remove(CurrentStoreOrder);
                AddOrderByMinus();
                CurrentStoreOrder = storeOrderCollection.FirstOrDefault();
            }
        }

        private void DeleteWaitingOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            bool isSuccess = CurrentStoreOrder.DeleteOrder();
            MainWindow.SingdeConnection.CloseConnection();
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
            {
                //storeOrderCollection.Remove(CurrentStoreOrder);
                AddOrderByMinus();
                CurrentStoreOrder = storeOrderCollection.FirstOrDefault();
            }
        }

        private void ToNextStatusAction()
        {
            if (CurrentStoreOrder.CheckOrder() && (CurrentStoreOrder.OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING || CurrentStoreOrder.OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || CurrentStoreOrder.OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || CurrentStoreOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING))
            {
                if ((CurrentStoreOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING || CurrentStoreOrder.OrderStatus == OrderStatusEnum.NORMAL_PROCESSING) && !CurrentStoreOrder.ChkPurchase())
                {
                    MessageWindow.ShowMessage("品項入庫量且小計大於0!", MessageType.WARNING);
                    return;
                }
                if (!CurrentStoreOrder.ChkPrice())
                {
                    ConfirmWindow confirmWindow = new ConfirmWindow($"本次進價與上次進價不同\n是否確認送出進貨單?", "", true);

                    if (!(bool)confirmWindow.DialogResult)
                        return;
                }

                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                CurrentStoreOrder.MoveToNextStatus();
                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
                //storeOrderCollection.ReloadCollection();
                //(20220510)確認收貨之後清空搜尋條件
                //StoreOrder currentOrder = null;
                if (currentStoreOrder.IsDoneOrder)//確認收貨
                {
                    SearchString = string.Empty;
                    //currentOrder = currentStoreOrder;
                    storeOrderCollection = StoreOrders.GetOrdersNotDone();
                    var orderedList = storeOrderCollection.OrderBy(_ => _.ReceiveID.StartsWith("1")).ToList();
                    StoreOrders result = new StoreOrders();
                    for (int i = orderedList.Count() - 1; i >= 0; i--)
                    {
                        result.Add(orderedList[i]);
                    }
                    storeOrderCollection = result;
                    InitData(storeOrderCollection);
                    StoreOrderCollectionView.MoveCurrentToFirst();
                }
            }
            else
            {
                return;
            }
        }

        private void ReloadAction()
        {
            InitVariables();
        }

        private void AddProductByInputAction(string searchString)
        {
            if (CurrentStoreOrder.SelectedItem != null && CurrentStoreOrder.SelectedItem.ID.Equals(searchString)) return;

            if (searchString.Length < 0)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            AddProductEnum addProductEnum = CurrentStoreOrder.OrderType == OrderTypeEnum.PURCHASE ? AddProductEnum.ProductPurchase : AddProductEnum.ProductReturn;

            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(searchString, addProductEnum, CurrentStoreOrder.OrderWarehouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum, CurrentStoreOrder.OrderStatus, CurrentStoreOrder.OrderWarehouse.ID, CurrentStoreOrder.OrderTypeIsOTC);
                productPurchaseReturnAddProductWindow.ShowDialog();
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum, CurrentStoreOrder.OrderStatus, CurrentStoreOrder.OrderWarehouse.ID, CurrentStoreOrder.OrderTypeIsOTC);
                Messenger.Default.Unregister(this);
            }
            else if (addProductEnum == AddProductEnum.ProductPurchase && productCount < 1)
            {
                MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
                ConfirmWindow confirmWindow = new ConfirmWindow($"是否跳轉至新增商品?", "", true);

                if (!(bool)confirmWindow.DialogResult)
                    return;
                InsertProductWindow insertProductWindow = new InsertProductWindow();
            }
            else
            {
                MessageWindow.ShowMessage("商品存貨量不足或輸入有誤", MessageType.WARNING);
            }
        }

        private void AddProductAction()
        {
            AddProductEnum addProductEnum = CurrentStoreOrder.OrderType == OrderTypeEnum.PURCHASE ? AddProductEnum.ProductPurchase : AddProductEnum.ProductReturn;

            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProductFromAddButton);
            ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow("", addProductEnum, CurrentStoreOrder.OrderWarehouse.ID);
            productPurchaseReturnAddProductWindow.ShowDialog();
            Messenger.Default.Unregister(this);
        }

        private void DeleteProductAction()
        {
            CurrentStoreOrder.DeleteSelectedProduct();
        }

        private void FilterOrderAction(string filterCondition)
        {
            if (filterCondition != null)
                filterStatus = (OrderFilterStatusEnum)int.Parse(filterCondition);
            StoreOrderCollectionView.Filter += OrderFilter;
        }

        private void SplitBatchAction(string productID)
        {
            (CurrentStoreOrder as PurchaseOrder).SplitBatch(productID);
        }

        private void MergeBatchAction(PurchaseProduct product)
        {
            (CurrentStoreOrder as PurchaseOrder).MergeBatch(product);
        }

        private void CloseTabAction()
        {
            if (CurrentStoreOrder != null)
                CurrentStoreOrder.SaveOrder();
        }

        private void ExportOrderDataAction()
        {
            if (CurrentStoreOrder.OrderStatus == OrderStatusEnum.WAITING)
            {
                MessageWindow.ShowMessage("等待處理訂單資料不齊全 無法匯出!", MessageType.ERROR);
                return;
            }

            IsBusy = true;
            BusyContent = "匯出資料";

            bool isSuccess = false;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                Collection<object> tempCollection = new Collection<object>() { CurrentStoreOrder };

                MainWindow.ServerConnection.OpenConnection();
                CurrentStoreOrder.SaveOrder();
                ExportExcelService service = new ExportExcelService(tempCollection, new ExportOrderRecordTemplate());
                isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\進退貨資料{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
                MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (isSuccess)
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                else
                    MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        public void InitData(StoreOrders storeOrders)
        {
            storeOrderCollection = storeOrders;
            StoreOrderCollectionView = CollectionViewSource.GetDefaultView(storeOrders);
            StoreOrderCollectionView.Filter += OrderFilter;

            if (!StoreOrderCollectionView.IsEmpty)
            {
                StoreOrderCollectionView.MoveCurrentToFirst();
                CurrentStoreOrder = StoreOrderCollectionView.CurrentItem as StoreOrder;
            }
        }

        private void InitVariables(string searchStr = "")
        {
            IsBusy = true;
            SearchString = searchStr;

            if (!initBackgroundWorker.IsBusy)
                initBackgroundWorker.RunWorkerAsync();
        }

        private void RegisterCommand()
        {
            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            DeleteWaitingOrderCommand = new RelayCommand(DeleteWaitingOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);
            NoSingdeCommand = new RelayCommand(NoSingdeAction);
            AddProductByInputCommand = new RelayCommand<string>(AddProductByInputAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            CalculateTotalPriceCommand = new RelayCommand(CalculateTotalPriceAction);
            FilterOrderCommand = new RelayCommand<string>(FilterOrderAction);
            SplitBatchCommand = new RelayCommand<string>(SplitBatchAction);
            MergeBatchCommand = new RelayCommand<PurchaseProduct>(MergeBatchAction);
            CloseTabCommand = new RelayCommand(CloseTabAction);
            ReturnOrderCalculateReturnAmountCommand = new RelayCommand(ReturnOrderCalculateReturnAmountAction);
            ReturnOrderRePurchaseCommand = new RelayCommand(ReturnOrderRePurchaseAction);
            ExportOrderDataCommand = new RelayCommand(ExportOrderDataAction);
            RealAmountMouseDoubleClickCommand = new RelayCommand<string>(DoubleClickRealAmount);
        }

        private void NoSingdeAction()
        {
            if (CurrentStoreOrder.CheckOrder())
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                CurrentStoreOrder.Note += "手動入庫";
                CurrentStoreOrder.MoveToNextStatusNoSingde();
                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
                storeOrderCollection.ReloadCollection();
                AddOrderByMinus();
                StoreOrderCollectionView.MoveCurrentToFirst();
            }
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, ShowOrderDetailByOrderID);
            Messenger.Default.Register<NotificationMessage>(this, ReloadProducts);
        }

        #region ///// Messenger Functions /////

        private void GetSelectedProduct(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductPurchaseReturnViewModel))
            {
                MainWindow.ServerConnection.OpenConnection();
                CurrentStoreOrder.AddProductByID(notificationMessage.Content.ID, false);
                MainWindow.ServerConnection.CloseConnection();
            }
        }

        private void GetSelectedProductFromAddButton(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductPurchaseReturnViewModel))
            {
                MainWindow.ServerConnection.OpenConnection();
                CurrentStoreOrder.AddProductByID(notificationMessage.Content.ID, true);
                MainWindow.ServerConnection.CloseConnection();
            }
        }

        private void ShowOrderDetailByOrderID(NotificationMessage<string> notificationMessage)
        {
            if (notificationMessage.Target == this)
            {
                MainWindow.Instance.AddNewTab("");

                InitVariables(notificationMessage.Content);
            }
        }

        private void ReloadProducts(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "UpdateUsableAmountMessage")
            {
                MainWindow.ServerConnection.OpenConnection();
                CurrentStoreOrder.SaveOrder();
                CurrentStoreOrder.GetOrderProducts();
                MainWindow.ServerConnection.CloseConnection();
            }
        }

        #endregion ///// Messenger Functions /////

        #region ///// Filter Functions /////

        private bool OrderFilter(object order)
        {
            bool returnValue = true;

            StoreOrder tempOrder = order as StoreOrder;

            if (!string.IsNullOrEmpty(SearchString))
            {
                returnValue = false;

                //Order ID Filter
                if (tempOrder.ReceiveID is null && tempOrder.ID.Contains(SearchString))
                    returnValue = true;
                else if (tempOrder.ReceiveID != null && tempOrder.ReceiveID.Contains(SearchString))
                    returnValue = true;

                //Order Note Filter
                if (tempOrder.Note != null && tempOrder.Note.Contains(SearchString))
                    returnValue = true;

                //Order Customer Filter
                if (tempOrder is PurchaseOrder && !string.IsNullOrEmpty((tempOrder as PurchaseOrder).PatientData) && (tempOrder as PurchaseOrder).PatientData.Contains(SearchString))
                    returnValue = true;

                //預定客戶
                if (string.IsNullOrEmpty(tempOrder.TargetPreOrderCustomer)  == false && tempOrder.TargetPreOrderCustomer.Contains(SearchString))
                    returnValue = true;

                //慢箋預約
                if (tempOrder is PurchaseOrder)
                {
                    if(string.IsNullOrEmpty((tempOrder as PurchaseOrder).PreOrderCustomer) == false && (tempOrder as PurchaseOrder).PreOrderCustomer.Contains(SearchString))
                        returnValue = true;
                }
                   

                //採購人
                if (string.IsNullOrEmpty(tempOrder.OrderEmployeeName) == false && tempOrder.OrderEmployeeName.Contains(SearchString))
                    returnValue = true;
                

                //Order Product ID Name Note Filter
                //if (tempOrder is PurchaseOrder && (tempOrder as PurchaseOrder).OrderProducts != null )
                //{
                //    foreach (var product in (tempOrder as PurchaseOrder).OrderProducts)
                //        if (product.Note != null && product.Note.Contains(SearchString))
                //        {
                //            returnValue = true;
                //            break;
                //        }
                //        else if (product.ID.ToUpper().Contains(SearchString.ToUpper()) || product.ChineseName.ToUpper().Contains(SearchString.ToUpper()) || product.EnglishName.ToUpper().Contains(SearchString.ToUpper()))
                //        {
                //            returnValue = true;
                //            break;
                //        }
                //}
                //else if (tempOrder is ReturnOrder && (tempOrder as ReturnOrder).ReturnProducts != null)
                //{
                //    foreach (var product in (tempOrder as ReturnOrder).ReturnProducts)
                //        if (product.Note != null && product.Note.Contains(SearchString))
                //        {
                //            returnValue = true;
                //            break;
                //        }
                //        else if (product.ID.ToUpper().Contains(SearchString.ToUpper()) || product.ChineseName.ToUpper().Contains(SearchString.ToUpper()) || product.EnglishName.ToUpper().Contains(SearchString.ToUpper()))
                //        {
                //            returnValue = true;
                //            break;
                //        }
                //}
            }

            //Order Status Filter
            switch (filterStatus)
            {
                case OrderFilterStatusEnum.ALL:
                    break;

                case OrderFilterStatusEnum.UNPROCESSING:
                    if (!(tempOrder.OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING))
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.WAITING:
                    if (tempOrder.OrderStatus != OrderStatusEnum.WAITING)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.PROCESSING:
                    if (!(tempOrder.OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING))
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.OTC:
                    if (!(tempOrder.OrderTypeIsOTC == "OTC"))
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.MED:
                    if (!(tempOrder.OrderTypeIsOTC == "藥品"))
                        returnValue = false;
                    break;
            }

            return returnValue;
        }

        #endregion ///// Filter Functions /////

        #endregion ----- Define Functions -----
    }
}