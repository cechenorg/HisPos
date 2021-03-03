using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

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
            this.ShowDialog();
        }

        public AddCustomerWindow(Customer customer)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseAddCustomerWindow":
                        Close();
                        break;

                    case "SuccessCloseAddCustomerWindow":
                        Close();
                        break;
                }
            });
            this.DataContext = new AddCustomerWindowViewModel(customer);
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            this.ShowDialog();
        }

        private void CusName_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusBirthday.Focus();
            CusBirthday.SelectionStart = 0;
        }

        private void CusBirthday_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                CusIdNumber.Focus();
                CusIdNumber.SelectionStart = 0;
            }
        }

        private void CusIdNumber_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusCellPhone.Focus();
            CusCellPhone.SelectionStart = 0;
        }

        private void CusCellPhone_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusTel.Focus();
            CusTel.SelectionStart = 0;
        }

        private void CusTel_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusAddress.Focus();
            CusAddress.SelectionStart = 0;
        }

        private void CusAddress_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusLine.Focus();
            CusLine.SelectionStart = 0;
        }

        private void CusLine_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            CusEmail.Focus();
            CusEmail.SelectionStart = 0;
        }

        private void CusEmail_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            CusContactNote.Focus();
            CusContactNote.SelectionStart = 0;
        }
    }
}