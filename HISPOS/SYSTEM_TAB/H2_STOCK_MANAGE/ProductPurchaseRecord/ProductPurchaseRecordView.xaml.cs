using System;
using System.Globalization;
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

        private void StartDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PresetToday(sender);
        }

        private void EndDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PresetToday(sender);
        }
        private void PresetToday(object sender)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox is null) return;
            if (maskedTextBox.Text == "---/--/--")
            {
                DateTime dt = DateTime.Today;
                TaiwanCalendar tc = new TaiwanCalendar();
                string year = tc.GetYear(dt).ToString().PadLeft(3, '0');
                string month = tc.GetMonth(dt).ToString().PadLeft(2, '0');
                string day = tc.GetDayOfMonth(dt).ToString().PadLeft(2, '0');
                string today = string.Format("{0}/{1}/{2}", year, month, day);
                maskedTextBox.Text = today;
            }
        }
    }
}