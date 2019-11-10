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
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CreateStoreOrder { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion

        public NotEnoughMedicinePurchaseViewModel(string note,NotEnoughMedicines purchaseList)
        {
            PurchaseList = purchaseList;
            Note = note;
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CreateStoreOrder = new RelayCommand(CreateStoreOrderAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, "0" }, "ShowProductDetail"));
        }

        private void CreateStoreOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            var result = StoreOrderDB.InsertNotEnoughPurchaseOrder(PurchaseList, Note);
            if (result.Rows.Count > 0)
            {
                PurchaseList.ToWaitingStatus(Note);
            }
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindowPurchase"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseNotEnoughMedicinePurchaseWindowCancel"));
        }
    }
}
