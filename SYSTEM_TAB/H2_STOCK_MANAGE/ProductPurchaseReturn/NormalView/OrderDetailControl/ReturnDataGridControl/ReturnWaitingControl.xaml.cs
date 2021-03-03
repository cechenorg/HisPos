using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.ReturnDataGridControl
{
    /// <summary>
    /// ReturnWaitingControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnWaitingControl : UserControl
    {
        public ReturnWaitingControl()
        {
            InitializeComponent();
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is ReturnProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ReturnProduct)cell.DataContext).ID, ((ReturnProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }
    }
}