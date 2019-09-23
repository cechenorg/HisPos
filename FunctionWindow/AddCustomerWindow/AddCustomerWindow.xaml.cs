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
            if (e.Key != Key.Enter) return;
            CusIdNumber.Focus();
            CusIdNumber.SelectionStart = 0;
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
