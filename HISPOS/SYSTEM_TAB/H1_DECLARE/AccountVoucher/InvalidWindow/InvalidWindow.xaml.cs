using GalaSoft.MvvmLight.Messaging;
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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.InvalidWindow
{
    /// <summary>
    /// InvalidWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InvalidWindow : Window
    {
        public InvalidWindow()
        {
            InitializeComponent();
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
        }
    }
}
