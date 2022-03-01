using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StoreOrder.Report;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    /// <summary>
    /// PurchaseReturnReportControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseReturnReportControl : UserControl
    {
        public PurchaseReturnReportControl()
        {
            InitializeComponent();
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            if (((ManufactoryOrderDetail)(sender as DataGridRow).Item).ID is null) return;

            ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

            Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, ((ManufactoryOrderDetail)(sender as DataGridRow).Item).ID, ""));
        }
    }
}