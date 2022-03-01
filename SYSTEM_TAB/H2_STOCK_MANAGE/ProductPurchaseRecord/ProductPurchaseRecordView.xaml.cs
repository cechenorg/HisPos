using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    /// <summary>
    /// ProductPurchaseRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseRecordView : UserControl
    {
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
        }

        private void StartDate_OnKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;

            if (maskedTextBox is null) return;

            if (e.Key == Key.Enter)
            {
                EndDate.Focus();
                EndDate.Select(0, 0);
            }
        }
    }
}