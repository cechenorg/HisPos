using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PrescriptionSendData;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow
{
   public class MedicinesSendSingdeViewModel : ViewModelBase
    {
        #region Commad
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion
        #region Var
        public bool IsReturn = false;
        private bool isAllSend;
        public bool IsAllSend
        {
            get => isAllSend;
            set
            {
                Set(() => IsAllSend, ref isAllSend, value);
                if (PrescriptionSendData is null) return;
                PrescriptionSendData.ConvertMedToSendData(Prescription.Medicines, Prescription.ID, IsAllSend);
            }
        }
        private PrescriptionSendDatas prescriptionSendData;
        public PrescriptionSendDatas PrescriptionSendData
        {
            get => prescriptionSendData;
            set
            {
                Set(() => PrescriptionSendData, ref prescriptionSendData, value);
            }
        }
        private Prescription Prescription;
        private string DecMasId { get; set; }
        #endregion
        public MedicinesSendSingdeViewModel(Prescription p)
        {
            Prescription = p;
            Init();
        }

        private void Init() {
            PrescriptionSendData = new PrescriptionSendDatas();
            PrescriptionSendData.ConvertMedToSendData(Prescription.Medicines,Prescription.ID,IsAllSend);
            IsReturn = false;
            SubmitCommand = new RelayCommand(SubmitAction);
            CancelCommand = new RelayCommand(CancelAction);
        }
        #region Action 

        private void SubmitAction() { 
           // PrescriptionDb.SendDeclareOrderToSingde("test",Prescription,PrescriptionSendData);
            Messenger.Default.Send(new NotificationMessage("CloseMedicinesSendSingde"));
        }

        private void CancelAction() {
            IsReturn = true;
            Messenger.Default.Send(new NotificationMessage("CloseMedicinesSendSingde"));
        }
        #endregion

    }
}
