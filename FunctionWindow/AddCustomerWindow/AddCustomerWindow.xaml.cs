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
using His_Pos.FunctionWindow.ErrorUploadWindow;

namespace His_Pos.FunctionWindow.AddCustomerWindow
{
    /// <summary>
    /// AddCustomerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        public AddCustomerWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseAddCustomerWindow":
                        Close();
                        break;
                }
            });
            this.DataContext = new AddCustomerWindowViewModel();
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
