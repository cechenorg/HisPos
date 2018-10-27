using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase
{
    /// <summary>
    /// BatchNumberDialog.xaml 的互動邏輯
    /// </summary>
    public partial class BatchNumberDialog : Window
    {
        public Collection<ReturnControl.BatchNumOverview> BatchNumOverviews { get; set; }
        public bool IsConfirmClicked { get; set; } = false;
        
        public BatchNumberDialog(Collection<ReturnControl.BatchNumOverview> batchNumOverviews)
        {
            InitializeComponent();
            DataContext = this;

            BatchNumOverviews = batchNumOverviews;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if(!IsReturnAmountValid())
            {
                MessageWindow messageWindow = new MessageWindow("退貨量不可高於庫存量!", Class.MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return;
            }

            IsConfirmClicked = true;
            Close();
        }

        private bool IsReturnAmountValid()
        {
            foreach(var batch in BatchNumOverviews)
            {
                if (batch.SelectedAmount > batch.Amount)
                    return false;
            }

            return true;
        }

        private void ReturnAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(!IsKeyAvailable(e.Key))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (textBox.Text.Equals("0") && IsNumbers(e.Key))
                textBox.Text = "";
            else if(textBox.Text.Length == 1 && (e.Key == Key.Back || e.Key == Key.Delete))
            {
                textBox.Text = "0";
                e.Handled = true;
            }
        }

        private bool IsNumbers(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;

            return false;
        }

        private bool IsKeyAvailable(Key key)
        {
            if (IsNumbers(key)) return true;
            if (key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right || key == Key.OemPeriod || key == Key.Decimal) return true;

            return false;
        }
    }
}
