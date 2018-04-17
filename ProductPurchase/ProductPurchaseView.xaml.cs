using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using MahApps.Metro.Controls;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// ProductPurchaseView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseView : UserControl
    {
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();
        public ObservableCollection<Product> ProductAutoCompleteCollection = new ObservableCollection<Product>();
        public ObservableCollection<StoreOrder> storeOrderCollection;
        private OrderType OrderTypeFilterCondition = OrderType.ALL; 
        public StoreOrder storeOrderData;
        private int orderIndex = 0;
        private bool IsFirst = true;
        private bool IsChanged = false;
        private int LastSelectedIndex = -1;
        public ProductPurchaseView()
        {
            InitializeComponent();
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
            if (storeOrderData != null && IsChanged)
            {
                UpdateOrderDetailStoreOrder();
                StoreOrderDb.SaveOrderDetail(storeOrderData);
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
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview("N");
            StoOrderOverview.ItemsSource = storeOrderCollection;
        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            if(storeOrderData != null && IsChanged) {
                UpdateOrderDetailStoreOrder();
                StoreOrderDb.SaveOrderDetail(storeOrderData);

                orderIndex = StoOrderOverview.SelectedIndex;
                UpdateUi();
                StoOrderOverview.SelectedIndex = orderIndex;
            }

            StoreOrder storeOrder =  (StoreOrder) (sender as DataGridCell).DataContext;
            UpdateOrderDetailUi(storeOrder.Type);
            UpdateOrderDetailData(storeOrder);
        }

        private void UpdateOrderDetailUi(OrderType type)
        {
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

        private void UpdateOrderDetailStoreOrder() {
            storeOrderData.Id = ID.Content.ToString();
            storeOrderData.OrdEmp = PurchaseEmp.Content.ToString();
            storeOrderData.TotalPrice = Total.Content.ToString();
            var temp =  MainWindow.ManufactoryTable.Select("MAN_NAME='" + ManufactoryAuto.Text + "'");
            if(temp.Length !=0) storeOrderData.Manufactory = new Manufactory(temp[0], DataSource.MANUFACTORY);
            storeOrderData.Category = OrderCategory.Text;
           
        }
        private void ClearOrderDetailData() {
            IsFirst = true;
            ID.Content = "";
            PurchaseEmp.Content = "";
            OrderCategory.Text = "";
            Total.Content = "";
            ManufactoryAuto.Text = "";
            Phone.Content ="";
            StoreOrderDetail.ItemsSource = null;
            TotalAmount.Content = "";
            IsChanged = false;
            IsFirst = false;
        }
        private void UpdateOrderDetailData(StoreOrder storeOrder)
        {
            IsFirst = true;
            ID.Content = storeOrder.Id;
            PurchaseEmp.Content = storeOrder.OrdEmp;
            OrderCategory.Text = storeOrder.Category;
            Total.Content = storeOrder.TotalPrice;
            ManufactoryAuto.Text = (storeOrder.Manufactory.Name is null)? "": storeOrder.Manufactory.Name;
            ButtonNewProduct.IsEnabled = (storeOrder.Manufactory.Name is null) ? false :true;
            Phone.Content = (storeOrder.Manufactory.Telphone is null)? "": storeOrder.Manufactory.Telphone;

            if (storeOrder.Products is null)
                storeOrder.Products = StoreOrderDb.GetStoreOrderCollectionById(storeOrder.Id);
            
            storeOrderData = storeOrder;
            StoreOrderDetail.ItemsSource = storeOrderData.Products;
            TotalAmount.Content = storeOrder.Products.Count.ToString();
            IsChanged = false;
            IsFirst = false;
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
                if (storeOrderData.Products.Contains(selectedItem)){
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }
                 
                StoreOrderDetail.SelectedItem = selectedItem;
                LastSelectedIndex = StoreOrderDetail.SelectedIndex;
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
            //List<string> proList = ManufactoryDb.GetProductByManId(storeOrderData.Manufactory.Id);
            var productAuto = sender as AutoCompleteBox;
            ProductAutoCompleteCollection.Clear();
            var tmp1 = MainWindow.OtcDataTable.Select("PRO_ID Like '%" + productAuto.Text + "%' OR PRO_NAME Like '%" + productAuto.Text + "%'");
            foreach (var d in tmp1.Take(50))
            {
                //if(proList.Contains(d["PRO_ID"].ToString()))
                //ProductAutoCompleteCollection.Add(new ProductPurchaseOtc(d, DataSource.OTC));
            }
            var tmp = MainWindow.MedicineDataTable.Select("PRO_ID Like '%" + productAuto.Text + "%' OR PRO_NAME Like '%" + productAuto.Text + "%'");
            foreach (var d in tmp.Take(50))
            {
                //if (proList.Contains(d["PRO_ID"].ToString()))
                //ProductAutoCompleteCollection.Add(new ProductPurchaseMedicine(d, DataSource.MEDICINE));
            }
            productAuto.ItemsSource = ProductAutoCompleteCollection;
            productAuto.PopulateComplete();
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StoreOrderDetail.SelectedIndex == -1) return;
            if (IsFirst == true) return;
            SetChanged();

            if ((sender as TextBox).Text == String.Empty)
                (sender as TextBox).Text = "0";

            int index = StoreOrderDetail.SelectedIndex;
            //storeOrderData.Products[index].TotalPrice = storeOrderData.Products[index].Amount * storeOrderData.Products[index].Price;
            //double count = 0;
            //foreach (var product in storeOrderData.Products) {
            //    count += product.TotalPrice;
            //}
            //storeOrderData.TotalPrice = count.ToString();
            Total.Content = storeOrderData.TotalPrice;
            TotalAmount.Content = storeOrderData.Products.Count.ToString();

        }
        
        private void ManufactoryAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
            if (autoCompleteBox is null || autoCompleteBox.SelectedItem is null) return;
            Phone.Content = ((Manufactory)autoCompleteBox.SelectedItem).Telphone;
            ButtonNewProduct.IsEnabled = (ManufactoryAuto.Text == string.Empty) ? false : true;
            storeOrderData.Manufactory = ((Manufactory)autoCompleteBox.SelectedItem);
        }

        private void AutoCompleteBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var productAuto = sender as AutoCompleteBox;

            if (productAuto is null) return;
            if (productAuto.SelectedItem is null) return;

            if (storeOrderData.Products.Count == StoreOrderDetail.SelectedIndex)
            {
                storeOrderData.Products.Add(productAuto.SelectedItem as Product);
                StoreOrderDetail.SelectedIndex--;
            }
            else
            {
                storeOrderData.Products[StoreOrderDetail.SelectedIndex] = productAuto.SelectedItem as Product;
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
            UpdateOrderDetailStoreOrder();
            if (!CheckNoEmptyData()) return;
            storeOrderData.Type = type;
            StoreOrderDb.SaveOrderDetail(storeOrderData);
            UpdateUi();

            for (int x = 0; x < storeOrderCollection.Count; x++)
            {
                if (storeOrderCollection[x].Id == storeOrderData.Id)
                {
                    StoOrderOverview.SelectedIndex = x;
                    StoOrderOverview.ScrollIntoView(storeOrderCollection[x]);
                }
            }
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            CofirmAndSave(OrderType.DONE);
            if (StoOrderOverview.Items.Count == 0)
                ClearOrderDetailData();
            else
                StoOrderOverview.SelectedIndex = 0;
        }
        private void ConfirmToProcess_OnClick(object sender, RoutedEventArgs e)
        {
            CofirmAndSave(OrderType.PROCESSING);
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (storeOrderData != null && IsChanged)
            {
                UpdateOrderDetailStoreOrder();
                StoreOrderDb.SaveOrderDetail(storeOrderData);
            }
        }

        private bool CheckNoEmptyData()
        {
            string errorMessage = storeOrderData.IsAnyDataEmpty();

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
            if (storeOrderData == null) return;
            StoreOrderDb.DeleteOrder(storeOrderData.Id);
            UpdateUi();
            if (StoOrderOverview.Items.Count == 0)
                ClearOrderDetailData();
            else
                StoOrderOverview.SelectedIndex = 0;
            
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            storeOrderData.Products.RemoveAt(StoreOrderDetail.SelectedIndex);
        }

        private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var ProductAuto = sender as AutoCompleteBox;
            if (ProductAuto is null) return;

            if ((ProductAuto.Text is null || ProductAuto.Text == String.Empty) && LastSelectedIndex != storeOrderData.Products.Count - 1)
            {
                storeOrderData.Products.RemoveAt(LastSelectedIndex);
            }
        }

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(ItemType.Product, storeOrderData.Manufactory.Id);

            newItemDialog.ShowDialog();

            if(newItemDialog.ConfirmButtonClicked)
            {
                storeOrderData.Products.Add(newItemDialog.SelectedItem as Product);
            }
        }
    }
}
