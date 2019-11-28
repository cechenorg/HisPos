using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using His_Pos.Service;
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
