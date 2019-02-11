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

namespace His_Pos.FunctionWindow.VerifyPharmacyWindow
{
    /// <summary>
    /// VerifyPharmacyWindow.xaml 的互動邏輯
    /// </summary>
    public partial class VerifyPharmacyWindow : Window
    {
        public VerifyPharmacyWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseVerifyPharmacyWindow":
                        Close();
                        break;
                }
            });
            ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}