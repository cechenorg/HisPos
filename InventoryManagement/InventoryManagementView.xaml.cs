using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.Class.Product;
using System.Collections;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl
    {
        private ObservableCollection<Product> _dataList = new ObservableCollection<Product>();

        public InventoryManagementView()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _dataList.Clear();

            CompareWithOtc();
            CompareWithMedicine();

            ProductList.ItemsSource = _dataList;
        }

        private void CompareWithMedicine()
        {
            var medicine = MainWindow.MedicineDataTable.Select("HISMED_ID Like '%" + ID.Text + "%' AND PRO_NAME Like '%" + Name.Text + "%'");

            foreach (var m in medicine)
            {
                _dataList.Add(new Medicine(m));
            }
        }

        private void CompareWithOtc()
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

            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            ProductList.SelectedItem = (sender as DataGridRow).Item;
        }
    }
}
