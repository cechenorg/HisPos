#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativePrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerPrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow;
using Application = System.Windows.Application;
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
#endregion
namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    public class PrescriptionDeclareViewModel : TabBase
    {
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
            set { Set(() => MedicalPersonnels,ref medicalPersonnels,value ); }
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
        private Medicines usableMedicines { get; set; }
        #endregion
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
                if(value is null) return;
                if (!string.IsNullOrEmpty(value) && value.Equals("Option2"))
                {
                    if (CurrentPrescription.Patient is null || !CurrentPrescription.Patient.CheckData())
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
        private IcCard currentCard;
        private PrescriptionService currentService;
        private bool setBuckleAmount;
        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode;
        private bool isNotInit;
        private bool isAdjusting;
        private bool isCardReading;
        #endregion
        #region Commands
        public RelayCommand ScanPrescriptionQRCode { get; set; }
        public RelayCommand<TextBox> GetCustomers { get; set; }
        public RelayCommand ShowCustomerEditWindow { get; set; }
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand ShowCustomerDetail { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand PharmacistChanged { get; set; }
        public RelayCommand AdjustDateChanged { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> CheckClearDisease { get; set; }
        public RelayCommand ChronicSequenceTextChanged { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand CopaymentSelectionChanged { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CountPrescriptionPoint { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand CopyPrescription { get; set; }
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
        #endregion
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
        }

        private void SetPharmacist()
        {
            if (MedicalPersonnels.SingleOrDefault(e => e.ID.Equals(VM.CurrentUser.ID)) != null)
            {
                SelectedPharmacist = MedicalPersonnels.SingleOrDefault(e => e.ID.Equals(VM.CurrentUser.ID));
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
        }

        [SuppressMessage("ReSharper", "MethodTooLong")]
        private void InitCommands()
        {
            ScanPrescriptionQRCode = new RelayCommand(ScanPrescriptionQRCodeAction);
            GetCooperativePres = new RelayCommand(GetCooperativePresAction);
            GetPatientData = new RelayCommand(GetPatientDataAction,CheckIsCardReading);
            ShowCustomerDetail = new RelayCommand(ShowCustomerDetailAction);
            GetCustomers = new RelayCommand<TextBox>(GetCustomersAction);
            ShowCustomerEditWindow = new RelayCommand(ShowCustomerEditWindowAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            PharmacistChanged = new RelayCommand(PharmacistChangedAction);
            AdjustDateChanged = new RelayCommand(AdjustDateChangedAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            CheckClearDisease = new RelayCommand<object>(CheckClearDiseaseAction);
            ChronicSequenceTextChanged = new RelayCommand(ChronicSequenceChangedAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CountPrescriptionPoint = new RelayCommand(CountMedicinePointAction);
            MedicineAmountChanged = new RelayCommand(MedicineAmountChangedAction,SetBuckleAmount);
            CopyPrescription = new RelayCommand(CopyPrescriptionAction);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            EditMedicineSet = new RelayCommand<string>(EditMedicineSetAction);
            CustomerDetailEdited = new RelayCommand(CustomerDetailEditedAction);
            CustomerRedoEdited = new RelayCommand(CustomerRedoEditedAction);
            SavePatientData = new RelayCommand(SavePatientDataAction);
            SendOrder = new RelayCommand(SendOrderAction);
            Clear = new RelayCommand(ClearAction);
            ErrorAdjust = new RelayCommand(ErrorAdjustAction, CheckIsAdjusting);
            DepositAdjust = new RelayCommand(DepositAdjustAction, CheckDepositAdjustEnable);
            Adjust = new RelayCommand(AdjustAction,CheckIsAdjusting);
            Register = new RelayCommand(RegisterAction,CheckIsAdjusting);
            PrescribeAdjust = new RelayCommand(PrescribeAdjustAction, CheckIsAdjusting);
        }
        #endregion
        #region CommandAction
        private void ScanPrescriptionQRCodeAction()
        {
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var receive = new QRCodeReceiveWindow();
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
        }

        private void GetCooperativePresAction()
        {
            //查詢合作診所處方
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cooPresWindow = new CooperativePrescriptionWindow();
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
        }

        private void GetCustomerPrescription(NotificationMessage<Prescription> receiveMsg)
        {
            isNotInit = false;
            setBuckleAmount = false;
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("QRCodePrescriptionScanned", GetCustomerPrescription);
            if (!CheckPatientEqual(receiveMsg.Content)) return;
            CurrentPrescription = receiveMsg.Content;
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

        private void ShowCustomerDetailAction()
        {
            if (CurrentPrescription.Patient.ID <= 0) return;
            var customerDetailWindow = new CustomerDetailWindow(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient = Customer.GetCustomerByCusId(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
        }

        //顧客查詢
        private void GetCustomersAction(TextBox condition)
        {
            Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
            if (!CheckConditionEmpty(condition.Name))
            {
                Messenger.Default.Unregister<NotificationMessage<Customer>>(this, GetSelectedCustomer);
                return;
            }
            ShowCustomerSearch(condition.Name);
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
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.IDNumber, CustomerSearchCondition.IDNumber);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
                case "PatientName":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Name, CustomerSearchCondition.Name);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
                case "PatientBirthday":
                    customerSearch = new CustomerSearchWindow((DateTime)CurrentPrescription.Patient.Birthday);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
                case "PatientTel":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Tel, CustomerSearchCondition.Tel);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
                case "PatientCellPhone":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.CellPhone, CustomerSearchCondition.Tel);
                    Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    break;
            }
        }

        private bool CheckConditionEmpty(string conditionName)
        {
            switch (conditionName)
            {
                case "PatientIDNumber" when CurrentPrescription.CheckPatientDataEmpty("IDNumber"):
                    MessageWindow.ShowMessage(Resources.身分證空值, MessageType.WARNING);
                    return false;
                case "PatientName" when CurrentPrescription.CheckPatientDataEmpty("Name"):
                    MessageWindow.ShowMessage(Resources.姓名空值, MessageType.WARNING);
                    return false;
                case "PatientBirthday" when CurrentPrescription.CheckPatientDataEmpty("Birthday"):
                    MessageWindow.ShowMessage(Resources.生日空值, MessageType.WARNING);
                    return false;
                case "PatientTel" when CurrentPrescription.CheckPatientDataEmpty("Tel"):
                    MessageWindow.ShowMessage(Resources.電話空值, MessageType.WARNING);
                    return false;
                case "PatientCellPhone" when CurrentPrescription.CheckPatientDataEmpty("CellPhone"):
                    MessageWindow.ShowMessage(Resources.電話空值, MessageType.WARNING);
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
            #endregion
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
            #endregion
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var commonHospitalsWindow = new CommonHospitalsWindow();
        }

        private void PharmacistChangedAction()
        {
            PrescriptionCount = UpdatePrescriptionCount();
            if (PrescriptionCount >= 80) MessageWindow.ShowMessage(Resources.調劑張數提醒 + prescriptionCount + "張", MessageType.WARNING);
            if(isNotInit && SelectedPharmacist != null)
                Messenger.Default.Send(new NotificationMessage(this, "FocusMedicalNumber"));
        }

        private void AdjustDateChangedAction()
        {
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(CurrentPrescription.AdjustDate??DateTime.Today);
            if(CurrentPrescription.AdjustDate != null)
                CurrentPrescription.UpdateMedicines();
            CheckDeclareStatus();
        }

        private void GetDiseaseCodeAction(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEquals(parameters))
            {
                DiseaseFocusNext(elementName);
                return;
            }
            //診斷碼查詢
            switch (elementName)
            {
                case "MainDiagnosis":
                    CurrentPrescription.MainDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
                case "SecondDiagnosis":
                    CurrentPrescription.SubDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
            }
        }

        private void CheckClearDiseaseAction(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID))
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
            if(CurrentPrescription.Copayment is null || CurrentPrescription.PrescriptionPoint is null) return;
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
            if(!CheckMedicineIDLength(medicineID)) return;
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
        }

        private void DeleteMedicineAction()
        {
            CurrentPrescription.DeleteMedicine();
            CurrentPrescription.CountPrescriptionPoint();
            CurrentPrescription.CountSelfPay();
            CurrentPrescription.PrescriptionPoint.CountAmountsPay();
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
            MessageWindow.ShowMessage("編輯成功",MessageType.SUCCESS);
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
            CheckCustomerEdited();
            if(!ErrorAdjustConfirm()) return;
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckPrescription(false)) return;
            StartErrorAdjust();
        }

        private void DepositAdjustAction()
        {
            CheckCustomerEdited();
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckPrescription(true)) return;
            StartDepositAdjust();
        }

        private void AdjustAction()
        {
            CheckCustomerEdited();
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckAdjustDatePast10Days()) return;
            if (!CheckPrescription(false)) return;
            CheckIsReadCard();
        }

        private void RegisterAction()
        {
            CheckCustomerEdited();
            isAdjusting = true;
            if (!CheckPrescription(false)) return;
            CurrentPrescription.PrescriptionStatus.IsSendOrder = true;
            StartRegister();
        }

        private void PrescribeAdjustAction()
        {
            CheckCustomerEdited();
            isAdjusting = true;
            if (!CheckMedicinesNegativeStock()) return;
            if (!CheckPrescription(false)) return;
            StartPrescribeAdjust();
        }
        #endregion
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
            var table = CustomerDb.CheckCustomerByCard(currentCard.IDNumber);
            if (table.Rows.Count > 0)
            {
                var patientFromDB = new Customer(table.Rows[0]);
                patientFromDB.CheckPatientWithCard(patientFromCard);
                checkedPatient = patientFromDB;
                checkedPatient.Save();
            }
            else
            {
                patientFromCard.InsertData();
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
                if(!CheckInsertCustomerData()) return;
            }
            else
                CurrentPrescription.Patient = receiveSelectedCustomer.Content;
            CurrentPrescription.Patient.UpdateEditTime();
            CurrentPrescription.Patient.GetHistories();
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
        #endregion
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
                Application.Current.Dispatcher.Invoke(() => MessageWindow.ShowMessage("讀卡作業異常，請重開處方登錄頁面並重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING));
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

        private void WriteCard()
        {
            if (!CheckIsGetMedicalNumber()) return;
            if(!CheckPatientMatch()) return;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = Resources.寫卡;
                currentService.SetCard(currentCard);
                currentService.CreateDailyUploadData(ErrorCode);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                StartNormalAdjust();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
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
        #endregion
        #region PatinetFunctions
        private void GetPatientDataComplete()
        {
            IsBusy = false;
            isCardReading = false;
            if (currentCard.IsRead)
                GetPatientFromIcCard();
            else
            {
                MessageWindow.ShowMessage("讀取卡片異常，請確認卡面及讀卡機燈號正常，如持續異常且其他健保卡可正常讀取請使用異常上傳。",MessageType.WARNING);
            }
        }
        private void CheckCustomPrescriptions()
        {
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cusPreWindow = new CustomerPrescriptionWindow(CurrentPrescription.Patient, currentCard);
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
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
                var savePatientData = new ConfirmWindow("顧客資料已被編輯，是否儲存變更?","顧客編輯確認");
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
        #endregion
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


        #endregion
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
            currentService.StartErrorAdjust();
            currentService.CloneTempPre();
            StartPrint(false);
            DeclareSuccess();
        }

        private void StartDepositAdjust()
        {
            CurrentPrescription.CountDeposit();
            currentService.StartDepositAdjust();
            currentService.CloneTempPre();
            StartPrint(true);
            DeclareSuccess();
        }

        private void StartRegister()
        {
            if (!currentService.StartRegister())
            {
                isAdjusting = false;
                return;
            }
            currentService.CloneTempPre();
            StartPrint(false);
            DeclareSuccess();
        }

        private void StartPrescribeAdjust()
        {
            CurrentPrescription.SetDetail();
            currentService.StartPrescribeAdjust();
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

        private bool CheckPrescription(bool noCard)
        {
            currentService = PrescriptionService.CreateService(CurrentPrescription);
            var setPharmacist = currentService.SetPharmacist(SelectedPharmacist, PrescriptionCount);
            if (!setPharmacist)
            {
                isAdjusting = false;
                return false;
            }
            MainWindow.ServerConnection.OpenConnection();
            var checkPrescription = currentService.CheckPrescription(noCard);
            MainWindow.ServerConnection.CloseConnection();
            if(!checkPrescription)
                isAdjusting = false;
            else
                CurrentPrescription.SetDetail();
            return checkPrescription;
        }

        private void DeclareSuccess()
        {
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionCount = UpdatePrescriptionCount();
            MainWindow.ServerConnection.CloseConnection();
            ClearAction();
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
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
        #endregion
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

        #endregion
        #region MedicineSetFunctions
        private void GetMedicinesFromMedicineSet()
        {
            CurrentSet.MedicineSetItems = new MedicineSetItems();
            CurrentSet.MedicineSetItems.GetItems(CurrentSet.ID);
            CurrentPrescription.GetMedicinesBySet(CurrentSet);
            CurrentPrescription.UpdateMedicines();
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

        #endregion
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
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckInsertCustomerData()
        {
            if (!CurrentPrescription.Patient.CheckData())
            {
                MessageWindow.ShowMessage(Resources.顧客資料不足, MessageType.WARNING);
                return false;
            }
            var insertCustomerConfirm = new ConfirmWindow("此病患為新病患，是否新增?", "新增確認", null);
            if (!(bool) insertCustomerConfirm.DialogResult) return false;
            CurrentPrescription.Patient.InsertData();
            return true;

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
            string result = string.Empty;
            result = CurrentPrescription.CheckMedicinesNegativeStock();
            if (!string.IsNullOrEmpty(result))
                isAdjusting = false;
            return string.IsNullOrEmpty(result);
        }

        private bool CheckAdjustDatePast10Days()
        {
            if (CurrentPrescription.AdjustDate is null)
            {
                MessageWindow.ShowMessage(" 調劑日不可為空", MessageType.ERROR);
                isAdjusting = false;
                return false;
            }
            if (DateTime.Compare(((DateTime)CurrentPrescription.AdjustDate).Date, DateTime.Today) >= 0) return true;
            var timeDiff = new TimeSpan(DateTime.Today.Ticks - ((DateTime)CurrentPrescription.AdjustDate).Ticks).TotalDays;
            if (timeDiff > 10)
            {
                MessageWindow.ShowMessage("處方調劑日已超過可過卡日(10日)，處方會被核刪，若是慢箋將影響病人下次看診領藥。如需以目前調劑日申報此處方或請使用異常結案或將調劑日改為今日以前十日內。", MessageType.ERROR);
                isAdjusting = false;
                return false;
            }
            return true;
        }
        #endregion
    }
}
