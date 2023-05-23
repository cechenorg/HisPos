using His_Pos.NewClass.StoreOrder;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView
{
    /// <summary>
    /// NormalView.xaml 的互動邏輯
    /// </summary>
    public partial class NormalView : UserControl
    {
        public NormalView()
        {
            InitializeComponent();
        }

        private void StoreOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid?.SelectedItem is null) return;

            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }

        public void Reload(object sender, int i)
        {
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            NormalViewModel normal = new NormalViewModel();
            if (item.DataContext is PurchaseOrder)
            {
                PurchaseOrder order = (PurchaseOrder)item.DataContext;
                normal.CurrentStoreOrder = order;
            }
            if (item.DataContext is ReturnOrder)
            {
                ReturnOrder order = (ReturnOrder)item.DataContext;
                normal.CurrentStoreOrder = order;
            }
            normal.ExportOrderDataAction();
        }
    }
}