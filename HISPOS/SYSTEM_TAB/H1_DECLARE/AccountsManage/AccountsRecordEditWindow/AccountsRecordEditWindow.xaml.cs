using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage.AccountsRecordEditWindow
{
    /// <summary>
    /// CashFlowRecordEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AccountsRecordEditWindow : Window
    {
        public bool EditResult { get; set; }

        public AccountsRecordEditWindow()
        {
            InitializeComponent();
        }

        public AccountsRecordEditWindow(AccountsRecordDetail selectedDetail)
        {
            InitializeComponent();
            DataContext = new AccountsRecordDetailEditViewModel(selectedDetail);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CashFlowRecordDetailEditSubmit":
                        EditResult = true;
                        Close();
                        break;

                    case "CashFlowRecordDetailEditCancel":
                        EditResult = false;
                        Close();
                        break;
                }
            });
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}