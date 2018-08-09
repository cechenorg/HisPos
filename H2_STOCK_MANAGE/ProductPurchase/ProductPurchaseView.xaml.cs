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
        public ObservableCollection<object> ProductAutoCompleteCollection;

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
            UpdateUi();
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

        public void UpdateUi()
        {
            StoreOrderCollection = StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);
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

        //private void GetProductAutoComplete()
        //{
        //    BackgroundWorker getProductAutobackground = new BackgroundWorker();

        //    getProductAutobackground.DoWork += (s, o) =>
        //    {
        //        ObservableCollection<object> temp = ProductDb.GetItemDialogProduct();
        //        Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            Products = temp;
        //        }));
        //    };

        //    getProductAutobackground.RunWorkerAsync();
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

        //public AutoCompleteFilterPredicate<object> ProductFilter
        //{
        //    get
        //    {
        //        return (searchText, obj) =>
        //            ((obj as Product).Id is null) ? false : (obj as Product).Id.ToLower().Contains(searchText.ToLower())
        //            || (obj as Product).ChiName.ToLower().Contains(searchText.ToLower()) || (obj as Product).EngName.ToLower().Contains(searchText.ToLower());
        //    }
        //}

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

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;

        //    if (textBox is null) return;

        //    if (textBox.Text == String.Empty)
        //        textBox.Text = "0";

        //    if (!textBox.Name.Equals("FreeAmount"))
        //        CalculateTotalPrice();
        //}

        //private void CalculateTotalPrice()
        //{
        //    double count = 0;
        //    foreach (var product in storeOrderData.Products)
        //    {
        //        count += ((ITrade)product).TotalPrice;
        //    }
        //    storeOrderData.TotalPrice = Math.Round(count, MidpointRounding.AwayFromZero).ToString();
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

        //private void NewProduct(object sender, RoutedEventArgs e)
        //{
        //    NewItemDialog newItemDialog = new NewItemDialog(ItemType.Product, Products);

        //    newItemDialog.ShowDialog();

        //    if (newItemDialog.ConfirmButtonClicked)
        //    {
        //        SetChanged();
        //        StoreOrderData.Products.Add(newItemDialog.SelectedItem as Product);
        //    }
        //}

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    var objectName = (sender as Control).Name;

        //    //按 Enter 下一欄
        //    if (e.Key == Key.Enter)
        //    {
        //        e.Handled = true;
        //        var nextTextBox = new List<TextBox>();
        //        var thisTextBox = new List<TextBox>();
        //        var currentRowIndex = GetCurrentRowIndex(sender);

        //        if (currentRowIndex == -1) return;

        //        switch (objectName)
        //        {
        //            case "Price":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Amount", ref nextTextBox);
        //                break;
        //            case "Amount":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "FreeAmount", ref nextTextBox);
        //                break;
        //            case "FreeAmount":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "TotalPrice", ref nextTextBox);
        //                break;
        //            case "TotalPrice":
        //                if (storeOrderData.Type == OrderType.UNPROCESSING)
        //                    NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
        //                else
        //                    NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref nextTextBox);
        //                break;
        //            case "BatchNumber":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref thisTextBox);

        //                if ((sender as TextBox).Text == String.Empty && currentRowIndex > 0 && thisTextBox.Count > 0)
        //                {
        //                    SetChanged();
        //                    (sender as TextBox).Text = thisTextBox[currentRowIndex - 1].Text;
        //                }

        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "ValidDate", ref nextTextBox);
        //                break;
        //            case "ValidDate":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "ValidDate", ref thisTextBox);
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref nextTextBox);
        //                break;
        //            case "Invoice":
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref thisTextBox);
        //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
        //                break;
        //            case "Notes":
        //                if (currentRowIndex == storeOrderData.Products.Count - 1)
        //                {
        //                    var autoList = new List<AutoCompleteBox>();
        //                    NewFunction.FindChildGroup<AutoCompleteBox>(StoreOrderDetail, "Id", ref autoList);
        //                    NewFunction.FindChildGroup<TextBox>(autoList[currentRowIndex + 1], "Text", ref nextTextBox);
        //                    nextTextBox[0].Focus();
        //                }
        //                else
        //                {
        //                    NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Price", ref nextTextBox);
        //                    nextTextBox[currentRowIndex + 1].Focus();
        //                }
        //                return;
        //        }

        //        if ((sender as TextBox).Text == String.Empty && currentRowIndex > 0 && thisTextBox.Count > 0)
        //        {
        //            SetChanged();
        //            (sender as TextBox).Text = thisTextBox[currentRowIndex - 1].Text;
        //        }


        //        nextTextBox[currentRowIndex].Focus();
        //    }

        //    //按 Up Down
        //    if (e.Key == Key.Up || e.Key == Key.Down)
        //    {
        //        e.Handled = true;
        //        var thisTextBox = new List<TextBox>();
        //        var currentRowIndex = GetCurrentRowIndex(sender);

        //        if (currentRowIndex == -1) return;

        //        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, objectName, ref thisTextBox);

        //        int newIndex = (e.Key == Key.Up) ? currentRowIndex - 1 : currentRowIndex + 1;

        //        if (newIndex < 0)
        //            newIndex = 0;
        //        else if (newIndex >= thisTextBox.Count)
        //            newIndex = thisTextBox.Count - 1;

        //        thisTextBox[newIndex].Focus();
        //    }

        //    ////按 Left
        //    //if (e.Key == Key.Left)
        //    //{
        //    //    e.Handled = true;
        //    //    var nextTextBox = new List<TextBox>();
        //    //    var currentRowIndex = GetCurrentRowIndex(sender);

        //    //    if (currentRowIndex == -1) return;

        //    //    switch (objectName)
        //    //    {
        //    //        case "Price":
        //    //            return;
        //    //        case "Amount":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Price", ref nextTextBox);
        //    //            break;
        //    //        case "FreeAmount":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Amount", ref nextTextBox);
        //    //            break;
        //    //        case "TotalPrice":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "FreeAmount", ref nextTextBox);
        //    //            break;
        //    //        case "BatchNumber":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "TotalPrice", ref nextTextBox);
        //    //            break;
        //    //        case "ValidDate":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref nextTextBox);
        //    //            break;
        //    //        case "Invoice":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "ValidDate", ref nextTextBox);
        //    //            break;
        //    //        case "Notes":
        //    //            if (storeOrderData.Type == OrderType.UNPROCESSING)
        //    //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "TotalPrice", ref nextTextBox);
        //    //            else
        //    //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref nextTextBox);
        //    //            break;
        //    //    }

        //    //    nextTextBox[currentRowIndex].Focus();
        //    //}

        //    ////按 Right
        //    //if (e.Key == Key.Right)
        //    //{
        //    //    e.Handled = true;
        //    //    var nextTextBox = new List<TextBox>();
        //    //    var thisTextBox = new List<TextBox>();
        //    //    var currentRowIndex = GetCurrentRowIndex(sender);

        //    //    if (currentRowIndex == -1) return;

        //    //    switch (objectName)
        //    //    {
        //    //        case "Price":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Amount", ref nextTextBox);
        //    //            break;
        //    //        case "Amount":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "FreeAmount", ref nextTextBox);
        //    //            break;
        //    //        case "FreeAmount":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "TotalPrice", ref nextTextBox);
        //    //            break;
        //    //        case "TotalPrice":
        //    //            if (storeOrderData.Type == OrderType.UNPROCESSING)
        //    //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
        //    //            else
        //    //                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref nextTextBox);
        //    //            break;
        //    //        case "BatchNumber":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "ValidDate", ref nextTextBox);
        //    //            break;
        //    //        case "ValidDate":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref nextTextBox);
        //    //            break;
        //    //        case "Invoice":
        //    //            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
        //    //            break;
        //    //        case "Notes":
        //    //            return;
        //    //    }

        //    //    nextTextBox[currentRowIndex].Focus();
        //    //}

        //    // Price Amount FreeAmount "0 打字 直接顯示數字"
        //    if (objectName.Equals("Price") || objectName.Equals("Amount") || objectName.Equals("FreeAmount"))
        //    {
        //        if (!IsKeyAvailable(e.Key))
        //            e.Handled = true;
        //        else
        //        {
        //            TextBox textBox = sender as TextBox;
        //            if ((e.Key == Key.Decimal || e.Key == Key.OemPeriod) && textBox.Text.Contains(".")) {
        //                e.Handled = true;
        //            }
        //            if (textBox.Text.Equals("0"))
        //            {
        //                SetChanged();
        //                if (!(e.Key == Key.Decimal || e.Key == Key.OemPeriod)) { 
        //                    textBox.Text = GetCharFromKey(e.Key);
        //                    e.Handled = true;
        //                }
        //                textBox.CaretIndex = 1;
        //            }
        //        }
        //    }
        //    else if (objectName.Equals("ValidDate")) {
        //        if (!IsKeyAvailable(e.Key))
        //            e.Handled = true;
        //        else
        //        {
        //            SetChanged();
        //            TextBox textBox = sender as TextBox;

        //            if (e.Key == Key.Back || e.Key == Key.Delete)
        //            {
        //                textBox.Text = string.Empty;
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        SetChanged();
        //    }

        //}

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

        //private int GetCurrentRowIndex(object sender)
        //{
        //    if (sender is TextBox)
        //    {
        //        List<TextBox> temp = new List<TextBox>();
        //        TextBox textBox = sender as TextBox;

        //        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, textBox.Name, ref temp);

        //        for (int x = 0; x < temp.Count; x++)
        //        {
        //            if (temp[x].Equals(sender))
        //            {
        //                return x;
        //            }
        //        }
        //    }
        //    else if (sender is DatePicker)
        //    {
        //        List<DatePicker> temp = new List<DatePicker>();
        //        DatePicker datePicker = sender as DatePicker;

        //        NewFunction.FindChildGroup<DatePicker>(StoreOrderDetail, datePicker.Name, ref temp);

        //        for (int x = 0; x < temp.Count; x++)
        //        {
        //            if (temp[x].Equals(sender))
        //            {
        //                return x;
        //            }
        //        }
        //    }
        //    else if (sender is Button)
        //    {
        //        List<Button> temp = new List<Button>();
        //        Button SplitBtn = sender as Button;

        //        NewFunction.FindChildGroup<Button>(StoreOrderDetail, SplitBtn.Name, ref temp);

        //        for (int x = 0; x < temp.Count; x++)
        //        {
        //            if (temp[x].Equals(sender))
        //            {
        //                return x;
        //            }
        //        }
        //    }

        //    return -1;
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

        //public AutoCompleteFilterPredicate<object> UserFilter
        //{
        //    get
        //    {
        //        return (searchText, obj) =>
        //            ((obj as Person).Id is null) ? true : (obj as Person).Id.Contains(searchText)
        //            || (obj as Person).Name.Contains(searchText);
        //    }
        //}
    }
    
}
