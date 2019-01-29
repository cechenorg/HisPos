using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSelectionWindow
{
    /// <summary>
    /// CustomerSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSelectionWindow : Window
    {
        public CustomerSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new CustomerSelectionViewModel("", 1);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
                if (notificationMessage.Notification.Equals("ShowCustomerSelection"))
                    ShowDialog();
            });
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
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
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
        }

        private void CustomerSelectionWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
        }
    }
}
