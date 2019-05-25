﻿using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow
{
    /// <summary>
    /// CooperativePrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativePrescriptionWindow : Window
    {

        public CooperativePrescriptionWindow()
        {
            InitializeComponent();
            DataContext = new CooperativePrescriptionViewModel();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCooperativePrescriptionWindow"))
                    Close();
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            ShowDialog();
        }
    }
}
