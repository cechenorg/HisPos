using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.OrderDetailControl.PurchaseDataGridControl
{
    /// <summary>
    /// PurchaseSingdeUnProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseSingdeUnProcessingControl : UserControl
    {
        public PurchaseSingdeUnProcessingControl()
        {
            InitializeComponent();
        }

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }

        private void InputTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }
    }
}
