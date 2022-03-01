using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.SameDeclarePrescriptions;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.SameDeclareConfirmWindow
{
    public class SameDeclareConfirmViewModel : ViewModelBase
    {
        public bool Result { get; set; }
        public string Patient { get; }
        public SameDeclarePrescriptions Prescriptions { get; }
        private SameDeclarePrescription selectedPrescription;

        public SameDeclarePrescription SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
                if (selectedPrescription != null)
                {
                    SelectedPrescription.GetMedicines();
                }
            }
        }

        public RelayCommand Back { get; set; }
        public RelayCommand Continue { get; set; }

        public SameDeclareConfirmViewModel(SameDeclarePrescriptions pres)
        {
            Prescriptions = pres;
            Patient = "姓名 :" + pres[0].PatientName + " 身分證 :" + pres[0].PatientIDNumber + " 重複處方";
            Back = new RelayCommand(BackAction);
            Continue = new RelayCommand(ContinueAction);
        }

        private void BackAction()
        {
            Result = false;
            Messenger.Default.Send(new NotificationMessage("CloseSameDeclareConfirmWindow"));
        }

        private void ContinueAction()
        {
            var c = new ConfirmWindow("此病患已有內容重複處方，可能有重複申報疑慮，確認繼續調劑?", "", false);
            Result = (bool)c.DialogResult;
            Messenger.Default.Send(new NotificationMessage("CloseSameDeclareConfirmWindow"));
        }
    }
}