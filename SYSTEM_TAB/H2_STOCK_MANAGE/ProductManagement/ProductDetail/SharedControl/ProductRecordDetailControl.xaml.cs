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
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.PrescriptionRefactoring.Service;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl
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
                    ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

                    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, ((ProductInventoryRecord)(sender as DataGridRow).Item).Name, ""));
                    break;
                case ProductInventoryRecordType.Prescription:
                    PrescriptionService.ShowPrescriptionEditWindowRefactoring(int.Parse(((ProductInventoryRecord)(sender as DataGridRow).Item).ID));
                    break;
                case ProductInventoryRecordType.StockTaking:
                    break;
            }
        }
    }
}
