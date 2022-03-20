using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport.EntryDetailControl
{
    /// <summary>
    /// PrescriptionDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class RewardDetailControl : UserControl
    {
        public RewardDetailControl()
        {
            InitializeComponent();
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            // Messenger.Default.Send(new NotificationMessage<string>(this, ((TradeProfitDetailRecordReport)row.Item).MasterID, "ShowProductDetail"));
        }
    }
}