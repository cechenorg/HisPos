using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking.StockTakingControl
{
    /// <summary>
    /// ResultInputControl.xaml 的互動邏輯
    /// </summary>
    public partial class ResultInputControl : UserControl
    {
        public ResultInputControl()
        {
            InitializeComponent();
        }

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();

            ResultDataGrid.SelectedItem = (textBox.DataContext as StockTakingProduct);
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