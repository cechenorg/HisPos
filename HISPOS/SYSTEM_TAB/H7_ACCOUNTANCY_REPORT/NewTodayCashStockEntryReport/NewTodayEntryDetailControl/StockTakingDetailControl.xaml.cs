using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport.NewTodayEntryDetailControl
{
    /// <summary>
    /// PrescriptionDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingDetailControl : UserControl
    {
        public StockTakingDetailControl()
        {
            InitializeComponent();
        }

        /* private void ShowProductDetail(object sender, MouseButtonEventArgs e)
         {
             DataGridRow row = sender as DataGridRow;

             if (row?.Item is null) return;

             ProductDetailWindow.ShowProductDetailWindow();

             Messenger.Default.Send(new NotificationMessage<string>(this, ((StockTakingDetailRecordReport)row.Item).MasterID, "ShowProductDetail"));
         }*/
    }
}