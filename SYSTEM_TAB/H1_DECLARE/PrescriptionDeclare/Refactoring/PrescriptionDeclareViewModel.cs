using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.MedicineRefactoring;
using His_Pos.NewClass.Person.Customer;
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
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using Resources = His_Pos.Properties.Resources;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

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
        private IcCard currentCard;
        private bool setBuckleAmount;
        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode;
        private bool isNotInit;
        private bool isAdjusting;
        private bool isCardReading;
        #endregion
        #region Commands
        public RelayCommand ScanPrescriptionQRCode { get; set; }
        public RelayCommand<TextBox> GetCustomers { get; set; }
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand PharmacistChanged { get; set; }
        public RelayCommand AdjustDateChanged { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand CountPrescriptionPoint { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand Adjust { get; set; }
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
            InitPrescriptionAndCard();
            InitCommands();
        }

        private void InitPrescriptionAndCard()
        {
            isNotInit = false;
            CurrentPrescription = new Prescription();
            CurrentPrescription.Init();
            currentCard = new IcCard();
            isNotInit = true;
        }

        private void InitCommands()
        {
            ScanPrescriptionQRCode = new RelayCommand(ScanPrescriptionQRCodeAction);
            GetCooperativePres = new RelayCommand(GetCooperativePresAction);
            GetPatientData = new RelayCommand(GetPatientDataAction);
            GetCustomers = new RelayCommand<TextBox>(GetCustomersAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            PharmacistChanged = new RelayCommand(PharmacistChangedAction);
            AdjustDateChanged = new RelayCommand(AdjustDateChangedAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            CountPrescriptionPoint = new RelayCommand(CountMedicinePointAction);
            MedicineAmountChanged = new RelayCommand(MedicineAmountChangedAction,SetBuckleAmount);
            Clear = new RelayCommand(InitPrescriptionAndCard);
            Adjust = new RelayCommand(AdjustAction,CheckIsAdjusting);
        }

        private bool CheckIsAdjusting()
        {
            return isAdjusting;
        }
        #endregion
        #region CommandAction
        private void ScanPrescriptionQRCodeAction()
        {

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
            if(!CheckPatientEqual(receiveMsg.Content)) return;
            CurrentPrescription = receiveMsg.Content;
            setBuckleAmount = true;
            isNotInit = true;
        }

        private void GetPatientDataAction()
        {
            //取得病患資料(讀卡)
            var result = false;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => { ReadCard(ref result); };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if(result)
                    GetPatientFromIcCard();
                else
                    AskErrorUpload();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ReadCard(ref bool result)
        {
            BusyContent = Resources.讀取健保卡;
            try
            {
                result = currentCard.Read();
            }
            catch (Exception e)
            {
                NewFunction.ExceptionLog(e.Message);
                Application.Current.Dispatcher.Invoke(() => MessageWindow.ShowMessage("讀卡作業異常，請重開處方登錄頁面並重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING));
            }
        }

        private void GetPatientFromIcCard()
        {
            var patient = new Customer(currentCard);
            MainWindow.ServerConnection.OpenConnection();
            patient.Check();
            MainWindow.ServerConnection.CloseConnection();
            var checkPatient = CurrentPrescription.CheckPatientWithCard(patient);
            if (checkPatient)
                GetSelectedCustomer(patient);
        }

        //顧客查詢
        private void GetCustomersAction(TextBox condition)
        {
            #region ReSharperDisable
            //ReSharper disable RedundantAssignment
            //ReSharper disable once UnusedVariable
            #endregion
            Messenger.Default.Register<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
            CustomerSearchWindow customerSearch;
            switch (condition.Name)
            {
                case "PatientIDNumber" when string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber):
                    MessageWindow.ShowMessage(Resources.身分證空值, MessageType.WARNING);
                    break;
                case "PatientIDNumber":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.IDNumber, CustomerSearchCondition.IDNumber);
                    break;
                case "PatientName" when string.IsNullOrEmpty(CurrentPrescription.Patient.Name):
                    MessageWindow.ShowMessage(Resources.姓名空值, MessageType.WARNING);
                    break;
                case "PatientName":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Name, CustomerSearchCondition.Name);
                    break;
                case "PatientBirthday" when CurrentPrescription.Patient.Birthday is null:
                    MessageWindow.ShowMessage(Resources.生日空值, MessageType.WARNING);
                    break;
                case "PatientBirthday":
                    customerSearch = new CustomerSearchWindow(DateTimeExtensions.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), CustomerSearchCondition.Birthday);
                    break;
                case "PatientTel" when string.IsNullOrEmpty(CurrentPrescription.Patient.Tel):
                    MessageWindow.ShowMessage(Resources.電話空值, MessageType.WARNING);
                    break;
                case "PatientTel":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.Tel, CustomerSearchCondition.Tel);
                    break;
                case "PatientCellPhone" when string.IsNullOrEmpty(CurrentPrescription.Patient.CellPhone):
                    MessageWindow.ShowMessage(Resources.電話空值, MessageType.WARNING);
                    break;
                case "PatientCellPhone":
                    customerSearch = new CustomerSearchWindow(CurrentPrescription.Patient.CellPhone, CustomerSearchCondition.Tel);
                    break;
            }
            Messenger.Default.Unregister<Customer>(this, "GetSelectedCustomer", GetSelectedCustomer);
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
            CurrentPrescription.UpdateMedicines();
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
        private bool CheckMedicineIDLength(string medicineID)
        {
            if (medicineID.Length < 5)
            {
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
            return true;
        }

        private void DeleteMedicineAction()
        {
            CurrentPrescription.DeleteMedicine();
        }

        private void CountMedicinePointAction()
        {
            CurrentPrescription.CountPrescriptionPoint();
        }

        private bool SetBuckleAmount()
        {
            return setBuckleAmount;
        }

        private void MedicineAmountChangedAction()
        {
            CurrentPrescription.SetBuckleAmount();
        }
        private void AdjustAction()
        {
            isAdjusting = true;
            var service = PrescriptionService.CreateService(CurrentPrescription);
            if (!service.SetPharmacist(SelectedPharmacist, PrescriptionCount))
            {
                isAdjusting = false;
                return;
            }
            if (!service.CheckPrescription())
            {
                isAdjusting = false;
                return;
            }
            if(!service.StartNormalAdjust())
            {
                isAdjusting = false;
                return;
            }

        }
        #endregion

        #region MessengerFunctions
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

        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = receiveSelectedInstitution;
            CurrentPrescription.UpdateMedicines();
            var notification = string.IsNullOrEmpty(CurrentPrescription.Division.ID) ? "FocusDivision" : "FocusMedicalNumber";
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
        #endregion
        #region Functions
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
        private void CustomerNotExist()
        {
            if (!CurrentPrescription.Patient.CheckData())
                MessageWindow.ShowMessage(Resources.顧客資料不足, MessageType.WARNING);
            else
            {
                var confirm = new ConfirmWindow(Resources.新增顧客確認, Resources.查無資料, true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool)confirm.DialogResult)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    if (CurrentPrescription.Patient.CheckIDNumberExist())
                    {
                        MessageWindow.ShowMessage("此身分證已存在，請確認顧客資料", MessageType.WARNING);
                    }
                    else
                    {
                        CurrentPrescription.Patient.InsertData();
                        CurrentPrescription.Patient.GetHistories();
                    }
                    MainWindow.ServerConnection.CloseConnection();
                }
            }
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
            MessageWindow.ShowMessage("代入處方病患資料與目前病患資料不符，請確認。",MessageType.ERROR);
            return false;
        }

        private int GetProductCount(string medicineID)
        {
            var wareHouse = CurrentPrescription.WareHouse;
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse is null ? "0" : wareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            return productCount;
        }
        private void AskErrorUpload()
        {
            var e = new ErrorUploadWindow(currentCard.IsGetMedicalNumber);
            e.ShowDialog();
            if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage(Resources.尚未選擇異常代碼, MessageType.WARNING);
                isAdjusting = false;
                isCardReading = false;
            }
            ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
        }
        #endregion
    }
}
