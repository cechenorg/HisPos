using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.FunctionWindow.ErrorUploadWindow
{
    /// <summary>
    /// ErrorUploadWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ErrorUploadWindow : Window
    {
        public ErrorUploadWindow(bool isGetMedicalNumber)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseErrorUploadWindow":
                        Close();
                        break;
                }
            });
            this.DataContext = new ErrorUploadWindowViewModel(isGetMedicalNumber);
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}