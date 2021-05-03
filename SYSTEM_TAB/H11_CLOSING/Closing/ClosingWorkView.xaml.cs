using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    /// <summary>
    /// BalanceSheetView.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingWorkView : UserControl
    {
        public ClosingWorkView()
        {
            InitializeComponent();
        }

        private void KeyIn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            textBox.Focus();
            ((TextBox)sender).SelectAll();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OTC_Card.IsChecked==false || OTC_Cash.IsChecked == false|| OTC_Ticket.IsChecked == false || OTC_CashTicket.IsChecked == false || HIS_Cash.IsChecked == false || OTHER_Cash.IsChecked == false ) {
                MessageWindow.ShowMessage("尚未勾選所有關班項目", MessageType.ERROR);
                return;
            }
        }
    }
}