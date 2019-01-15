using System;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionView;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    public class PrescriptionDeclare : TabBase
    {
        public PrescriptionDeclare()
        {
        }

        public override TabBase getTab()
        {
            return this;
        }
        
        private Prescription currentPrescription;
        public Prescription CurrentPrescription
        {
            get => currentPrescription;
            set
            {
                if (currentPrescription != value)
                {
                    Set(() => CurrentPrescription, ref currentPrescription, value);
                }
            }
        }

        #region ShowWindowCommand
        private RelayCommand showCooperativeSelectionWindow;
        public RelayCommand ShowCooperativeSelectionWindow
        {
            get
            {
                if (showCooperativeSelectionWindow == null)
                    showCooperativeSelectionWindow = new RelayCommand(() => ExcuteShowCooperativeWindow());
                return showCooperativeSelectionWindow;

            }
            set { showCooperativeSelectionWindow = value; }
        }

        private void ExcuteShowCooperativeWindow()
        {
            var cooperativeSelectionWindow = new CooperativeSelectionWindow();
            cooperativeSelectionWindow.ShowDialog();
            if(((CooperativeSelectionViewModel)cooperativeSelectionWindow.DataContext).SelectedPrescription != null)
                CurrentPrescription = ((CooperativeSelectionViewModel) cooperativeSelectionWindow.DataContext).SelectedPrescription;
        }
        #endregion 
    }
}
