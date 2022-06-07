using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow
{
    /// <summary>
    /// CustomerSearchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSearchWindow : Window
    {
        private CustomerSearchViewModel customerSearchViewModel { get; set; }

        public CustomerSearchWindow(CustomerSearchCondition condition, string search = null)
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

        public CustomerSearchWindow(CustomerSearchCondition condition, int phone, string search = null)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSearchWindow"))
                    Close();
            });
            customerSearchViewModel = new CustomerSearchViewModel(search, condition, 0);
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

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            FocusSearchTextBox();
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FocusSearchTextBox();
        }

        private void FocusSearchTextBox()
        {
            if (!SearchStringTextBox.IsFocused)
                SearchStringTextBox.Focus();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FocusSearchTextBox();
        }
    }
}