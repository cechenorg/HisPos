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
        public class NewItemProduct
        {
            public bool IsThisMan { get; }
            public Product Product { get; }

            public NewItemProduct(bool isThisMan, Product product)
            {
                IsThisMan = isThisMan;
                Product = product;
            }
        }

        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection;

        public ObservableCollection<object> Products;

        public Collection<PurchaseProduct> ProductAutoCompleteCollection;

        public ObservableCollection<StoreOrder> storeOrderCollection;
        public static ProductPurchaseView Instance;
        
        private PurchaseControl purchaseControl = new PurchaseControl();
        private ReturnControl returnControl = new ReturnControl();

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
        private bool IsFirst = true;
        private bool IsChanged = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProductPurchaseView()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            this.Loaded += UserControl1_Loaded;
            StoOrderOverview.SelectedIndex = 0;

            CurrentControl = purchaseControl;

            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetProductPurchaseData(this);

            loadingWindow.Show();

        }


        void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, CancelEventArgs e)
        {
            if (StoreOrderData != null && IsChanged)
            {
                SaveOrder();
            }
        }

        private void ShowOrderDetail(object sender, SelectionChangedEventArgs e)
        {
            if (StoreOrderData != null && IsChanged)
            {
                 SaveOrder();
            }

            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.SelectedItem is null) return;

            StoreOrder storeOrder = (StoreOrder)dataGrid.SelectedItem;
            
            if (storeOrder.Products is null)
                storeOrder.Products = StoreOrderDb.GetStoreOrderCollectionById(storeOrder.Id);

            StoreOrderData = storeOrder;

            SetCurrentControl();
        }

        private void SetCurrentControl()
        {
            switch (StoreOrderData.Category.CategoryName)
            {
                case "進貨":
                    CurrentControl = purchaseControl;
                    purchaseControl.SetDataContext(StoreOrderData);
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
        }

        //private void ClearOrderDetailData() {
        //    IsFirst = true;
        //    StoreOrderData = null;
        //    IsChanged = false;
        //    IsFirst = false;
        //}

        private void AddNewOrder(object sender, MouseButtonEventArgs e)
        {
            AddNewOrderDialog addNewOrderDialog = new AddNewOrderDialog(ManufactoryAutoCompleteCollection);

            addNewOrderDialog.ShowDialog();

            if (addNewOrderDialog.ConfirmButtonClicked)
            {
                switch (addNewOrderDialog.AddOrderType)
                {
                    case AddOrderType.ADDALLBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE);
                        break;
                    case AddOrderType.ADDBYMANUFACTORY:
                        AddNewOrderByUm(addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDALLTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC);
                        break;
                    case AddOrderType.ADDALLGOODSALES:
                        AddGoodSales();
                        break;
                    case AddOrderType.ADDBYMANUFACTORYBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC, addNewOrderDialog.SelectedManufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYGOODSALES:
                        AddGoodSales(addNewOrderDialog.SelectedManufactory);
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
                //ClearOrderDetailData();
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



        //private void ProductAuto_Populating(object sender, PopulatingEventArgs e)
        //{
        //    var productAuto = sender as AutoCompleteBox;

        //    if (String.IsNullOrEmpty(storeOrderData.Manufactory.Id) || productAuto is null || Products is null) return;

        //    var result = Products.Where(x => (((NewItemProduct)x).Product.Id.ToLower().Contains(productAuto.Text.ToLower()) || ((NewItemProduct)x).Product.ChiName.ToLower().Contains(productAuto.Text.ToLower()) || ((NewItemProduct)x).Product.EngName.ToLower().Contains(productAuto.Text.ToLower())) && ((IProductPurchase)((NewItemProduct)x).Product).Status).Take(50).Select(x => ((NewItemProduct)x).Product);
        //    ProductAutoCompleteCollection = new ObservableCollection<object>(result.ToList());

        //    productAuto.ItemsSource = ProductAutoCompleteCollection;
        //    productAuto.ItemFilter = ProductFilter;
        //    productAuto.PopulateComplete();
        //}

        

        //private void AutoCompleteBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    var productAuto = sender as AutoCompleteBox;
        //    SetChanged();
        //    if (productAuto is null) return;
        //    if (productAuto.SelectedItem is null) {
        //        if (productAuto.Text != string.Empty && (productAuto.ItemsSource as ObservableCollection<object>).Count != 0 && productAuto.Text.Length >= 4)
        //            productAuto.SelectedItem = (productAuto.ItemsSource as ObservableCollection<object>)[0];
        //        else
        //            return;
        //    }

        //    StoreOrderData.Products.Add(((ICloneable)productAuto.SelectedItem).Clone() as Product);

        //    productAuto.Text = "";
        //}

        //private void SetChanged() {
        //    if (IsFirst == true) return;
        //    IsChanged = true;
        //}

        //private void SetIsChanged(object sender, EventArgs e)
        //{
        //    SetChanged();
        //}

        //private void Confirm_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!CheckNoEmptyData()) return;
        //    StoreOrderData.Type = OrderType.DONE;
        //    SaveOrder();
        //    IsChanged = false;
        //    storeOrderCollection.Remove(storeOrderData);
        //    InventoryManagementView.DataChanged = true;
        //    ProductPurchaseRecordView.DataChanged = true;
        //    StockTakingView.DataChanged = true;

        //    if (StoOrderOverview.Items.Count == 0)
        //        ClearOrderDetailData();
        //    else
        //        StoOrderOverview.SelectedIndex = 0;
        //}

        //private void ConfirmToProcess_OnClick(object sender, RoutedEventArgs e)
        //{
        //    int oldIndex = storeOrderCollection.IndexOf(storeOrderData);
        //    int newIndex = storeOrderCollection.Count - 1;

        //    for (int x = 0; x < storeOrderCollection.Count; x++)
        //    {
        //        if (storeOrderCollection[x].type == OrderType.PROCESSING)
        //        {
        //            newIndex = x - 1;
        //            break;
        //        }
        //    }
        //    if (!CheckNoEmptyData()) return;
        //    StoreOrderData.Type = OrderType.PROCESSING;
        //    SaveOrder();
        //    storeOrderCollection.Move(oldIndex, newIndex);
        //    StoOrderOverview.SelectedItem = storeOrderData;
        //    StoOrderOverview.ScrollIntoView(storeOrderData);
        //    UpdateOrderDetailUi(OrderType.PROCESSING);
        //}

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    if (StoreOrderData != null && IsChanged)
        //    {
        //        SaveOrder();
        //    }
        //}

        //private bool CheckNoEmptyData()
        //{
        //    string errorMessage = StoreOrderData.IsAnyDataEmpty();

        //    if (errorMessage != String.Empty)
        //    {
        //        MessageWindow messageWindow = new MessageWindow(errorMessage, MessageType.ERROR);
        //        messageWindow.ShowDialog();
        //        return false;
        //    }
        //    return true;
        //}

        //private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        //{
        //    if (StoreOrderData == null) return;
        //    StoreOrderDb.DeleteOrder(StoreOrderData.Id);
        //    StoreOrderCollection.Remove(StoreOrderData);

        //    if (StoOrderOverview.Items.Count == 0)
        //        ClearOrderDetailData();
        //    else
        //        StoOrderOverview.SelectedIndex = 0;
        //}

        //private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    SetChanged();
        //    StoreOrderData.Products.RemoveAt(StoreOrderDetail.SelectedIndex);
        //    CalculateTotalPrice();
        //}



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

        //private bool IsKeyAvailable(Key key)
        //{
        //    if (key >= Key.D0 && key <= Key.D9) return true;
        //    if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;
        //    if (key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right || key == Key.OemPeriod || key == Key.Decimal) return true;

        //    return false;
        //}

        //private void SplitBatchNumber_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is null) return;

        //    var currentRowIndex = GetCurrentRowIndex(sender);

        //    double left = ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount % 2;

        //    ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount = ((int)((ITrade)StoreOrderData.Products[currentRowIndex]).Amount / 2);

        //    StoreOrderData.Products.Insert(currentRowIndex + 1, ((ICloneable)StoreOrderData.Products[currentRowIndex]).Clone() as Product);

        //    if (left != 0)
        //        ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount += left;
        //}
    }
    
}
