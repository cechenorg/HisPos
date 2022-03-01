using His_Pos.Service;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage
{
    /// <summary>
    /// CustomerManageControl.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerManageControl : UserControl
    {
        public CustomerManageControl()
        {
            InitializeComponent();
        }

        private void PatientBirthday_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }
    }
}