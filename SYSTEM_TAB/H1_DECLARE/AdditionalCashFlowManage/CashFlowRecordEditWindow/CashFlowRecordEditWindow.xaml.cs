using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage.CashFlowRecordEditWindow
{
    /// <summary>
    /// CashFlowRecordEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CashFlowRecordEditWindow : Window
    {
        public bool EditResult { get; set; }

        public CashFlowRecordEditWindow()
        {
            InitializeComponent();
        }

        public CashFlowRecordEditWindow(CashFlowRecordDetail selectedDetail)
        {
            InitializeComponent();
            DataContext = new CashFlowRecordDetailEditViewModel(selectedDetail);
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