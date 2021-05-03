using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.PurchaseDataGridControl
{
    /// <summary>
    /// PurchaseSingdeProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseSingdeProcessingControl : UserControl
    {
        public PurchaseSingdeProcessingControl()
        {
            InitializeComponent();
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is PurchaseProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((PurchaseProduct)cell.DataContext).ID, ((PurchaseProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }
    }
}