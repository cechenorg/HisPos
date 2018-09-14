using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl;
using His_Pos.Interface;
using His_Pos.InventoryManagement;
using His_Pos.ProductPurchaseRecord;
using His_Pos.Service;
using His_Pos.StockTaking;
using His_Pos.Struct.Product;
using MahApps.Metro.Controls;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// ProductPurchaseView.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class ProductPurchaseView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection;

        public ObservableCollection<StoreOrder> storeOrderCollection;
        public static ProductPurchaseView Instance;
        
        private PurchaseControl purchaseControl = new PurchaseControl();
        private ReturnControl returnControl = new ReturnControl();
        private WaitControl waitControl = new WaitControl();

        public StoreOrder StoreOrderData { get; set; }

        private UserControl currentControl;

        public UserControl CurrentControl
        {
            get
            {
                return currentControl;
            }
            set
            {
                currentControl = value;
                NotifyPropertyChanged("CurrentControl");
            }
        }

        public ObservableCollection<StoreOrder> StoreOrderCollection
        {
            get
            {
                return storeOrderCollection;
            }
            set
            {
                storeOrderCollection = value;
                NotifyPropertyChanged("StoreOrderCollection");
            }
        }

        private OrderType OrderTypeFilterCondition = OrderType.ALL;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public ProductPurchaseView()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            this.Loaded += UserControl1_Loaded;
            StoOrderOverview.SelectedIndex = 0;
            
            InitData();

            purchaseControl.DeleteOrder.Click += DeleteOrder_Click;
            purchaseControl.ConfirmToProcess.Click += ConfirmToProcess_OnClick;
            purchaseControl.Confirm.Click += Confirm_Click;

        }

        private void InitData()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetProductPurchaseData(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }

        void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, CancelEventArgs e)
        {
            if (StoreOrderData != null && StoreOrderData.IsDataChanged)
            {
                SaveOrder();
            }
        }

        private void ShowOrderDetail(object sender, SelectionChangedEventArgs e)
        {
            if (StoreOrderData != null && StoreOrderData.IsDataChanged)
            {
                 SaveOrder();
            }

            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.SelectedItem is null) return;

            StoreOrder storeOrder = (StoreOrder)dataGrid.SelectedItem;
            
            storeOrder.Products = StoreOrderDb.GetStoreOrderCollectionById(storeOrder.Id);

            StoreOrderData = storeOrder;

            SetCurrentControl();
        }

        internal void SetControlProduct(Collection<PurchaseProduct> tempProduct)
        {
            purchaseControl.ProductCollection = tempProduct;
        }

        private void SetCurrentControl()
        {
            switch (StoreOrderData.Category.CategoryName)
            {
                case "進貨":
                    if (StoreOrderData.Type == OrderType.WAITING)
                    {
                        CurrentControl = waitControl;
                        waitControl.SetDataContext(StoreOrderData);
                    }
                    else
                    {
                        CurrentControl = purchaseControl;
                        purchaseControl.SetDataContext(StoreOrderData);
                    }
                    return;
                case "退貨":
                    CurrentControl = returnControl;
                    returnControl.SetDataContext(StoreOrderData);
                    return;
            }
        }

        private void SaveOrder()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            Saving.Visibility = Visibility.Visible;

            StoreOrder saveOrder = StoreOrderData.Clone() as StoreOrder;

            backgroundWorker.DoWork += (s, o) =>
            {
                StoreOrderDb.SaveOrderDetail(saveOrder);
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Saving.Visibility = Visibility.Hidden;
                }));
            };

            backgroundWorker.RunWorkerAsync();

            StoreOrderData.IsDataChanged = false;
        }

        private void ClearOrderDetailData()
        {
            StoreOrderData = null;

            purchaseControl.ClearControl();
            returnControl.ClearControl();
            waitControl.ClearControl();
        }

        private void AddNewOrder(object sender, MouseButtonEventArgs e)
        {
            AddNewOrderDialog addNewOrderDialog = new AddNewOrderDialog(ManufactoryAutoCompleteCollection);

            addNewOrderDialog.ShowDialog();

            if (addNewOrderDialog.ConfirmButtonClicked)
            {
                switch (addNewOrderDialog.AddOrderType)
                {
                    case AddOrderType.ADDALLBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE, addNewOrderDialog.SelectedWareHouse);
                        break;
                    case AddOrderType.ADDBYMANUFACTORY:
                        AddNewOrderByUm(addNewOrderDialog.SelectedWareHouse, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDALLTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC, addNewOrderDialog.SelectedWareHouse);
                        break;
                    case AddOrderType.ADDALLGOODSALES:
                        AddGoodSales();
                        break;
                    case AddOrderType.ADDBYMANUFACTORYBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE, addNewOrderDialog.SelectedWareHouse, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC, addNewOrderDialog.SelectedWareHouse, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYGOODSALES:
                        AddGoodSales(addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.RETURNBYMANUFACTORY:
                        AddReturn(addNewOrderDialog.SelectedWareHouse, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.RETURNBYORDER:
                        AddReturnByOrder(addNewOrderDialog.SelectedOrderId);
                        break;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            OrderTypeFilterCondition = (OrderType)Int16.Parse(radioButton.Tag.ToString());

            if (StoOrderOverview is null) return;
            StoOrderOverview.Items.Filter = OrderTypeFilter;

            if (StoOrderOverview.Items.Count == 0)
            {
                ClearOrderDetailData();
            }

            StoOrderOverview.SelectedIndex = 0;
        }
        private bool OrderTypeFilter(object item)
        {
            if (OrderTypeFilterCondition == OrderType.ALL) return true;

            if (((StoreOrder)item).Type == OrderTypeFilterCondition)
                return true;
            return false;
        }
        

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckNoEmptyData()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認完成處理單?\n(資料內容將不能修改)", MessageType.ONLYMESSAGE);
            confirmWindow.ShowDialog();
            if (!confirmWindow.Confirm) return;

            StoreOrderData.Type = OrderType.DONE;
            StoreOrderData.RecEmp = MainWindow.CurrentUser.Name;
            SaveOrder();
            
            if (StoreOrderData.CheckIfOrderNotComplete())
            {
                confirmWindow = new ConfirmWindow("最後收貨數量少於預訂量, 是否需要將不足部分保留成新訂單?", MessageType.WARNING);
                confirmWindow.ShowDialog();
                
                storeOrderCollection.Remove(StoreOrderData);

                if (confirmWindow.Confirm)
                {
                    StoreOrder storeOrder = new StoreOrder(StoreOrderCategory.PURCHASE, MainWindow.CurrentUser, StoreOrderData.Warehouse, StoreOrderData.Manufactory);
                    storeOrder.Type = OrderType.PROCESSING;

                    List<Product> newOrderProduct = StoreOrderData.Products.Where(p => ((ITrade)p).Amount < ((IProductPurchase)p).OrderAmount).ToList();

                    foreach(var product in newOrderProduct)
                    {
                        ((IProductPurchase)product).OrderAmount -= ((ITrade)product).Amount;
                        ((ITrade)product).Amount = 0;
                        ((IProductPurchase)product).BatchNumber = "";
                        ((IProductPurchase)product).ValidDate = "";
                        ((IProductPurchase)product).Invoice = "";
                        ((IProductPurchase)product).Note = "訂單 " + storeOrder.Id + " 缺貨 待補貨";
                    }
                    
                    int newIndex = storeOrderCollection.Count - 1;

                    for (int x = 0; x < storeOrderCollection.Count; x++)
                    {
                        if (storeOrderCollection[x].type == OrderType.PROCESSING)
                        {
                            newIndex = x;
                            break;
                        }
                    }

                    StoreOrderData = storeOrder;
                    
                    storeOrderCollection.Insert(newIndex, StoreOrderData);
                    StoOrderOverview.SelectedItem = StoreOrderData;
                    StoOrderOverview.ScrollIntoView(StoreOrderData);

                    StoreOrderData.Products = new ObservableCollection<Product>(newOrderProduct);
                    SetCurrentControl();
                    SaveOrder();
                }
            }

            MessageWindow messageWindow = new MessageWindow("處理單已完成, 可前往處方單紀錄查詢!", MessageType.SUCCESS);
            messageWindow.ShowDialog();

            InventoryManagementView.DataChanged = true;
            ProductPurchaseRecordView.DataChanged = true;
            StockTakingView.DataChanged = true;
        }

        private void ConfirmToProcess_OnClick(object sender, RoutedEventArgs e)
        {
            int oldIndex = storeOrderCollection.IndexOf(StoreOrderData);
            int newIndex = storeOrderCollection.Count - 1;

            for (int x = 0; x < storeOrderCollection.Count; x++)
            {
                if (storeOrderCollection[x].type == OrderType.PROCESSING)
                {
                    newIndex = x - 1;
                    break;
                }
            }

            if (!CheckNoEmptyData()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認轉成處理單?\n(部分資訊將不能修改)", MessageType.ONLYMESSAGE);
            confirmWindow.ShowDialog();
            if (!confirmWindow.Confirm) return;

            if (StoreOrderData.Manufactory.Id == "0")
                StoreOrderData.Type = OrderType.WAITING;
            else
                StoreOrderData.Type = OrderType.PROCESSING;

            SaveOrder();

            UpdateOneTheWayAmount();

            storeOrderCollection.Move(oldIndex, newIndex);
            StoOrderOverview.SelectedItem = StoreOrderData;
            StoOrderOverview.ScrollIntoView(StoreOrderData);

            SetCurrentControl();

            if (StoreOrderData.Type == OrderType.WAITING)
                SendStoreOrderToSinde();
        }

        private void UpdateOneTheWayAmount()
        {
            foreach(var product in StoreOrderData.Products)
            {
                PurchaseProduct purchaseProduct = purchaseControl.ProductCollection.Single(p => p.Id == product.Id && p.WarId == StoreOrderData.Warehouse.Id);

                purchaseControl.ProductCollection.Remove(purchaseProduct);

                purchaseProduct.OnTheWayAmount = (Int32.Parse(purchaseProduct.OnTheWayAmount) + ((IProductPurchase)product).OrderAmount).ToString();

                purchaseControl.ProductCollection.Add(purchaseProduct);
            }
        }

        private void SendStoreOrderToSinde()
        {

        }

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    if (StoreOrderData != null && IsChanged)
        //    {
        //        SaveOrder();
        //    }
        //}

        private bool CheckNoEmptyData()
        {
            string errorMessage = StoreOrderData.IsAnyDataEmpty();

            if (errorMessage != String.Empty)
            {
                MessageWindow messageWindow = new MessageWindow(errorMessage, MessageType.ERROR);
                messageWindow.ShowDialog();
                return false;
            }
            return true;
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (StoreOrderData == null) return;
            StoreOrderDb.DeleteOrder(StoreOrderData.Id);
            StoreOrderCollection.Remove(StoreOrderData);

            if (StoOrderOverview.Items.Count == 0)
                ClearOrderDetailData();
            else
                StoOrderOverview.SelectedIndex = 0;
        }
        
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //private string GetCharFromKey(Key key)
        //{
        //    if (key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right) return "0";

        //    int num = (int)key;

        //    if (num > 50)
        //        return (num - 74).ToString();
        //    else
        //        return (num - 34).ToString();
        //}
    }
    
}
