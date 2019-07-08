using System;
using System.Collections.Generic;
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
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl
{
    /// <summary>
    /// StockControl.xaml 的互動邏輯
    /// </summary>
    public partial class StockControl : UserControl
    {
        public StockControl()
        {
            InitializeComponent();
        }

        private void GetStockDetail(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if(textBlock is null) return;

            MainWindow.ServerConnection.OpenConnection();
            (textBlock.DataContext as MedicineControlViewModel).StockDetail.GetStockDetailByID((textBlock.DataContext as MedicineControlViewModel).Medicine.ID, (textBlock.DataContext as MedicineControlViewModel).SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}
