using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Class;
using System;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl
    {
        private ObservableCollection<Product> _dataList = new ObservableCollection<Product>();
        SearchType searchType = SearchType.ALL;

        public InventoryManagementView()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _dataList.Clear();

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

            ProductList.ItemsSource = _dataList;
        }

        private void AddMedicineResult()
        {
            var medicine = MainWindow.MedicineDataTable.Select("HISMED_ID Like '%" + ID.Text + "%' AND PRO_NAME Like '%" + Name.Text + "%'");

            foreach (var m in medicine)
            {
                _dataList.Add(new Medicine(m));
            }
        }

        private void AddOtcResult()
        {
            var otc = MainWindow.OtcDataTable.Select("PRO_ID Like '%" + ID.Text + "%' AND PRO_NAME Like '%" + Name.Text + "%'");

            foreach (var o in otc)
            {
                _dataList.Add(new Otc(o));
            }
        }

        private void showProductDetail(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is Otc )
            {
                OtcDetail productDetail = new OtcDetail((Otc)selectedItem);
                productDetail.Show();
            }
            else if (selectedItem is Medicine )
            {
                MedicineDetail medcineDetail = new MedicineDetail((Medicine)selectedItem);
                medcineDetail.Show();
            }
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
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(740);
                    break;
                case SearchType.MED:
                    searchType = SearchType.MED;
                    InventoryGrid.RowDefinitions[1].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[2].Height = new GridLength(50);
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(740);
                    break;
                case SearchType.ALL:
                    searchType = SearchType.ALL;
                    InventoryGrid.RowDefinitions[1].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[2].Height = new GridLength(0);
                    InventoryGrid.RowDefinitions[3].Height = new GridLength(790);
                    break;
            }
        }
    }
}
