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

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl
    {
        private readonly ObservableCollection<Product> _dataList = new ObservableCollection<Product>();
        public InventoryManagementView()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            foreach (Otc otc in OTCDb.GetOTC(start.Text, end.Text, Name.Text, ID.Text)) {
                _dataList.Add(otc);
            }
           
            DataGrid.ItemsSource = _dataList;
        }
        

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            row.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
        }

        private ICommand _showCustomDialogCommand;

        private ICommand ShowCustomDialogCommand
        {
            get
            {
                return _showCustomDialogCommand ?? (_showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private void RunCustomFromVm()
        {
            var selected = (Otc)DataGrid.SelectedItem;
            ProductDetail productDetail = new ProductDetail();
            productDetail.ProductName.Content = selected.Name;
            productDetail.ProductId.Content = selected.Id;
            productDetail.ProductAmount.Content = selected.Inventory;
            productDetail.ProductSafeAmount.Content = selected.SafeAmount;
            productDetail.ProductManufacturers.Content = selected.ManufactoryName;
            productDetail.Show();
        }
    }
}
