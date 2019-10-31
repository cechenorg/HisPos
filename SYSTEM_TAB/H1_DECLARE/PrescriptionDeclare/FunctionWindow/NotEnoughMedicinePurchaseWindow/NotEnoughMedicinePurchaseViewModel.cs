using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
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
        private NotEnoughMedicine selectedMedicine;
        public NotEnoughMedicine SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = true;
            }
        }
        private string Note { get; set; }
        #endregion

        #region Commands
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CreateStoreOrder { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion

        public NotEnoughMedicinePurchaseViewModel(string note,NotEnoughMedicines purchaseList)
        {
            PurchaseList = purchaseList;
            Note = note;
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CreateStoreOrder = new RelayCommand(CreateStoreOrderAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void DeleteMedicineAction()
        {
            if (PurchaseList.Count == 1)
            {
                var deleteConfirm = new ConfirmWindow("訂單只剩此品項，刪除後將自動關閉視窗，確認刪除?","刪除品項確認");
                if(!(bool)deleteConfirm.DialogResult) 
                    return;
            }
            PurchaseList.Remove(SelectedMedicine);
            if(PurchaseList.Count == 0)
                Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindow"));
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, "0" }, "ShowProductDetail"));
        }

        private void CreateStoreOrderAction()
        {
            var orderProList = new NotEnoughMedicines(false);
            foreach (var p in PurchaseList.Where(p => p.Amount > 0))
            {
                orderProList.Add(p);
            }
            if (orderProList.Count > 0)
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                var result = StoreOrderDB.InsertNotEnoughPurchaseOrder(orderProList,Note);
                if (result.Rows.Count > 0)
                {
                    PurchaseList.ToWaitingStatus(Note);
                }
                MainWindow.ServerConnection.CloseConnection();
                MainWindow.SingdeConnection.CloseConnection();
            }
            else
                MessageWindow.ShowMessage("訂單已無訂購量大於0的品項。",MessageType.WARNING);
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindow"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindow"));
        }
    }
}
