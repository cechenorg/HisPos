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
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.NotEnoughMedicinePurchaseWindow
{
    /// <summary>
    /// NotEnoughMedicinePurchaseWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NotEnoughMedicinePurchaseWindow : Window
    {
        public NotEnoughMedicinePurchaseWindow()
        {
            InitializeComponent();
        }

        public NotEnoughMedicinePurchaseWindow(string note,string cusName,NotEnoughMedicines purchaseList)
        {
            InitializeComponent();
            DataContext = new NotEnoughMedicinePurchaseViewModel(note, cusName, purchaseList);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseNotEnoughMedicinePurchaseWindowPurchase":
                        DialogResult = true;
                        Close();
                        break;
                    case "CloseNotEnoughMedicinePurchaseWindowCancel":
                        DialogResult = false;
                        Close();
                        break;
                }
            });
            ShowDialog();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is NotEnoughMedicine med)) return;
            ((NotEnoughMedicinePurchaseViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }
    }
}
