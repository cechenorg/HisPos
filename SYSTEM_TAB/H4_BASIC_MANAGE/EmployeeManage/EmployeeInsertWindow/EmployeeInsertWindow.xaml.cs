﻿using GalaSoft.MvvmLight.Messaging;
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

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeInsertWindow
{
    /// <summary>
    /// EmployeeInsertWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeInsertWindow : Window
    {
        public EmployeeInsertWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseEmployeeInsertWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}
