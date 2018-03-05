using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.AbstractClass;
using His_Pos.PrescriptionInquire;
using His_Pos.Class.Product;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView
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
            row?.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() {MouseAction = MouseAction.LeftDoubleClick}));
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
            var detail = new ProductDetail
            {
                ProductName = {Content = selected.Name},
                ProductId = {Content = selected.Id},
                ProductAmount = {Content = selected.Inventory},
                ProductSafeAmount = {Content = selected.SafeAmount},
                ProductManufacturers = {Content = selected.ManufactoryName}
            };
            detail.Show();
        }
    }
}
