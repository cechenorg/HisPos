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
using His_Pos.Service;
using MahApps.Metro.Controls;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// ProductPurchaseView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseView : UserControl, INotifyPropertyChanged
    {
        public DataTable ProductPurchaseMedicine;
        public DataTable ProductPurchaseOtc;

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
        private int orderIndex = 0;
        private bool IsFirst = true;
        private bool IsChanged = false;
        private int LastSelectedIndex = -1;

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
            ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;
            ManufactoryAuto.ItemFilter = ManufactoryFilter;
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
            AddNewProduct.IsEnabled = !string.IsNullOrEmpty(StoreOrderData.Manufactory.Id);

            switch (type)
            {
                case OrderType.PROCESSING:
                    Confirm.Visibility = Visibility.Visible;
                    ConfirmToProcess.Visibility = Visibility.Collapsed;
                    DeleteOrder.Visibility = Visibility.Collapsed;
                    AddNewProduct.Visibility = Visibility.Collapsed;
                    ManufactoryAuto.IsEnabled = false;
                    OrderCategory.IsEnabled = false;
                    EmptySpace.Width = 570;
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
                    AddNewProduct.Visibility = Visibility.Visible;
                    ManufactoryAuto.IsEnabled = true;
                    OrderCategory.IsEnabled = true;
                    EmptySpace.Width = 300;
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

            if (!String.IsNullOrEmpty(storeOrderData.Manufactory.Id))
            {
                GetProductAutoComplete();
            }

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
                    case AddOrderType.ADDBYUSER:
                        AddNewOrderByUm();
                        break;
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
            
            var result = Products.Where(x => ((Product)x).Id.Contains(productAuto.Text) || ((Product)x).Name.Contains(productAuto.Text)).Take(50);
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

        private void ManufactoryAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
            if (autoCompleteBox is null || autoCompleteBox.SelectedItem is null) return;
            AddNewProduct.IsEnabled = (ManufactoryAuto.Text != string.Empty);
            StoreOrderData.Manufactory = (Manufactory)((Manufactory)autoCompleteBox.SelectedItem).Clone();
            GetProductAutoComplete();
            SetChanged();
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

        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Manufactory).Id is null) ? true : (obj as Manufactory).Id.Contains(searchText)
                    || (obj as Manufactory).Name.Contains(searchText);
            }
        }
        private void SetChanged() {
            if (IsFirst == true) return;
            IsChanged = true;
        }
        private void SetIsChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        private void CofirmAndSave(OrderType type) {
            if (!CheckNoEmptyData()) return;
            StoreOrderData.Type = type;
            SaveOrder();
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            CofirmAndSave(OrderType.DONE);

            storeOrderCollection.Remove(storeOrderData);

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

            CofirmAndSave(OrderType.PROCESSING);
            
            storeOrderCollection.Move(oldIndex, newIndex);
            StoOrderOverview.SelectedItem = storeOrderData;
            StoOrderOverview.ScrollIntoView(storeOrderData);
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
            NewItemDialog newItemDialog = new NewItemDialog(ItemType.Product, StoreOrderData.Manufactory.Id);

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
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                List<TextBox> temp = new List<TextBox>();
                TextBox textBox = sender as TextBox;
                int currentRowIndex = -1;

                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, textBox.Name, ref temp);

                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        currentRowIndex = x;
                        break;
                    }
                }

                if (currentRowIndex == -1) return;

                temp.Clear();

                switch (textBox.Name)
                {
                    case "Price":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Amount", ref temp);
                        break;
                    case "Amount":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "FreeAmount", ref temp);
                        break;
                    case "FreeAmount":
                        NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Notes", ref temp);
                        break;
                    case "Notes":
                        if (currentRowIndex == storeOrderData.Products.Count - 1)
                        {
                            List<AutoCompleteBox> autoList = new List<AutoCompleteBox>();
                            NewFunction.FindChildGroup<AutoCompleteBox>(StoreOrderDetail, "Id", ref autoList);
                            autoList[currentRowIndex + 1].Focus();

                        }
                        else
                        {
                            NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, "Price", ref temp);
                            temp[currentRowIndex + 1].Focus();
                        }
                        return;
                }
                temp[currentRowIndex].Focus();
            }
        }
    }
    public class AutoCompleteIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Equals("")) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
