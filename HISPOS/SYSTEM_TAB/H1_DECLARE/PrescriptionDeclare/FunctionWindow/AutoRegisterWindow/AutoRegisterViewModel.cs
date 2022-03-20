using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using System;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow
{
    public class AutoRegisterViewModel : ViewModelBase
    {
        #region Variables

        public Prescription CurrentPrescription { get; }
        public Prescriptions RegisterList { get; set; }

        #endregion Variables

        #region Command

        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }

        #endregion Command

        public AutoRegisterViewModel(Prescription current, Prescriptions registerList)
        {
            CurrentPrescription = current;
            RegisterList = registerList;
            Submit = new RelayCommand(SubmitAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void SubmitAction()
        {
            if (RegisterList.Count(p => p.AdjustDate != null) == 0)
            {
                MessageWindow.ShowMessage("請填寫欲一併登錄處方之調劑日。", MessageType.ERROR);
                return;
            }
            var adjustDateOutOfRange = false;
            foreach (var r in RegisterList)
            {
                if (r.AdjustDate != null && DateTime.Compare((DateTime)r.AdjustDate, (DateTime)CurrentPrescription.AdjustDate) < 0)
                    adjustDateOutOfRange = true;
            }
            if (adjustDateOutOfRange)
            {
                MessageWindow.ShowMessage("自動登錄之預約處方調劑日不可小於目前處方", MessageType.ERROR);
                return;
            }
            Messenger.Default.Send(new NotificationMessage("AutoRegisterSubmit"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("AutoRegisterCancel"));
        }
    }
}