﻿using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow
{
    /// <summary>
    /// MedicinesSendSingdeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicinesSendSingdeWindow : Window
    {
        public MedicinesSendSingdeWindow(Prescription p)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseMedicinesSendSingde"))
                    Close();
            });
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
            DataContext = new MedicinesSendSingdeViewModel(p);
            ShowDialog();
        }
        
    }
}
