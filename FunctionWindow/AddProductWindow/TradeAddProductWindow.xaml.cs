using System.Data;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// TradeAddProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class TradeAddProductWindow : Window
    {
        public DataRow SelectedProduct { get; set; }

        public TradeAddProductWindow(DataTable dt)
        {
            InitializeComponent();

            ResultGrid.ItemsSource = dt.DefaultView;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)

        {
            System.Windows.Controls.DataGrid dataGrid = sender as System.Windows.Controls.DataGrid;
            dataGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = (DataRowView)ResultGrid.SelectedItem;
            SelectedProduct = drv.Row;
            Close();
        }

        private void ResultGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DataRowView drv = (DataRowView)ResultGrid.SelectedItem;
                SelectedProduct = drv.Row;
                Close();
                return;
            }
            base.OnKeyDown(e);
        }
    }
}