﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.FromSourceWindow
{
    /// <summary>
    /// FromSourceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class FromSourceWindow : Window
    {
        public FromSourceWindow(DataTable table)
        {
            InitializeComponent();
            ((FromSourceViewModel)DataContext).SoureTable = table;
            ((FromSourceViewModel)DataContext).FilterAction();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "YesAction":
                        ((FromSourceViewModel)DataContext).IsSubmit = true;
                        Close();
                        break;

                    case "NoAction":
                        ((FromSourceViewModel)DataContext).IsSubmit = false;
                        Close();
                        break;
                }
            });
        }
    }
}
