﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.Interface;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
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
using StringRes = His_Pos.Properties.Resources;
using HisAPI = His_Pos.HisApi.HisApiFunction;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;

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
        #region ItemsSources
        public Institutions Institutions { get; set; }
        public Divisions Divisions { get; set; }
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
        #endregion
        private PrescriptionDeclareStatus declareStatus;
        public PrescriptionDeclareStatus DeclareStatus
        {
            get => declareStatus;
            set
            {
                Set(() => DeclareStatus, ref declareStatus, value);
                if(CurrentPrescription is null) return;
                if ((DateTime)CurrentPrescription.Treatment.AdjustDate!=null && CurrentPrescription.Treatment.AdjustCase.ID.Equals("2") 
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
        private Medicine selectedMedicine;
        public Medicine SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (SelectedMedicine is MedicineNHI || SelectedMedicine is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial)
                    ((IDeletableProduct) SelectedMedicine).IsSelected = false;
                Set(() => SelectedMedicine, ref selectedMedicine, value);
                if (SelectedMedicine is MedicineNHI || SelectedMedicine is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial)
                    ((IDeletableProduct)SelectedMedicine).IsSelected = true;
            }
        }

        public int priviousSelectedIndex { get; set; }
        private int selectedMedicinesIndex;
        public int SelectedMedicinesIndex
        {
            get => selectedMedicinesIndex;
            set
            {
                if (value != -1)
                {
                    Set(() => SelectedMedicinesIndex, ref selectedMedicinesIndex, value);
                }
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
        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode { get; set; }
        #endregion
        #region Commands
        public RelayCommand ShowCooperativeSelectionWindow { get; set; }
        public RelayCommand GetPatientData { get; set; }
        // ReSharper disable once InconsistentNaming
        public RelayCommand SearchCustomerByIDNumber { get; set; }
        public RelayCommand SearchCustomerByName { get; set; }
        public RelayCommand SearchCustomerByBirthday { get; set; }
        public RelayCommand SearchCustomerByTel { get; set; }
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
        public RelayCommand ResetCardReader { get; set; }
        public RelayCommand NoCardAdjust { get; set; }
        public RelayCommand MedicineNoBuckleClick { get; set; }
        public RelayCommand SendOrderCommand { get; set; }
        public RelayCommand ErrorCodeSelect { get; set; }
        public RelayCommand DivisionSelectionChanged { get; set; }
        public RelayCommand SelfPayTextChanged { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            Initial();
        }
        ~PrescriptionDeclareViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        #region InitialFunctions
        private void Initial()
        {
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
            InitializeVariables();
        }
        private void InitializeVariables()
        {
            SelectedMedicinesIndex = 0;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            NotPrescribe = true;
            CanAdjust = true;
            IsReadCard = false;
            ErrorCode = null;
            CanSendOrder = false;
            IsAdjusting = false;
            InitialPrescription();
        }
        private void InitialItemsSources()
        {
            Institutions = VM.Institutions;
            Divisions = VM.Divisions;
            MedicalPersonnels = VM.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = VM.AdjustCases;
            PaymentCategories = VM.PaymentCategories;
            PrescriptionCases = VM.PrescriptionCases;
            Copayments = VM.Copayments;
            SpecialTreats = VM.SpecialTreats;
        }
        private void InitialCommandActions()
        {
            SearchCustomerByIDNumber = new RelayCommand(SearchCusByIDNumAction);
            SearchCustomerByName = new RelayCommand(SearchCusByNameAction);
            SearchCustomerByBirthday = new RelayCommand(SearchCusByBirthAction);
            SearchCustomerByTel = new RelayCommand(SearchCustomerByTelAction);
            ResetCardReader = new RelayCommand(ResetCardReaderAction);
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
            MedicineNoBuckleClick = new RelayCommand(MedicineNoBuckleAction);
            SelfPayTextChanged = new RelayCommand(SelfPayTextChangedAction);
            SendOrderCommand = new RelayCommand(CheckDeclareStatus);
            ClearButtonClick = new RelayCommand(ClearPrescription, CheckIsAdjusting);
            NoCardAdjust = new RelayCommand(NoCardAdjustAction, CheckIsNoCard);
            AdjustButtonClick = new RelayCommand(AdjustButtonClickAction,CheckIsAdjusting);
            RegisterButtonClick = new RelayCommand(RegisterButtonClickAction);
            PrescribeButtonClick = new RelayCommand(PrescribeButtonClickAction);
        }

        private void InitialPrescription()
        {
            CurrentPrescription = new Prescription();
            CurrentPrescription.InitialCurrentPrescription();
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionCount = UpdatePrescriptionCount();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionDeclareViewModel)+"InsSelected", GetSelectedInstitution);
            Messenger.Default.Register<NotificationMessage>("AdjustDateChanged", AdjustDateChanged);
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
            var medList = CurrentPrescription.Medicines.Where(m => m is MedicineNHI || m is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial).ToList();
            if (medList.Count > 0)
            {
                foreach (var m in medList)
                {
                    if(m.PaySelf) continue;
                    m.PaySelf = true;
                }
            }
        }
        private void AdjustDateChanged(NotificationMessage adjustChange)
        {
            if (!adjustChange.Notification.Equals("AdjustDateChanged")) return;
            CheckDeclareStatus();
        }
        #endregion
        #region Actions
        private void SearchCusByIDNumAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber))
                SearchCustomer();
            else
            {
                if (CurrentPrescription.Patient.IDNumber.Length != 10)
                    MessageWindow.ShowMessage(StringRes.身分證格式錯誤, MessageType.WARNING);
                else
                {
                    if (CurrentPrescription.Patient.Count() == 0)
                        AskAddCustomerData();
                    else
                    {
                        Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
                        customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.IDNumber, 3);
                    }
                }
            }
        }
        private void SearchCusByNameAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                SearchCustomer();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                    AskAddCustomerData();
                else
                {
                    Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
                    customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Name, 2);
                }
            }
        }
        private void SearchCusByBirthAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (CurrentPrescription.Patient.Birthday is null)
                SearchCustomer();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                    AskAddCustomerData();
                else
                {
                    Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
                    customerSelectionWindow = new CusSelectWindow(DateTimeEx.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), 1);
                }
            }
        }
        private void SearchCustomerByTelAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel))
                SearchCustomer();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                    AskAddCustomerData();
                else
                {
                    Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
                    customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Tel, 4);
                }
            }
        }
        private void ResetCardReaderAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.重置讀卡機;
                HisApiBase.csSoftwareReset(3);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
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
                BusyContent = StringRes.取得合作處方;
                cooperative.GetCooperativePrescriptions(VM.CurrentPharmacy.ID, DateTime.Today, DateTime.Today);
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
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "4", MessageType.WARNING);
                return;
            }
            if (CurrentPrescription.Treatment.Institution != null && !string.IsNullOrEmpty(CurrentPrescription.Treatment.Institution.FullName) && search.Equals(CurrentPrescription.Treatment.Institution.FullName))
            {
                Messenger.Default.Send(new NotificationMessage(this,"FocusDivision"));
                return;
            }
            CurrentPrescription.Treatment.Institution = null;
            var result = Institutions.Where(i => i.ID.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    CurrentPrescription.Treatment.Institution = result[0];
                    break;
                default:
                    var institutionSelectionWindow = new InsSelectWindow(search, ViewModelEnum.PrescriptionDeclare);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            var commonInsSelectionWindow = new CommonHospitalsWindow(ViewModelEnum.PrescriptionDeclare);
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
                MessageWindow.ShowMessage(StringRes.調劑張數提醒 + prescriptionCount + "張", MessageType.WARNING);
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
                if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                    CurrentPrescription.Patient = CurrentPrescription.Patient.GetCustomerByCusId(0);
                CurrentPrescription.Treatment.Clear();
                SetMedicinesPaySelf();
            }
            else
                NotPrescribe = true;
            CheckPrescriptionVariable();
        }
        private void CopaymentSelectionChangedAction()
        {
            if (CurrentPrescription.CheckFreeCopayment()) return;
            CurrentPrescription.Treatment.Copayment = VM.GetCopayment(CurrentPrescription.PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
        }
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare);
            MainWindow.ServerConnection.CloseConnection();
            if(productCount == 0)
                MessageWindow.ShowMessage(StringRes.查無藥品, MessageType.WARNING);
            else
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionDeclare);
                if (productCount > 1)
                    MedicineWindow.ShowDialog();
            }
        }
        private void DeleteMedicineAction()
        {
            CurrentPrescription.Medicines.RemoveAt(CurrentPrescription.Medicines.IndexOf(SelectedMedicine));
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
            if (SelectedMedicine is MedicineNHI || SelectedMedicine is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial)
            {
                if (CurrentPrescription.Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
                    SelectedMedicine.BuckleAmount = 0;
                else
                {
                    SelectedMedicine.BuckleAmount = SelectedMedicine.Amount;
                }
            }
            
        }

        private void MedicineNoBuckleAction()
        {
            SelectedMedicine.IsBuckle = !SelectedMedicine.IsBuckle;
        }
        private void SelfPayTextChangedAction()
        {
            if (NotPrescribe)
                CurrentPrescription.PrescriptionPoint.AmountsPay = CurrentPrescription.PrescriptionPoint.CopaymentPoint + CurrentPrescription.PrescriptionPoint.AmountSelfPay;
            else
                CurrentPrescription.PrescriptionPoint.AmountsPay = CurrentPrescription.PrescriptionPoint.AmountSelfPay;
        }
        private void NoCardAdjustAction()
        {
            if (CurrentPrescription.Patient is null || CurrentPrescription.Patient.ID == 0)
            {
                MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
                return;
            }
            if (!CheckMissingCooperativeContinue()) return;
            CurrentPrescription.CheckIsCooperativePrescribe();
            if (CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe)
            {
                MessageWindow.ShowMessage("此合作診所處方藥品皆為自費，不須押金欠卡，請直接按下調劑即可。", MessageType.WARNING);
                IsAdjusting = false;
                return;
            }
            var noCard = new ConfirmWindow(StringRes.欠卡確認, StringRes.欠卡調劑, true);
            if (!(bool)noCard.DialogResult) return;
            IsAdjusting = true;
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
                IsAdjusting = false;
                return;
            }
            CurrentPrescription.PrescriptionPoint.CountDeposit();
            PrintConfirm(PrescriptionDeclareStatus.NoCard);
        }
        private void AdjustButtonClickAction()
        {
            if(CurrentPrescription.Patient is null || CurrentPrescription.Patient.ID == 0)
            {
                MessageWindow.ShowMessage("尚未選擇客戶",MessageType.ERROR);
                return;
            }
            IsAdjusting = true;
            if (!CheckCooperativePrescribeContinue()) return;//檢查合作診所自費並確認是否繼續調劑
            if(!CheckMissingCooperativeContinue()) return;//檢查是否為合作診所漏傳手動輸入之處方
            if (!CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe)//合作診所自費不檢查健保規則
            {
                var error = CurrentPrescription.CheckPrescriptionRule(ErrorCode == null);//檢查健保規則
                if (!string.IsNullOrEmpty(error))
                {
                    MessageWindow.ShowMessage(error, MessageType.ERROR);
                    IsAdjusting = false;
                    return;
                }
            }
            PrintConfirm(PrescriptionDeclareStatus.Adjust);
        }
        private bool CheckCooperativePrescribeContinue()
        {
            CurrentPrescription.CheckIsCooperativePrescribe();//檢查是否為合作診所全自費處方
            if (!CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe) return true;
            var confirm = new ConfirmWindow("此合作診所處方藥品皆為自費，處方不申報，是否將案件轉為自費調劑?", "合作自費確認");
            bool result = (bool) confirm.DialogResult;
            if (!result)
                IsAdjusting = false;
            return result;
        }

        private bool CheckMissingCooperativeContinue()
        {
            if (CurrentPrescription.Source == PrescriptionSource.Normal && CurrentPrescription.PrescriptionStatus.IsCooperative)
            {
                var e = new CooperativeRemarkInsertWindow();
                CurrentPrescription.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                if (string.IsNullOrEmpty(CurrentPrescription.Remark) || CurrentPrescription.Remark.Length != 16)
                {
                    IsAdjusting = false;
                    return false;
                }
                if(!CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe)
                    CheckIsCooperativeVIP();
                return true;
            }
            return true;
        }

        private void RegisterButtonClickAction()
        {
            if (CurrentPrescription.Patient is null || CurrentPrescription.Patient.ID == 0)
            {
                MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
                return;
            }
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
                return;
            }
            PrintConfirm(PrescriptionDeclareStatus.Register);
        }

        private void PrescribeButtonClickAction()
        {
            if (CurrentPrescription.Patient.ID == 0)
            {
                if (!CurrentPrescription.Patient.Name.Equals("匿名") || !CurrentPrescription.Patient.IDNumber.Equals("A111111111") || CurrentPrescription.Patient.Birthday is null || DateTime.Compare(new DateTime(1995, 1, 1), (DateTime)CurrentPrescription.Patient.Birthday) != 0 || !string.IsNullOrEmpty(CurrentPrescription.Patient.Tel))
                {
                    MessageWindow.ShowMessage("匿名資料不得修改，若要填寫顧客請新增客戶或查詢現有顧客", MessageType.ERROR);
                    return;
                }
            }
            PrintConfirm(PrescriptionDeclareStatus.Prescribe);
        }
        #endregion
        #region MessengerReceiveActions
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            if (receiveSelectedCustomer is null)
                return;
            CurrentPrescription.Patient = receiveSelectedCustomer;
            CheckCustomPrescriptions();
        }
        private void GetSelectedPrescription(CustomPrescriptionStruct pre)
        {
            Messenger.Default.Unregister<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            Prescription p = new Prescription();
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
            CurrentPrescription.GetCompletePrescriptionData(true, false, false);
            CurrentPrescription.CountPrescriptionPoint(true);
            priviousSelectedIndex = CurrentPrescription.Medicines.Count - 1;
            CanAdjust = true;
        }
        private void GetCooperativePrescription(NotificationMessage<Prescription> msg)
        {
            Messenger.Default.Unregister<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            msg.Content.GetCompletePrescriptionData(true,true,false);
            MainWindow.ServerConnection.OpenConnection();
            msg.Content.Card = CurrentPrescription.Card;
            if(msg.Sender is CooperativeSelectionViewModel)
                msg.Content.Patient.Check();
            else
            {
                msg.Content.Patient = CurrentPrescription.Patient;
                CurrentPrescription.Patient.Check();
            }
            MainWindow.ServerConnection.CloseConnection();
            CurrentPrescription = msg.Content;
            CurrentPrescription.CountPrescriptionPoint(true);
            CurrentPrescription.PrescriptionStatus.IsCooperative = true;
            priviousSelectedIndex = CurrentPrescription.Medicines.Count - 1;
            CanAdjust = true;
            if (CurrentPrescription.PrescriptionStatus.IsCooperativeVIP)
                MessageWindow.ShowMessage("病患為合作診所VIP，請藥師免收部分負擔。",MessageType.WARNING);
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            CurrentPrescription.Treatment.Institution = receiveSelectedInstitution;
            CurrentPrescription.CheckIsCooperative();
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionDeclareViewModel)) return;
            var selected = CurrentPrescription.Medicines.IndexOf(SelectedMedicine);
            if (selected < 0 || selected >= CurrentPrescription.Medicines.Count) return;
            CurrentPrescription.AddMedicineBySearch(msg.Content.ID, selected);
            CurrentPrescription.CountPrescriptionPoint(true);
            if (selected == CurrentPrescription.Medicines.Count - 1)
                CurrentPrescription.Medicines.Add(new Medicine());
            Messenger.Default.Send(new NotificationMessage<int>(this, selected, "FocusDosage"));
        }
        #endregion
        #region GeneralFunctions
        private void AskAddCustomerData()
        {
            var poorData = string.IsNullOrEmpty(CurrentPrescription.Patient.Name) || string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber);
            if (poorData)
                MessageWindow.ShowMessage(StringRes.顧客資料不足, MessageType.WARNING);
            else
            {
                var confirm = new ConfirmWindow(StringRes.新增顧客確認, StringRes.查無資料, true);
                if ((bool)confirm.DialogResult)
                    CurrentPrescription.Patient.Check();
            }
        }
        private void SearchCustomer()
        {
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            customerSelectionWindow = new CusSelectWindow();
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
                    BusyContent = StringRes.讀取健保卡;
                    isGetCard = CurrentPrescription.GetCard();
                }
                catch (Exception e)
                {
                    NewFunction.ExceptionLog(e.Message);
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MessageWindow.ShowMessage("讀卡作業異常，請重開處方登錄頁面並重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING);
                    }));
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
                        CurrentPrescription.Treatment.GetLastMedicalNumber();
                        CheckCustomPrescriptions();
                    }
                    else
                    {
                        CusSelectWindow customerSelectionWindow = null;
                        SearchCustomer();
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
            CheckDeclareStatus();
        }
        private void CheckDeclareStatus()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID)) return;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID) && CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
                DeclareStatus = PrescriptionDeclareStatus.Prescribe;
            else
            {
                var adjust = CurrentPrescription.Treatment.AdjustDate;
                if (adjust is null)
                    DeclareStatus = PrescriptionDeclareStatus.Adjust;
                else
                {
                    if (DateTime.Today.Date == ((DateTime)adjust).Date)
                    {
                        if (CurrentPrescription.Treatment.ChronicSeq != null && CurrentPrescription.Treatment.ChronicSeq > 0 && CurrentPrescription.Treatment.AdjustCase.ID.Equals("2") && CurrentPrescription.PrescriptionStatus.IsSendOrder)
                            DeclareStatus = PrescriptionDeclareStatus.Register;
                        else
                            DeclareStatus = PrescriptionDeclareStatus.Adjust;
                    }
                    else if (DateTime.Today.Date < ((DateTime)adjust).Date)
                        DeclareStatus = PrescriptionDeclareStatus.Register;
                    else if (DateTime.Today.Date > ((DateTime)adjust).Date)
                        DeclareStatus = PrescriptionDeclareStatus.Adjust;
                }
            }
        }
        private void ClearPrescription()
        {
            InitializeVariables();
            InitialPrescription();
        }
        private void PrintConfirm(PrescriptionDeclareStatus status)
        {
            var noCard = status == PrescriptionDeclareStatus.NoCard;
            var printResult = NewFunction.CheckPrint(CurrentPrescription);
            var printMedBag = printResult[0];
            var printSingle = printResult[1];
            var printReceipt = printResult[2];
            if (printMedBag is null || printReceipt is null)
            {
                IsAdjusting = false;
                return;
            }
            if ((bool)printMedBag && printSingle is null)
            {
                IsAdjusting = false;
                return;
            }
            var printWorker = new BackgroundWorker();
            printWorker.DoWork += (o, ea) =>
            {
                if (CurrentPrescription.Patient.ID != 0)
                {
                    BusyContent = StringRes.更新病患資料;
                    CurrentPrescription.Patient.Save();
                }
                PrintMedBag(noCard, (bool)printMedBag, printSingle, (bool)printReceipt);
            };
            printWorker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                switch (status)
                {
                    case PrescriptionDeclareStatus.Adjust:
                        if(CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe)
                            StartCooperativePrescribe();
                        else
                            StartNormalAdjust();
                        break;
                    case PrescriptionDeclareStatus.Register:
                        StartRegister();
                        break;
                    case PrescriptionDeclareStatus.Prescribe:
                        StartPrescribe();
                        break;
                    case PrescriptionDeclareStatus.NoCard:
                        StartNoCardAdjust();
                        break;
                }
            };
            IsBusy = true;
            printWorker.RunWorkerAsync();
        }


        private void StartCooperativePrescribe()
        {
            if (IsReadCard)
            {
                var resetWorker = new BackgroundWorker();
                resetWorker.DoWork += (obj, arg) =>
                {
                    BusyContent = StringRes.重置讀卡機;
                    HisApiBase.csSoftwareReset(3);
                };
                resetWorker.RunWorkerCompleted += (obj, arg) =>
                {
                    IsBusy = false;
                    InsertAdjustData();
                };
                IsBusy = true;
                resetWorker.RunWorkerAsync();
            }
            else
                InsertAdjustData();
        }
        private void StartNormalAdjust()
        {
            if (ErrorCode is null)
            {
                if (!IsReadCard)
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
                    if (!InsertRegisterData())
                        return;
                    break;
                case PrescriptionSource.Cooperative:
                    MessageWindow.ShowMessage(StringRes.登錄合作診所處方, MessageType.ERROR);
                    return;
            }
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
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
                PurchaseOrder.InsertPrescriptionOrder(CurrentPrescription, ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).PrescriptionSendData);
            //紀錄訂單and送單
            else if (CurrentPrescription.PrescriptionStatus.IsSendOrder && ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn == false
                && CurrentPrescription.PrescriptionStatus.IsSendToSingde)
            {
                PurchaseOrder.UpdatePrescriptionOrder(CurrentPrescription, ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).PrescriptionSendData);
            } //更新傳送藥健康
            CurrentPrescription.PrescriptionStatus.IsSendToSingde = true;
            CurrentPrescription.PrescriptionStatus.UpdateStatus(CurrentPrescription.Id);
            return true;
        }
        private void InsertPrescribeData()
        {
            CurrentPrescription.PrescriptionStatus.SetPrescribeStatus();
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.Prescribe();//自費調劑金流
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
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
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                HisApiBase.csSoftwareReset(3);
            };
            worker.RunWorkerAsync();
            BusyContent = StringRes.產生每日上傳資料;
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
            CusPreSelectWindow customPrescriptionWindow = null;
            if(customPresChecked) return;
            Messenger.Default.Register<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            Messenger.Default.Register<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            customPrescriptionWindow = new CusPreSelectWindow(CurrentPrescription.Patient.ID, CurrentPrescription.Patient.IDNumber, CurrentPrescription.Card);
        }

        private void GetMedicalNumber()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                CurrentPrescription.PrescriptionStatus.IsGetCard = true;
                BusyContent = StringRes.檢查就醫次數;
                CurrentPrescription.Card.GetRegisterBasic();
                if (CurrentPrescription.Card.AvailableTimes != null && CurrentPrescription.Card.AvailableTimes == 0)
                {
                    BusyContent = StringRes.更新卡片;
                    CurrentPrescription.Card.UpdateCard();
                }
                BusyContent = StringRes.取得就醫序號;
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
                BusyContent = StringRes.寫卡;
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
                InsertAdjustData();
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
                MessageWindow.ShowMessage(StringRes.重新過卡或押金, MessageType.WARNING);
                IsAdjusting = false;
                IsCardReading = false;
                return false;
            }
            ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            return true;
        }

        private void InsertAdjustData()
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.SetAdjustStatus();//設定處方狀態
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    if (!CurrentPrescription.Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
                        CurrentPrescription.NormalAdjust(false);
                    else
                    {
                        CurrentPrescription.Medicines.SetBuckle(false);
                        CurrentPrescription.CooperativeAdjust(false);
                    }
                    break;
                case PrescriptionSource.Cooperative:
                    CurrentPrescription.Medicines.SetBuckle(false);
                    CurrentPrescription.CooperativeAdjust(false);
                    break;
                case PrescriptionSource.ChronicReserve:
                    CurrentPrescription.ChronicAdjust(false);
                    break;
            }
            CheckDailyUpload();
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }

        private void CheckDailyUpload()
        {
            if (CurrentPrescription.PrescriptionStatus.IsCooperativePrescribe) return;
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
            var isVip = new ConfirmWindow(StringRes.免收部分負擔, StringRes.免收確認);
            CurrentPrescription.PrescriptionStatus.IsCooperativeVIP = (bool)isVip.DialogResult;
        }

        private void PrintMedBag(bool noCard,bool printMedBag,bool? printSingle, bool printReceipt)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (printMedBag)
                {
                    BusyContent = StringRes.藥袋列印;
                    CurrentPrescription.PrintMedBag((bool)printSingle);
                }
                if (printReceipt)
                {
                    BusyContent = StringRes.收據列印;
                    CurrentPrescription.PrintReceipt();
                }
                if (noCard)
                {
                    BusyContent = StringRes.押金單據列印;
                    CurrentPrescription.PrintDepositSheet();
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void StartPrescribe()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                CurrentPrescription.Patient = new Customer(CustomerDb.GetCustomerByCusId(0).Rows[0]);
            InsertPrescribeData();
        }
        private void StartNoCardAdjust()
        {
            CurrentPrescription.PrescriptionStatus.SetNoCardSatus();
            MainWindow.ServerConnection.OpenConnection();
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    if (CurrentPrescription.Treatment.Institution.ID != VM.CooperativeInstitutionID)
                        CurrentPrescription.NormalAdjust(true);
                    else
                    {
                        CurrentPrescription.Medicines.SetBuckle(false);
                        CurrentPrescription.CooperativeAdjust(true);
                    }
                    break;
                case PrescriptionSource.Cooperative:
                    CurrentPrescription.Medicines.SetBuckle(false);
                    CurrentPrescription.CooperativeAdjust(false);
                    break;
                case PrescriptionSource.ChronicReserve:
                    CurrentPrescription.ChronicAdjust(true);
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }
        private int UpdatePrescriptionCount()//計算處方張數
        { 
           return CurrentPrescription.Treatment.Pharmacist != null 
                ? PrescriptionDb.GetPrescriptionCountByID(CurrentPrescription.Treatment.Pharmacist.IdNumber).Rows[0].Field<int>("PrescriptionCount")
                : 0; 
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
