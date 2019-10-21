using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow
{
    /// <summary>
    /// CustomerSearchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSearchWindow : Window
    {
        private CustomerSearchViewModel customerSearchViewModel { get; set; }
        public CustomerSearchWindow(CustomerSearchCondition condition,string search = null)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) => 
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSearchWindow"))
                    Close();
            });
            customerSearchViewModel = new CustomerSearchViewModel(search, condition);
            DataContext = customerSearchViewModel;
            SearchStringTextBox.Focus();
            if (customerSearchViewModel.ShowDialog)
                ShowDialog();
        }

        public CustomerSearchWindow(DateTime? birth)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSearchWindow"))
                    Close();
            });
            customerSearchViewModel = new CustomerSearchViewModel(birth);
            DataContext = customerSearchViewModel;
            if (customerSearchViewModel.ShowDialog)
                ShowDialog();
        }
    }
}
