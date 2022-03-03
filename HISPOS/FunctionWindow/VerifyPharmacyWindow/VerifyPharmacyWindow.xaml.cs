using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.FunctionWindow.VerifyPharmacyWindow
{
    /// <summary>
    /// VerifyPharmacyWindow.xaml 的互動邏輯
    /// </summary>
    public partial class VerifyPharmacyWindow : Window
    {
        public VerifyPharmacyWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseVerifyPharmacyWindow":
                        Close();
                        break;
                }
            });
            ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}