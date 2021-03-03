using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.FunctionWindow
{
    /// <summary>
    /// YesNoMessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow(string message, string title, bool? focus = null)
        {
            InitializeComponent();
            DataContext = new ConfirmWindowViewModel(message, title);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "YesAction":
                        DialogResult = true;
                        Close();
                        break;

                    case "NoAction":
                        DialogResult = false;
                        Close();
                        break;
                }
            });
            if (focus != null)
            {
                if ((bool)focus)
                    YesButton.Focus();
                else
                    NoButton.Focus();
            }
            ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}