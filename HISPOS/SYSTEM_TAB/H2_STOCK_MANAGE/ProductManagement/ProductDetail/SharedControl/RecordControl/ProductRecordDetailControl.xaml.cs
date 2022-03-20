using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.RecordControl
{
    /// <summary>
    /// ProductRecordDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class ProductRecordDetailControl : UserControl
    {
        public ProductRecordDetailControl()
        {
            InitializeComponent();
        }

        private void ShowRecordDetail(object sender, MouseButtonEventArgs e)
        {
            if (sender is null) return;

            switch (((ProductInventoryRecord)(sender as DataGridRow).Item).Type)
            {
                case ProductInventoryRecordType.PurchaseReturn:
                    if (((ProductInventoryRecord)(sender as DataGridRow).Item).Name is null) return;

                    ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

                    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, ((ProductInventoryRecord)(sender as DataGridRow).Item).Name, ""));
                    break;

                case ProductInventoryRecordType.Prescription:
                    PrescriptionService.ShowPrescriptionEditWindow(int.Parse(((ProductInventoryRecord)(sender as DataGridRow).Item).ID));
                    break;

                case ProductInventoryRecordType.StockTaking:
                    break;
            }
        }
    }
}