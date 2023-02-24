#region Using

using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Extention;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddCustomerWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Medicine.MedicineSet;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.CustomerHistoryProduct;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativePrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerPrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application = System.Windows.Application;
using Label = System.Windows.Controls.Label;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using Resources = His_Pos.Properties.Resources;
using TextBox = System.Windows.Controls.TextBox;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

// ReSharper disable ClassTooBig
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Global

#endregion Using

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    public class PrescriptionDeclareViewModel : TabBase
    {

        public delegate void GetCustomerPrescriptionDelegate(Prescription receiveMsg);
        public static TabBase TabThis;

        public static TabBase getThis()
        {
            return TabThis;
        }

        public override TabBase getTab()
        {
            return this;
        }

        #region ItemsSource

        public Divisions Divisions => VM.Divisions;
        private Employees medicalPersonnels;

        public Employees MedicalPersonnels
        {
            get => medicalPersonnels;
            set { Set(() => MedicalPersonnels, ref medicalPersonnels, value); }
        }

        public AdjustCases AdjustCases => VM.AdjustCases;
        public PaymentCategories PaymentCategories => VM.PaymentCategories;
        public PrescriptionCases PrescriptionCases => VM.PrescriptionCases;
        public Copayments Copayments => VM.Copayments;

        public SpecialTreats SpecialTreats => VM.SpecialTreats;
        private MedicineSets medicineSets;

        public MedicineSets MedicineSets
        {
            get => medicineSets;
            set
            {
                Set(() => MedicineSets, ref medicineSets, value);
            }
        }

        #endregion ItemsSource

        #region Variables

        private BackgroundWorker worker;
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
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }

        public bool CanSearchPatient => CurrentPrescription != null &&
                                        (CurrentPrescription.Patient is null || CurrentPrescription.Patient.ID < 0);

        private bool customerEdited;

        public bool CustomerEdited
        {
            get => customerEdited;
            private set
            {
                Set(() => CustomerEdited, ref customerEdited, value);
            }
        }

        private Prescription currentPrescription;

        public Prescription CurrentPrescription
        {
            get => currentPrescription;
            set
            {
                Set(() => CurrentPrescription, ref currentPrescription, value);
            }
        }

        private int prescriptionCount;

        public int PrescriptionCount
        {
            get => prescriptionCount;
            set
            {
                if (prescriptionCount != value)
                    Set(() => PrescriptionCount, ref prescriptionCount, value);
            }
        }

        private Employee selectedPharmacist;

        public Employee SelectedPharmacist
        {
            get => selectedPharmacist;
            set
            {
                Set(() => SelectedPharmacist, ref selectedPharmacist, value);
            }
        }

        private CustomerHistory selectedHistory;

        public CustomerHistory SelectedHistory
        {
            get => selectedHistory;
            set
            {
                if (value != null)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    value.Products = new CustomerHistoryProducts();
                    value.Products.GetCustomerHistoryProducts(value.SourceId, value.Type);
                    MainWindow.ServerConnection.CloseConnection();
                }
                Set(() => SelectedHistory, ref selectedHistory, value);
            }
        }

        private bool canSendOrder;

        public bool CanSendOrder
        {
            get => canSendOrder;
            set
            {
                Set(() => CanSendOrder, ref canSendOrder, value);
            }
        }

        private PrescriptionDeclareStatus declareStatus;

        public PrescriptionDeclareStatus DeclareStatus
        {
            get => declareStatus;
            set
            {
                Set(() => DeclareStatus, ref declareStatus, value);
                if (CurrentPrescription is null) return;
                if (CurrentPrescription.CheckCanSendOrder())
                    CanSendOrder = true;
                if (!CanSendOrder)
                    CurrentPrescription.PrescriptionStatus.IsSendOrder = false;
            }
        }

        private MedicineSet currentSet;

        public MedicineSet CurrentSet
        {
            get => currentSet;
            set
            {
                Set(() => CurrentSet, ref currentSet, value);
            }
        }

        private string selectedPatientDetail;

        public string SelectedPatientDetail
        {
            get => selectedPatientDetail;
            set
            {
                if (value is null) return;
                if (!string.IsNullOrEmpty(value) && value.Equals("Option2"))
                {
                    if (CurrentPrescription.Patient is null || CurrentPrescription.Patient.ID <= 0)
                    {
                        MessageWindow.ShowMessage("尚未選擇客戶", MessageType.WARNING);
                        value = "Option1";
                    }
                    else
                    {
                        if (CurrentPrescription.Patient.Name.Equals("匿名"))
                        {
                            MessageWindow.ShowMessage("匿名資料不可編輯", MessageType.WARNING);
                            value = "Option1";
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(value) && value.Equals("Option1"))
                {
                    CheckCustomerEdited();
                }
                Set(() => SelectedPatientDetail, ref selectedPatientDetail, value);
            }
        }

        private string reserveStatus;

        public string ReserveStatus
        {
            get => reserveStatus;
            set
            {
                Set(() => ReserveStatus, ref reserveStatus, value);
            }
        }

        private string cusFromPOS;

        public string CusFromPOS
        {
            get => cusFromPOS;
            set
            {
                cusFromPOS = value;
                if (cusFromPOS.Length > 1)
                {
                    GetCustomersFromPOSAction();
                }
            }
        }

        private string _displayPatientCellPhone;

        public string DisplayPatientCellPhone
        {
            get {
                var cellphone = currentPrescription.Patient.CellPhone;
                return cellphone is null ? string.Empty : cellphone.ToPatientCellPhone();
               
            }
            set
            {
                string cellphone = value.Replace("-","");
                currentPrescription.Patient.CellPhone = cellphone;
                Set(() => DisplayPatientCellPhone, ref _displayPatientCellPhone, value);
            }
        }

        public string DisplayPatientSecondPhone
        {
            get
            {
                var cellphone = currentPrescription.Patient.SecondPhone;
                return cellphone is null ? string.Empty : cellphone.ToPatientCellPhone();
            }
            set
            {
                string cellphone = value.Replace("-", "");
                currentPrescription.Patient.SecondPhone = cellphone;
            }
        }

        private string _displayPatientTel;

        public string DisplayPatientTel
        {
            get
            {
                var tel = currentPrescription.Patient.Tel;
                return tel is null ? string.Empty : tel.ToPatientTel();
            }
            set
            {
                string tel = value.Replace("-", "");
                currentPrescription.Patient.Tel = tel;
                Set(() => DisplayPatientTel, ref _displayPatientTel, value);
            }
        }
        private bool _isLineEnable = false;
        public bool IsLineEnable
        {
            get
            {
                var line = CurrentPrescription.Patient.Line;
                return line is null ? false : true;
            }
            set
            {
                Set(() => IsLineEnable, ref _isLineEnable, value);
            }
        }

        public string DisplayPatientContactNote
        {
            get
            {
                var contactnote = currentPrescription.Patient.ContactNote;
                return contactnote is null ? string.Empty : contactnote.ToPatientContactNote();
            }
            set
            {
                currentPrescription.Patient.ContactNote = value;
            }
        }

        private IcCard currentCard;
        private PrescriptionService currentService;
        private bool setBuckleAmount;
        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode;
        private bool isNotInit;
        private bool isAdjusting;
        private bool isCardReading;

        #endregion Variables

        #region Commands

        public RelayCommand OpenCustomerManage { get; set; }
        public RelayCommand ScanPrescriptionQRCode { get; set; }
        public RelayCommand<TextBox> GetCustomers { get; set; }
        public RelayCommand<Label> GetCustomersEditedToday { get; set; }
        public RelayCommand ClearPatient { get; set; }
        public RelayCommand CustomerDataEdited { get; set; }
        public RelayCommand ShowCustomerEditWindow { get; set; }
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand AddCustomer { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand PharmacistChanged { get; set; }
        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }
        public RelayCommand AdjustDateChanged { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> CheckClearDisease { get; set; }
        public RelayCommand ChronicSequenceTextChanged { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand CopaymentSelectionChanged { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand ChangeMedicineIDToMostPriced { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CountPrescriptionPoint { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand AdjustNoBuckle { get; set; }
        public RelayCommand IsClosed { get; set; }
        public RelayCommand ResetBuckleAmount { get; set; }
        public RelayCommand ClearBuckleAmount { get; set; }
        public RelayCommand CopyPrescription { get; set; }
        public RelayCommand CheckDeclareStatusCmd { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand<string> EditMedicineSet { get; set; }
        public RelayCommand CustomerDetailEdited { get; set; }
        public RelayCommand CustomerRedoEdited { get; set; }
        public RelayCommand SavePatientData { get; set; }
        public RelayCommand SendOrder { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand ErrorAdjust { get; set; }
        public RelayCommand DepositAdjust { get; set; }
        public RelayCommand Adjust { get; set; }
        public RelayCommand Register { get; set; }
        public RelayCommand PrescribeAdjust { get; set; }
        public RelayCommand CustomerFromPOS { get; set; }

        #endregion Commands

        public PrescriptionDeclareViewModel()
        {
            Init();
        }

        #region InitFunctions

        private void Init()
        {
            InitCommands();
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(DateTime.Today);
            MainWindow.ServerConnection.OpenConnection();
            MedicineSets = new MedicineSets();
            ClearAction();
            SetPharmacist();
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Register<NotificationMessage>("UpdateUsableAmountMessage", UpdateInventories);
            TabThis = this;
        }

        private void UpdateInventories(NotificationMessage msg)
        {
            if (msg.Notification == "UpdateUsableAmountMessage" && CurrentPrescription != null)
            {
                if (CurrentPrescription.Medicines != null && CurrentPrescription.Medicines.Count > 0)
                    CurrentPrescription.UpdateMedicines();
            }
        }

        private void SetPharmacist()
        {
            var currentMedicalPerson = MedicalPersonnels.SingleOrDefault(e => e.ID.Equals(VM.CurrentUser.ID));
            if (currentMedicalPerson != null)
            {
                SelectedPharmacist = currentMedicalPerson;
                PrescriptionCount = UpdatePrescriptionCount();
            }
        }

        private void InitLocalVariables()
        {
            SelectedPatientDetail = "Option1";
            CustomerEdited = false;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            CanSendOrder = false;
            isAdjusting = false;
            ErrorCode = null;
        }

        private void ClearAction()
        {
            isNotInit = false;
            InitLocalVariables();
            CurrentPrescription = new Prescription();
            CurrentPrescription.Init();
            currentCard = new IcCard();
            setBuckleAmount = true;
            isNotInit = true;
            RaisePropertyChanged("CanSearchPatient");
        }

        [SuppressMessage("ReSharper", "MethodTooLong")]
        private void InitCommands()
        {
            OpenCustomerManage = new RelayCommand(OpenCustomerManageAction);
            ScanPrescriptionQRCode = new RelayCommand(ScanPrescriptionQRCodeAction);
            GetCooperativePres = new RelayCommand(GetCooperativePresAction);
            GetPatientData = new RelayCommand(GetPatientDataAction, CheckIsCardReading);
            AddCustomer = new RelayCommand(AddCustomerAction);
            GetCustomers = new RelayCommand<TextBox>(GetCustomersAction);
            GetCustomersEditedToday = new RelayCommand<Label>(GetCustomersEditedTodayAction);
            ClearPatient = new RelayCommand(ClearPatientAction);
            CustomerDataEdited = new RelayCommand(CustomerDataEditedAction);
            ShowCustomerEditWindow = new RelayCommand(ShowCustomerEditWindowAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            PharmacistChanged = new RelayCommand(PharmacistChangedAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            AdjustDateChanged = new RelayCommand(AdjustDateChangedAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            CheckClearDisease = new RelayCommand<object>(CheckClearDiseaseAction);
            ChronicSequenceTextChanged = new RelayCommand(ChronicSequenceChangedAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ChangeMedicineIDToMostPriced = new RelayCommand(ChangeMedicineIDToMostPricedAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CountPrescriptionPoint = new RelayCommand(CountMedicinePointAction);
            MedicineAmountChanged = new RelayCommand(MedicineAmountChangedAction, SetBuckleAmount);
            AdjustNoBuckle = new RelayCommand(AdjustNoBuckleAction);
            IsClosed = new RelayCommand(IsClosedAction);
            ResetBuckleAmount = new RelayCommand(ResetBuckleAmountAction);
            ClearBuckleAmount = new RelayCommand(ClearBuckleAmountAction);
            CopyPrescription = new RelayCommand(CopyPrescriptionAction);
            CheckDeclareStatusCmd = new RelayCommand(CheckDeclareStatus);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            EditMedicineSet = new RelayCommand<string>(EditMedicineSetAction);
            CustomerDetailEdited = new RelayCommand(CustomerDetailEditedAction);
            CustomerRedoEdited = new RelayCommand(CustomerRedoEditedAction);
            SavePatientData = new RelayCommand(SavePatientDataAction);
            SendOrder = new RelayCommand(SendOrderAction);
            Clear = new RelayCommand(ClearAction);
            ErrorAdjust = new RelayCommand(ErrorAdjustAction, CheckIsAdjusting);
            DepositAdjust = new RelayCommand(DepositAdjustAction, CheckDepositAdjustEnable);
            Adjust = new RelayCommand(AdjustAction, CheckIsAdjusting);
            Register = new RelayCommand(RegisterAction, CheckIsAdjusting);
            PrescribeAdjust = new RelayCommand(PrescribeAdjustAction, CheckIsAdjusting);
            CustomerFromPOS = new RelayCommand(GetCustomersFromPOSAction);
        }

        #endregion InitFunctions

        #region CommandAction

        private void OpenCustomerManageAction()
        {
            var viewModel = (App.Current.Resources["Locator"] as ViewModelLocator)?.CustomerManageView;
            Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, CurrentPrescription.Patient.ID.ToString(), "CustomerManageResearch"));
        }

        private void ScanPrescriptionQRCodeAction()
        {
            var receive = new QRCodeReceiveWindow(GetCustomerPrescription);
        }

        private void GetCooperativePresAction()
        {
            //查詢合作診所處方

            var cooPresWindow = new CooperativePrescriptionWindow();
            CooperativePrescriptionViewModel cooperativePrescriptionViewModel = new CooperativePrescriptionViewModel(GetCustomerPrescription);
            cooPresWindow.DataContext = cooperativePrescriptionViewModel;
            cooPresWindow.ShowDialog();
        }

        private void GetCustomerPrescription(Prescription prescription)
        {
            isNotInit = false;
            setBuckleAmount = false;
            if (!CheckPatientEqual(prescription)) return;
            CurrentPrescription = prescription;
            CurrentPrescription.IsBuckle = CurrentPrescription.WareHouse != null;
            CheckNewCustomer();
            CountMedicinePointAction();
            CurrentPrescription.UpdateMedicines();
            setBuckleAmount = true;
            isNotInit = true;
        }

        private void GetPatientDataAction()
        {
            //取得病患資料(讀卡)
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => { ReadCard(); };
            worker.RunWorkerCompleted += (o, ea) => { GetPatientDataComplete(); };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void AddCustomerAction()
        {
            Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
            var newCustomerWindow = new AddCustomerWindow(CurrentPrescription.Patient);
            Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
        }

        //顧客查詢
        private void GetCustomersAction(TextBox condition)
        {
            Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
            if (!CheckConditionEmpty(condition.Name))
            {
                ShowCustomerSearchEditedToday(condition.Name);
            }
            else
            {
                ShowCustomerSearch(condition.Name);
            }
            if (ProductTransactionView.FromHISCuslblcheck == null)
            {
                return;
            }
            else if (CurrentPrescription.Patient.CellPhone != null || CurrentPrescription.Patient.CellPhone != "")
            {
                ProductTransactionView.FromHISCuslblcheck.Text = CurrentPrescription.Patient.CellPhone;
            }
            else if (CurrentPrescription.Patient.Tel != null || CurrentPrescription.Patient.Tel != "" || ProductTransactionView.FromHISCuslblcheck != null)
            {
                ProductTransactionView.FromHISCuslblcheck.Text = CurrentPrescription.Patient.Tel;
            }
            else
            {
                return;
            }
        }

        public void GetCustomersFromPOSAction()
        {
            if (CusFromPOS.Length > 1)
            {
                GetCustomersFromPOSAction(CusFromPOS);
                CusFromPOS = "";
            }
        }

        private void GetCustomersFromPOSAction(string condition)
        {
            Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
            ShowCustomerFromPOSSearch(condition);
        }

        private void GetCustomersEditedTodayAction(Label condition)
        {
            Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
            ShowCustomerSearchEditedToday(condition.Name);
        }

        private void ClearPatientAction()
        {
            CurrentPrescription.Patient = new Customer();
            RaisePropertyChanged("CanSearchPatient");
        }

        private void CustomerDataEditedAction()
        {
            if (CurrentPrescription.Patient is null) return;
            if (CurrentPrescription.Patient.ID > 0)
                CustomerEdited = true;
        }

        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private void ShowCustomerSearch(string conditionName)
        {
            MainWindow.ServerConnection.OpenConnection();
            CustomerSearchWindow customerSearch;
            switch (conditionName)
            {
                case "PatientIDNumber":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.IDNumber, CurrentPrescription.Patient.IDNumber.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientName":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.Name, CurrentPrescription.Patient.Name.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientBirthday":
                    customerSearch = new CustomerSearchWindow((DateTime)CurrentPrescription.Patient.Birthday);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientTel":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.Tel, CurrentPrescription.Patient.Tel.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientCellPhone":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.CellPhone, CurrentPrescription.Patient.CellPhone.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
            }
        }

        private void ShowCustomerFromPOSSearch(string conditionName)
        {
            if (conditionName.Length > 1)
            {
                MainWindow.ServerConnection.OpenConnection();
                CustomerSearchWindow customerSearch;
                bool isCell = conditionName.StartsWith("09");
                if (isCell)
                {
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.CellPhone, 0, conditionName.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                }
                else
                {
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.Tel, 0, conditionName.Trim());
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                }
            }
        }

        private void ShowCustomerSearchEditedToday(string conditionName)
        {
            MainWindow.ServerConnection.OpenConnection();
            CustomerSearchWindow customerSearch;
            switch (conditionName)
            {
                case "PatientIDNumber":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.IDNumber);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientName":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.Name);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientBirthday":
                    customerSearch = new CustomerSearchWindow(null);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientTel":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.Tel);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;

                case "PatientCellPhone":
                    customerSearch = new CustomerSearchWindow(CustomerSearchCondition.CellPhone);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
            }
        }

        private bool CheckConditionEmpty(string conditionName)
        {
            switch (conditionName)
            {
                case "PatientIDNumber" when CurrentPrescription.CheckPatientDataEmpty("IDNumber"):
                    return false;

                case "PatientName" when CurrentPrescription.CheckPatientDataEmpty("Name"):
                    return false;

                case "PatientBirthday" when CurrentPrescription.CheckPatientDataEmpty("Birthday"):
                    return false;

                case "PatientTel" when CurrentPrescription.CheckPatientDataEmpty("Tel"):
                    return false;

                case "PatientCellPhone" when CurrentPrescription.CheckPatientDataEmpty("CellPhone"):
                    return false;

                default:
                    return true;
            }
        }

        [SuppressMessage("ReSharper", "TooManyChainedReferences")]
        private void ShowCustomerEditWindowAction()
        {
            if (CurrentPrescription.Patient.ID <= 0) return;
            var customerDetailWindow = new CustomerDetailWindow(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient = Customer.GetCustomerByCusId(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
        }

        //院所查詢
        private void GetInstitutionAction(string insID)
        {
            #region ReSharperDisable

            // ReSharper disable RedundantAssignment
            // ReSharper disable once UnusedVariable

            #endregion ReSharperDisable

            if (CheckFocusDivision(insID))
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusDivision"));
                return;
            }
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var institutionSelectionWindow = new InstitutionSelectionWindow(insID);
        }

        //常用院所查詢
        private void GetCommonInstitutionAction()
        {
            #region ReSharperDisable

            // ReSharper disable RedundantAssignment
            // ReSharper disable once UnusedVariable

            #endregion ReSharperDisable

            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var commonHospitalsWindow = new CommonHospitalsWindow();
        }

        private void PharmacistChangedAction()
        {
            PrescriptionCount = UpdatePrescriptionCount();
            if (PrescriptionCount >= 80) MessageWindow.ShowMessage(Resources.調劑張數提醒 + prescriptionCount + "張", MessageType.WARNING);
            if (isNotInit && SelectedPharmacist != null)
                Messenger.Default.Send(new NotificationMessage(this, "FocusMedicalNumber"));
        }

        private void DateMouseDoubleClickAction(MaskedTextBox sender)
        {
            switch (sender.Name)
            {
                case "AdjustDateTextBox":
                    CurrentPrescription.AdjustDate = DateTime.Today;
                    break;

                case "TreatDateTextBox":
                    CurrentPrescription.TreatDate = DateTime.Today;
                    break;
            }
        }

        private void AdjustDateChangedAction()
        {
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(CurrentPrescription.AdjustDate ?? DateTime.Today);
            if (CurrentPrescription.AdjustDate != null)
                CurrentPrescription.UpdateMedicines();
            CheckDeclareStatus();
        }

        private void GetDiseaseCodeCheck(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEquals(parameters))
            {
                DiseaseFocusNext(elementName);
                return;
            }
            else if (!string.IsNullOrEmpty(diseaseID))

            {
                switch (elementName)
                {
                    case "MainDiagnosis":
                        CurrentPrescription.MainDisease = new DiseaseCode();
                        break;

                    case "SecondDiagnosis":
                        CurrentPrescription.SubDisease = new DiseaseCode();
                        break;
                }
            }
            //診斷碼查詢

            switch (elementName)
            {
                case "MainDiagnosis":

                    CurrentPrescription.MainDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (CurrentPrescription.MainDisease == null)
                    {
                        Messenger.Default.Send(new NotificationMessage(this, "FocusMainDisease"));

                        return;
                    }
                    break;

                case "SecondDiagnosis":
                    CurrentPrescription.SubDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (CurrentPrescription.SubDisease == null)
                    {
                        Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));

                        return;
                    }
                    break;
            }
        }

        private void GetDiseaseCodeAction(object sender)
        {
            GetDiseaseCodeCheck(sender);
        }

        private void CheckClearDiseaseAction(object sender)
        {
            //LostFocus()

            GetDiseaseCodeCheck(sender);
        }

        private void DiseaseFocusNext(string elementName)
        {
            if (elementName == "MainDiagnosis")
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                return;
            }
            Messenger.Default.Send(new NotificationMessage(this, "FocusChronicTotal"));
        }

        private void ChronicSequenceChangedAction()
        {
            CurrentPrescription.CheckPrescriptionVariable();
            CheckDeclareStatus();
        }

        private void AdjustCaseSelectionChangedAction()
        {
            CheckDeclareStatus();
            if (CurrentPrescription.AdjustCase.CheckIsPrescribe())
                CurrentPrescription.Medicines.SetToPaySelf();
        }

        private void CopaymentSelectionChangedAction()
        {
            if (CurrentPrescription.Copayment is null || CurrentPrescription.PrescriptionPoint is null) return;
            switch (CurrentPrescription.Copayment.Id)
            {
                case "I21" when CurrentPrescription.PrescriptionPoint.MedicinePoint > 100:
                    CurrentPrescription.Copayment = VM.GetCopayment("I20");
                    break;

                case "I20" when CurrentPrescription.PrescriptionPoint.MedicinePoint <= 100:
                    CurrentPrescription.Copayment = VM.GetCopayment("I21");
                    break;
            }
        }

        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (!CheckMedicineIDLength(medicineID)) return;
            var productCount = GetProductCount(medicineID);
            if (productCount == 0)
                MessageWindow.ShowMessage(Resources.查無藥品, MessageType.WARNING);
            else
            {
                var wareHouse = CurrentPrescription.WareHouse;
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedMedicine);
                var addMedicineWindow = wareHouse is null ? new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionDeclare, "0") : new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse.ID);
                if (productCount > 1)
                    addMedicineWindow.ShowDialog();
            }
            CountMedicinePointAction();
        }

        private void DeleteMedicineAction()
        {
            CurrentPrescription.DeleteMedicine();
            CurrentPrescription.CountPrescriptionPoint();
            CurrentPrescription.CountSelfPay();
            CurrentPrescription.PrescriptionPoint.CountAmountsPay();
        }

        private void ChangeMedicineIDToMostPricedAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.AddMedicine(((MedicineNHI)CurrentPrescription.SelectedMedicine).MostPricedID);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            var wareID = CurrentPrescription.WareHouse is null ? "0" : CurrentPrescription.GetWareHouseID();
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, wareID }, "ShowProductDetail"));
        }

        private void MedicineAmountChangedAction()
        {
            CurrentPrescription.IsBuckle = CurrentPrescription.WareHouse != null;
        }

        private void AdjustNoBuckleAction()
        {
            switch (CurrentPrescription.SelectedMedicine.AdjustNoBuckle)
            {
                case true:
                    CurrentPrescription.SelectedMedicine.AdjustNoBuckle = false;
                    CurrentPrescription.SelectedMedicine.BuckleAmount = CurrentPrescription.SelectedMedicine.Amount;
                    break;

                case false:
                    CurrentPrescription.SelectedMedicine.AdjustNoBuckle = true;
                    CurrentPrescription.SelectedMedicine.BuckleAmount = 0;
                    break;
            }
        }

        private void IsClosedAction()
        {
            switch (CurrentPrescription.SelectedMedicine.IsClosed)
            {
                case true:
                    CurrentPrescription.SelectedMedicine.IsClosed = false;
                    CurrentPrescription.SelectedMedicine.AdjustNoBuckle = false;
                    break;

                case false:
                    CurrentPrescription.SelectedMedicine.IsClosed = true;
                    if (CurrentPrescription.SelectedMedicine.BuckleAmount == 0)
                        CurrentPrescription.SelectedMedicine.AdjustNoBuckle = true;
                    break;
            }
        }
        private void ResetBuckleAmountAction()
        {
            CurrentPrescription.SelectedMedicine?.ResetBuckleAmount();
        }
        private void ClearBuckleAmountAction()
        {
            CurrentPrescription.SelectedMedicine?.ClearBuckleAmount();
        }

        private void CountMedicinePointAction()
        {
            CurrentPrescription.CheckPrescriptionVariable();
            CurrentPrescription.CountPrescriptionPoint();
            CurrentPrescription.CountSelfPay();
            CurrentPrescription.PrescriptionPoint.CountAmountsPay();
            CheckDeclareStatus();
        }

        private void ShowPrescriptionEditWindowAction()
        {
            if (SelectedHistory is null) return;
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            PrescriptionService.ShowPrescriptionEditWindow(SelectedHistory.SourceId);
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }

        private void CopyPrescriptionAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            var prescription = SelectedHistory.GetPrescriptionRefactoringByID();
            MainWindow.ServerConnection.CloseConnection();
            prescription.TreatDate = null;
            prescription.AdjustDate = null;
            prescription.TempMedicalNumber = null;
            prescription.Patient = CurrentPrescription.Patient;
            prescription.PrescriptionStatus.Init();
            prescription.ID = 0;
            prescription.Reset();
            CurrentPrescription = prescription;
            CurrentPrescription.ID = 0;
            CheckDeclareStatus();
            CountMedicinePointAction();
        }

        private void EditMedicineSetAction(string mode)
        {
            if (CurrentSet is null && !mode.Equals("Add"))
            {
                MessageWindow.ShowMessage("尚未選擇藥品組合", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            switch (mode)
            {
                case "Get":
                    GetMedicinesFromMedicineSet();
                    break;

                case "Add":
                    AddMedicineSet();
                    break;

                case "Edit":
                    EditCurrentMedicineSet();
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
        }

        private void CustomerDetailEditedAction()
        {
            CustomerEdited = true;
        }

        private void CustomerRedoEditedAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient = Customer.GetCustomerByCusId(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
            CustomerEdited = false;
        }

        private void SavePatientDataAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.Save();
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("編輯成功", MessageType.SUCCESS);
            CustomerEdited = false;
        }

        private void SendOrderAction()
        {
            CheckMedicinePrepared();
            CheckDeclareStatus();
        }

        private void CheckMedicinePrepared()
        {
            if (!CurrentPrescription.PrescriptionStatus.IsSendOrder) return;
            if (CurrentPrescription.PrescriptionStatus.ReserveSend != null && (bool)CurrentPrescription.PrescriptionStatus.ReserveSend)
                MessageWindow.ShowMessage("此預約處方已備藥。", MessageType.ONLYMESSAGE);
        }

        private void ErrorAdjustAction()
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            if (!currentService.CheckCustomerSelected())
                return;
            CheckCustomerEdited();
            if (!CheckAdjustDate()) return;
            CheckWay();
            if (!ErrorAdjustConfirm()) return;
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            CheckChronicCopayment();
            if (!CheckPrescription(false, true)) return;
            StartErrorAdjust();
        }

        private void DepositAdjustAction()
        {
            CheckCustomerValid();
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            if (!currentService.CheckCustomerSelected())
                return;
            CheckCustomerEdited();
            if (!CheckAdjustDate()) return;
            CheckWay();
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckPrescription(true, false)) return;
            StartDepositAdjust();
        }

        private void AdjustAction()
        {            
            CheckCustomerValid();
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            if (!currentService.CheckCustomerSelected())
                return;

            CheckCustomerEdited();
            if (!CheckAdjustDate()) 
                return;
            CheckWay();

            isAdjusting = true;

            if (!CheckPrescriptionBeforeOrder(false, false))
                return;

            if (!CheckMedicinesNegativeStock())
                return;

            CheckChronicCopayment();

            if (!CheckPrescription(false, false)) 
                return;

            if (VM.CurrentPharmacy.NewInstitution)
            {
                SetNewInstitutionUploadData();
                StartNormalAdjust();
            }
            else
                CheckIsReadCard();
        }

        private void CheckCustomerValid()
        {
            if (CanSearchPatient)
                CheckNewCustomer();
        }

        private void CheckChronicCopayment()
        {
            CurrentPrescription.MedicineDays = CurrentPrescription.Medicines.CountMedicineDays();
            CurrentPrescription.PrescriptionPoint.MedicinePoint = CurrentPrescription.Medicines.CountMedicinePoint();
            if (CurrentPrescription.AdjustCase.IsChronic() && CurrentPrescription.MedicineDays < 28)
            {
                var confirm = new ConfirmWindow("此處方為28天以下之慢性病處方箋，請確認藥品是否為同一療程(如荷爾蒙製劑)。如為同一療程免收部分負擔，是否計算部分負擔?", "部分負擔確認");
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool)confirm.DialogResult)
                {
                    CurrentPrescription.Copayment = VM.GetCopayment(CurrentPrescription.PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
                }
                else
                {
                    CurrentPrescription.Copayment = VM.GetCopayment("I22");
                }
            }
        }

        private void RegisterAction()
        {
            CheckCustomerValid();
            CheckCustomerEdited();
            isAdjusting = true;
            CurrentPrescription.PrescriptionStatus.IsSendOrder = true;
            if (!CheckRegisterPrescription(false, false))
            {
                CurrentPrescription.PrescriptionStatus.IsSendOrder = false;
                return;
            }
            StartRegister();
        }

        private void PrescribeAdjustAction()
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            if (!currentService.CheckCustomerSelected())
                return;
            CheckCustomerEdited();
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckPrescription(false, false)) return;
            StartPrescribeAdjust();
        }

        #endregion CommandAction

        #region MessengerFunctions

        private void GetPatientFromIcCard()
        {
            var patientFromCard = new Customer(currentCard);
            CurrentPrescription.PrescriptionStatus.IsGetCard = true;
            CheckCustomerByCard(patientFromCard);
        }

        private void CheckCustomerByCard(Customer patientFromCard)
        {
            Customer checkedPatient;
            MainWindow.ServerConnection.OpenConnection();
            var patientFromDB = CustomerDb.CheckCustomerByCard(currentCard.IDNumber);
            if (patientFromDB != null)
            {
                patientFromDB.CheckPatientWithCard(patientFromCard);
                checkedPatient = patientFromDB;
                checkedPatient.Save();
            }
            else
            {
                var insertResult = patientFromCard.InsertData();
                if (!insertResult)
                {
                    MessageWindow.ShowMessage("請重新讀取卡片。", MessageType.WARNING);
                    return;
                }
                checkedPatient = patientFromCard;
            }
            GetSelectedCustomer(new NotificationMessage<Customer>(checkedPatient, "GetSelectedCustomer"));
        }

        private void GetSelectedCustomer(NotificationMessage<Customer> receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
            if (receiveSelectedCustomer.Content is null)
            {
                if (!receiveSelectedCustomer.Notification.Equals("AskAddCustomerData")) return;
                if (!CheckInsertCustomerData()) return;
            }
            else
                CurrentPrescription.Patient = receiveSelectedCustomer.Content;
            RaisePropertyChanged("CanSearchPatient");
            CurrentPrescription.Patient.UpdateEditTime();
            CurrentPrescription.Patient.GetHistories();
            CurrentPrescription.Patient.GetRecord();
            MainWindow.ServerConnection.CloseConnection();
            CheckCustomPrescriptions();
            CustomerEdited = false;
        }

        [SuppressMessage("ReSharper", "TooManyChainedReferences")]
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = receiveSelectedInstitution;
            CurrentPrescription.UpdateMedicines();
            var notification = string.IsNullOrEmpty(CurrentPrescription.Division?.ID) ? "FocusDivision" : "FocusMedicalNumber";
            Messenger.Default.Send(new NotificationMessage(this, notification));
        }

        private void GetSelectedMedicine(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionDeclareViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.AddMedicine(msg.Content.ID);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void Refresh(NotificationMessage msg)
        {
            if (!msg.Notification.Equals("PrescriptionEdited")) return;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
        }

        #endregion MessengerFunctions

        #region ICCardFunctions

        private void CheckIsReadCard()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (!currentCard.IsRead)
                    ReadCard();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                isCardReading = false;
                if (CheckReadCardResult())
                    WriteCard();

                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private bool CheckReadCardResult()
        {
            if (currentCard.IsRead)
            {
                GetMedicalNumber();
                return true;
            }
            AskErrorUpload();
            return ErrorCode != null;
        }

        private void GetMedicalNumber()
        {
            CurrentPrescription.PrescriptionStatus.IsGetCard = true;
            BusyContent = Resources.檢查就醫次數;
            currentCard.GetRegisterBasic();
            if (currentCard.CheckNeedUpdate())
            {
                BusyContent = Resources.更新卡片;
                currentCard.UpdateCard();
            }
            var res = currentCard.GetMedicalNumber(1);
            if (res != 5003) return;
            BusyContent = Resources.更新卡片;
            currentCard.UpdateCard();
            BusyContent = Resources.取得就醫序號;
            currentCard.GetMedicalNumber(1);
        }

        private void ReadCard()
        {
            isCardReading = true;
            BusyContent = Resources.讀取健保卡;
            try
            {
                currentCard = new IcCard();
                currentCard.Read();
            }
            catch (Exception e)
            {
                NewFunction.ExceptionLog(e.Message);
                NewFunction.ShowMessageFromDispatcher(e.Message, MessageType.WARNING);
                NewFunction.ShowMessageFromDispatcher("讀卡作業異常，請重開處方登錄頁面並重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING);
            }
        }

        private bool AskErrorUpload()
        {
            var e = new ErrorUploadWindow(currentCard.IsGetMedicalNumber);
            e.ShowDialog();
            if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage(Resources.尚未選擇異常代碼, MessageType.WARNING);
                isAdjusting = false;
                isCardReading = false;
                return false;
            }
            ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            return true;
        }

        private void SetNewInstitutionUploadData()
        {
            ErrorCode = new ErrorUploadWindowViewModel.IcErrorCode("G000", "新特約使用");
            currentService.SetCard(currentCard);
            currentService.SetCreateSign();
        }

        private void WriteCard()
        {
            if (!CheckIsGetMedicalNumber()) return;
            if (!CheckPatientMatch()) return;

            BusyContent = Resources.寫卡;
            currentService.SetCard(currentCard);
            currentService.CreateDailyUploadData(ErrorCode);
            StartNormalAdjust();

        }

        private bool CheckPatientMatch()
        {
            if (currentCard.IsRead && !CurrentPrescription.Patient.IDNumber.Equals(currentCard.IDNumber))
            {
                MessageWindow.ShowMessage("卡片資料與目前病患資料不符，請確認。", MessageType.ERROR);
                IsBusy = false;
                isAdjusting = false;
                return false;
            }
            return true;
        }

        private bool CheckIsGetMedicalNumber()
        {
            if (currentCard.IsGetMedicalNumber || !(ErrorCode is null)) return true;
            if (AskErrorUpload()) return true;
            IsBusy = false;
            isAdjusting = false;
            return false;
        }

        private bool CheckIsCardReading()
        {
            return !isCardReading;
        }

        #endregion ICCardFunctions

        #region PatinetFunctions

        private void GetPatientDataComplete()
        {
            IsBusy = false;
            isCardReading = false;
            if (currentCard.IsRead)
                GetPatientFromIcCard();
            else
            {
                MessageWindow.ShowMessage("讀取卡片異常，請確認卡面及讀卡機燈號正常，如持續異常且其他健保卡可正常讀取請使用異常上傳。", MessageType.WARNING);
            }
        }

        private void CheckCustomPrescriptions()
        {
            CustomerPrescriptionViewModel customerPrescriptionViewModel= new CustomerPrescriptionViewModel(CurrentPrescription.Patient, currentCard,GetCustomerPrescription);

            if (customerPrescriptionViewModel.ShowDialog)
            {
                var cusPreWindow = new CustomerPrescriptionWindow()
                {
                    DataContext = customerPrescriptionViewModel
                };
                cusPreWindow.ShowDialog();
            }
        }

        private bool CheckPatientEqual(Prescription receive)
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber)) return true;
            if (CurrentPrescription.Patient.IDNumber.Equals(receive.Patient.IDNumber))
                return true;
            MessageWindow.ShowMessage("代入處方病患資料與目前病患資料不符，請確認。", MessageType.ERROR);
            return false;
        }

        private void CheckCustomerEdited()
        {
            if (CustomerEdited)
            {
                var savePatientData = new ConfirmWindow("顧客資料已被編輯，是否儲存變更?", "顧客編輯確認");
                if ((bool)savePatientData.DialogResult)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    CurrentPrescription.Patient.Save();
                    MainWindow.ServerConnection.CloseConnection();
                    CustomerEdited = false;
                }
                else
                {
                    CustomerRedoEditedAction();
                    CustomerEdited = false;
                }
            }
        }

        #endregion PatinetFunctions

        #region MedicinesFunctions

        private int GetProductCount(string medicineID)
        {
            var wareHouse = CurrentPrescription.WareHouse;
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse is null ? "0" : wareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            return productCount;
        }

        private bool CheckMedicineIDLength(string medicineID)
        {
            if (medicineID.Length >= 5) return true;
            switch (medicineID)
            {
                case "R001":
                case "R002":
                case "R003":
                case "R004":
                case "R005":
                    CurrentPrescription.Medicines.Add(new MedicineVirtual(medicineID));
                    break;

                default:
                    MessageWindow.ShowMessage(Resources.搜尋字串長度不足 + "5", MessageType.WARNING);
                    break;
            }
            return false;
        }

        private bool SetBuckleAmount()
        {
            return setBuckleAmount;
        }

        #endregion MedicinesFunctions

        #region AdjustFunctions

        private void StartNormalAdjust()
        {
            if (!currentService.StartNormalAdjust())
            {
                isAdjusting = false;
                return;
            }
            currentService.CheckDailyUpload(ErrorCode);
            currentService.CloneTempPre();
            StartPrint(false);
            HisApiFunction.CheckDailyUpload100();
            DeclareSuccess();
        }

        private void StartErrorAdjust()
        {
            if (!currentService.StartErrorAdjust())
            {
                isAdjusting = false;
                return;
            }
            currentService.CloneTempPre();
            StartPrint(false);
            DeclareSuccess();
        }

        private void StartDepositAdjust()
        {
            CurrentPrescription.CountDeposit();
            if (!currentService.StartDepositAdjust())
            {
                isAdjusting = false;
                return;
            }
            currentService.CloneTempPre();
            StartPrint(true);
            DeclareSuccess();
        }

        private void StartRegister()
        {
            var result = true;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "處方登錄中...";
                if (currentService.StartRegister())
                {

                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        currentService.CloneTempPre();
                    });
                    StartPrint(false);
                    BusyContent = "取得預約處方...";
                    CheckAutoRegister();
                }
                else
                {
                    isAdjusting = false;
                    result = false;
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if (result)
                    DeclareSuccess();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void CheckAutoRegister()
        {
            var registerList = new Prescriptions();
            MainWindow.ServerConnection.OpenConnection();
            registerList.GetAutoRegisterReserve(CurrentPrescription);
            MainWindow.ServerConnection.CloseConnection();
            if (registerList.Count > 0 && RegisterConfirm(registerList))
            {
                foreach (var p in registerList.Where(r => r.AdjustDate != null))
                {
                    p.PrescriptionStatus.IsSendOrder = true;
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        StartAutoRegister(p);
                    });
                }
            }
        }

        private bool RegisterConfirm(Prescriptions registerList)
        {
            var result = false;
            Application.Current.Dispatcher.Invoke(delegate
            {
                var registerWindow = new AutoRegisterWindow(CurrentPrescription, registerList);
                result = registerWindow.RegisterResult;
            });
            return result;
        }

        private void StartPrescribeAdjust()
        {
            CurrentPrescription.SetDetail();
            if (!currentService.StartPrescribeAdjust())
            {
                isAdjusting = false;
                return;
            }
            currentService.CloneTempPre();
            StartPrint(false);
            DeclareSuccess();
        }

        private bool ErrorAdjustConfirm()
        {
            var errorAdjustConfirm = new ConfirmWindow("確認異常結案?(非必要請勿使用此功能，若過卡率低於九成，將被勸導限期改善並列為輔導對象，最重可能勒令停業)", "異常確認");
            Debug.Assert(errorAdjustConfirm.DialogResult != null, "errorAdjustConfirm.DialogResult != null");
            return (bool)errorAdjustConfirm.DialogResult;
        }

        private bool CheckPrescription(bool noCard, bool errorAdjust)
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            var setPharmacist = currentService.SetPharmacist(SelectedPharmacist, PrescriptionCount);
            if (!setPharmacist)
            {
                isAdjusting = false;
                return false;
            }
            MainWindow.ServerConnection.OpenConnection();
            var checkPrescription = currentService.CheckPrescription(noCard, errorAdjust);
            MainWindow.ServerConnection.CloseConnection();
            if (!checkPrescription)
                isAdjusting = false;
            else
                CurrentPrescription.SetDetail();
            return checkPrescription;
        }

        private bool CheckPrescriptionBeforeOrder(bool noCard, bool errorAdjust)
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            var setPharmacist = currentService.SetPharmacist(SelectedPharmacist, PrescriptionCount);
            if (!setPharmacist)
            {
                isAdjusting = false;
                return false;
            }
            MainWindow.ServerConnection.OpenConnection();
            var checkPrescriptionBeforeOrder = currentService.CheckPrescriptionBeforeOrder(noCard, errorAdjust);
            MainWindow.ServerConnection.CloseConnection();
            if (!checkPrescriptionBeforeOrder)
                isAdjusting = false;
            return checkPrescriptionBeforeOrder;
        }
        private bool CheckRegisterPrescription(bool noCard, bool errorAdjust)
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            var setPharmacist = currentService.SetRegisterPharmacist(SelectedPharmacist, PrescriptionCount);
            if (!setPharmacist)
            {
                isAdjusting = false;
                return false;
            }
            MainWindow.ServerConnection.OpenConnection();
            var checkPrescription = currentService.CheckPrescription(noCard, errorAdjust);
            MainWindow.ServerConnection.CloseConnection();
            if (!checkPrescription)
                isAdjusting = false;
            else
                CurrentPrescription.SetDetail();
            return checkPrescription;
        }

        private void StartAutoRegister(Prescription p)
        {
            var service = PrescriptionService.CreateService(p);
            service.SetPharmacistWithoutCheckCount(SelectedPharmacist);
            MainWindow.ServerConnection.OpenConnection();
            ((NormalPrescriptionService)service).CheckPrescriptionFromAutoRegister();
            MainWindow.ServerConnection.CloseConnection();
            p.SetDetail();
            service.StartRegister();
            service.CloneTempPre();
            service.Print(false);
        }

        private void DeclareSuccess()
        {
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionCount = UpdatePrescriptionCount();
            MainWindow.ServerConnection.CloseConnection();
            ClearAction();
            //MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            isAdjusting = false;
        }

        private void StartPrint(bool noCard)
        {
            currentService.Print(noCard);
        }

        private bool CheckIsAdjusting()
        {
            return !isAdjusting;
        }

        private bool CheckDepositAdjustEnable()
        {
            return !isAdjusting && string.IsNullOrEmpty(currentCard.CardNumber);
        }

        #endregion AdjustFunctions

        #region CheckDeclareStatus

        private void CheckDeclareStatus()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.AdjustCase.ID)) return;
            if (CheckPrescribe()) return;
            if (CheckAdjustDateNull()) return;
            Debug.Assert(CurrentPrescription.AdjustDate != null, "CurrentPrescription.AdjustDate != null");
            var adjust = ((DateTime)CurrentPrescription.AdjustDate).Date;
            if (CheckAdjustToday(adjust)) return;
            if (CheckAdjustFuture(adjust)) return;
            CheckAdjustPast(adjust);
        }

        private bool CheckAdjustToday(DateTime adjust)
        {
            if (DateTime.Compare(adjust, DateTime.Today) != 0) return false;
            //填寫領藥次數且調劑案件為慢箋 => 登錄，其餘為調劑
            if (CurrentPrescription.CheckChronicSeqValid() && CurrentPrescription.AdjustCase.ID.Equals("2") && CurrentPrescription.PrescriptionStatus.IsSendOrder)
                DeclareStatus = PrescriptionDeclareStatus.Register;
            else
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
            return true;
        }

        private bool CheckAdjustFuture(DateTime adjust)
        {
            if (DateTime.Compare(adjust, DateTime.Today) <= 0) return false;
            //調劑日為未來且為慢箋 => 登錄
            if (CurrentPrescription.CheckChronicSeqValid() && CurrentPrescription.AdjustCase.ID.Equals("2"))
                DeclareStatus = PrescriptionDeclareStatus.Register;
            else
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
            return true;
        }

        private void CheckAdjustPast(DateTime adjust)
        {
            //調劑日為過去 => 調劑
            if (DateTime.Compare(adjust, DateTime.Today) < 0)
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
        }

        private bool CheckPrescribe()
        {
            if (CurrentPrescription.AdjustCase.CheckIsPrescribe())
            {
                DeclareStatus = PrescriptionDeclareStatus.Prescribe;
                CurrentPrescription.SetPrescribeAdjustCase();
                return true;
            }
            if (!CurrentPrescription.IsPrescribe)
                return false;
            DeclareStatus = PrescriptionDeclareStatus.Prescribe;
            CurrentPrescription.SetPrescribeAdjustCase();
            return true;
        }

        private bool CheckAdjustDateNull()
        {
            if (!(CurrentPrescription.AdjustDate is null)) return false;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            return true;
        }

        #endregion CheckDeclareStatus

        #region MedicineSetFunctions

        private void GetMedicinesFromMedicineSet()
        {
            CurrentSet.MedicineSetItems = new MedicineSetItems();
            CurrentSet.MedicineSetItems.GetItems(CurrentSet.ID);
            CurrentPrescription.GetMedicinesBySet(CurrentSet);
            CurrentPrescription.UpdateMedicines();
            CurrentPrescription.Medicines.ReOrder();
            CountMedicinePointAction();
        }

        private void AddMedicineSet()
        {
            var tempID = 0;
            var medicineSetWindow = new MedicineSetWindow(MedicineSetMode.Add);
            medicineSetWindow.ShowDialog();
            if (CurrentSet != null)
                tempID = CurrentSet.ID;
            MedicineSets = new MedicineSets();
            if (CurrentSet != null)
                CurrentSet = MedicineSets.SingleOrDefault(s => s.ID.Equals(tempID));
        }

        private void EditCurrentMedicineSet()
        {
            var medicineSetWindow = new MedicineSetWindow(MedicineSetMode.Edit, CurrentSet);
            medicineSetWindow.ShowDialog();
            var tempID = CurrentSet.ID;
            MedicineSets = new MedicineSets();
            CurrentSet = MedicineSets.SingleOrDefault(s => s.ID.Equals(tempID));
        }

        #endregion MedicineSetFunctions

        #region OtherFunctions

        private void CheckNewCustomer()
        {
            // ReSharper disable once TooManyChainedReferences
            MainWindow.ServerConnection.OpenConnection();
            var customers = CurrentPrescription.Patient.Check();
            switch (customers.Count)
            {
                case 0:
                    CheckInsertCustomerData();
                    break;

                case 1:
                    CurrentPrescription.Patient = customers[0];
                    CurrentPrescription.Patient.GetHistories();
                    break;
            }
            RaisePropertyChanged("CanSearchPatient");
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckInsertCustomerData()
        {
            return CurrentPrescription.Patient.CheckData() && CurrentPrescription.Patient.InsertData();
        }

        private bool CheckFocusDivision(string insID)
        {
            return CurrentPrescription.Institution != null &&
                   !string.IsNullOrEmpty(CurrentPrescription.Institution.FullName) &&
                   insID.Equals(CurrentPrescription.Institution.FullName);
        }

        private int UpdatePrescriptionCount()//計算處方張數
        {
            return SelectedPharmacist != null
                ? PrescriptionDb.GetPrescriptionCountByID(SelectedPharmacist.IDNumber).Rows[0].Field<int>("PrescriptionCount")
                : 0;
        }

        private bool CheckMedicinesNegativeStock()
        {
            var result = string.Empty;
            result = CurrentPrescription.CheckMedicinesNegativeStock();
            if (!string.IsNullOrEmpty(result))
                isAdjusting = false;
            return string.IsNullOrEmpty(result);
        }

        private bool CheckAdjustDate()
        {
            if (CurrentPrescription.AdjustDate is null)
            {
                MessageWindow.ShowMessage("請填寫調劑日期", MessageType.ERROR);
                return false;
            }
            return true;
        }

        private void CheckWay()
        {
            foreach (var c in CurrentPrescription.Medicines)
            {
                if (c.PositionID == null)
                {
                }
                else if (c.PositionID.Length > 4)
                {
                    c.PositionID = c.PositionID.Substring(0, 4);
                }
            }
        }

        #endregion OtherFunctions
    }
}