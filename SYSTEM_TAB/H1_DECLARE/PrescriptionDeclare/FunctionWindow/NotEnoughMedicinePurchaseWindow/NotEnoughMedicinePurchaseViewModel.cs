﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.NotEnoughMedicinePurchaseWindow
{
    public class NotEnoughMedicinePurchaseViewModel : ViewModelBase
    {
        #region Variables
        private NotEnoughMedicines purchaseList;
        public NotEnoughMedicines PurchaseList
        {
            get => purchaseList;
            private set
            {
                Set(() => PurchaseList, ref purchaseList, value);
            }
        }
        private string wareHouseID { get; set; }
        private string Note { get; set; }
        #endregion

        #region Commands
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CreateStoreOrder { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion

        public NotEnoughMedicinePurchaseViewModel(string wareID,string note,NotEnoughMedicines purchaseList)
        {
            wareHouseID = wareID;
            PurchaseList = purchaseList;
            Note = note;
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CreateStoreOrder = new RelayCommand(CreateStoreOrderAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, wareHouseID }, "ShowProductDetail"));
        }

        private void CreateStoreOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            var result = StoreOrderDB.InsertNotEnoughPurchaseOrder(purchaseList,Note);
            if (result.Rows.Count > 0)
            {
                purchaseList.ToWaitingStatus(Note);
            }
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindow"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindow"));
        }
    }
}
