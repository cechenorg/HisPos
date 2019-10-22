using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow
{
    public class AutoRegisterViewModel : ViewModelBase
    {
        #region Variables
        public Prescription CurrentPrescription { get; }
        public Prescriptions RegisterList { get; set; }
        #endregion

        #region Command
        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion

        public AutoRegisterViewModel(Prescription current, Prescriptions registerList)
        {
            CurrentPrescription = current;
            RegisterList = registerList;
            Submit = new RelayCommand(SubmitAction,CanSubmit);
            Cancel = new RelayCommand(CancelAction);
        }

        private bool CanSubmit()
        {
            return RegisterList.Count(p => p.AdjustDate != null) > 0;
        }

        private void SubmitAction()
        {
            Messenger.Default.Send(new NotificationMessage("AutoRegisterSubmit"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("AutoRegisterCancel"));
        }
    }
}
