﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<StoreOrder> storeOrderCollection;
        private OrderType OrderTypeFilterCondition = OrderType.ALL; 
        public StoreOrder storeOrderData;
        private int orderIndex = 0;
        private bool IsFirst = true;
        private bool IsChanged = false;
        public ProductPurchaseView()
        {
            InitializeComponent();
            InitManufactory();
            UpdateUi();
            StoOrderOverview.SelectedIndex = 0;
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

        private void UpdateUi()
        {
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview();
            StoOrderOverview.ItemsSource = storeOrderCollection;
        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            if(storeOrderData != null && IsChanged) {
                UpdateOrderDetailStoreOrder();
                StoreOrderDb.SaveOrderDetail(storeOrderData);
                UpdateUi();
            }
            
            UpdateOrderDetailUi((StoreOrder)(sender as DataGridCell).DataContext);
           
        }
       

        private void UpdateOrderDetailStoreOrder() {
            storeOrderData.Id = ID.Content.ToString();
            storeOrderData.OrdEmp = PurchaseEmp.Text;
            storeOrderData.TotalPrice = Total.Content.ToString();
            var temp =  MainWindow.ManufactoryTable.Select("MAN_NAME='" + ManufactoryAuto.Text + "'");
            if(temp.Length !=0) storeOrderData.Manufactory = new Manufactory(temp[0], DataSource.MANUFACTORY);
            storeOrderData.Category = OrderCategory.Text;
           
        }
        private void UpdateOrderDetailUi(StoreOrder storeOrder)
        {
            IsFirst = true;
            ID.Content = storeOrder.Id;
            PurchaseEmp.Text = storeOrder.OrdEmp;
            OrderCategory.Text = storeOrder.Category;
            Total.Content = storeOrder.TotalPrice;
            ManufactoryAuto.Text = (storeOrder.Manufactory.Name is null)? "": storeOrder.Manufactory.Name;
            Phone.Content = (storeOrder.Manufactory.Telphone is null)? "": storeOrder.Manufactory.Telphone;

            if(storeOrder.Products is null)
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
                        AddBelowSafeAmount();
                        break;
                    case AddOrderType.ADDBYMANUFACTORY:
                        AddNewOrderByUm(addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDALLTOBASICAMOUNT:
                        AddToBasicAmount();
                        break;
                    case AddOrderType.ADDALLGOODSALES:
                        AddGoodSales();
                        break;
                    case AddOrderType.ADDBYMANUFACTORYBELOWSAFEAMOUNT:
                        AddBelowSafeAmount(addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYTOBASICAMOUNT:
                        AddToBasicAmount(addNewOrderDialog.Manufactory);
                        break;
                    case AddOrderType.ADDBYMANUFACTORYGOODSALES:
                        AddGoodSales(addNewOrderDialog.Manufactory);
                        break;
                }
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {

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
            ProductAutoCompleteCollection.Clear();
            var tmp1 = MainWindow.OtcDataTable.Select("PRO_ID Like '%" + productAuto.Text + "%' OR PRO_NAME Like '%" + productAuto.Text + "%'");
            foreach (var d in tmp1.Take(50))
            {
                ProductAutoCompleteCollection.Add(new Otc(d, DataSource.OTC));
            }
            var tmp = MainWindow.MedicineDataTable.Select("PRO_ID Like '%" + productAuto.Text + "%' OR PRO_NAME Like '%" + productAuto.Text + "%'");
            foreach (var d in tmp.Take(50))
            {
                ProductAutoCompleteCollection.Add(new Medicine(d, DataSource.MEDICINE));
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
            storeOrderData.Products[index].TotalPrice = storeOrderData.Products[index].Amount * storeOrderData.Products[index].Price;
            double count = 0;
            foreach (var product in storeOrderData.Products) {
                count += product.TotalPrice;
            }
            storeOrderData.TotalPrice = count.ToString();
            Total.Content = storeOrderData.TotalPrice;
            TotalAmount.Content = storeOrderData.Products.Count.ToString();

        }
        
        private void ManufactoryAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
            if (autoCompleteBox is null || autoCompleteBox.SelectedItem is null) return;
            Phone.Content = ((Manufactory)autoCompleteBox.SelectedItem).Telphone;
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

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
