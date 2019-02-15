using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using Cus = His_Pos.NewClass.Person.Customer.Customer;
using IcCard = His_Pos.NewClass.Prescription.IcCard;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow
{
    public class CustomPrescriptionViewModel:ViewModelBase
    {
        public Prescriptions CooperativePrescriptions { get; set; }
        public Prescriptions ReservedPrescriptions { get; set; }
        public Prescriptions RegisteredPrescriptions { get; set; }
        public Prescription SelectedPrescription { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public Cus Patient { get; set; }
        public IcCard Card { get; set; }
        private bool isSelectCooperative { get; set; }
        private Visibility cooperativeVisible;

        public Visibility CooperativeVisible
        {
            get => cooperativeVisible;
            set
            {
                Set(() => CooperativeVisible, ref cooperativeVisible, value);
            }
        }

        private Visibility reservedVisible;

        public Visibility ReservedVisible
        {
            get => reservedVisible;
            set
            {
                Set(() => ReservedVisible, ref reservedVisible, value);
            }
        }

        private Visibility registeredVisible;

        public Visibility RegisteredVisible
        {
            get => registeredVisible;
            set
            {
                Set(() => RegisteredVisible, ref registeredVisible, value);
            }
        }
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }
        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            private set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        private bool showDialog;
        public bool ShowDialog
        {
            get => showDialog;
            set
            {
                Set(() => ShowDialog, ref showDialog, value);
            }
        }

        public CustomPrescriptionViewModel(Cus cus, IcCard card)
        {
            Patient = cus;
            Card = card;
            InitializePrescription();
            RegisterMessenger();
            InitialCommand();
        }

        private void InitialCommand()
        {
            PrescriptionSelected = new RelayCommand(CustomPrescriptionSelected);
        }

        private void InitializePrescription()
        {
            SelectedPrescription = null;
            CooperativePrescriptions = new Prescriptions();
            MainWindow.ServerConnection.OpenConnection();
            CooperativePrescriptions.GetCooperaPrescriptionsByCusIDNumber(Patient.IDNumber);
            ReservedPrescriptions = new Prescriptions();
            ReservedPrescriptions.GetReservePrescriptionByCusId(Patient.ID);
            RegisteredPrescriptions.GetRegisterPrescriptionByCusId(Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
            if (CooperativePrescriptions.Count == 0)
                CooperativeVisible = Visibility.Collapsed;
            if(ReservedPrescriptions.Count == 0)
                ReservedVisible = Visibility.Collapsed;
            if (RegisteredPrescriptions.Count == 0)
                ReservedVisible = Visibility.Collapsed;
            if (CooperativePrescriptions.Count > 0 || ReservedPrescriptions.Count > 0 || RegisteredPrescriptions.Count > 0)
                ShowDialog = true;
            else
                ShowDialog = false;
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<Prescription>(this, "ReservedSelected", GetReservePrescription);
            Messenger.Default.Register<Prescription>(this,"ReservedSelected", GetReservePrescription);
            Messenger.Default.Register<Prescription>(this, "CooperativeSelected", GetCooperativePrescription);
        }

        private void GetReservePrescription(Prescription p)
        {
            isSelectCooperative = false;
            p.Treatment.AdjustDate = DateTime.Today;
            SelectedPrescription = p;
        }

        private void GetCooperativePrescription(Prescription p)
        {
            isSelectCooperative = true;
            SelectedPrescription = p;
        }

        private void CustomPrescriptionSelected()
        {
            if(SelectedPrescription is null) return;
            SelectedPrescription.GetCompletePrescriptionData(true,true);
            Messenger.Default.Send(SelectedPrescription,"CustomPrescriptionSelected");
            Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
        }
    }
}
