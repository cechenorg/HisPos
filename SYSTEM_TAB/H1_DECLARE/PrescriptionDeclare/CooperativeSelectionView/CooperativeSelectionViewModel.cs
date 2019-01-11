using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionView
{
    public class CooperativeSelectionViewModel : ViewModelBase
    {
        #region Property

        private Prescriptions cooperativePrescriptions;

        public Prescriptions CooperativePrescriptions
        {
            get => cooperativePrescriptions;
            set { cooperativePrescriptions = value; RaisePropertyChanged(() => CooperativePrescriptions); }
        }

        private Prescription selectedPrescription;
        public Prescription SelectedPrescription
        {
            get => selectedPrescription;
            set { selectedPrescription = value; RaisePropertyChanged(() => SelectedPrescription); }
        }

        #endregion
        #region Command

        private RelayCommand prescriptionSelectedCommand;
        public RelayCommand PrescriptionSelectedCommand
        {
            get
            {
                if (prescriptionSelectedCommand == null)
                    prescriptionSelectedCommand = new RelayCommand(() => ExcutePrescriptionSelectedCommand());
                return prescriptionSelectedCommand;

            }
            set => prescriptionSelectedCommand = value;
        }

        private void ExcutePrescriptionSelectedCommand()
        {

        }

        #endregion
        public CooperativeSelectionViewModel()
        {
            CooperativePrescriptions.GetCooperativePrescriptions();
        }
    }
}
