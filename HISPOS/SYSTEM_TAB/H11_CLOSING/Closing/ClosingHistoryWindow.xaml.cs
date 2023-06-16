using ClosingHistoryViewModelHis_Pos.SYSTEM_TAB.H11_CLOSING.Closing;
using His_Pos.Extention;
using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    /// <summary>
    /// StrikeHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingHistoryWindow : Window
    {
        public ClosingHistoryWindow()
        {
            InitializeComponent();
            DataContext = new ClosingHistoryViewModel();
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