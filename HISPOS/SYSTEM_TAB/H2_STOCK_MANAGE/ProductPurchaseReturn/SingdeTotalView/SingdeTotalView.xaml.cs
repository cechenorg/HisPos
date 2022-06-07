using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.SingdeTotalView
{
    /// <summary>
    /// SingdeTotalView.xaml 的互動邏輯
    /// </summary>
    public partial class SingdeTotalView : UserControl
    {
        public SingdeTotalView()
        {
            InitializeComponent();
        }

        private void StoreOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid?.SelectedItem is null) return;

            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }
    }
}