using System;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    /// <summary>
    /// TransferControl.xaml 的互動邏輯
    /// </summary>
    public partial class BankControl : UserControl
    {
        public BankControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }
    }
}
