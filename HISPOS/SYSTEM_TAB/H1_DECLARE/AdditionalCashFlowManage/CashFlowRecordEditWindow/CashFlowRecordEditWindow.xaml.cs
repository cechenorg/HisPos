using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Report.CashFlow;
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

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

        public CashFlowRecordEditWindow(CashFlowRecordDetail selectedDetail, List<CashFlowAccount> CashFlowAccountsSource)
        {
            InitializeComponent();
            DataContext = new CashFlowRecordDetailEditViewModel(selectedDetail, CashFlowAccountsSource);
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