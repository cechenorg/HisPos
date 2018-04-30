using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
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
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;
using His_Pos.InventoryManagement;
using His_Pos.ProductPurchaseRecord;
using His_Pos.Service;
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

        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();
        public ObservableCollection<object> Products;
        public ObservableCollection<object> ProductAutoCompleteCollection;
        public ObservableCollection<StoreOrder> storeOrderCollection;
        
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
        private StoreOrder storeOrderData;
        public StoreOrder StoreOrderData {
            get
            {
                return storeOrderData;
            }
            set
            {
                storeOrderData = value;
                NotifyPropertyChanged("StoreOrderData");
            }
        }
        private bool IsFirst = true;
        private bool IsChanged = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProductPurchaseView()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += UserControl1_Loaded;
            InitManufactory();
            UpdateUi();
            StoOrderOverview.SelectedIndex = 0;
        }
        
        void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            if (StoreOrderData != null && IsChanged)
            {
                SaveOrder();
            }
        }
       
        private void InitManufactory()
        {
            foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            {
                ManufactoryAutoCompleteCollection.Add(new Manufactory(row, DataSource.MANUFACTORY));
            }
        }

        public void UpdateUi()
        {
            StoreOrderCollection = StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);
        }

        private void ShowOrderDetail(object sender, SelectionChangedEventArgs e)
        {
            if (storeOrderData != null && IsChanged)
            {
                SaveOrder();
            }

            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.SelectedItem is null) return;

            StoreOrder storeOrder = (StoreOrder)dataGrid.SelectedItem;
            UpdateOrderDetailData(storeOrder);
            UpdateOrderDetailUi(storeOrder.Type);
        }

        private void SaveOrder()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            Saving.Visibility = Visibility.Visible;

            backgroundWorker.DoWork += (s, o) =>
            {
                StoreOrderDb.SaveOrderDetail(storeOrderData);
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

        private void UpdateOrderDetailUi(OrderType type)
        {
            AddNewProduct.IsEnabled = true;
            DeleteOrder.IsEnabled = true;
            ConfirmToProcess.IsEnabled = true;
            Confirm.IsEnabled = true;
            ReceiveEmp.IsEnabled = true;
            Note.IsEnabled = true;

            switch (type)
            {
                case OrderType.PROCESSING:
                    Confirm.Visibility = Visibility.Visible;
                    ConfirmToProcess.Visibility = Visibility.Collapsed;
                    DeleteOrder.Visibility = Visibility.Collapsed;
                    OrderCategory.IsEnabled = false;
                    EmptySpace.Width = 400;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Collapsed;
                    break;
                case OrderType.UNPROCESSING:
                    Confirm.Visibility = Visibility.Collapsed;
                    ConfirmToProcess.Visibility = Visibility.Visible;
                    DeleteOrder.Visibility = Visibility.Visible;
                    OrderCategory.IsEnabled = true;
                    EmptySpace.Width = 270;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ClearOrderDetailData() {
            IsFirst = true;
            StoreOrderData = null;
            IsChanged = false;
            IsFirst = false;
        }
        private void UpdateOrderDetailData(StoreOrder storeOrder)
        {
            IsFirst = true;
            if (storeOrder.Products is null)
                storeOrder.Products = StoreOrderDb.GetStoreOrderCollectionById(storeOrder.Id);
            
            StoreOrderData = storeOrder;
            
             GetProductAutoComplete();

            IsChanged = false;
            IsFirst = false;
        }

        private void GetProductAutoComplete()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, o) =>
            {
                ObservableCollection<object> temp = ProductDb.GetItemDialogProduct(storeOrderData.Manufactory.Id);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Products = temp;
                }));
            };

            backgroundWorker.RunWorkerAsync();
        }

        private void AddNewOrder(object sender, MouseButtonEventArgs e)
        {
            AddNewOrderDialog addNewOrderDialog = new AddNewOrderDialog(ManufactoryAutoCompleteCollection);

            addNewOrderDialog.ShowDialog();

            if (addNewOrderDialog.ConfirmButtonClicked)
            {
                switch(addNewOrderDialog.AddOrderType)
                {
                    case AddOrderType.ADDALLBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE);
                        break;
                    case AddOrderType.ADDBYMANUFACTORY:
                        AddNewOrderByUm(addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDALLTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC);
                        break;
                    case AddOrderType.ADDALLGOODSALES:
                        AddGoodSales();
                        break;
                    case AddOrderType.ADDBYMANUFACTORYBELOWSAFEAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.SAFE, addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYTOBASICAMOUNT:
                        AddBasicOrSafe(StoreOrderProductType.BASIC, addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYGOODSALES:
                        AddGoodSales(addNewOrderDialog.Manufactory);
                        break;
                }
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
            
            if (selectedItem is IDeletable)
            {
                if (StoreOrderData.Products.Contains(selectedItem)){
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }
                 
                StoreOrderDetail.SelectedItem = selectedItem;
            }
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;
           
             if (leaveItem is IDeletable)
            {
                (leaveItem as IDeletable).Source = string.Empty;
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

                OrderCategory.IsEnabled = false;
                AddNewProduct.IsEnabled = false;
                DeleteOrder.IsEnabled = false;
                ConfirmToProcess.IsEnabled = false;
                Confirm.IsEnabled = false;
                ReceiveEmp.IsEnabled = false;
                Note.IsEnabled = false;
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

        private void ProductAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var productAuto = sender as AutoCompleteBox;

            if (String.IsNullOrEmpty(storeOrderData.Manufactory.Id) || productAuto is null || Products is null) return;
            
            var result = Products.Where(x => ((NewItemProduct)x).Product.Id.Contains(productAuto.Text) || ((NewItemProduct)x).Product.Name.Contains(productAuto.Text)).Take(50).Select(x => ((NewItemProduct)x).Product);
            ProductAutoCompleteCollection = new ObservableCollection<object>(result.ToList());

            productAuto.ItemsSource = ProductAutoCompleteCollection;
            productAuto.PopulateComplete();
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            
            if(textBox is null) return;
            
            if (textBox.Text == String.Empty)
                textBox.Text = "0";

            if( !textBox.Name.Equals("FreeAmount") )
                CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            double count = 0;
            foreach (var product in storeOrderData.Products)
            {
                count += ((ITrade)product).TotalPrice;
            }

            storeOrderData.TotalPrice = count.ToString();
        }
        
        private void AutoCompleteBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var productAuto = sender as AutoCompleteBox;
            SetChanged();
            if (productAuto is null) return;
            if (productAuto.SelectedItem is null) return;
            if (StoreOrderData.Products.Count == StoreOrderDetail.SelectedIndex)
            {
                StoreOrderData.Products.Add(productAuto.SelectedItem as Product);
                StoreOrderDetail.SelectedIndex--;
            }
            else
            {
                StoreOrderData.Products[StoreOrderDetail.SelectedIndex] = productAuto.SelectedItem as Product;
                return;
            }
           
            productAuto.Text = "";
        }
        
        private void SetChanged() {
            if (IsFirst == true) return;
            IsChanged = true;
        }
        private void SetIsChanged(object sender, EventArgs e)
        {
            SetChanged();
        }
        
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckNoEmptyData()) return;
            StoreOrderData.Type = OrderType.DONE;
            SaveOrder();
            IsChanged = false;
            storeOrderCollection.Remove(storeOrderData);
            InventoryManagementView.DataChanged = true;
            ProductPurchaseRecordView.DataChanged = true;

            if (StoOrderOverview.Items.Count == 0)
                ClearOrderDetailData();
            else
                StoOrderOverview.SelectedIndex = 0;
        }
        private void ConfirmToProcess_OnClick(object sender, RoutedEventArgs e)
        {
            int oldIndex = storeOrderCollection.IndexOf(storeOrderData);
            int newIndex = storeOrderCollection.Count - 1;

            for( int x = 0; x < storeOrderCollection.Count; x++ )
            {
                if (storeOrderCollection[x].type == OrderType.PROCESSING)
                {
                    newIndex = x - 1;
                    break;
                }
            }
            if (!CheckNoEmptyData()) return;
            StoreOrderData.Type = OrderType.PROCESSING;
            SaveOrder();
            storeOrderCollection.Move(oldIndex, newIndex);
            StoOrderOverview.SelectedItem = storeOrderData;
            StoOrderOverview.ScrollIntoView(storeOrderData);
            UpdateOrderDetailUi(OrderType.PROCESSING);
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (StoreOrderData != null && IsChanged)
            {
                SaveOrder();
            }
        }

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

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChanged();
            StoreOrderData.Products.RemoveAt(StoreOrderDetail.SelectedIndex);
            CalculateTotalPrice();
        }

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(ItemType.Product, Products);

            newItemDialog.ShowDialog();

            if(newItemDialog.ConfirmButtonClicked)
            {
                SetChanged();
                StoreOrderData.Products.Add(newItemDialog.SelectedItem as Product);
            }
        }
        
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var objectName = (sender as Control).Name;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var nextTextBox = new List<TextBox>();
                var thisTextBox = new List<TextBox>();
                var pickerList = new List<DatePicker>();
                var currentRowIndex = GetCurrentRowIndex(sender);
                
                if (currentRowIndex == -1) return;

                switch (objectName)
                {
                    case "Price":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Amount", ref nextTextBox);
                        break;
                    case "Amount":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "FreeAmount", ref nextTextBox);
                        break;
                    case "FreeAmount":
                        if (storeOrderData.Type == OrderType.UNPROCESSING)
                            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
                        else if (storeOrderData.Products[currentRowIndex] is ProductPurchaseMedicine)
                            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref nextTextBox);
                        else
                        {
                            NewFunction.FindChildGroup<DatePicker>(StoreOrderDetail, "ValidDate", ref pickerList);
                            pickerList[currentRowIndex].Focus();
                            return;
                        }

                        break;
                    case "BatchNumber":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "BatchNumber", ref thisTextBox);

                        if ((sender as TextBox).Text == String.Empty && currentRowIndex > 0 && thisTextBox.Count > 0)
                            (sender as TextBox).Text = thisTextBox[currentRowIndex - 1].Text;

                        NewFunction.FindChildGroup<DatePicker>(StoreOrderDetail, "ValidDate", ref pickerList);
                        pickerList[currentRowIndex].Focus();
                        return;
                    case "ValidDate":
                        NewFunction.FindChildGroup<DatePicker>(StoreOrderDetail, "ValidDate", ref pickerList);
                        if ((sender as DatePicker).Text == String.Empty && currentRowIndex > 0)
                            (sender as DatePicker).Text = thisTextBox[currentRowIndex - 1].Text;

                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref nextTextBox);
                        nextTextBox[currentRowIndex].Focus();
                        return;
                    case "Invoice":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Invoice", ref thisTextBox);
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref nextTextBox);
                        break;
                    case "Notes":
                        if (currentRowIndex == storeOrderData.Products.Count - 1)
                        {
                            var autoList = new List<AutoCompleteBox>();
                            NewFunction.FindChildGroup<AutoCompleteBox>(StoreOrderDetail, "Id", ref autoList);
                            NewFunction.FindChildGroup<TextBox>(autoList[currentRowIndex + 1], "Text", ref nextTextBox);
                            nextTextBox[0].Focus();
                        }
                        else
                        {
                            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Price", ref nextTextBox);
                            nextTextBox[currentRowIndex + 1].Focus();
                        }
                        return;
                }

                if ((sender as TextBox).Text == String.Empty && currentRowIndex > 0 && thisTextBox.Count > 0)
                    (sender as TextBox).Text = thisTextBox[currentRowIndex - 1].Text;

                nextTextBox[currentRowIndex].Focus();
            }

            if(!IsKeyAvailable(e.Key) && (objectName.Equals("Price") || objectName.Equals("Amount") || objectName.Equals("FreeAmount")))
                e.Handled = true;
        }

        private bool IsKeyAvailable(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;
            if( key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right) return true;

            return false;
        }

        private int GetCurrentRowIndex(object sender)
        {
            if (sender is TextBox)
            {
                List<TextBox> temp = new List<TextBox>();
                TextBox textBox = sender as TextBox;

                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, textBox.Name, ref temp);

                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        return x;
                    }
                }
            }
            else if (sender is DatePicker)
            {
                List<DatePicker> temp = new List<DatePicker>();
                DatePicker datePicker = sender as DatePicker;

                NewFunction.FindChildGroup<DatePicker>(StoreOrderDetail, datePicker.Name, ref temp);

                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        return x;
                    }
                }
            }

            return -1;
        }
    }
    public class AutoCompleteIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value.ToString().Equals("")) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class LastRowIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class BatchNamberIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductPurchaseMedicine) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
