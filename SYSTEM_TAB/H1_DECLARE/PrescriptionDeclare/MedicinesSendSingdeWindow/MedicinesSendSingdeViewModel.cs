using GalaSoft.MvvmLight; 
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product; 

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.MedicinesSendSingdeWindow
{
   public class MedicinesSendSingdeViewModel : ViewModelBase
    {
        #region Var
        private PrescriptionSendDatas prescription;
        public PrescriptionSendDatas Prescription
        {
            get => prescription;
            set
            {
                Set(() => Prescription, ref prescription, value);
            }
        } 
        private string DecMasId { get; set; }
        #endregion
        public MedicinesSendSingdeViewModel(Prescription p) {

        }
    }
}
