using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.StockControl
{
    /// <summary>
    /// MedicineStockControl.xaml 的互動邏輯
    /// </summary>
    public partial class OTCStockControl : UserControl
    {
        public OTCStockControl()
        {
            InitializeComponent();
        }

        private void GetStockDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as OTCStockViewModel).GetStockDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void GetMedBagDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as OTCStockViewModel).GetMedBagDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void GetOnTheWayDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as OTCStockViewModel).GetOnTheWayDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}