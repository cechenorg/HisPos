using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
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
                    selectedMedicine.IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    selectedMedicine.IsSelected = true;
            }
        }

        private string Note { get; set; }
        private string CusName { get; set; }
        private int PreId { get; set; }

        private bool _isVisibleSingdeNoEnoughMedMessage;

        public bool IsVisibleSingdeNoEnoughMedMessage
        {
            get => _isVisibleSingdeNoEnoughMedMessage;
            set
            {
                Set(() => IsVisibleSingdeNoEnoughMedMessage, ref _isVisibleSingdeNoEnoughMedMessage, value);
            }
        }

        #endregion Variables

        #region Commands

        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand<Window> CreateStoreOrder { get; set; }
        public RelayCommand<Window> Cancel { get; set; }

        #endregion Commands

        public NotEnoughMedicinePurchaseViewModel(string note, string cusName, NotEnoughMedicines purchaseList)
        {
            PurchaseList = purchaseList;
            Note = note;
            CusName = cusName;

            SetIsVisibleSingdeNoEnoughMedMessage(purchaseList);

            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CreateStoreOrder = new RelayCommand<Window>(CreateStoreOrderAction);
            Cancel = new RelayCommand<Window>(CancelAction);
        }

        private void SetIsVisibleSingdeNoEnoughMedMessage(NotEnoughMedicines meds)
        {
            IsVisibleSingdeNoEnoughMedMessage = meds.Count(_ => _.SingdeInv < _.Amount) > 0;


        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, "0" }, "ShowProductDetail"));
        }

        private void CreateStoreOrderAction(Window window)
        {
            MainWindow.SingdeConnection.OpenConnection();
            PurchaseList.CreateOrder(Note, CusName);
            MainWindow.ServerConnection.CloseConnection();

            window.DialogResult = true;
            window.Close();
        }

        private void CancelAction(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}