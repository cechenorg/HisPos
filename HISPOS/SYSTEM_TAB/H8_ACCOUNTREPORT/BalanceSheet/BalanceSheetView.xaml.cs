using His_Pos.Extention;
using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    /// <summary>
    /// BalanceSheetView.xaml 的互動邏輯
    /// </summary>
    public partial class BalanceSheetView : UserControl
    {
        public BalanceSheetView()
        {
            InitializeComponent();
        }

        private void tb_Value_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void MaskedTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MaskedTextBox textBox = (MaskedTextBox)sender;
            if (textBox != null)
            {
                textBox.Text = DateTimeFormatExtention.ToTaiwanDateTime(DateTime.Today);
            }
        }
    }
}