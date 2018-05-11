﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Class;
using System;
using System.Data;
using System.Linq;
using His_Pos.Interface;
using His_Pos.Service;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Data;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl, INotifyPropertyChanged
    {
        public static InventoryManagementView Instance;
        public DataTable InventoryMedicines;
        public DataTable InventoryOtcs;
        private SearchType searchType = SearchType.ALL;
        public double selectStockValue = 0;
        public double searchCount = 0;
        public ListCollectionView ProductTypeCollection;
        private ObservableCollection<Product> _dataList = new ObservableCollection<Product>();
        public static ProductDetail productDetail = null;
        public ObservableCollection<Product> _DataList 
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                NotifyPropertyChanged("_DataList");
            }
        }

        public static bool DataChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public InventoryManagementView()
        {
            InitializeComponent();
            Instance = this;
            MergingData();
            DataContext = this;
            SetOtcTypeUi();
        }
        public void SetOtcTypeUi() {
            ProductTypeCollection = ProductDb.GetProductType();
            OtcType.ItemsSource = ProductTypeCollection;
            OtcType.SelectedValue = "無";
        }
        public void MergingData()
        {
            Search.IsEnabled = false;

            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.MergeProductInventory(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;

            DataChanged = false;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            selectStockValue = 0;
            searchCount = 0;
            SearchData();
            SearchCount.Content = searchCount;
            SelectStockValue.Content = selectStockValue.ToString("0.#");
        }

        public void SearchData()
        {
            ProductList.Items.Filter = OrderTypeFilter;
        }
        public bool OrderTypeFilter(object item)
        {
            bool reply = false;
            switch (searchType) {
                case SearchType.OTC:
                    if (item is InventoryOtc)
                    {
                        if (
                            (((InventoryOtc)item).Id.Contains(ID.Text) || ID.Text == string.Empty) //ID filter
                        && (((InventoryOtc)item).ChiName.Contains(Name.Text) || ((InventoryOtc)item).EngName.Contains(Name.Text) || Name.Text == string.Empty) //Name filter
                       && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter
                        && (((InventoryOtc)item).ProductType.Name.Contains(OtcType.SelectedValue.ToString()) || OtcType.SelectedItem == null || OtcType.SelectedValue.ToString() == "無")
                        && ((((InventoryOtc)item).Stock.Inventory == 0 && (bool)NoneInventory.IsChecked) || (((InventoryOtc)item).Stock.Inventory != 0 && (bool)!NoneInventory.IsChecked))
                        ) reply = true;
                    }
                    if (reply) {
                        searchCount++;
                        selectStockValue += Convert.ToDouble(((InventoryOtc)item).StockValue);
                    } 
                    break;
                case SearchType.MED:
                    if (item is InventoryMedicine)
                    {
                        if ((((InventoryMedicine)item).Id.Contains(ID.Text) || ID.Text == string.Empty) //ID filter
                       && (((InventoryMedicine)item).ChiName.Contains(Name.Text) || ((InventoryMedicine)item).EngName.Contains(Name.Text) || Name.Text == string.Empty) //Name filter
                        && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter
                        && (((InventoryMedicine)item).Ingredient.Contains(Ingredient.Text) || Ingredient.Text == string.Empty)
                       && ((((InventoryMedicine)item).Stock.Inventory == 0 && (bool)NoneInventory.IsChecked) || (((InventoryMedicine)item).Stock.Inventory != 0 && (bool)!NoneInventory.IsChecked))
                        ) reply = true;
                    if (reply)
                        {
                            searchCount++;
                            selectStockValue += Convert.ToDouble(((InventoryMedicine)item).StockValue);
                        }
                    }
                    break;
                case SearchType.ALL:
                    if (
                        (((Product)item).Id.Contains(ID.Text) || ID.Text == string.Empty) //ID filter
                          && (((Product)item).ChiName.Contains(Name.Text) || ((Product)item).EngName.Contains(Name.Text) || Name.Text == string.Empty) //Name filter
                           && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter              
                        && ((((IInventory)item).Stock.Inventory == 0 && (bool)NoneInventory.IsChecked) || (((IInventory)item).Stock.Inventory != 0 && (bool)!NoneInventory.IsChecked))
                        ) reply = true;
                    if (reply)
                    {
                        searchCount++;
                        selectStockValue += Convert.ToDouble(((IInventory)item).StockValue);
                    }
                    break;
            }
            return reply;
        }
        private void showProductDetail(object sender, MouseButtonEventArgs e)
        {
            if (!Search.IsEnabled) return;
            var selectedItem = (sender as DataGridRow).Item;

            if (productDetail is null)
            {
                productDetail = new ProductDetail();
                productDetail.Show();
            }

            productDetail.AddNewTab((Product)selectedItem);
            productDetail.Focus();
        }

        private void ComfirmChangeButtonOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Product product;
            if (sender is OtcDetail)
            {
                product = (sender as OtcDetail).InventoryOtc;
               InventoryOtcs.Select("PRO_ID='" + product.Id + "'")[0]["PRO_SAFEQTY"] = ((InventoryOtc)product).Stock.SafeAmount;
               InventoryOtcs.Select("PRO_ID='" + product.Id + "'")[0]["PRO_BASICQTY"] = ((InventoryOtc)product).Stock.BasicAmount;
            }
            else
            {
                product = (sender as MedicineDetail).InventoryMedicine;
                InventoryMedicines.Select("PRO_ID='" + product.Id + "'")[0]["PRO_SAFEQTY"] = ((InventoryMedicine)product).Stock.SafeAmount;
                InventoryMedicines.Select("PRO_ID='" + product.Id + "'")[0]["PRO_BASICQTY"] = ((InventoryMedicine)product).Stock.BasicAmount;
            }
            SearchData();
        }
        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            ProductList.SelectedItem = (sender as DataGridRow).Item;
        }

        private void SearchConditionChanged(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            switch((SearchType)Int16.Parse(radioButton.Tag.ToString()))
            {
                case SearchType.OTC:
                    searchType = SearchType.OTC;
                    InventoryGrid.RowDefinitions[1].Height = new GridLength(50);
                    InventoryGrid.RowDefinitions[2].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(690);
                    break;
                case SearchType.MED:
                    searchType = SearchType.MED;
                    InventoryGrid.RowDefinitions[1].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[2].Height = new GridLength(50);
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(690);
                    break;
                case SearchType.ALL:
                    searchType = SearchType.ALL;
                    InventoryGrid.RowDefinitions[1].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[2].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(740);
                    break;
            }
        }

        private void DeleteKeyUp(object sender, KeyEventArgs e)
        {
            if(sender is null) return;

            if (e.Key == Key.Delete)
                ((TextBox) sender).Text = "";
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchData();
        }
    }
}
