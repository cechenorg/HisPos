using System;
using System.ComponentModel;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CustomerSelectionWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    public class PrescriptionDeclareViewModel : TabBase
    {
        public PrescriptionDeclareViewModel()
        {
            CurrentPrescription = new Prescription();
            Messenger.Default.Register<Customer>(this, "SelectedCustomer",GetSelectedCustomer);
            Messenger.Default.Register<Prescription>(this, "SelectedPrescription", GetSelectedPrescription);
        }

        ~PrescriptionDeclareViewModel()
        {
            Messenger.Default.Unregister(this);
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

        private string tempMedicalNumber;
        public string TempMedicalNumber
        {
            get => tempMedicalNumber;
            set
            {
                if (tempMedicalNumber != value)
                {
                    Set(() => TempMedicalNumber, ref tempMedicalNumber, value);
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                Set(() => IsBusy, ref _isBusy, value);
            }
        }
        private string _busyContent;

        public string BusyContent
        {
            get => _busyContent;
            private set
            {
                Set(() => BusyContent, ref _busyContent, value);
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
            var cooperativePrescriptions = new Prescriptions();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得合作診所處方...";
                cooperativePrescriptions.GetCooperativePrescriptions(MainWindow.CurrentPharmacy.Id, DateTime.Today, DateTime.Today);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                var cooperativeSelectionWindow = new CooperativeSelectionWindow.CooperativeSelectionWindow();
                Messenger.Default.Send(cooperativePrescriptions, "CooperativePrescriptions");
                cooperativeSelectionWindow.ShowDialog();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
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
        // ReSharper disable once InconsistentNaming
        private RelayCommand searchCustomerByIDNumber;
        // ReSharper disable once InconsistentNaming
        public RelayCommand SearchCustomerByIDNumber
        {
            get =>
                searchCustomerByIDNumber ??
                (searchCustomerByIDNumber = new RelayCommand(ExecuteSearchCustomerByIDNumber));
            set => searchCustomerByIDNumber = value;
        }
        private void ExecuteSearchCustomerByIDNumber()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.IDNumber,3);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private RelayCommand searchCustomerByName;
        public RelayCommand SearchCustomerByName
        {
            get =>
                searchCustomerByName ??
                (searchCustomerByName = new RelayCommand(ExecuteSearchCustomerByName));
            set => searchCustomerByName = value;
        }
        private void ExecuteSearchCustomerByName()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Name, 2);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private RelayCommand searchCustomerByBirthday;
        public RelayCommand SearchCustomerByBirthday
        {
            get =>
                searchCustomerByBirthday ??
                (searchCustomerByBirthday = new RelayCommand(ExecuteSearchCustomerByBirthday));
            set => searchCustomerByBirthday = value;
        }
        private void ExecuteSearchCustomerByBirthday()
        {
            if (CurrentPrescription.Patient.Birthday is null) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(DateTimeExtensions.ConvertToTaiwanCalender((DateTime)CurrentPrescription.Patient.Birthday,false), 1);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private RelayCommand searchCustomerByTel;
        public RelayCommand SearchCustomerByTel
        {
            get =>
                searchCustomerByTel ??
                (searchCustomerByTel = new RelayCommand(ExecuteSearchCustomerByTel));
            set => searchCustomerByTel = value;
        }
        private void ExecuteSearchCustomerByTel()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Tel, 4);
            customerSelectionWindow.ShowDialog();
        }
        #endregion 
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            CurrentPrescription.Patient = receiveSelectedCustomer;
        }

        private void GetSelectedPrescription(Prescription receiveSelectedPrescription)
        {
            CurrentPrescription = receiveSelectedPrescription;
        }
    }
}
