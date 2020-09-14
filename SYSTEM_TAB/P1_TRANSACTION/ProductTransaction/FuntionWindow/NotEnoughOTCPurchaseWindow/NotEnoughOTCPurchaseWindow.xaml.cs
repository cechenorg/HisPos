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
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.NotEnoughOTCPurchaseWindow;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.FunctionWindow.NotEnoughOTCPurchaseWindow
{
    /// <summary>
    /// NotEnoughMedicinePurchaseWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NotEnoughOTCPurchaseWindow : Window
    {
        public NotEnoughOTCPurchaseWindow()
        {
            InitializeComponent();
        }

        public NotEnoughOTCPurchaseWindow(string note,string cusName,NotEnoughMedicines purchaseList)
        {
            InitializeComponent();
            DataContext = new NotEnoughOTCPurchaseViewModel(note, cusName, purchaseList);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseNotEnoughOTCPurchaseWindowPurchase":
                        DialogResult = true;
                        //Close();
                        break;
                    case "CloseNotEnoughOTCPurchaseWindowCancel":
                        //DialogResult = false;
                        Close();
                        break;
                }
            });
            ShowDialog();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is NotEnoughMedicine med)) return;
            ((NotEnoughOTCPurchaseViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }
    }
}
