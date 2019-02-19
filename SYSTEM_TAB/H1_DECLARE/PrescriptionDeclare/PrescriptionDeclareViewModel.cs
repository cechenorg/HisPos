using System;
using System.ComponentModel;
using System.Linq;
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
using His_Pos.NewClass;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
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
        private bool? isDeposit;
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
        private bool customPresChecked { get; set; }
        private readonly string CooperativeInstitutionID = WebApi.GetCooperativeClinicId(VM.CurrentPharmacy.ID);
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
        #endregion
        public PrescriptionDeclareViewModel()
        {
            SelectedMedicinesIndex = 0;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            InitialView();
            InitializeVariables();
        }

        private void InitialView()
        {
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
        }

        ~PrescriptionDeclareViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        #region CommandActions
        private void ShowCooperativeWindowAction()
        {
            var cooperativePrescriptions = new Prescriptions();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.GetCooperativePrescriptions;
                cooperativePrescriptions.GetCooperativePrescriptions(VM.CurrentPharmacy.ID, DateTime.Today, DateTime.Today);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                var cooperativeSelectionWindow = new CooPreSelectWindow();
                Messenger.Default.Send(cooperativePrescriptions, "CooperativePrescriptions");
                cooperativeSelectionWindow.ShowDialog();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void GetPatientDataAction()
        {
            customPresChecked = false;
            IsReadCard = true;
            ReadCard(true);
        }
        private void SearchCusByIDNumAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber))
                customerSelectionWindow = new CusSelectWindow();
            else
            {
                if(CurrentPrescription.Patient.IDNumber.Length != 10)
                    MessageWindow.ShowMessage("身分證/居留證號長度須為10", MessageType.WARNING);
                else
                {
                    if (CurrentPrescription.Patient.Count() == 0)
                    {
                        if (!string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                        {
                            var confirm = new ConfirmWindow("查無顧客資料,是否新增?", "查無資料");
                            if ((bool)confirm.DialogResult)
                                CurrentPrescription.Patient.Check();
                        }
                        else
                        {
                            MessageWindow.ShowMessage("查無資料，若要新增請至少填寫姓名與身分證", MessageType.WARNING);
                        }
                    }
                    else
                    {
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
                customerSelectionWindow = new CusSelectWindow();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber))
                    {
                        ConfirmWindow confirm = new ConfirmWindow("查無顧客資料,是否新增?", "查無資料");
                        if ((bool)confirm.DialogResult)
                            CurrentPrescription.Patient.Check();
                        else
                            return;
                    }
                    else
                    {
                        MessageWindow.ShowMessage("查無資料，若要新增請至少填寫姓名與身分證", MessageType.WARNING);
                    }
                }
                else
                    customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Name, 2);
            }
        }
        private void SearchCusByBirthAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (CurrentPrescription.Patient.Birthday is null)
                customerSelectionWindow = new CusSelectWindow();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && !string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                    {
                        ConfirmWindow confirm = new ConfirmWindow("查無顧客資料,是否新增?", "查無資料");
                        if ((bool)confirm.DialogResult)
                            CurrentPrescription.Patient.Check();
                        else
                            return;
                    }
                    else
                    {
                        MessageWindow.ShowMessage("查無資料，若要新增請至少填寫姓名與身分證", MessageType.WARNING);
                    }
                }
                else
                    customerSelectionWindow = new CusSelectWindow(DateTimeEx.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), 1);
            }
        }
        private void SearchCustomerByTelAction()
        {
            customPresChecked = false;
            customerSelectionWindow = null;
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel))
                customerSelectionWindow = new CusSelectWindow();
            else
            {
                if (CurrentPrescription.Patient.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && !string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
                    {
                        ConfirmWindow confirm = new ConfirmWindow("查無顧客資料,是否新增?", "查無資料");
                        if ((bool)confirm.DialogResult)
                            CurrentPrescription.Patient.Check();
                        else
                            return;
                    }
                    else
                    {
                        MessageWindow.ShowMessage("查無資料，若要新增請至少填寫姓名與身分證", MessageType.WARNING);
                    }
                }
                else
                    customerSelectionWindow = new CusSelectWindow(CurrentPrescription.Patient.Tel, 4);
            }
        }
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString+"4",MessageType.WARNING);
                return;
            }
            if (CurrentPrescription.Treatment.Institution != null && !string.IsNullOrEmpty(CurrentPrescription.Treatment.Institution.FullName) && search.Equals(CurrentPrescription.Treatment.Institution.FullName))
            {
                Messenger.Default.Send(new NotificationMessage("FocusDivision"));
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
                    var institutionSelectionWindow = new InsSelectWindow(search,ViewModelEnum.PrescriptionDeclare);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            var commonInsSelectionWindow = new CommonHospitalsWindow(ViewModelEnum.PrescriptionDeclare);
            commonInsSelectionWindow.ShowDialog();
        }
        private void PharmacistChangedAction()
        {
            PrescriptionCount = CurrentPrescription.UpdatePrescriptionCount();
        }
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString+"5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                MedicineWindow = new MedSelectWindow(medicineID,AddProductEnum.PrescriptionDeclare);
                MedicineWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionDeclare);
            }
            else
            {
                MessageWindow.ShowMessage(StringRes.MedicineNotFound, MessageType.WARNING);
            }
        }
        private void AdjustButtonClickAction()
        {
            var error = CurrentPrescription.CheckPrescriptionRule(ErrorCode == null);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error,MessageType.ERROR);
            }
            else
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    CurrentPrescription.Patient.Save();
                    PrintMedBag(false);
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    if (ErrorCode is null)
                    {
                        if (!IsReadCard)
                            ReadCard(false);
                        else
                        {
                            GetMedicalNumber();
                        }
                    }
                    else
                    {
                        StartAdjust();
                    }
                };
                worker.RunWorkerAsync();
            }
        }

        private void RegisterButtonClickAction()
        {
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
            }
            else
            {
                CurrentPrescription.Patient.Save();
                MainWindow.ServerConnection.OpenConnection();
                switch (CurrentPrescription.Source)
                {
                    case PrescriptionSource.Normal:
                        NormalRegister();
                        break;
                    case PrescriptionSource.Cooperative:
                        MessageWindow.ShowMessage(StringRes.登錄合作診所處方, MessageType.ERROR);
                        return; 
                    case PrescriptionSource.ChronicReserve:
                        ChronicRegister();
                        break;
                }
                MainWindow.ServerConnection.CloseConnection();
                MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
                ClearPrescription();
            }

        }
        private void PrescribeButtonClickAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            PrescribeFunction();
            MainWindow.ServerConnection.CloseConnection();
            var receiptPrint = false;
            var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm);
            if (receiptResult.DialogResult != null)
                receiptPrint = (bool)receiptResult.DialogResult;
            if (receiptPrint)
                CurrentPrescription.PrintReceipt();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }

        private void MedicineNoBuckleAction()
        {
            SelectedMedicine.IsBuckle = !SelectedMedicine.IsBuckle; 
        }

        private void NoCardAdjustAction()
        {
            var noCard = new ConfirmWindow("確認欠卡調劑", "欠卡確認");
            if (!(bool)noCard.DialogResult) return;
            var error = CurrentPrescription.CheckPrescriptionRule(true);
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
            }
            else
            {
                StartNoCardAdjust();
            }
        }
        private void ErrorCodeSelectAction()
        {
            var e = new ErrorUploadWindow(false); //詢問異常上傳
            e.ShowDialog();
            var errCode = ((ErrorUploadWindowViewModel) e.DataContext).SelectedIcErrorCode;
            if (errCode != null)
                ErrorCode = errCode;
        }
        #endregion
        #region InitialFunctions
        private void InitializeVariables()
        {
            NotPrescribe = true;
            CanAdjust = true;
            IsReadCard = false;
            ErrorCode = null;
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
            ShowCooperativeSelectionWindow = new RelayCommand(ShowCooperativeWindowAction);
            GetPatientData = new RelayCommand(GetPatientDataAction);
            SearchCustomerByIDNumber = new RelayCommand(SearchCusByIDNumAction);
            SearchCustomerByName = new RelayCommand(SearchCusByNameAction);
            SearchCustomerByBirthday = new RelayCommand(SearchCusByBirthAction);
            SearchCustomerByTel = new RelayCommand(SearchCustomerByTelAction);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ShowCommonInsSelectionWindowAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            PharmacistSelectionChanged = new RelayCommand(PharmacistChangedAction);
            GetMainDiseaseCodeById = new RelayCommand<string>(GetMainDiseaseCodeByIdAction);
            GetSubDiseaseCodeById = new RelayCommand<string>(GetSubDiseaseCodeByIdAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            AdjustButtonClick = new RelayCommand(AdjustButtonClickAction);
            RegisterButtonClick = new RelayCommand(RegisterButtonClickAction);
            PrescribeButtonClick = new RelayCommand(PrescribeButtonClickAction);
            ClearButtonClick = new RelayCommand(ClearPrescription);
            ChronicSequenceTextChanged = new RelayCommand(CheckPrescriptionVariable);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ResetCardReader = new RelayCommand(ResetCardReaderAction);
            NoCardAdjust = new RelayCommand(NoCardAdjustAction);
            MedicineNoBuckleClick = new RelayCommand(MedicineNoBuckleAction);
            SendOrderCommand = new RelayCommand(CheckDeclareStatus);
            ErrorCodeSelect = new RelayCommand(ErrorCodeSelectAction);
        }

        private void InitialPrescription()
        {
            CurrentPrescription = new Prescription();
            CurrentPrescription.InitialCurrentPrescription();
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionCount = CurrentPrescription.UpdatePrescriptionCount();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ClearPrescription()
        {
            InitializeVariables();
            InitialPrescription();
            isDeposit = null;
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            Messenger.Default.Register<Prescription>(this, "SelectedPrescription", GetSelectedPrescription);
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionDeclareViewModel)+"InsSelected", GetSelectedInstitution);
            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this,GetSelectedProduct);
            Messenger.Default.Register<NotificationMessage>("AdjustDateChanged", AdjustDateChanged);
            Messenger.Default.Register<Prescription>(this, "CustomPrescriptionSelected", GetCustomPrescription);
            Messenger.Default.Register<NotificationMessage>("CustomPresChecked",(notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CustomPresChecked"))
                    customPresChecked = true;
            });
        }
        #endregion
        #region EventAction
        private void AdjustCaseSelectionChangedAction()
        {
            if (CurrentPrescription.Treatment.AdjustCase != null &&
                CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
            {
                NotPrescribe = false;
                CurrentPrescription.Treatment.Clear();
                SetMedicinesPaySelf();
            }
            else
            {
                NotPrescribe = true;
            }
            CurrentPrescription.CheckPrescriptionVariable();
            CheckDeclareStatus();
        }
        private void CopaymentSelectionChangedAction()
        {
            CurrentPrescription.CountPrescriptionPoint(false);
        }

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

        private void GetMainDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id) || CurrentPrescription.Treatment.MainDisease is null || (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MainDisease.FullName) && id.Equals(CurrentPrescription.Treatment.MainDisease.FullName)))
            {
                Messenger.Default.Send(new NotificationMessage("FocusSubDisease"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                CurrentPrescription.Treatment.MainDisease = result;
            }
        }

        private void GetSubDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id) || CurrentPrescription.Treatment.SubDisease is null ||(!string.IsNullOrEmpty(CurrentPrescription.Treatment.SubDisease.FullName) && id.Equals(CurrentPrescription.Treatment.SubDisease.FullName)))
            {
                Messenger.Default.Send(new NotificationMessage("FocusChronicTotal"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                CurrentPrescription.Treatment.SubDisease = result;
            }
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification == nameof(PrescriptionDeclareViewModel))
            {
                var selected = CurrentPrescription.Medicines.IndexOf(SelectedMedicine);
                if (selected < 0 || selected >= CurrentPrescription.Medicines.Count) return;
                CurrentPrescription.AddMedicineBySearch(msg.Content.ID, selected);
                if (selected == CurrentPrescription.Medicines.Count - 1)
                    CurrentPrescription.Medicines.Add(new Medicine());
                Messenger.Default.Send(selected, "FocusDosage");
            }
        }
        private void AdjustDateChanged(NotificationMessage adjustChange)
        {
            if (!adjustChange.Notification.Equals("AdjustDateChanged")) return;
            CheckDeclareStatus();
        }
        #endregion
        #region MessengerReceiveActions
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            if((receiveSelectedCustomer != null && CurrentPrescription.Patient != null) && receiveSelectedCustomer.ID == CurrentPrescription.Patient.ID)
                return;
            CurrentPrescription.Patient = receiveSelectedCustomer;
            CheckCustomPrescriptions();
        }
        private void GetSelectedPrescription(Prescription receiveSelectedPrescription)
        {
            CurrentPrescription = receiveSelectedPrescription;
            CurrentPrescription.CountPrescriptionPoint(true);
            priviousSelectedIndex = CurrentPrescription.Medicines.Count - 1;
            CanAdjust = true;
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            CurrentPrescription.Treatment.Institution = receiveSelectedInstitution;
        }
        private void DeleteMedicineAction()
        {
            CurrentPrescription.Medicines.RemoveAt(CurrentPrescription.Medicines.IndexOf(SelectedMedicine));
        }
        private void GetCustomPrescription(Prescription p)
        {
            p.Treatment.AdjustDate = DateTime.Today;
            CurrentPrescription = p;
            CurrentPrescription.CountPrescriptionPoint(true);
        }
        #endregion
        #region GeneralFunctions
        private void CheckDeclareStatus()
        {
            if(string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID)) return;
            if(!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.ID) && CurrentPrescription.Treatment.AdjustCase.ID.Equals("0"))
                DeclareStatus = PrescriptionDeclareStatus.Prescribe;
            else
            {
                var adjust = CurrentPrescription.Treatment.AdjustDate;
                if (adjust is null)
                {
                    DeclareStatus = PrescriptionDeclareStatus.Adjust;
                }
                else
                {
                    if (DateTime.Today.Date == ((DateTime) adjust).Date)
                    {
                        if ((CurrentPrescription.Treatment.ChronicSeq != null && CurrentPrescription.Treatment.ChronicSeq > 0) &&
                            CurrentPrescription.Treatment.AdjustCase.ID.Equals("2") && CurrentPrescription.PrescriptionStatus.IsSendOrder)
                            DeclareStatus = PrescriptionDeclareStatus.Register;
                        else
                        {
                            DeclareStatus = PrescriptionDeclareStatus.Adjust;
                        }
                    }
                    else if (DateTime.Today.Date < ((DateTime)adjust).Date)
                    {
                        DeclareStatus = PrescriptionDeclareStatus.Register;
                    }
                    else if (DateTime.Today.Date > ((DateTime)adjust).Date)
                    {
                        DeclareStatus = PrescriptionDeclareStatus.Adjust;
                    }
                }
            }
        }
        private void NormalAdjust(bool noCard)
        { 
            if (CurrentPrescription.Id == 0)
                CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
            else 
                CurrentPrescription.Update();
            CurrentPrescription.ProcessInventory("處方調劑", "PreMasID", CurrentPrescription.Id.ToString());
            CurrentPrescription.ProcessEntry("調劑耗用", "PreMasId", CurrentPrescription.Id);
            CurrentPrescription.ProcessCopaymentCashFlow("部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("自費");
            if(noCard)
                CurrentPrescription.ProcessSelfPayCashFlow("押金");
        }
        private void CooperativeAdjust(bool noCard)
        {
            CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
            CurrentPrescription.InsertCooperAdjust();
            if(CurrentPrescription.PrescriptionStatus.IsCooperativeVIP)
                CurrentPrescription.ProcessVipCopaymentCashFlow("合作部分負擔");
            else
                CurrentPrescription.ProcessCopaymentCashFlow("合作部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("合作自費");
            if (noCard)
                CurrentPrescription.ProcessSelfPayCashFlow("合作押金");
            CurrentPrescription.UpdateCooperativePrescriptionStatus();
        }
        private void ChronicAdjust(bool noCard)
        {
            CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
            CurrentPrescription.AdjustPredictResere(); 
            CurrentPrescription.ProcessInventory("處方調劑", "PreMasID", CurrentPrescription.Id.ToString());
            CurrentPrescription.ProcessEntry("調劑耗用", "PreMasId", CurrentPrescription.Id);
            CurrentPrescription.ProcessCopaymentCashFlow("部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("自費");
            if (noCard)
                CurrentPrescription.ProcessSelfPayCashFlow("押金");
        }
        private void NormalRegister() {
            MedSendWindow medicinesSendSingdeWindow = null;
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder) {
                medicinesSendSingdeWindow = new MedSendWindow(CurrentPrescription);
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn) {  
                    return;
                }  
            }
            CurrentPrescription.PrescriptionStatus.IsDeclare = false;
            if (CurrentPrescription.Id == 0) {
                CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
                CurrentPrescription.PredictResere();
            } 
            else
                CurrentPrescription.Update();
           
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder && ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn == false) 
                PurchaseOrder.InsertPrescriptionOrder(CurrentPrescription, ((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).PrescriptionSendData);
            //紀錄訂單and送單
        }
        
        private void ChronicRegister() {
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder)
            {
                MedSendWindow medicinesSendSingdeWindow = new MedSendWindow(CurrentPrescription);
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                { 
                    return;
                }
            }
            CurrentPrescription.PrescriptionStatus.IsDeclare = false;
            CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
            CurrentPrescription.AdjustPredictResere();
        }
        private void PrescribeFunction()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
            {
                var confirm = new ConfirmWindow("未填寫顧客資料，是否以匿名取代?", "查無資料");
                if ((bool)confirm.DialogResult)
                    CurrentPrescription.Patient = new Customer(CustomerDb.GetCustomerByCusId(0).Rows[0]);
                else
                    return;
            }
            else if(!string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber) && !string.IsNullOrEmpty(CurrentPrescription.Patient.Name))
            {
                CurrentPrescription.Patient.Check();
            }
            else
            {
                MessageWindow.ShowMessage("顧客資料請至少輸入身分證字號與姓名",MessageType.WARNING);
                return;
            }
            CurrentPrescription.PrescriptionStatus.IsDeclare = false;
            CurrentPrescription.Id = CurrentPrescription.InsertPrescription();
            CurrentPrescription.ProcessInventory("自費調劑", "PreMasID", CurrentPrescription.Id.ToString());
            CurrentPrescription.ProcessEntry("調劑耗用", "PreMasId", CurrentPrescription.Id);
            CurrentPrescription.ProcessCopaymentCashFlow("部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("自費");
            CurrentPrescription.ProcessSelfPayCashFlow("押金");
        }
      
        private void CreateDailyUploadData(ErrorUploadWindowViewModel.IcErrorCode error = null)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (CurrentPrescription.PrescriptionStatus.IsGetCard || error != null)
                {
                    if (CurrentPrescription.Card.IsGetMedicalNumber)
                    {
                        if (CreatePrescriptionSign())
                        {
                            Reset(false);
                            HisAPI.CreatDailyUploadData(CurrentPrescription, false);
                        }
                    }
                    else
                    {
                        if (isDeposit != null && (bool)isDeposit)
                            CurrentPrescription.PrescriptionStatus.IsDeclare = false;
                        else
                            HisAPI.CreatErrorDailyUploadData(CurrentPrescription, false ,error);
                    }
                }
                else
                    CurrentPrescription.PrescriptionStatus.IsDeclare = false;
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MainWindow.ServerConnection.OpenConnection();
                CurrentPrescription.PrescriptionStatus.UpdateStatus(CurrentPrescription.Id);
                MainWindow.ServerConnection.CloseConnection();
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
                });
                ClearPrescription();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private bool CreatePrescriptionSign()
        {
            BusyContent = StringRes.寫卡;
            CurrentPrescription.PrescriptionSign = HisAPI.WritePrescriptionData(CurrentPrescription);
            BusyContent = StringRes.產生每日上傳資料;
            if (CurrentPrescription.PrescriptionSign.Count !=
                CurrentPrescription.Medicines.Count(m =>
                    (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf))
            {
                bool? isDone = null;
                ErrorUploadWindowViewModel.IcErrorCode errorCode;
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage(StringRes.寫卡異常, MessageType.ERROR);
                    var e = new ErrorUploadWindow(CurrentPrescription.Card.IsGetMedicalNumber); //詢問異常上傳
                    e.ShowDialog();
                    while (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
                    {
                        e = new ErrorUploadWindow(CurrentPrescription.Card.IsGetMedicalNumber);
                        e.ShowDialog();
                    }
                    errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
                    if(isDone is null)
                        HisAPI.CreatErrorDailyUploadData(CurrentPrescription, true,errorCode);
                    isDone = true;
                });
                return false;
            }
            return true;
        }
        private void CountMedicinePoint()
        {
            CurrentPrescription.CountMedicineDays();
            CurrentPrescription.CheckIfSimpleFormDeclare();
            CurrentPrescription.CountPrescriptionPoint(false);
        }
        private void CheckCustomPrescriptions()
        {
            CusPreSelectWindow customPrescriptionWindow = null;
            if(customPresChecked) return;
            customPrescriptionWindow = new CusPreSelectWindow(CurrentPrescription.Patient, CurrentPrescription.Card);
        }

        private void ReadCard(bool showCusWindow)
        {
            CanAdjust = false;
            var isGetCard = false;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.讀取健保卡;
                isGetCard = CurrentPrescription.GetCard();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if (showCusWindow)
                {
                    CanAdjust = true;
                    if (isGetCard)
                    {
                        CheckCustomPrescriptions();
                        CurrentPrescription.Treatment.GetLastMedicalNumber();
                    }
                    else
                    {
                        CusSelectWindow customerSelectionWindow = null;
                        customerSelectionWindow = new CusSelectWindow();
                    }
                }
                else
                {
                    if (isGetCard)
                        GetMedicalNumber();
                    else
                    {
                        StartAdjust();
                    }
                }
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void GetMedicalNumber()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                CurrentPrescription.PrescriptionStatus.IsGetCard = true;
                BusyContent = StringRes.檢查就醫次數;
                CurrentPrescription.Card.GetRegisterBasic();
                if (CurrentPrescription.Card.AvailableTimes != null)
                {
                    if (CurrentPrescription.Card.AvailableTimes == 0)
                    {
                        BusyContent = StringRes.更新卡片;
                        CurrentPrescription.Card.UpdateCard();
                    }
                }
                BusyContent = StringRes.取得就醫序號;
                CurrentPrescription.Card.GetMedicalNumber(1);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                StartAdjust();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void StartAdjust()
        {
            if (!CurrentPrescription.Card.IsGetMedicalNumber && CurrentPrescription.PrescriptionPoint.Deposit == 0 && isDeposit is null && ErrorCode is null)
            {
                var e = new ErrorUploadWindow(CurrentPrescription.Card.IsGetMedicalNumber); //詢問異常上傳
                e.ShowDialog();
                if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
                {
                    MessageWindow.ShowMessage(StringRes.重新過卡或押金, MessageType.WARNING);
                    isDeposit = true;
                    return;
                }
                ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            }
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.PrescriptionStatus.IsDeposit = false;
            CurrentPrescription.PrescriptionStatus.IsAdjust = true;
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    if (CurrentPrescription.Treatment.Institution.ID != CooperativeInstitutionID)
                        NormalAdjust(false);
                    else
                    {
                        var e = new CooperativeRemarkInsertWindow();
                        CurrentPrescription.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                        if (string.IsNullOrEmpty(CurrentPrescription.Remark) || CurrentPrescription.Remark.Length != 16)
                            return;
                        var isVip = new ConfirmWindow("是否免收部分負擔?", "是否免收部分負擔");
                        CurrentPrescription.PrescriptionStatus.IsCooperativeVIP = (bool)isVip.DialogResult;
                        CooperativeAdjust(false);
                    }
                    break;
                case PrescriptionSource.Cooperative:
                    CooperativeAdjust(false);
                    break;
                case PrescriptionSource.ChronicReserve:
                    ChronicAdjust(false);
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
            CreateDailyUploadData(ErrorCode);
        }

        private void PrintMedBag(bool noCard)
        {
            bool? printmedBag = null;
            Application.Current.Dispatcher.Invoke(delegate
            {
                var medBagPrint = new ConfirmWindow(StringRes.PrintMedBag, StringRes.PrintConfirm);
                printmedBag = medBagPrint.DialogResult;
            });
            if (printmedBag != null && (bool)printmedBag)
            {
                bool? printSingle = null;
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var printBySingleMode = new MedBagSelectionWindow();
                    printSingle = (bool)printBySingleMode.ShowDialog();
                });
                var receiptPrint = false;
                if (CurrentPrescription.PrescriptionPoint.CopaymentPoint + CurrentPrescription.PrescriptionPoint.AmountSelfPay > 0)
                {
                    bool? printReceipt = null;
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm);
                        printReceipt = receiptResult.DialogResult;
                    });
                    if (printReceipt != null)
                        receiptPrint = (bool)printReceipt;
                }
                var worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    BusyContent = "藥袋列印中...";
                    CurrentPrescription.PrintMedBag((bool)printSingle);
                    if (receiptPrint)
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
                    if (noCard)
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
                        });
                        ClearPrescription();
                    }
                };
                IsBusy = true;
                worker.RunWorkerAsync();
            }
            else
            {
                if (noCard)
                {
                    var worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {
                        BusyContent = StringRes.押金單據列印;
                        CurrentPrescription.PrintDepositSheet();
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        IsBusy = false;
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
                        });
                        ClearPrescription();
                    };
                    IsBusy = true;
                    worker.RunWorkerAsync();
                }
            }
        }
        private void ResetCardReaderAction()
        {
            Reset(true);
        }
        private void Reset(bool busy)
        {
            if (busy)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    BusyContent = StringRes.重置讀卡機;
                    if (VM.CurrentPharmacy.NewReader)
                        HisApiBaseNew.csSoftwareReset(3);
                    else
                        HisApiBase.csSoftwareReset(3);
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    IsBusy = false;
                };
                IsBusy = true;
                worker.RunWorkerAsync();
            }
            else
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    if (VM.CurrentPharmacy.NewReader)
                        HisApiBaseNew.csSoftwareReset(3);
                    else
                        HisApiBase.csSoftwareReset(3);
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                };
                worker.RunWorkerAsync();
            }
        }
        private void CheckPrescriptionVariable()
        {
            CurrentPrescription.CheckPrescriptionVariable();
            CheckDeclareStatus();
        }
        private void StartNoCardAdjust()
        {
            CurrentPrescription.PrescriptionStatus.SetNoCardSatus();
            CurrentPrescription.PrescriptionPoint.CountDeposit();
            MainWindow.ServerConnection.OpenConnection();
            CurrentPrescription.PrescriptionStatus.IsDeposit = true;
            CurrentPrescription.PrescriptionStatus.IsAdjust = true;
            switch (CurrentPrescription.Source)
            {
                case PrescriptionSource.Normal:
                    if (CurrentPrescription.Treatment.Institution.ID != CooperativeInstitutionID)
                        NormalAdjust(true);
                    else
                    {
                        var e = new CooperativeRemarkInsertWindow();
                        CurrentPrescription.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                        if (string.IsNullOrEmpty(CurrentPrescription.Remark) || CurrentPrescription.Remark.Length != 16)
                            return;
                        CooperativeAdjust(true);
                    }
                    break;
                case PrescriptionSource.Cooperative:
                    CooperativeAdjust(true);
                    break;
                case PrescriptionSource.ChronicReserve:
                    ChronicAdjust(true);
                    break;
            }
            CurrentPrescription.PrescriptionStatus.UpdateStatus(CurrentPrescription.Id);
            MainWindow.ServerConnection.CloseConnection();
            PrintMedBag(true);
        }
        #endregion
    }
}
