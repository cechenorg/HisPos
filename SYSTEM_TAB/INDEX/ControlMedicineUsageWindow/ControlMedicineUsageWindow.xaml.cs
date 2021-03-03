using His_Pos.Service;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.INDEX.ControlMedicineUsageWindow
{
    /// <summary>
    /// ControlMedicineUsageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ControlMedicineUsageWindow : Window
    {
        public ControlMedicineUsageWindow()
        {
            InitializeComponent();
            DataContext = new ControlMedicineUsageViewModel();
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}