﻿using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerPrescriptionWindow
{
    /// <summary>
    /// CustomerPrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerPrescriptionWindow : Window
    {
        public CustomerPrescriptionWindow(Customer customer,IcCard card)
        {
            InitializeComponent();
            DataContext = new CustomerPrescriptionViewModel(customer, card);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerPrescriptionWindow"))
                {
                    Close();
                    Messenger.Default.Unregister(this);
                }
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            if (((CustomerPrescriptionViewModel)DataContext).ShowDialog)
                ShowDialog();
            else
            {
                Messenger.Default.Unregister(this);
            }
        }
    }
}
