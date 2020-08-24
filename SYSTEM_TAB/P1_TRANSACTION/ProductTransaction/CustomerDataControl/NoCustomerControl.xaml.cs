using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl
{
    /// <summary>
    /// NoCustomerControl.xaml 的互動邏輯
    /// </summary>
    public partial class NoCustomerControl : UserControl
    {
        public NoCustomerControl()
        {
            InitializeComponent();
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddNewCustomerWindow acw = new AddNewCustomerWindow();
            acw.ShowDialog();
        }
    }
}
