using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.StockControl
{
    /// <summary>
    /// MedicineStockControl.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineStockControl : UserControl
    {
        public MedicineStockControl()
        {
            InitializeComponent();
        }

        private void GetStockDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as MedicineStockViewModel).GetStockDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void GetMedBagDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as MedicineStockViewModel).GetMedBagDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void GetOnTheWayDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as MedicineStockViewModel).GetOnTheWayDetailByID();
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}