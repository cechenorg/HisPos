using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction;
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
            else {
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
            else {
                InvoiceNum.IsEnabled = false;
                InvoiceCOM.IsEnabled = false;
            }
        }
    }
}
