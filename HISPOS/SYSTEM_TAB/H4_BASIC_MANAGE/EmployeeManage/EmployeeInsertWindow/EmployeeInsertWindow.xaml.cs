using GalaSoft.MvvmLight.Messaging;
using His_Pos.Service;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeInsertWindow
{
    /// <summary>
    /// EmployeeInsertWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeInsertWindow : Window
    {
        public EmployeeInsertWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseEmployeeInsertWindow")
                    Close();
            });
            ShowDialog();
        }

        private void DateMaskedTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}