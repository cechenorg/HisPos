﻿using System;
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
using His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails;

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
    }
}
