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
using GalaSoft.MvvmLight.Messaging;

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
