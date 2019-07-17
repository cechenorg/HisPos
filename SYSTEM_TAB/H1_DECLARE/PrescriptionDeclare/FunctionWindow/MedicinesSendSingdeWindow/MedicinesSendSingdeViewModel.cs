using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow
{
   public class MedicinesSendSingdeViewModel : ViewModelBase
    {
        #region Commad
        public RelayCommand SubmmitCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion
        #region Var
        public bool IsReturn = false;
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
        public void Init() {
            PrescriptionSendData = new PrescriptionSendDatas();
            PrescriptionSendData.ConvertMedToSendData(Prescription.Medicines);
            IsReturn = false;
            SubmmitCommand = new RelayCommand(SubmmitAction);
            CancelCommand = new RelayCommand(CancelAction);
        }
        #region Action 
        public void SubmmitAction() { 
           // PrescriptionDb.SendDeclareOrderToSingde("test",Prescription,PrescriptionSendData);
            Messenger.Default.Send(new NotificationMessage("CloseMedicinesSendSingde"));
        }
        public void CancelAction() {
            IsReturn = true;
            Messenger.Default.Send(new NotificationMessage("CloseMedicinesSendSingde"));
        }
        #endregion

    }
}
