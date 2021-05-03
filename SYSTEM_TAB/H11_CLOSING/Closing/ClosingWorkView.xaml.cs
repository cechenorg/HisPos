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
    }
}