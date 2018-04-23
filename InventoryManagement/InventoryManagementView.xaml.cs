using System.Collections.ObjectModel;
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
        public double selectStockValue = 0;
        public double searchCount = 0;
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
                        if(
                            (((InventoryOtc)item).Id.Contains(ID.Text) || ID.Text == string.Empty) //ID filter
                        && (((InventoryOtc)item).Name.Contains(Name.Text) ||  Name.Text == string.Empty) //Name filter
                       && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter
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
                        && (((InventoryMedicine)item).Name.Contains(Name.Text) || Name.Text == string.Empty) //Name filter
                        && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter
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
                           && (((Product)item).Name.Contains(Name.Text) || Name.Text == string.Empty) //Name filter
                           && ((((IInventory)item).Status && !(bool)IsStop.IsChecked) || (!((IInventory)item).Status && (bool)IsStop.IsChecked)) //Status filter
                        && (((((IInventory)item).Stock.Inventory <= Convert.ToDouble(((IInventory)item).Stock.SafeAmount)) && (bool)BelowSafeAmount.IsChecked) || !(bool)BelowSafeAmount.IsChecked) // SafeAmount filter
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
