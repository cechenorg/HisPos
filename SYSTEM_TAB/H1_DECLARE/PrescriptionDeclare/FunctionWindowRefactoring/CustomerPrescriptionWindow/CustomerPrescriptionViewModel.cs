using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions;
using His_Pos.Service;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow
{
    public enum CustomerPrescriptionType
    {
        Orthopedics = 0,
        Cooperative = 1,
        Register = 2,
        Reserve = 3
    }
    public class CustomerPrescriptionViewModel : ViewModelBase
    {
        #region Variable
        private string windowTitle;
        public string WindowTitle
        {
            get => windowTitle;
            set
            {
                Set(() => WindowTitle, ref windowTitle, value);
            }
        }
        private string selectedRadioButton;
        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set
            {
                Set(() => SelectedRadioButton, ref selectedRadioButton, value);
                if (string.IsNullOrEmpty(selectedRadioButton)) return;
                switch (selectedRadioButton)
                {
                    case "Option1":
                        SelectedType = CustomerPrescriptionType.Orthopedics;
                        break;
                    case "Option2":
                        SelectedType = CustomerPrescriptionType.Cooperative;
                        break;
                    case "Option3":
                        SelectedType = CustomerPrescriptionType.Register;
                        break;
                    case "Option4":
                        SelectedType = CustomerPrescriptionType.Reserve;
                        break;
                }
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
        public IcCard Card { get; set; }
        private Customer patient;

        public Customer Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
            }
        }

        private string orthopedicsContent;
        public string OrthopedicsContent
        {
            get => orthopedicsContent;
            set
            {
                Set(() => OrthopedicsContent, ref orthopedicsContent, value);
            }
        }
        private string cooperativeContent;
        public string CooperativeContent
        {
            get => cooperativeContent;
            set
            {
                Set(() => CooperativeContent, ref cooperativeContent, value);
            }
        }
        private string registerContent;
        public string RegisterContent
        {
            get => registerContent;
            set
            {
                Set(() => RegisterContent, ref registerContent, value);
            }
        }
        private string reserveContent;
        public string ReserveContent
        {
            get => reserveContent;
            set
            {
                Set(() => ReserveContent, ref reserveContent, value);
            }
        }

        private CustomerPrescriptionType selectedType;

        public CustomerPrescriptionType SelectedType
        {
            get => selectedType;
            set
            {
                Set(() => SelectedType, ref selectedType, value);
            }
        }
        public bool UngetCardGridVisible => UngetCardPres != null && UngetCardPres.Count > 0;
        public bool OrthopedicsRadioBtnEnable => OrthopedicsPres != null && OrthopedicsPres.Count > 0;
        public bool CooperativeRadioBtnEnable => CooperativePres != null && CooperativePres.Count > 0;
        public bool RegisterRadioBtnEnable => ChronicRegisterPres != null && ChronicRegisterPres.Count > 0;
        public bool ReserveRadioBtnEnable => ChronicReservePres != null && ChronicReservePres.Count > 0;
        public CusPrePreviewBases OrthopedicsPres { get; set; }
        public CusPrePreviewBases CooperativePres { get; set; }
        public CusPrePreviewBases ChronicRegisterPres { get; set; }
        public CusPrePreviewBases ChronicReservePres { get; set; }
        public CusPrePreviewBases UngetCardPres { get; set; }
        private CusPrePreviewBase selectedPrescription;
        public CusPrePreviewBase SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
                if (SelectedPrescription != null)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    SelectedPrescription.GetMedicines();
                    MainWindow.ServerConnection.CloseConnection();
                }
            }
        }
        private CusPrePreviewBase makeUpPrescription;
        public CusPrePreviewBase MakeUpPrescription
        {
            get => makeUpPrescription;
            set
            {
                Set(() => MakeUpPrescription, ref makeUpPrescription, value);
            }
        }
        #endregion
        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public CustomerPrescriptionViewModel(Customer customer, IcCard card)
        {
            Patient = customer;
            Card = card.DeepCloneViaJson();
            InitCommands();
            InitVariable();
        }

        private void InitCommands()
        {
            PrescriptionSelected = new RelayCommand(PrescriptionSelectedAction);
        }

        private void InitVariable()
        {
            WindowTitle = Patient.Name + " 可調劑處方";
            OrthopedicsPres = new CusPrePreviewBases();
            CooperativePres = new CusPrePreviewBases();
            ChronicRegisterPres = new CusPrePreviewBases();
            ChronicReservePres = new CusPrePreviewBases();
            MainWindow.ServerConnection.OpenConnection();
            OrthopedicsPres.GetOrthopedicsByCustomerIDNumber(Patient.IDNumber);
            CooperativePres.GetCooperativeByCusIDNumber(Patient.IDNumber);
            ChronicRegisterPres.GetRegisterByCusId(Patient.ID);
            ChronicReservePres.GetReserveByCusId(Patient.ID);
            if(Card != null && !string.IsNullOrEmpty(Card.CardNumber))
                UngetCardPres.GetUngetCardByCusId(Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
            OrthopedicsContent = "骨科 " + OrthopedicsPres.Count + " 張";
            CooperativeContent = "合作 " + CooperativePres.Count + " 張";
            RegisterContent = "登錄 " + ChronicRegisterPres.Count + " 張";
            ReserveContent = "預約 " + ChronicReservePres.Count + " 張";
            if (ChronicReservePres.Count > 0)
            {
                SelectedType = CustomerPrescriptionType.Reserve;
                SelectedRadioButton = "Option4";
            }

            if (ChronicRegisterPres.Count > 0)
            {
                SelectedType = CustomerPrescriptionType.Register;
                SelectedRadioButton = "Option3";
            }

            if (CooperativePres.Count > 0)
            {
                SelectedType = CustomerPrescriptionType.Cooperative;
                SelectedRadioButton = "Option2";
            }

            if (OrthopedicsPres.Count > 0 || (CooperativePres.Count == 0 && ChronicRegisterPres.Count == 0 && ChronicReservePres.Count == 0))
            {
                SelectedType = CustomerPrescriptionType.Orthopedics;
                SelectedRadioButton = "Option1";
            }
        }

        private void PrescriptionSelectedAction()
        {
            if(SelectedPrescription is null) return;
            Messenger.Default.Send(new NotificationMessage<Prescription>(this, SelectedPrescription.CreatePrescription(), "CustomerPrescriptionSelected"));
            Messenger.Default.Send(new NotificationMessage("CloseCooperativePrescriptionWindow"));
        }
    }
}
