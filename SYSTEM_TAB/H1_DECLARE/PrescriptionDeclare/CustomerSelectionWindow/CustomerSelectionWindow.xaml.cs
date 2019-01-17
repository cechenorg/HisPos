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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CustomerSelectionWindow
{
    /// <summary>
    /// CustomerSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSelectionWindow : Window
    {
        public CustomerSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new CustomerSelectionViewModel("",1);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
            });
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        public CustomerSelectionWindow(string condition,int option)
        {
            InitializeComponent();
            this.DataContext = new CustomerSelectionViewModel(condition,option);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
            });
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
