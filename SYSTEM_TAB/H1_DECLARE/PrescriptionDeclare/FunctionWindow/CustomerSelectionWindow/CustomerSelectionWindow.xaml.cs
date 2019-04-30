using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSelectionWindow
{
    /// <summary>
    /// CustomerSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSelectionWindow : Window
    {
        private CustomerSelectionViewModel customerSelection { get; set; }
        public CustomerSelectionWindow(int condition,Customers customers)
        {
            InitializeComponent();
            customerSelection = new CustomerSelectionViewModel("", condition, customers);
            DataContext = customerSelection;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                {
                    Close();
                }
            });
            if (customerSelection.ShowDialog)
                ShowDialog();
        }

        public CustomerSelectionWindow(string condition,int option,Customers customers)
        {
            InitializeComponent();
            customerSelection = new CustomerSelectionViewModel(condition, option, customers);
            DataContext = customerSelection;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSelection"))
                    Close();
            });
            if (customerSelection.ShowDialog)
                ShowDialog();
        }

        private void CustomerSelectionWindow_OnClosing(object sender, CancelEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            Messenger.Default.Unregister(this);
            e.Cancel = true;
        }
    }
}
