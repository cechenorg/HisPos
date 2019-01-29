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
        private CustomerSelectionViewModel customerSelection { get; set; }
        public CustomerSelectionWindow()
        {
            InitializeComponent();
            customerSelection = new CustomerSelectionViewModel("", 1);
            DataContext = customerSelection;
            if (customerSelection.ShowDialog)
                ShowDialog();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
            });
        }

        public CustomerSelectionWindow(string condition,int option)
        {
            InitializeComponent();
            customerSelection = new CustomerSelectionViewModel(condition, option);
            DataContext = new CustomerSelectionViewModel(condition,option);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
            });
        }

        private void CustomerSelectionWindow_OnClosing(object sender, CancelEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            Messenger.Default.Unregister(this);
            e.Cancel = true;
        }
    }
}
