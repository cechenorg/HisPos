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

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl, INotifyPropertyChanged
    {
        public DataTable InventoryMedicines;
        public DataTable InventoryOtcs;
        private SearchType searchType = SearchType.ALL;
        private double selectStockValue = 0;
        private string selectProductId = string.Empty;
        private ObservableCollection<Product> _dataList = new ObservableCollection<Product>();
        public ObservableCollection<Product> _DataList 
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                NotifyPropertyChanged("_DataList");
            }
        }

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

            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.MergeProductInventory(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;

            DataContext = this;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchData();
        }

        public void SearchData()
        {
            _DataList.Clear();
            selectStockValue = 0;

            switch (searchType)
            {
                case SearchType.OTC:
                    AddOtcResult();
                    break;
                case SearchType.MED:
                    AddMedicineResult();
                    break;
                case SearchType.ALL:
                    AddOtcResult();
                    AddMedicineResult();
                    break;
            }

            SearchCount.Content = _DataList.Count.ToString();
            SelectStockValue.Content = selectStockValue.ToString("0.#");
            ProductList.ItemsSource = _DataList;
        }

        private void AddMedicineResult()
        {
            string searchCondition = "PRO_ID Like '%" + ID.Text + "%' AND PRO_NAME Like '%" + Name.Text + "%'";

            if (ControlMed.IsChecked == true )
            {
                searchCondition += "AND HISMED_CONTROL = " + ControlMed.IsChecked;
            }

            if(FreezeMed.IsChecked == true)
            {
                searchCondition += " AND HISMED_FROZ = " + FreezeMed.IsChecked;
            }
            if (BelowSafeAmount.IsChecked is true)
                searchCondition += " AND PRO_INVENTORY <= PRO_SAFEQTY";

            if (IsStop.IsChecked is true)
                searchCondition += " AND PRO_STATUS == '0'";
            else
                searchCondition += " AND PRO_STATUS == '1'";

            var medicines = InventoryMedicines.Select(searchCondition);
            
            foreach (var m in medicines)
            {
                InventoryMedicine medicine = new InventoryMedicine(m);

                _DataList.Add(medicine);

                selectStockValue += Double.Parse(medicine.StockValue);
            }
        }

        private void AddOtcResult()
        {
            string condition = "PRO_ID Like '%" + ID.Text + "%' AND PRO_NAME Like '%" + Name.Text + "%'";
            if (BelowSafeAmount.IsChecked is true)
                condition += " AND PRO_INVENTORY <= PRO_SAFEQTY";
            if (IsStop.IsChecked is true)
                condition += " AND PRO_STATUS = '0'";
            else
                condition += " AND PRO_STATUS = '1'";

            var otcs = InventoryOtcs.Select(condition);
            
            foreach (var o in otcs)
            {
                InventoryOtc otc = new InventoryOtc(o);

                _DataList.Add(otc);

                selectStockValue += Double.Parse(otc.StockValue);
            }
        }

        private void showProductDetail(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
            selectProductId = ((Product)selectedItem).Id;
            if (selectedItem is InventoryOtc)
            {
                OtcDetail productDetail = new OtcDetail((InventoryOtc)selectedItem);
                
                productDetail.mouseButtonEventHandler += ComfirmChangeButtonOnMouseLeftButtonUp;
               
                productDetail.Show();
            }
            else if (selectedItem is InventoryMedicine)
            {
                MedicineDetail medcineDetail = new MedicineDetail((InventoryMedicine)selectedItem);
                medcineDetail.mouseButtonEventHandler += ComfirmChangeButtonOnMouseLeftButtonUp;
                medcineDetail.Show();
            }
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
    }
}
