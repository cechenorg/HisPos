#region Using
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.MedicineRefactoring;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.PrescriptionRefactoring.Service;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.CustomerHistoryProduct;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Application = System.Windows.Application;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;
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
namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring
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

        public SpecialTreats SpecialTreats = VM.SpecialTreats;
        private MedicineSets medicineSets;
        public MedicineSets MedicineSets
        {
            get => medicineSets;
            set
            {
                Set(() => MedicineSets, ref medicineSets, value);
            }
        }
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
                if (CurrentPrescription.CheckCanRegister())
                    CanSendOrder = true;
                if (!CanSendOrder)
                    CurrentPrescription.PrescriptionStatus.IsSendOrder = false;
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
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand PharmacistChanged { get; set; }
        public RelayCommand AdjustDateChanged { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand ChronicSequenceTextChanged { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand CountPrescriptionPoint { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand CopyPrescription { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SendOrder { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand ErrorAdjust { get; set; }
        public RelayCommand DepositAdjust { get; set; }
        public RelayCommand Adjust { get; set; }
        public RelayCommand Register { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            Init();
        }
        #region InitFunctions
        private void Init()
        {
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(DateTime.Today);
            MainWindow.ServerConnection.OpenConnection();
            MedicineSets = new MedicineSets();
            MainWindow.ServerConnection.CloseConnection();
            InitLocalVariables();
            ClearAction();
            InitCommands();
        }

        private void InitLocalVariables()
        {
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
            GetCustomers = new RelayCommand<TextBox>(GetCustomersAction);
            ShowCustomerEditWindow = new RelayCommand(ShowCustomerEditWindowAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            PharmacistChanged = new RelayCommand(PharmacistChangedAction);
            AdjustDateChanged = new RelayCommand(AdjustDateChangedAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            ChronicSequenceTextChanged = new RelayCommand(ChronicSequenceChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            CountPrescriptionPoint = new RelayCommand(CountMedicinePointAction);
            MedicineAmountChanged = new RelayCommand(MedicineAmountChangedAction,SetBuckleAmount);
            CopyPrescription = new RelayCommand(CopyPrescriptionAction);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SendOrder = new RelayCommand(CheckDeclareStatus);
            Clear = new RelayCommand(ClearAction);
            ErrorAdjust = new RelayCommand(ErrorAdjustAction, CheckIsAdjusting);
            DepositAdjust = new RelayCommand(DepositAdjustAction, CheckIsAdjusting);
            Adjust = new RelayCommand(AdjustAction,CheckIsAdjusting);
            Register = new RelayCommand(RegisterAction,CheckIsAdjusting);
        }

        private void ChronicSequenceChangedAction()
        {
            CurrentPrescription.CheckPrescriptionVariable();
            CheckDeclareStatus();
        }
        #endregion
        #region CommandAction
        private void ScanPrescriptionQRCodeAction()
        {
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var receive = new QRCodeReceiveWindow();
        }

        private void GetCooperativePresAction()
        {
            //查詢合作診所處方
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cooPresWindow = new CooperativePrescriptionWindow();
        }

        private void GetCustomerPrescription(NotificationMessage<Prescription> receiveMsg)
        {
            isNotInit = false;
            setBuckleAmount = false;
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("QRCodePrescriptionScanned", GetCustomerPrescription);
            if (!CheckPatientEqual(receiveMsg.Content)) return;
            CurrentPrescription = receiveMsg.Content;
            CheckDeclareStatus();
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

        //顧客查詢
        private void GetCustomersAction(TextBox condition)
        {
            Messenger.Default.Register<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
            if (!CheckConditionEmpty(condition.Name))
            {
                Messenger.Default.Unregister<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
                return;
            }
            ShowCustomerSearch(condition.Name);
        }

        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private void ShowCustomerSearch(string conditionName)
        {
            CustomerSearchWindow customerSearch;
            switch (conditionName)
            {
                case "PatientIDNumber":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.IDNumber, CustomerSearchCondition.IDNumber);
                    break;
                case "PatientName":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Name, CustomerSearchCondition.Name);
                    break;
                case "PatientBirthday":
                    customerSearch = new CustomerSearchWindow(DateTimeExtensions.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), CustomerSearchCondition.Birthday);
                    break;
                case "PatientTel":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Tel, CustomerSearchCondition.Tel);
                    break;
                case "PatientCellPhone":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.CellPhone, CustomerSearchCondition.Tel);
                    break;
            }
            Messenger.Default.Unregister<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
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
            if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEquals(parameters)) DiseaseFocusNext(elementName);
            //診斷碼查詢
            switch (elementName)
            {
                case "MainDiagnosis":
                    CurrentPrescription.MainDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
                case "SecondDiagnosis":
                    if(!string.IsNullOrEmpty(diseaseID))
                        CurrentPrescription.SubDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
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
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            var wareID = CurrentPrescription.WareHouse is null ? "0" : CurrentPrescription.GetWareHouseID();
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, wareID }, "ShowProductDetail"));
        }

        private void CountMedicinePointAction()
        {
            CurrentPrescription.CountPrescriptionPoint();
            CheckDeclareStatus();
        }

        private void ShowPrescriptionEditWindowAction()
        {
            if (SelectedHistory is null) return;
            var pSource = SelectedHistory.GetPrescriptionSourceFromHistoryType();
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            var prescriptionEdit = new PrescriptionEditWindow(SelectedHistory.SourceId, pSource);
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }

        private void MedicineAmountChangedAction()
        {
            CurrentPrescription.SetBuckleAmount();
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
        }

        private void ErrorAdjustAction()
        {
            if(!ErrorAdjustConfirm()) return;
            isAdjusting = true;
            if (!CheckPrescription(false))
            {
                isAdjusting = false;
                return;
            }
            StartErrorAdjust();
        }

        private void DepositAdjustAction()
        {
            isAdjusting = true;
            if (!CheckPrescription(true))
            {
                isAdjusting = false;
                return;
            }
            StartDepositAdjust();
        }

        private void AdjustAction()
        {
            isAdjusting = true;
            if (!CheckPrescription(false))
            {
                isAdjusting = false;
                return;
            }
            CheckIsReadCard();
        }

        private void RegisterAction()
        {
            isAdjusting = true;
            if (!CheckPrescription(false))
            {
                isAdjusting = false;
                return;
            }
            StartRegister();
        }
        #endregion
        #region MessengerFunctions
        private void GetPatientFromIcCard()
        {
            CurrentPrescription.PrescriptionStatus.IsGetCard = true;
            var patientFromCard = new Customer(currentCard);
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
            MainWindow.ServerConnection.CloseConnection();
            GetSelectedCustomer(checkedPatient);
        }

        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
            if (receiveSelectedCustomer is null) return;
            CurrentPrescription.Patient = receiveSelectedCustomer;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.UpdateEditTime();
            CurrentPrescription.Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
            CheckCustomPrescriptions();
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
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this, GetSelectedMedicine);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.AddMedicine(msg.Content.ID);
            MainWindow.ServerConnection.CloseConnection();
            CurrentPrescription.CountPrescriptionPoint();
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
                AskErrorUpload();
        }
        private void CheckCustomPrescriptions()
        {
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cusPreWindow = new CustomerPrescriptionWindow(CurrentPrescription.Patient, currentCard);
        }

        private bool CheckPatientEqual(Prescription receive)
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber)) return true;
            if (CurrentPrescription.Patient.IDNumber.Equals(receive.Patient.IDNumber))
                return true;
            MessageWindow.ShowMessage("代入處方病患資料與目前病患資料不符，請確認。", MessageType.ERROR);
            return false;
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
            StartPrint(false);
            DeclareSuccess();
        }

        private void StartErrorAdjust()
        {
            currentService.StartErrorAdjust();
            StartPrint(false);
            DeclareSuccess();
        }

        private void StartDepositAdjust()
        {
            currentService.StartDepositAdjust();
            StartPrint(true);
            DeclareSuccess();
        }

        private void StartRegister()
        {
            if (!currentService.StartRegister()) return;
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
            return currentService.SetPharmacist(SelectedPharmacist, PrescriptionCount) && currentService.CheckPrescription(noCard);
        }

        private void DeclareSuccess()
        {
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearAction();
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
            if (DateTime.Compare(adjust, DateTime.Today) == 0) return false;
            //填寫領藥次數且調劑案件為慢箋 => 登錄，其餘為調劑
            if (CurrentPrescription.CheckChronicSeqValid() && CurrentPrescription.AdjustCase.ID.Equals("2") && CurrentPrescription.PrescriptionStatus.IsSendOrder)
                DeclareStatus = PrescriptionDeclareStatus.Register;
            else
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
            return true;
        }

        private bool CheckAdjustFuture(DateTime adjust)
        {
            if (DateTime.Compare(adjust, DateTime.Today) > 0) return false;
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
            if ((DateTime.Compare(adjust, DateTime.Today) < 0))
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
        }

        private bool CheckPrescribe()
        {
            if (!CurrentPrescription.IsPrescribe)
            {
                CurrentPrescription.AdjustCase = VM.GetAdjustCase("1");
                return false;
            }
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
        #region OtherFunctions
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
        #endregion
    }
}
