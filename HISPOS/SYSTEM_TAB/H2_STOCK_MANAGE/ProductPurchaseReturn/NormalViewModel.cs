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
using System.Collections.Generic;
using System.Data;
using System.Windows.Media;

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
        private OrderFilterStatusEnum filterStatus = OrderFilterStatusEnum.PROCESSING;
        private BackgroundWorker initBackgroundWorker;

        private string btnScrapContent;
        public string BtnScrapContent
        {
            get => btnScrapContent;
            set { Set(() => BtnScrapContent, ref btnScrapContent, value); }
        }
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
        private bool allBtnCheck = true;
        public bool AllBtnCheck
        {
            get => allBtnCheck;
            set { Set(() => AllBtnCheck, ref allBtnCheck, value); }
        }
        
        private bool isAllSelected = false;
        public RelayCommand CheckAllCommand { get; set; }
        public RelayCommand CheckCommand { get; set; }
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set { Set(() => IsAllSelected, ref isAllSelected, value); }
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
                {
                    IsCanDelete = currentStoreOrder.IsCanDelete;
                    BtnScrapContent = (IsCanDelete == true) ? "作廢" : "取消作廢";
                }
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
                if (currentStoreOrder is ReturnOrder)
                {
                    IsAllSelected = false;
                }
            }
        }
        #region 進退貨管理左側(成立訂單、暫存訂單、作廢訂單)IsCheck
        private bool isPROCESSING;
        private bool isUNPROCESSING;
        private bool isSCRAPING;
        public bool IsPROCESSING 
        {
            get { return isPROCESSING; }
            set { Set(() => IsPROCESSING, ref isPROCESSING, value); }
        }
        public bool IsUNPROCESSING
        {
            get { return isUNPROCESSING; }
            set { Set(() => IsUNPROCESSING, ref isUNPROCESSING, value); }
        }
        public bool IsSCRAPING
        {
            get { return isSCRAPING; }
            set { Set(() => IsSCRAPING, ref isSCRAPING, value); }
        }
        #endregion
        public string SearchString
        {
            get => searchString;
            set { Set(() => SearchString, ref searchString, value); }
        }

        #endregion ----- Define Variables -----

        public NormalViewModel()
        {
            IsPROCESSING = true;
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
                SearchString = string.Empty;
                AllBtnCheck = true;
                IsUNPROCESSING = true;
                AddOrderByMinus();
                storeOrderCollection = (StoreOrders)StoreOrderCollectionView.SourceCollection;
                if (filterStatus != OrderFilterStatusEnum.UNPROCESSING)
                    filterStatus = OrderFilterStatusEnum.UNPROCESSING;
                StoreOrderCollectionView.Filter += OrderFilter;
                string tempId = Convert.ToString(viewModel.NewStoreOrder.ID);
                int count = GetOrderIndex(tempId);
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
            bool isSuccess;
            if (CurrentStoreOrder.IsCanDelete)
            {
                isSuccess = CurrentStoreOrder.DeleteOrder();//執行作廢
                if (isSuccess)
                {
                    MessageWindow.ShowMessage("已作廢刪除！", MessageType.SUCCESS);
                    ReloadData();
                }
            }
            else
            {
                isSuccess = CurrentStoreOrder.ReductOrder();//執行取消作廢
                if (isSuccess)
                {
                    MessageWindow.ShowMessage("已取消作廢！", MessageType.SUCCESS);
                    ReloadData();
                }
            }
            MainWindow.SingdeConnection.CloseConnection();
            MainWindow.ServerConnection.CloseConnection();
        }
        public void ReloadData()
        {
            storeOrderCollection = StoreOrders.GetOrdersNotDone();
            InitData(storeOrderCollection);
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
                //(20220510)確認收貨之後清空搜尋條件
                if (currentStoreOrder.IsDoneOrder)//確認收貨
                {
                    ReloadData();
                    SearchString = string.Empty;
                    IsPROCESSING = true;
                    filterStatus = OrderFilterStatusEnum.PROCESSING;
                    StoreOrderCollectionView.Filter += OrderFilter;
                    CurrentStoreOrder = (StoreOrder)StoreOrderCollectionView.CurrentItem;
                }
                else
                {
                    string id = currentStoreOrder.ID;
                    ReloadData();
                    SearchString = string.Empty;
                    IsPROCESSING = true;
                    filterStatus = OrderFilterStatusEnum.PROCESSING;
                    StoreOrderCollectionView.Filter += OrderFilter;
                    int count = GetOrderIndex(id);
                    if (count >= 0 && count < storeOrderCollection.Count)
                    {
                        CurrentStoreOrder = storeOrderCollection[count];
                    }
                    else
                    {
                        CurrentStoreOrder = storeOrderCollection[0];
                    }
                }
            }
            else
            {
                return;
            }
        }

        private int GetOrderIndex(string id)
        {
            int count = -1;
            if (!string.IsNullOrEmpty(id))
            {
                foreach (StoreOrder item in storeOrderCollection)
                {
                    string orderId = Convert.ToString(item.ID);
                    if (!string.IsNullOrEmpty(orderId) && id == orderId)
                    {
                        count = storeOrderCollection.IndexOf(item);
                        break;
                    }
                }
            }
            return count;
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
            if(StoreOrderCollectionView.CanFilter)
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
            if(StoreOrderCollectionView.CanFilter)
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
            CheckAllCommand = new RelayCommand(OnCheckAll);
            CheckCommand = new RelayCommand(OnCheck);
        }
        /// <summary>
        /// 全選
        /// </summary>
        private void OnCheckAll()
        {
            if(currentStoreOrder != null)
            {
                if(currentStoreOrder is ReturnOrder)
                {
                    ReturnOrder returnOrder = (ReturnOrder)currentStoreOrder;
                    if (returnOrder.ReturnProducts != null && returnOrder.ReturnProducts.Count > 0)
                    {
                        foreach (ReturnMedicine item in ((ReturnOrder)currentStoreOrder).ReturnProducts)
                        {
                            item.IsChecked = IsAllSelected;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 明細判斷全選
        /// </summary>
        private void OnCheck()
        {
            if (currentStoreOrder != null)
            {
                if (currentStoreOrder is ReturnOrder)
                {
                    bool undo = true;
                    ReturnOrder returnOrder = (ReturnOrder)currentStoreOrder;
                    if (returnOrder.ReturnProducts != null && returnOrder.ReturnProducts.Count > 0)
                    {
                        foreach (ReturnMedicine item in ((ReturnOrder)currentStoreOrder).ReturnProducts)
                        {
                            if (item.IsChecked == false)
                            {
                                IsAllSelected = false;
                                undo = false;
                                break;
                            }
                        }
                    }
                    IsAllSelected = undo ? true : false;
                }
            }
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
                if(tempOrder.ReceiveID != null && (tempOrder.ID.Contains(SearchString) || tempOrder.ReceiveID.Contains(SearchString)))
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
               
            }
            
            //Order Status Filter
            switch (filterStatus)
            {
                case OrderFilterStatusEnum.ALL:
                    if (tempOrder.IsEnable == false)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.UNPROCESSING:
                    if (!(tempOrder.OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING) || tempOrder.IsEnable == false)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.WAITING:
                    if (tempOrder.OrderStatus != OrderStatusEnum.WAITING)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.PROCESSING:
                    if (!(tempOrder.OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || tempOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING) || tempOrder.IsEnable == false)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.OTC:
                    if (!(tempOrder.OrderTypeIsOTC == "OTC") || tempOrder.IsEnable == false)
                        returnValue = false;
                    break;

                case OrderFilterStatusEnum.MED:
                    if (!(tempOrder.OrderTypeIsOTC == "藥品") || tempOrder.IsEnable == false)
                        returnValue = false;
                    break;
                case OrderFilterStatusEnum.SCRAP://作廢訂單
                    if(tempOrder.IsEnable == true)
                        returnValue = false;
                    break;
            }

            return returnValue;
        }

        #endregion ///// Filter Functions /////

        #endregion ----- Define Functions -----
    }
}