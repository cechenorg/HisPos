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
            if (InvoiceCheck.IsChecked == true)
            {
                ProductTransactionView.InvoiceNumLable.Content = Properties.Settings.Default.InvoiceNumber.ToString();
            }
            else {
                ProductTransactionView.InvoiceNumLable.Content = "";
            }
        }
    }
}
