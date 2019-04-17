using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    /// <summary>
    /// MedicineControlView.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineControlView : UserControl
    {
        public MedicineControlView()
        {
            InitializeComponent();
        }

        private void ShowRecordDetail(object sender, MouseButtonEventArgs e)
        {
            if(sender is null) return;

            switch (((ProductInventoryRecord)(sender as DataGridRow).Item).Type)
            {
                case ProductInventoryRecordType.PurchaseReturn:
                    ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

                    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, ((ProductInventoryRecord)(sender as DataGridRow).Item).ID, "" ));
                    break;
                case ProductInventoryRecordType.Prescription:
                    PrescriptionEditWindow prescriptionEditWindow = new PrescriptionEditWindow(int.Parse(((ProductInventoryRecord)(sender as DataGridRow).Item).ID));
                    prescriptionEditWindow.ShowDialog();
                    break;
                case ProductInventoryRecordType.StockTaking:
                    break;
            }
        }
    }
}
