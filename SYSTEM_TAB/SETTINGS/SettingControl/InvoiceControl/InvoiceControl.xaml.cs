using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.InvoiceControl
{
    /// <summary>
    /// MyPharmacyControl.xaml 的互動邏輯
    /// </summary>
    public partial class InvoiceControl : UserControl
    {
        public InvoiceControl()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductTransactionView.InvoiceNumLable == null) { return; }

            if (InvoiceNum.Text.Length != 10)
            {
                return;
            }
            if (InvoiceCheck.IsChecked == true)
            {
                ProductTransactionView.InvoiceNumLable.Content = InvoiceNum.Text;
            }
            else
            {
                ProductTransactionView.InvoiceNumLable.Content = "";
            }
        }

        private void InvoiceCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (InvoiceNum == null) { return; }
            if (InvoiceCOM == null) { return; }
            InvoiceNum.IsEnabled = true;
            InvoiceCOM.IsEnabled = true;
        }

        private void InvoiceCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if (InvoiceNum == null) { return; }
            if (InvoiceCOM == null) { return; }
            InvoiceNum.IsEnabled = false;
            InvoiceCOM.IsEnabled = false;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (InvoiceNum == null) { return; }
            if (InvoiceCOM == null) { return; }
            if (InvoiceCheck.IsChecked == true)
            {
                InvoiceNum.IsEnabled = true;
                InvoiceCOM.IsEnabled = true;
            }
            else
            {
                InvoiceNum.IsEnabled = false;
                InvoiceCOM.IsEnabled = false;
            }
        }
    }
}