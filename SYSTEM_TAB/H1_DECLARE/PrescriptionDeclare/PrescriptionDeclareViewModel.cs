using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.CustomerPrescription;
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
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using CooPreSelectWindow = His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeSelectionWindow.CooperativeSelectionWindow;
using CusPreSelectWindow = His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow.CustomPrescriptionWindow;
using MedSendWindow = His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow.MedicinesSendSingdeWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;
using CusSelectWindow = His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSelectionWindow.CustomerSelectionWindow;
using InsSelectWindow = His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow.InstitutionSelectionWindow;
using MedSelectWindow = His_Pos.FunctionWindow.AddProductWindow.AddMedicineWindow;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using Resources = His_Pos.Properties.Resources;
using HisAPI = His_Pos.HisApi.HisApiFunction;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using Xceed.Wpf.Toolkit;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow;
using His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow;

// ReSharper disable InconsistentNaming
namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    public class PrescriptionDeclareViewModel : TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
        public enum RadioOptions { Option1 }
        private string selectedRadioButton;
        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set
            {
                Set(() => SelectedRadioButton, ref selectedRadioButton, value);
            }
        }
        #region ItemsSources
        public Institutions Institutions { get; set; }
        public Divisions Divisions { get; set; }
        public Employees MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
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
        private PrescriptionDeclareStatus declareStatus;
        public PrescriptionDeclareStatus DeclareStatus
        {
            get => declareStatus;
            set
            {
                Set(() => DeclareStatus, ref declareStatus, value);
                if(CurrentPrescription is null) return;
                if (CurrentPrescription.Treatment.AdjustDate!=null && CurrentPrescription.Treatment.AdjustCase.ID.Equals("2") 
                    && DateTime.Compare((DateTime)CurrentPrescription.Treatment.AdjustDate, DateTime.Today) >= 0)
                    CanSendOrder = true;
                if (!CanSendOrder)
                    CurrentPrescription.PrescriptionStatus.IsSendOrder = false;
            }
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
        private Prescription tempPre;
        public Prescription TempPre
        {
            get => tempPre;
            set
            {
                if (tempPre != value)
                {
                    Set(() => TempPre, ref tempPre, value);
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
        private int prescriptionCount;
        public int PrescriptionCount
        {
            get => prescriptionCount;
            set
            {
                if (prescriptionCount != value)
                {
                    Set(() => PrescriptionCount, ref prescriptionCount, value);
                }
            }
        }
        private bool notPrescribe;
        public bool NotPrescribe
        {
            get => notPrescribe;
            private set
            {
                Set(() => NotPrescribe, ref notPrescribe, value);
            }
        }
        private CusSelectWindow customerSelectionWindow { get; set; }
        private MedSelectWindow MedicineWindow { get; set; }
        private bool canSendOrder;
        public bool CanSendOrder
        {
            get => canSendOrder;
            set
            {
                Set(() => CanSendOrder, ref canSendOrder, value);
            }
        }
        private bool canAdjust;
        public bool CanAdjust
        {
            get => canAdjust;
            set
            {
                Set(() => CanAdjust, ref canAdjust, value);
            }
        }
        private bool isReadCard;
        public bool IsReadCard
        {
            get => isReadCard;
            set
            {
                Set(() => IsReadCard, ref isReadCard, value);
            }
        }
        private bool isAdjusting;
        public bool IsAdjusting
        {
            get => isAdjusting;
            set
            {
                Set(() => IsAdjusting, ref isAdjusting, value);
            }
        }
        private bool isCardReading;
        public bool IsCardReading
        {
            get => isCardReading;
            set
            {
                Set(() => IsCardReading, ref isCardReading, value);
            }
        }
        private bool customPresChecked { get; set; }
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
        private Employee selectedPharmacist;
        public Employee SelectedPharmacist
        {
            get => selectedPharmacist;
            set
            {
                Set(() => SelectedPharmacist, ref selectedPharmacist, value);
            }
        }
        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode { get; set; }
        private MedicineSet currentSet;
        public MedicineSet CurrentSet
        {
            get => currentSet;
            set
            {
                Set(() => CurrentSet, ref currentSet, value);
            }
        }
        private List<bool?> printResult { get; set; }
        #endregion
        #region Commands
        public RelayCommand ShowCooperativeSelectionWindow { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand<object> SearchCustomerByConditions { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand PharmacistSelectionChanged { get; set; }
        public RelayCommand<string> GetMainDiseaseCodeById { get; set; }
        public RelayCommand<string> GetSubDiseaseCodeById { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand CopaymentSelectionChanged { get; set; }
        public RelayCommand ShowCommonInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand MedicinePriceChanged { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand AdjustButtonClick { get; set; }
        public RelayCommand RegisterButtonClick { get; set; }
        public RelayCommand PrescribeButtonClick { get; set; }
        public RelayCommand ClearButtonClick { get; set; }
        public RelayCommand ChronicSequenceTextChanged { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand ErrorAdjust { get; set; }
        public RelayCommand NoCardAdjust { get; set; }
        public RelayCommand SendOrderCommand { get; set; }
        public RelayCommand ErrorCodeSelect { get; set; }
        public RelayCommand DivisionSelectionChanged { get; set; }
        public RelayCommand SelfPayTextChanged { get; set; }
        public RelayCommand CopyPrescription { get; set; }
        public RelayCommand<string> EditMedicineSet { get; set; }
        public RelayCommand ShowCustomerDetailCommand { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            Initial(true);
        }
        ~PrescriptionDeclareViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        #region InitialFunctions
        private void Initial(bool setPharmacist)
        {
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
            InitializeVariables(setPharmacist);
        }
        private void InitializeVariables(bool setPharmacist)
        {
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            NotPrescribe = true;
            CanAdjust = true;
            IsReadCard = false;
            ErrorCode = null;
            CanSendOrder = false;
            IsAdjusting = false;
            IsBusy = false;
            SelectedRadioButton = "Option1";
            InitialPrescription(setPharmacist);
        }
        private void InitialItemsSources()
        {
            Institutions = VM.Institutions;
            Divisions = VM.Divisions;
            MedicalPersonnels = VM.CurrentPharmacy.MedicalPersonnels.GetLocalPharmacist();
            AdjustCases = VM.AdjustCases;
            PaymentCategories = VM.PaymentCategories;
            PrescriptionCases = VM.PrescriptionCases;
            Copayments = VM.Copayments;
            SpecialTreats = VM.SpecialTreats;
            MainWindow.ServerConnection.OpenConnection();
            MedicineSets = new MedicineSets();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void InitialCommandActions()
        {
            SearchCustomerByConditions = new RelayCommand<object>(SearchCusAction);
            ErrorCodeSelect = new RelayCommand(ErrorCodeSelectAction);
            ShowCooperativeSelectionWindow = new RelayCommand(ShowCooperativeWindowAction);
            GetPatientData = new RelayCommand(GetPatientDataAction, CheckIsCardReading);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ShowCommonInsSelectionWindowAction);
            DivisionSelectionChanged = new RelayCommand(CheckPrescriptionCase);
            PharmacistSelectionChanged = new RelayCommand(PharmacistChangedAction);
            GetMainDiseaseCodeById = new RelayCommand<string>(GetMainDiseaseCodeByIdAction);
            GetSubDiseaseCodeById = new RelayCommand<string>(GetSubDiseaseCodeByIdAction);
            ChronicSequenceTextChanged = new RelayCommand(CheckPrescriptionVariable);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            MedicineAmountChanged = new RelayCommand(SetBuckleAmount);
            SelfPayTextChanged = new RelayCommand(SelfPayTextChangedAction);
            SendOrderCommand = new RelayCommand(CheckDeclareStatus);
            ClearButtonClick = new RelayCommand(ClearPrescription, CheckIsAdjusting);
            ErrorAdjust = new RelayCommand(ErrorAdjustAction, CheckIsAdjusting);
            NoCardAdjust = new RelayCommand(NoCardAdjustAction, CheckIsNoCard);
            AdjustButtonClick = new RelayCommand(AdjustButtonClickAction,CheckIsAdjusting);
            RegisterButtonClick = new RelayCommand(RegisterButtonClickAction);
            PrescribeButtonClick = new RelayCommand(PrescribeButtonClickAction);
            CopyPrescription = new RelayCommand(CopyPrescriptionAction);
            EditMedicineSet = new RelayCommand<string>(EditMedicineSetAction);
            ShowCustomerDetailCommand = new RelayCommand(ShowCustomerDetailAction);
        }

        private void InitialPrescription(bool setPharmacist)
        {
            CurrentPrescription = new Prescription();
            CurrentPrescription.InitialCurrentPrescription();
            if (setPharmacist)
                SelectedPharmacist = VM.CurrentPharmacy.GetPharmacist();
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionCount = UpdatePrescriptionCount();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage>("AdjustDateChanged", AdjustDateChanged);
            Messenger.Default.Register<NotificationMessage>(nameof(PrescriptionDeclareView) + "ShowPrescriptionEditWindow", ShowPrescriptionEditWindowAction);
            Messenger.Default.Register<NotificationMessage>("CustomPresChecked",(notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CustomPresChecked"))
                    customPresChecked = true;
            });
        }
        #endregion
        #region EventAction
        private void SetMedicinesPaySelf()
        {
            if (CurrentPrescription.Medicines.Count <= 0) return;
            foreach (var m in CurrentPrescription.Medicines)
            {
                if(m.PaySelf) continue;
                m.PaySelf = true;
            }
        }
        private void AdjustDateChanged(NotificationMessage adjustChange)
        {
            if (!adjustChange.Notification.Equals("AdjustDateChanged")) return;
            CheckDeclareStatus();
        }
        private void ShowPrescriptionEditWindowAction(NotificationMessage msg)
        {
            if (SelectedHistory is null || !msg.Notification.Equals(nameof(PrescriptionDeclareView) + "ShowPrescriptionEditWindow")) return;
            var pSource = SelectedHistory.Type.Equals(HistoryType.ReservedPrescription) ? PrescriptionSource.ChronicReserve : PrescriptionSource.Normal;
            var prescriptionEdit = new PrescriptionEditWindow(SelectedHistory.SourceId, pSource);
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            prescriptionEdit.ShowDialog();
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }
        private void Refresh(NotificationMessage msg)
        {
            if (!msg.Notification.Equals(nameof(PrescriptionDeclareViewModel) + "PrescriptionEdited")) return;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void CopyPrescriptionAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            var prescription = SelectedHistory.GetPrescriptionByID();
            prescription.AdjustMedicinesType();
            MainWindow.ServerConnection.CloseConnection();
            prescription.Treatment.TreatDate = null;
            prescription.Treatment.AdjustDate = null;
            prescription.Treatment.TempMedicalNumber = null;
            prescription.Card = CurrentPrescription.Card;
            prescription.Patient = CurrentPrescription.Patient;
            prescription.PrescriptionStatus.Init();
            CurrentPrescription = prescription;
            CurrentPrescription.Id = 0;
            CurrentPrescription.CheckIsCooperative();
        }
        private void EditMedicineSetAction(string mode)
        {
            if (CurrentSet is null && !mode.Equals("Add"))
            {
                MessageWindow.ShowMessage("尚未選擇藥品組合",MessageType.ERROR);
                return;
            }
            MedicineSetWindow medicineSetWindow;
            int tempID = 0;
            switch (mode)
            {
                case "Get":
                    MainWindow.ServerConnection.OpenConnection();
                    CurrentPrescription.CheckWareHouse();
                    CurrentSet.MedicineSetItems = new MedicineSetItems();
                    CurrentSet.MedicineSetItems.GetItems(CurrentSet.ID);
                    CurrentPrescription.Medicines.GetMedicineBySet(CurrentSet, CurrentPrescription.WareHouse is null ? "0" : CurrentPrescription.WareHouse.ID);
                    CurrentPrescription.CountPrescriptionPoint(true);
                    CurrentPrescription.CheckIsCooperative();
                    MainWindow.ServerConnection.CloseConnection();
                    break;
                case "Add":
                    medicineSetWindow = new MedicineSetWindow(MedicineSetMode.Add);
                    medicineSetWindow.ShowDialog();
                    if(CurrentSet != null)
                        tempID = CurrentSet.ID;
                    MainWindow.ServerConnection.OpenConnection();
                    MedicineSets = new MedicineSets();
                    MainWindow.ServerConnection.CloseConnection();
                    if (CurrentSet != null)
                        CurrentSet = MedicineSets.SingleOrDefault(s => s.ID.Equals(tempID));
                    break;
                case "Edit":
                    medicineSetWindow = new MedicineSetWindow(MedicineSetMode.Edit,CurrentSet);
                    medicineSetWindow.ShowDialog();
                    tempID = CurrentSet.ID;
                    MainWindow.ServerConnection.OpenConnection();
                    MedicineSets = new MedicineSets();
                    MainWindow.ServerConnection.CloseConnection();
                    CurrentSet = MedicineSets.SingleOrDefault(s => s.ID.Equals(tempID));
                    break;
            }
        }
        #endregion
        #region Actions
        private void ShowCustomerDetailAction() {
            if (CurrentPrescription.Patient.ID == -1) return;
            CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient = Customer.GetCustomerByCusId(CurrentPrescription.Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
        }
        private void SearchCusAction(object sender)
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            var customers = CurrentPrescription.Patient.Check();
            switch (customers.Count)
            {
                case 0:
                    AskAddCustomerData();
                    break;
                case 1:
                    CurrentPrescription.Patient = customers[0];
                    MainWindow.ServerConnection.OpenConnection();
                    CurrentPrescription.Patient.UpdateEditTime();
                    CurrentPrescription.Patient.GetHistories();
                    MainWindow.ServerConnection.CloseConnection();
                    CheckCustomPrescriptions();
                    break;
                default:
                    switch (sender)
                    {
                        case MaskedTextBox _ when CurrentPrescription.Patient.Birthday is null:
                            SearchCustomer(1, customers);
                            break;
                        case MaskedTextBox _:
                            customerSelectionWindow = new CusSelectWindow(DateTimeEx.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), 1, customers);
                            break;
                        case TextBox t:
                            switch (t.Name)
                            {
                                case "PatientName":
                                    if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                                        SearchCustomer(2, customers);
                                    else
                                        customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Name,
                                            2, customers);
                                    break;
                                case "PatientIDNumber":
                                    if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber))
                                        SearchCustomer(3, customers);
                                    else
                                        customerSelectionWindow =
                                            new CusSelectWindow(CurrentPrescription.Patient.IDNumber, 3, customers);
                                    break;
                                case "PatientTel":
                                    if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel))
                                        SearchCustomer(4, customers);
                                    else
                                        customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Name,
                                            4, customers);
                                    break;
                            }

                            break;
                    }
                    break;
            }
        }
        private void ErrorCodeSelectAction()
        {
            var e = new ErrorUploadWindow(false); //詢問異常上傳
            e.ShowDialog();
            var errCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            if (errCode != null)
                ErrorCode = errCode;
        }
        private void ShowCooperativeWindowAction()
        {
            var cooperative = new Prescriptions();
            var getCooperativePresWorker = new BackgroundWorker();
            getCooperativePresWorker.DoWork += (o, ea) =>
            {
                BusyContent = Resources.取得合作處方;
                XmlOfPrescriptions.GetFile();
                cooperative.GetCooperativePrescriptions(VM.CurrentPharmacy.ID, DateTime.Today.AddDays(-10), DateTime.Today); 
                cooperative.GetXmlOfPrescriptions(DateTime.Today.AddDays(-10), DateTime.Today);
            };
            getCooperativePresWorker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                Messenger.Default.Register<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
                var cooperativeSelect = new CooPreSelectWindow();
                Messenger.Default.Send(cooperative, "CooperativePrescriptions");
                cooperativeSelect.ShowDialog();
            };
            IsBusy = true;
            getCooperativePresWorker.RunWorkerAsync();
        }

        private void GetPatientDataAction()
        {
            ReadCard(true);
            customPresChecked = false;
            IsReadCard = true;
            IsCardReading = true;
        }

        private void ShowInsSelectionWindowAction(string search)
        {
            if (CurrentPrescription.Treatment.Institution != null && !string.IsNullOrEmpty(CurrentPrescription.Treatment.Institution.FullName) && search.Equals(CurrentPrescription.Treatment.Institution.FullName))
            {
                Messenger.Default.Send(new NotificationMessage(this,"FocusDivision"));
                return;
            }
            CurrentPrescription.Treatment.Institution = null;
            var result = Institutions.Where(i => i.ID.Contains(search) || i.Name.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    CurrentPrescription.Treatment.Institution = result[0];
                    CurrentPrescription.CheckIsCooperative();
                    break;
                default:
                    Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
                    var institutionSelectionWindow = new InsSelectWindow(search);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            var commonInsSelectionWindow = new CommonHospitalsWindow();
            commonInsSelectionWindow.ShowDialog();
        }
        private void CheckPrescriptionCase()
        {
            if (CurrentPrescription.Treatment.Division is null) return;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.Division.ID))
                CurrentPrescription.Treatment.PrescriptionCase = VM.GetPrescriptionCases(CurrentPrescription.Treatment.Division.Name.Equals("牙科") ? "19" : "09");
        }
        private void PharmacistChangedAction()
        {
            PrescriptionCount = UpdatePrescriptionCount();
            if (PrescriptionCount >= 80)
                MessageWindow.ShowMessage(Resources.調劑張數提醒 + prescriptionCount + "張", MessageType.WARNING);
        }
        private void GetMainDiseaseCodeByIdAction(string ID)
        {
            if (string.IsNullOrEmpty(ID) || CurrentPrescription.Treatment.MainDisease is null || (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MainDisease.FullName) && ID.Equals(CurrentPrescription.Treatment.MainDisease.FullName)))
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                return;
            }
            SetDiseaseCode(ID, true);
        }

        private void GetSubDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id) || CurrentPrescription.Treatment.SubDisease is null || (!string.IsNullOrEmpty(CurrentPrescription.Treatment.SubDisease.FullName) && id.Equals(CurrentPrescription.Treatment.SubDisease.FullName)))
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusChronicTotal"));
                return;
            }
            SetDiseaseCode(id, false);
        }
        private void SetDiseaseCode(string ID,bool main)
        {
            var result = DiseaseCode.GetDiseaseCodeByID(ID);
            if (result == null) return;
            if(main)
                CurrentPrescription.Treatment.MainDisease = result;
            else
                CurrentPrescription.Treatment.SubDisease = result;
        }
        private void AdjustCaseSelectionChangedAction()
        {
            if (CurrentPrescription.Treatment.AdjustCase != null && CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
            {
                NotPrescribe = false;
                CurrentPrescription.Treatment.Clear();
                SetMedicinesPaySelf();
            }
            else
                NotPrescribe = true;
            CheckPrescriptionVariable();
            CurrentPrescription.CheckIsCooperative();
        }
        private void CopaymentSelectionChangedAction()
        {
            if (CurrentPrescription.CheckFreeCopayment())
            {
                CurrentPrescription.PrescriptionPoint.CopaymentPoint = CurrentPrescription.CountCopaymentPoint();
                return;
            }
            CurrentPrescription.Treatment.Copayment = VM.GetCopayment(CurrentPrescription.PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
        }
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MedicineVirtual m = new MedicineVirtual();
                switch (medicineID)
                {
                    case "R001":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "處方箋遺失或毀損，提前回診";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        CurrentPrescription.Medicines.Add(m);
                        return;
                    case "R002":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "醫師請假，提前回診";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        CurrentPrescription.Medicines.Add(m);
                        return;
                    case "R003":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        CurrentPrescription.Medicines.Add(m);
                        return;
                    case "R004":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "其他提前回診或慢箋提前領藥";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        CurrentPrescription.Medicines.Add(m);
                        return;
                    default:
                        MessageWindow.ShowMessage(Resources.搜尋字串長度不足 + "5", MessageType.WARNING);
                        return;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            var wareHouse = VM.CooperativeClinicSettings.GetWareHouseByPrescription(CurrentPrescription.Treatment.Institution, CurrentPrescription.Treatment.AdjustCase.ID);
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse is null ? "0" : wareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            if(productCount == 0)
                MessageWindow.ShowMessage(Resources.查無藥品, MessageType.WARNING);
            else
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = wareHouse is null ? new MedSelectWindow(medicineID, AddProductEnum.PrescriptionDeclare,"0") : new MedSelectWindow(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse.ID);
                if (productCount > 1)
                    MedicineWindow.ShowDialog();
            }
        }
        private void DeleteMedicineAction()
        {
            CurrentPrescription.Medicines.Remove(CurrentPrescription.SelectedMedicine);
            CurrentPrescription.CountPrescriptionPoint(true);
        }
        private void CountMedicinePoint()
        {
            CurrentPrescription.CountMedicineDays();
            if (!CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
                CurrentPrescription.CheckIfSimpleFormDeclare();
            CurrentPrescription.CountPrescriptionPoint(true);
        }

        private void SetBuckleAmount()
        {
            if (!string.IsNullOrEmpty(VM.CooperativeInstitutionID) && CurrentPrescription.Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
                CurrentPrescription.SelectedMedicine.BuckleAmount = 0;
            else
            {
                CurrentPrescription.SelectedMedicine.BuckleAmount = CurrentPrescription.SelectedMedicine.Amount;
            }
        }
        private void SelfPayTextChangedAction()
        {
            if (NotPrescribe)
                CurrentPrescription.PrescriptionPoint.AmountsPay = CurrentPrescription.PrescriptionPoint.CopaymentPoint + CurrentPrescription.PrescriptionPoint.AmountSelfPay;
            else
                CurrentPrescription.PrescriptionPoint.AmountsPay = CurrentPrescription.PrescriptionPoint.AmountSelfPay;
        }
        private void ErrorAdjustAction()
        {
            var errorAdjustConfirm = new ConfirmWindow("確認異常結案?(非必要請勿使用此功能，若過卡率低於九成，將被勸導限期改善並列為輔導對象，最重可能勒令停業)", "異常確認");
            Debug.Assert(errorAdjustConfirm.DialogResult != null, "errorAdjustConfirm.DialogResult != null");
            if(!(bool)errorAdjustConfirm.DialogResult)
                return;
            if (!CheckCooperativePrescribeContinue()) return;//檢查合作診所自費並確認是否繼續調劑
            if (CurrentPrescription.PrescriptionStatus.IsPrescribe)
            {
                if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name) || string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) || CurrentPrescription.Patient.Birthday is null)
                {
                    var confirm = new ConfirmWindow("尚未選擇客戶或資料不全，是否以匿名取代?", "");
                    Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                    if ((bool)confirm.DialogResult)
                        CurrentPrescription.Patient = Customer.GetCustomerByCusId(0);
                    else
                        return;
                }
            }
            if (CheckEmptyCustomer()) return;
            if (!CheckCustomer()) return;
            SetPharmacist();
            if (!CheckPrescriptionCount()) return;
            IsAdjusting = true;
            if (!CheckMissingCooperativeContinue()) return;//檢查是否為合作診所漏傳手動輸入之處方
            if (!CheckSameOrIDEmptyMedicine())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.CheckMedicalNumber())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.PrescriptionStatus.IsPrescribe)//合作診所自費不檢查健保規則
            {
                if(!CurrentPrescription.Treatment.CheckAdjustDate())
                {
                    IsAdjusting = false;
                    return;
                }
                var error = CurrentPrescription.CheckPrescriptionRule(false);//檢查健保規則
                if (!string.IsNullOrEmpty(error))
                {
                    MessageWindow.ShowMessage(error, MessageType.ERROR);
                    IsAdjusting = false;
                    return;
                }
            }
            else
            {
                MessageWindow.ShowMessage("此為全自費處方不須異常結案，請按調劑按紐完成調劑", MessageType.ERROR);
                IsAdjusting = false;
                return;
            }
            if (!PrintConfirm()) return;
            SavePatientData();
            InsertAdjustData(false,true);
        }
        private void NoCardAdjustAction()
        {
            var noCard = new ConfirmWindow(Resources.欠卡確認, Resources.欠卡調劑, true);
            Debug.Assert(noCard.DialogResult != null, "noCard.DialogResult != null");
            if (!(bool)noCard.DialogResult) return;
            if (CheckEmptyCustomer()) return;
            if (!CheckCustomer()) return;
            CurrentPrescription.Treatment.Pharmacist = SelectedPharmacist;
            if (!CheckPrescriptionCount()) return;
            CurrentPrescription.CheckIsPrescribe();
            if (CurrentPrescription.PrescriptionStatus.IsPrescribe)
            {
                MessageWindow.ShowMessage("此處方藥品皆為自費，不須押金欠卡，請直接按下調劑即可。", MessageType.WARNING);
                IsAdjusting = false;
                return;
            }
            if (!CheckMissingCooperativeContinue()) return;
            IsAdjusting = true;
            if (!CheckSameOrIDEmptyMedicine())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.Treatment.CheckAdjustDate())
            {
                IsAdjusting = false;
                return;
            }
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
                IsAdjusting = false;
                return;
            }
            CurrentPrescription.PrescriptionPoint.CountDeposit();
            if(!PrintConfirm()) return;
            SavePatientData();
            StartNoCardAdjust();
        }

        private void AdjustButtonClickAction()
        {
            if (!CheckCooperativePrescribeContinue()) return;//檢查合作診所自費並確認是否繼續調劑
            if (CurrentPrescription.PrescriptionStatus.IsPrescribe)
            {
                if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name) || string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) || CurrentPrescription.Patient.Birthday is null)
                {
                    var confirm = new ConfirmWindow("尚未選擇客戶或資料不全，是否以匿名取代?", "");
                    Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                    if ((bool)confirm.DialogResult)
                        CurrentPrescription.Patient = Customer.GetCustomerByCusId(0);
                    else
                        return;
                }
            }
            if (CheckEmptyCustomer()) return;
            if(!CheckCustomer()) return;
            SetPharmacist();
            if(!CheckPrescriptionCount()) return;
            IsAdjusting = true;
            if(!CheckMissingCooperativeContinue()) return;//檢查是否為合作診所漏傳手動輸入之處方
            if (!CheckSameOrIDEmptyMedicine())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.CheckMedicalNumber())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.PrescriptionStatus.IsPrescribe)//合作診所自費不檢查健保規則
            {
                if (!CurrentPrescription.Treatment.CheckAdjustDate())
                {
                    IsAdjusting = false;
                    return;
                }
                var error = CurrentPrescription.CheckPrescriptionRule(false);//檢查健保規則
                if (!string.IsNullOrEmpty(error))
                {
                    MessageWindow.ShowMessage(error, MessageType.ERROR);
                    IsAdjusting = false;
                    return;
                }
            }
            else
            {
                if(!CurrentPrescription.Treatment.CheckAdjustDate())
                {
                    IsAdjusting = false;
                    return;
                }
                var error = CurrentPrescription.CheckPrescribeRule();
                if (!string.IsNullOrEmpty(error))
                {
                    MessageWindow.ShowMessage(error, MessageType.ERROR);
                    IsAdjusting = false;
                    return;
                }
            }
            if (!PrintConfirm()) return;
            SavePatientData();
            if (CurrentPrescription.PrescriptionStatus.IsPrescribe)
                StartCooperativePrescribe();
            else
                StartNormalAdjust();
        }

        private bool CheckPrescriptionCount()
        {
            if (PrescriptionCount >= 80)
            {
                var confirm = new ConfirmWindow(Resources.調劑張數提醒 + prescriptionCount + "張，是否繼續調劑?","調劑張數提醒",true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                return (bool)confirm.DialogResult;
            }
            return true;
        }

        private void SetPharmacist()
        {
            if (SelectedPharmacist is null)
            {
                SelectedPharmacist = MedicalPersonnels.SingleOrDefault(m => m.ID.Equals(VM.CurrentUser.ID)) ?? MedicalPersonnels[0];
            }
            CurrentPrescription.Treatment.Pharmacist = SelectedPharmacist;
        }

        private bool CheckCooperativePrescribeContinue()
        {
            CurrentPrescription.CheckIsPrescribe();//檢查是否為全自費處方
            if (!CurrentPrescription.PrescriptionStatus.IsPrescribe) return true;
            var confirm = new ConfirmWindow("此處方藥品皆為自費，處方不申報，是否將案件轉為自費調劑?", "自費確認");
            Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
            var result = (bool) confirm.DialogResult;
            if (!result)
                IsAdjusting = false;
            return result;
        }

        private bool CheckMissingCooperativeContinue()
        {
            if (CurrentPrescription.Source == PrescriptionSource.Cooperative && CurrentPrescription.Treatment.AdjustCase.ID != "2" && string.IsNullOrEmpty(CurrentPrescription.Remark))
            {
                var e = new CooperativeRemarkInsertWindow();
                CurrentPrescription.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                if (string.IsNullOrEmpty(CurrentPrescription.Remark) || CurrentPrescription.Remark.Length != 16)
                {
                    IsAdjusting = false;
                    return false;
                }
                if(!CurrentPrescription.PrescriptionStatus.IsPrescribe)
                    CheckIsCooperativeVIP();
                return true;
            }
            return true;
        }

        private void RegisterButtonClickAction()
        {
            if (CheckEmptyCustomer()) return;
            if (!CheckCustomer()) return;
            SetPharmacist();
            if (!CurrentPrescription.Treatment.CheckAdjustDate())
            {
                IsAdjusting = false;
                return;
            }
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
                return;
            }
            if (!CheckSameOrIDEmptyMedicine())
            {
                IsAdjusting = false;
                return;
            }
            if (!CurrentPrescription.CheckMedicalNumber())
            {
                IsAdjusting = false;
                return;
            }
            if (!PrintConfirm()) return;
            SavePatientData();
            StartRegister();
        }

        private void PrescribeButtonClickAction()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name) || string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) || CurrentPrescription.Patient.Birthday is null)
            {
                var confirm = new ConfirmWindow("尚未選擇客戶，是否以匿名取代?", "");
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool)confirm.DialogResult)
                    CurrentPrescription.Patient = Customer.GetCustomerByCusId(0);
                else
                    return;
            }
            if (!CheckCustomer()) return;
            SetPharmacist();
            if (CurrentPrescription.Medicines.Count == 0)
            {
                MessageWindow.ShowMessage("未填寫藥品",MessageType.WARNING);
                return;
            }
            var medicinesAmountZero = CurrentPrescription.CheckMedicines();
            if (!string.IsNullOrEmpty(medicinesAmountZero))
            {
                MessageWindow.ShowMessage(medicinesAmountZero, MessageType.WARNING);
                return;
            }
            if (!CheckSameOrIDEmptyMedicine())
            {
                IsAdjusting = false;
                return;
            }
            if (!PrintConfirm()) return;
            SavePatientData();
            InsertPrescribeData();
        }

        #endregion
        #region MessengerReceiveActions
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            if (receiveSelectedCustomer is null)
                return;
            CurrentPrescription.Patient = receiveSelectedCustomer;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.UpdateEditTime();
            CurrentPrescription.Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
            CheckCustomPrescriptions();
        }
        private void GetSelectedPrescription(CustomPrescriptionStruct pre)
        {
            try
            {
                Messenger.Default.Unregister<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
                Messenger.Default.Unregister<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
                var p = new Prescription();
                MainWindow.ServerConnection.OpenConnection();
                switch (pre.Source)
                {
                    case PrescriptionSource.ChronicReserve:
                        p = new Prescription(PrescriptionDb.GetReservePrescriptionByID((int)pre.ID).Rows[0], PrescriptionSource.ChronicReserve);
                        break;
                    case PrescriptionSource.Normal:
                        p = new Prescription(PrescriptionDb.GetPrescriptionByID((int)pre.ID).Rows[0], PrescriptionSource.Normal);
                        break;
                }
                MainWindow.ServerConnection.CloseConnection();
                p.Card = CurrentPrescription.Card;
                p.Patient = CurrentPrescription.Patient;
                CurrentPrescription.Patient.Check();
                CurrentPrescription = p;
                CurrentPrescription.GetCompletePrescriptionData(false, false);
                CurrentPrescription.CountPrescriptionPoint(true);
                CanAdjust = true;
                switch (pre.Source)
                {
                    case PrescriptionSource.ChronicReserve:
                        CurrentPrescription.CheckIsCooperative();
                        break;
                    case PrescriptionSource.Normal:
                        CurrentPrescription.CheckIsBuckleAndSource();
                        break;
                }
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage(Resources.代入處方錯誤, MessageType.WARNING);
            }
        }
        private void GetCooperativePrescription(NotificationMessage<Prescription> msg)
        {
            try
            {
                Messenger.Default.Unregister<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
                Messenger.Default.Unregister<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
                msg.Content.GetCompletePrescriptionData(true, false);
                MainWindow.ServerConnection.OpenConnection();
                msg.Content.Card = CurrentPrescription.Card;
                if (msg.Sender is CooperativeSelectionViewModel)
                {
                    var customers = msg.Content.Patient.Check();
                    switch (customers.Count)
                    {
                        case 0:
                            AskAddCustomerData();
                            break;
                        case 1:
                            CurrentPrescription.Patient = customers[0];
                            MainWindow.ServerConnection.OpenConnection();
                            CurrentPrescription.Patient.UpdateEditTime();
                            CurrentPrescription.Patient.GetHistories();
                            MainWindow.ServerConnection.CloseConnection();
                            break;
                    }
                }
                else
                {
                    msg.Content.Patient = CurrentPrescription.Patient;
                }
                MainWindow.ServerConnection.CloseConnection();
                CurrentPrescription = msg.Content;
                CurrentPrescription.CountPrescriptionPoint(true);
                CurrentPrescription.PrescriptionStatus.IsCooperative = true;
                CanAdjust = true;
                if (CurrentPrescription.PrescriptionStatus.IsCooperativeVIP)
                    MessageWindow.ShowMessage("病患為合作診所VIP，請藥師免收部分負擔。", MessageType.WARNING);
                CurrentPrescription.CheckIsCooperative();
            }
            catch (Exception)
            {
                MessageWindow.ShowMessage("代入處方發生問題，為確保處方資料完整請重新取得病患資料並代入處方。", MessageType.WARNING);
            }
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this,"GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Treatment.Institution = receiveSelectedInstitution;
            CurrentPrescription.CheckIsCooperative();
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionDeclareViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.AddMedicineBySearch(msg.Content.ID);
            MainWindow.ServerConnection.CloseConnection();
            CurrentPrescription.CountPrescriptionPoint(true);
        }
        #endregion
        #region GeneralFunctions
        private void AskAddCustomerData()
        {
            if (!CurrentPrescription.Patient.CheckData())
                MessageWindow.ShowMessage(Resources.顧客資料不足, MessageType.WARNING);
            else
            {
                var confirm = new ConfirmWindow(Resources.新增顧客確認, Resources.查無資料, true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool) confirm.DialogResult)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    if (CurrentPrescription.Patient.CheckIDNumberExist())
                    {
                        MessageWindow.ShowMessage("此身分證已存在，請確認顧客資料", MessageType.WARNING);
                        MainWindow.ServerConnection.CloseConnection();
                    }
                    else
                    {
                        CurrentPrescription.Patient.InsertData();
                        CurrentPrescription.Patient.GetHistories();
                        MainWindow.ServerConnection.CloseConnection();
                    }
                }
            }
        }
        private void SearchCustomer(int condition,Customers customers)
        {
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            customerSelectionWindow = new CusSelectWindow(condition, customers);
        }
        private void ReadCard(bool showCusWindow)
        {
            CanAdjust = false;
            var isGetCard = false;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    BusyContent = Resources.讀取健保卡;
                    MainWindow.ServerConnection.OpenConnection();
                    isGetCard = CurrentPrescription.GetCard();
                    MainWindow.ServerConnection.CloseConnection();
                }
                catch (Exception e)
                {
                    NewFunction.ExceptionLog(e.Message);
                    Application.Current.Dispatcher.Invoke(() => MessageWindow.ShowMessage("讀卡作業異常，請重開處方登錄頁面並重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING));
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsCardReading = false;
                if (showCusWindow)
                {
                    IsBusy = false;
                    CanAdjust = true;
                    if (isGetCard)
                    {
                        MainWindow.ServerConnection.OpenConnection();
                        CurrentPrescription.Patient.GetHistories();
                        MainWindow.ServerConnection.CloseConnection();
                        SelectedRadioButton = "Option1";
                        CheckCustomPrescriptions();
                    }
                    else
                    {
                        MainWindow.ServerConnection.OpenConnection();
                        var customers = new Customers();
                        customers.Init();
                        MainWindow.ServerConnection.CloseConnection();
                        SearchCustomer(1, customers);
                    }
                }
                else
                {
                    if (isGetCard)
                        GetMedicalNumber();
                    else
                        WriteICCardData();
                }
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void CheckPrescriptionVariable()
        {
            CurrentPrescription.CheckPrescriptionVariable();
            if (CurrentPrescription.Treatment.AdjustCase.ID.Equals("2"))
                CurrentPrescription.Treatment.PaymentCategory = PaymentCategories[0];
            CheckDeclareStatus();
        }
        private void CheckDeclareStatus()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID)) return;
            //自費調劑
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID) && CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
            {
                DeclareStatus = PrescriptionDeclareStatus.Prescribe;
                return;
            }
            var adjust = CurrentPrescription.Treatment.AdjustDate;
            if (adjust is null)
            {
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
                return;
            }
            //調劑日為今天
            if (DateTime.Today.Date == ((DateTime)adjust).Date)
            {
                //填寫領藥次數且調劑案件為慢箋 => 登錄，其餘為調劑
                if (CurrentPrescription.Treatment.ChronicSeq != null && CurrentPrescription.Treatment.ChronicSeq > 0 && CurrentPrescription.Treatment.AdjustCase.ID.Equals("2") && CurrentPrescription.PrescriptionStatus.IsSendOrder)
                    DeclareStatus = PrescriptionDeclareStatus.Register;
                else
                    DeclareStatus = PrescriptionDeclareStatus.Adjust;
                return;
            }
            //調劑日為未來 => 登錄
            if (DateTime.Today.Date < ((DateTime) adjust).Date)
            {
                DeclareStatus = PrescriptionDeclareStatus.Register;
                return;
            }
            //調劑日為過去 => 調劑
            if (DateTime.Today.Date > ((DateTime)adjust).Date)
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
        }
        private void ClearPrescription()
        {
            InitializeVariables(false);
        }
        private bool PrintConfirm()
        {
            printResult = NewFunction.CheckPrint(CurrentPrescription);
            var printMedBag = printResult[0];
            var printSingle = printResult[1];
            var printReceipt = printResult[2];
            if (printMedBag is null || printReceipt is null)
            {
                IsAdjusting = false;
                return false;
            }
            if ((bool)printMedBag && printSingle is null)
            {
                IsAdjusting = false;
                return false;
            }
            TempPre = new Prescription();
            TempPre = (Prescription)CurrentPrescription.Clone();
            TempPre.AdjustMedicinesType();
            return true;
        }


        private void StartCooperativePrescribe()
        {
            InsertAdjustData(false,false);
        }
        private void StartNormalAdjust()
        {
            if (ErrorCode is null)
            {
                if (string.IsNullOrEmpty(CurrentPrescription.Card.PatientBasicData.CardNumber))
                    ReadCard(false);
                else
                    GetMedicalNumber();
            }
            else
                WriteICCardData();
        }
        private void StartRegister()
        {
            MainWindow.ServerConnection.OpenConnection();
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                case PrescriptionSource.ChronicReserve:
                case PrescriptionSource.XmlOfPrescription:
                case PrescriptionSource.Cooperative:
                    if (!InsertRegisterData())
                        return;
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }
        private bool InsertRegisterData() {
            MedSendWindow medicinesSendSingdeWindow = null;
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder)
            {
                medicinesSendSingdeWindow = new MedSendWindow(CurrentPrescription);
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                    return false;
            }
            CurrentPrescription.PrescriptionStatus.SetRegisterStatus();
            if(CurrentPrescription.Source == PrescriptionSource.Normal)
                CurrentPrescription.NormalRegister();
            else
                CurrentPrescription.ChronicRegister();
             
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder && ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn == false
                       && !CurrentPrescription.PrescriptionStatus.IsSendToSingde)
                CurrentPrescription.PrescriptionStatus.IsSendToSingde = PurchaseOrder.InsertPrescriptionOrder(CurrentPrescription, ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).PrescriptionSendData);
            //紀錄訂單and送單

            else if (CurrentPrescription.PrescriptionStatus.IsSendOrder && ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn == false
                && CurrentPrescription.PrescriptionStatus.IsSendToSingde) {
                PurchaseOrder.UpdatePrescriptionOrder(CurrentPrescription, ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).PrescriptionSendData);
            } //更新傳送藥健康
            CurrentPrescription.PrescriptionStatus.UpdateStatus(CurrentPrescription.Id);
            StartPrint(false);
            return true;
        }
        private void InsertPrescribeData()
        {
            CurrentPrescription.Treatment.Pharmacist = SelectedPharmacist;
            CurrentPrescription.PrescriptionStatus.SetPrescribeStatus();
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Prescribe();//自費調劑金流
            MainWindow.ServerConnection.CloseConnection();
            StartPrint(false);
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }

        private void CreateDailyUploadData(ErrorUploadWindowViewModel.IcErrorCode error = null)
        {
            if (CurrentPrescription.PrescriptionStatus.IsGetCard || error != null)
            {
                if (CurrentPrescription.Card.IsGetMedicalNumber)
                    CreatePrescriptionSign();
                else
                    CurrentPrescription.PrescriptionStatus.IsCreateSign = false;
            }
            else
                CurrentPrescription.PrescriptionStatus.IsDeclare = false;
        }

        private void CreatePrescriptionSign()
        {
            CurrentPrescription.PrescriptionSign = HisAPI.WritePrescriptionData(CurrentPrescription);
            BusyContent = Resources.產生每日上傳資料;
            if (CurrentPrescription.WriteCardSuccess != 0)
            {
                Application.Current.Dispatcher.Invoke(delegate {
                    var description = MainWindow.GetEnumDescription((ErrorCode)CurrentPrescription.WriteCardSuccess);
                    MessageWindow.ShowMessage("寫卡異常 " + CurrentPrescription.WriteCardSuccess + ":" + description, MessageType.WARNING);
                });
                CurrentPrescription.PrescriptionStatus.IsCreateSign = null;
            }
            else
            {
                CurrentPrescription.PrescriptionStatus.IsCreateSign = true;
            }
        }
        
        private void CheckCustomPrescriptions()
        {
            if(customPresChecked) return;
            Messenger.Default.Register<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            Messenger.Default.Register<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            var cusPreSelectWindow = new CusPreSelectWindow(CurrentPrescription.Patient.ID, CurrentPrescription.Patient.IDNumber, CurrentPrescription.Card);
        }

        private void GetMedicalNumber()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                CurrentPrescription.PrescriptionStatus.IsGetCard = true;
                BusyContent = Resources.檢查就醫次數;
                CurrentPrescription.Card.GetRegisterBasic();
                if ((CurrentPrescription.Card.AvailableTimes != null && CurrentPrescription.Card.AvailableTimes == 0) || DateTime.Compare(CurrentPrescription.Card.ValidityPeriod,DateTime.Today) < 0)
                {
                    BusyContent = Resources.更新卡片;
                    CurrentPrescription.Card.UpdateCard();
                }
                BusyContent = Resources.取得就醫序號;
                CurrentPrescription.Card.GetMedicalNumber(1);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                WriteICCardData();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void WriteICCardData()
        {
            if (!CurrentPrescription.Card.IsGetMedicalNumber && ErrorCode is null)
            {
                IsBusy = false;
                if (!AskErrorUpload())
                    return;
            }
            if (ErrorCode != null && !CurrentPrescription.PrescriptionStatus.IsGetCard)
                CurrentPrescription.PrescriptionStatus.IsGetCard = true;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = Resources.寫卡;
                CreateDailyUploadData(ErrorCode);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if (CurrentPrescription.PrescriptionStatus.IsCreateSign is null)
                {
                    MessageWindow.ShowMessage("寫卡異常，請重新讀取卡片或選擇異常代碼。",MessageType.ERROR);
                    IsAdjusting = false;
                    IsCardReading = false;
                    return;
                }
                InsertAdjustData(true, false);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private bool AskErrorUpload()
        {
            var e = new ErrorUploadWindow(CurrentPrescription.Card.IsGetMedicalNumber); //詢問異常上傳
            e.ShowDialog();
            if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage(Resources.尚未選擇異常代碼, MessageType.WARNING);
                IsAdjusting = false;
                IsCardReading = false;
                return false;
            }
            ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            return true;
        }

        private void InsertAdjustData(bool normal,bool errorAdjust)
        {
            CurrentPrescription.Treatment.Pharmacist = SelectedPharmacist;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.SetAdjustStatus(errorAdjust);//設定處方狀態
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    CurrentPrescription.NormalAdjust(false);
                    break;
                case PrescriptionSource.Cooperative:
                    CurrentPrescription.CooperativeAdjust(false);
                    break;
                case PrescriptionSource.ChronicReserve:
                    if (!CurrentPrescription.PrescriptionStatus.IsBuckle)
                        CurrentPrescription.Medicines.SetNoBuckle();
                    CurrentPrescription.ChronicAdjust(false);
                    break;
                case PrescriptionSource.XmlOfPrescription:
                    if(!CurrentPrescription.PrescriptionStatus.IsBuckle)
                        CurrentPrescription.Medicines.SetNoBuckle();
                    CurrentPrescription.XmlOfPrescriptionAdjust(false);
                    break;
            }
            if (normal)
                CheckDailyUpload();
            MainWindow.ServerConnection.CloseConnection();
            StartPrint(false);
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }

        private void StartPrint(bool noCard)
        {
            var printWorker = new BackgroundWorker();
            var printMedBag = printResult[0];
            var printSingle = printResult[1];
            var printReceipt = printResult[2];
            printWorker.DoWork += (o, ea) =>
            {
                Print(noCard, (bool)printMedBag, printSingle, (bool)printReceipt);
            };
            printWorker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            printWorker.RunWorkerAsync();
        }

        private void CheckDailyUpload()
        {
            if (CurrentPrescription.PrescriptionStatus.IsPrescribe) return;
            if (CurrentPrescription.Card.IsGetMedicalNumber)
            {
                if (CurrentPrescription.PrescriptionStatus.IsCreateSign != null && (bool)CurrentPrescription.PrescriptionStatus.IsCreateSign)
                    HisAPI.CreatDailyUploadData(CurrentPrescription, false);
            }
            else if (CurrentPrescription.PrescriptionStatus.IsCreateSign != null && !(bool)CurrentPrescription.PrescriptionStatus.IsCreateSign)
                HisAPI.CreatErrorDailyUploadData(CurrentPrescription, false, ErrorCode);
        }

        private void CheckIsCooperativeVIP()
        {
            var isVip = new ConfirmWindow(Resources.收部分負擔, Resources.免收確認);
            Debug.Assert(isVip.DialogResult != null, "isVip.DialogResult != null");
            CurrentPrescription.PrescriptionStatus.IsCooperativeVIP = (bool)!isVip.DialogResult;
        }

        private void Print(bool noCard,bool printMedBag,bool? printSingle, bool printReceipt)
        {
            if (printMedBag)
            {
                BusyContent = Resources.藥袋列印;
                Debug.Assert(printSingle != null, nameof(printSingle) + " != null");
                TempPre.PrintMedBag((bool)printSingle);
            }
            if (printReceipt)
            {
                BusyContent = Resources.收據列印;
                TempPre.PrintReceipt();
            }
            if (noCard)
            {
                BusyContent = Resources.押金單據列印;
                TempPre.PrintDepositSheet();
            }
        }
        private void StartNoCardAdjust()
        {
            CurrentPrescription.Treatment.Pharmacist = SelectedPharmacist;
            CurrentPrescription.PrescriptionStatus.SetNoCardSatus();
            MainWindow.ServerConnection.OpenConnection();
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    CurrentPrescription.NormalAdjust(true);
                    break;
                case PrescriptionSource.Cooperative:
                    CurrentPrescription.Medicines.SetNoBuckle();
                    CurrentPrescription.CooperativeAdjust(true);
                    break;
                case PrescriptionSource.ChronicReserve:
                    if (!CurrentPrescription.PrescriptionStatus.IsBuckle)
                        CurrentPrescription.Medicines.SetNoBuckle();
                    CurrentPrescription.ChronicAdjust(true);
                    break;
                case PrescriptionSource.XmlOfPrescription:
                    if (!CurrentPrescription.PrescriptionStatus.IsBuckle)
                        CurrentPrescription.Medicines.SetNoBuckle();
                    CurrentPrescription.XmlOfPrescriptionAdjust(true);
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
            StartPrint(true);
            MessageWindow.ShowMessage(Resources.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }
        private int UpdatePrescriptionCount()//計算處方張數
        { 
           return SelectedPharmacist != null 
                ? PrescriptionDb.GetPrescriptionCountByID(SelectedPharmacist.IDNumber).Rows[0].Field<int>("PrescriptionCount")
                : 0; 
        }

        private bool CheckCustomer()
        {
            if (!CurrentPrescription.Patient.CheckData())
            {
                MessageWindow.ShowMessage(Resources.顧客資料不足, MessageType.WARNING);
                return false;
            }
            if (CurrentPrescription.Patient.ID == -1 || (CurrentPrescription.Patient.ID == 0 && !CurrentPrescription.Patient.Name.Equals("匿名")))//新顧客
            {
                var customers = CurrentPrescription.Patient.Check();
                switch (customers.Count)
                {
                    case 0:
                        MainWindow.ServerConnection.OpenConnection();
                        if (CurrentPrescription.Patient.CheckIDNumberExist())
                        {
                            MessageWindow.ShowMessage("此身分證已存在，請確認顧客資料", MessageType.WARNING);
                            MainWindow.ServerConnection.CloseConnection();
                        }
                        else
                        {
                            CurrentPrescription.Patient.InsertData();
                            MainWindow.ServerConnection.CloseConnection();
                        }
                        return true;
                    case 1:
                        CurrentPrescription.Patient = customers[0];
                        return true;
                    default:
                        MessageWindow.ShowMessage("顧客資料查詢結果超過一人符合，請確認顧客資料", MessageType.WARNING);
                        return false;
                }
            }
            return true;
        }
        private void SavePatientData()
        {
            if (CurrentPrescription.Patient.ID == 0 || CurrentPrescription.Patient.ID == -1) return;
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Patient.Save();
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckEmptyCustomer()
        {
            if (!string.IsNullOrEmpty(CurrentPrescription.Patient.Name) || !string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) || !(CurrentPrescription.Patient.Birthday is null)) return false;
            MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
            return true;
        }
        private bool CheckSameOrIDEmptyMedicine()
        {
            var medicinesSame = CurrentPrescription.CheckMedicines();
            if (string.IsNullOrEmpty(medicinesSame)) return true;
            MessageWindow.ShowMessage(medicinesSame, MessageType.WARNING);
            return false;
        }
        #endregion
        #region CommandExecuteChecking
        private bool CheckIsCardReading()
        {
            return !IsCardReading;
        }
        private bool CheckIsAdjusting()
        {
            return !IsAdjusting;
        }
        private bool CheckIsNoCard()
        {
            return !CurrentPrescription.PrescriptionStatus.IsGetCard && !IsAdjusting;
        }
        #endregion
    }
}
