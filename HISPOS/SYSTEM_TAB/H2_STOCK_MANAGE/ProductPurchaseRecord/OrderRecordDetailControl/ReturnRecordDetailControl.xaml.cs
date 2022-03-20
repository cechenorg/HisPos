using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord.OrderRecordDetailControl
{
    /// <summary>
    /// ReturnRecordDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnRecordDetailControl : UserControl
    {
        public ReturnRecordDetailControl()
        {
            InitializeComponent();
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ReturnProduct)row.Item).ID, ((ReturnProduct)row.Item).WareHouseID.ToString() }, "ShowProductDetail"));
        }
    }
}