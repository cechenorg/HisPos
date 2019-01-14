using System;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CustomerSelectionWindow;
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
                    showCooperativeSelectionWindow = new RelayCommand(() => ExecuteShowCooperativeWindow());
                return showCooperativeSelectionWindow;

            }
            set { showCooperativeSelectionWindow = value; }
        }
        private void ExecuteShowCooperativeWindow()
        {
            var cooperativeSelectionWindow = new CooperativeSelectionWindow.CooperativeSelectionWindow();
            cooperativeSelectionWindow.ShowDialog();
            if(((CooperativeSelectionViewModel)cooperativeSelectionWindow.DataContext).SelectedPrescription != null)
                CurrentPrescription = ((CooperativeSelectionViewModel) cooperativeSelectionWindow.DataContext).SelectedPrescription;
        }

        private RelayCommand showCustomerSelectionWindow;
        public RelayCommand ShowCustomerSelectionWindow
        {
            get =>
                showCustomerSelectionWindow ??
                (showCustomerSelectionWindow = new RelayCommand(ExecuteShowCustomerWindow));
            set => showCooperativeSelectionWindow = value;
        }
        private void ExecuteShowCustomerWindow()
        {
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow();
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        #endregion 
    }
}
