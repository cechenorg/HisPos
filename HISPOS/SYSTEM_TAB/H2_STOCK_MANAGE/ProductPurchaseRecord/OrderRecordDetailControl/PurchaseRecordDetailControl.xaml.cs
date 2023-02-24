using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord.OrderRecordDetailControl
{
    /// <summary>
    /// PurchaseRecordDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseRecordDetailControl : UserControl
    {
        public PurchaseRecordDetailControl()
        {
            InitializeComponent();
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((PurchaseProduct)row.Item).ID, ((PurchaseProduct)row.Item).OrderDetailWarehouse.ID.ToString() }, "ShowProductDetail"));
        }
    }
}