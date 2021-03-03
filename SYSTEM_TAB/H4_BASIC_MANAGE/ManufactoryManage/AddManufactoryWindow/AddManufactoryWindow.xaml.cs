using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage.AddManufactoryWindow
{
    /// <summary>
    /// AddManufactoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddManufactoryWindow : Window
    {
        public AddManufactoryWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddManufactoryWindow") && notificationMessage.Sender is AddManufactoryWindowViewModel)
                    Close();
            });

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}