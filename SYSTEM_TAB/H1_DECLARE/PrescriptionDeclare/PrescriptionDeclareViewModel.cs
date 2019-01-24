using System;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
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
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.MedicinesSendSingdeWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using StringRes = His_Pos.Properties.Resources;
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
                if (declareStatus != value)
                {
                    Set(() => DeclareStatus, ref declareStatus, value);
                }
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
                Set(() => SelectedMedicine, ref selectedMedicine, value);
            }
        }
        public int SelectedMedicinesIndex { get; set; }
        private FunctionWindow.AddProductWindow.AddMedicineWindow MedicineWindow { get; set; }
        #endregion
        #region Commands
        public RelayCommand ShowCooperativeSelectionWindow { get; set; }
        public RelayCommand GetPatinetData { get; set; }
        // ReSharper disable once InconsistentNaming
        public RelayCommand SearchCustomerByIDNumber { get; set; }
        public RelayCommand SearchCustomerByName { get; set; }
        public RelayCommand SearchCustomerByBirthday { get; set; }
        public RelayCommand SearchCustomerByTel { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand PharmacistSelectionChanged { get; set; }
        public RelayCommand<string> GetDiseaseCodeById { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand ShowCommonInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand<string> EditMedicine { get; set; }
        public RelayCommand MedicinePriceChanged { get; set; }
        public RelayCommand AdjustButtonClick { get; set; }
        public RelayCommand RegisterButtonClick { get; set; }
        public RelayCommand PrescribeButtonClick { get; set; }
        public RelayCommand ClearButtonClick { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            SelectedMedicinesIndex = 0;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            InitializeVariables();
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
                cooperativePrescriptions.GetCooperativePrescriptions(ViewModelMainWindow.CurrentPharmacy.Id, DateTime.Today, DateTime.Today);
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
        private void GetPatientDataAction()
        {
            var isGetCard = false;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.讀取健保卡;
                isGetCard = CurrentPrescription.GetCard();
                if (isGetCard)
                {
                    BusyContent = StringRes.取得就醫序號;
                    CurrentPrescription.Treatment.GetLastMedicalNumber();
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if (isGetCard)
                {
                    CurrentPrescription.PrescriptionStatus.IsGetCard = true;
                    CurrentPrescription.Card.GetMedicalNumber(1);
                    return;
                }
                var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow();
                customerSelectionWindow.ShowDialog();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void SearchCusByIDNumAction()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.IDNumber, 3);
            customerSelectionWindow.ShowDialog();
        }
        private void SearchCusByNameAction()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Name, 2);
            customerSelectionWindow.ShowDialog();
        }
        private void SearchCusByBirthAction()
        {
            if (CurrentPrescription.Patient.Birthday is null) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(DateTimeExtensions.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), 1);
            customerSelectionWindow.ShowDialog();
        }
        private void SearchCustomerByTelAction()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Tel, 4);
            customerSelectionWindow.ShowDialog();
        }
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString+"4",MessageType.WARNING);
                return;
            }
            CurrentPrescription.Treatment.Institution = null;
            var result = Institutions.Where(i => i.Id.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    CurrentPrescription.Treatment.Institution = result[0];
                    break;
                default:
                    var institutionSelectionWindow = new InstitutionSelectionWindow.InstitutionSelectionWindow(search);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            
        }
        private void PharmacistChangedAction()
        {
            PrescriptionCount = CurrentPrescription.UpdatePrescriptionCount();
        }
        private void AddMedicineAction(string medicineID)
        {
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString+"5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructsBySearchString(medicineID).Count;
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                MedicineWindow = new FunctionWindow.AddProductWindow.AddMedicineWindow(medicineID);
                MedicineWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                MedicineWindow = new FunctionWindow.AddProductWindow.AddMedicineWindow(medicineID);
            }
            else
            {
                MessageWindow.ShowMessage(StringRes.MedicineNotFound, MessageType.WARNING);
            }
        }
        private void AdjustButtonClickAction()
        {
            var error = CurrentPrescription.CheckPrescriptionRule();
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error,MessageType.ERROR);
            }
            else
            {
                if (!CurrentPrescription.Card.IsGetMedicalNumber && !CurrentPrescription.PrescriptionStatus.IsDeposit)
                {
                    //詢問異常上傳
                }
                MainWindow.ServerConnection.OpenConnection();
                switch (CurrentPrescription.Source)
                {
                    case PrescriptionSource.Normal:
                        NormalAdjust();
                        break;
                    case PrescriptionSource.Cooperative:
                        CooperativeAdjust(); 
                        break;
                    case PrescriptionSource.ChronicReserve:
                        ChronicAdjust();
                        break;
                }
                MainWindow.ServerConnection.CloseConnection();
                if (CurrentPrescription.PrescriptionStatus.IsGetCard)
                {
                    CreateDailyUploadData();
                }
                PrintMedBag();
                ClearPrescription();
            }
        }

        private void PrintMedBag()
        {
            var medBagPrint = new ConfirmWindow(StringRes.PrintMedBag, StringRes.PrintConfirm);
            if (medBagPrint.DialogResult != null && (bool)medBagPrint.DialogResult)
            {
                var printBySingleMode = new MedBagSelectionWindow();
                var singleMode = false;
                if (printBySingleMode.DialogResult != null)
                    singleMode = printBySingleMode.DialogResult != null && (bool)printBySingleMode.DialogResult;
                var receiptPrint = false;
                if (CurrentPrescription.PrescriptionPoint.AmountsPay > 0)
                {
                    var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm);
                    if (receiptResult.DialogResult != null)
                        receiptPrint = (bool) receiptResult.DialogResult;
                }
                CurrentPrescription.PrintMedBag(singleMode, receiptPrint);
            }
        }

        private void RegisterButtonClickAction()
        {
            var error = CurrentPrescription.CheckPrescriptionRule();
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
            }
            else
            {
                MainWindow.ServerConnection.OpenConnection();
                switch (CurrentPrescription.Source)
                {
                    case PrescriptionSource.Normal:
                        NormalRegister();
                        break;
                    case PrescriptionSource.Cooperative:
                        MessageWindow.ShowMessage("合作診所處方不可登陸 請將調劑日期設定為今天", MessageType.ERROR);
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
            //自費調劑
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
            ClearPrescription();
        }
        #endregion
        #region InitialFunctions
        private void InitializeVariables()
        {
            NotPrescribe = true;
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
            InitialPrescription();
        }
        private void InitialItemsSources()
        {
            Institutions = ViewModelMainWindow.Institutions;
            Divisions = ViewModelMainWindow.Divisions;
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            PaymentCategories = ViewModelMainWindow.PaymentCategories;
            PrescriptionCases = ViewModelMainWindow.PrescriptionCases;
            Copayments = ViewModelMainWindow.Copayments;
            SpecialTreats = ViewModelMainWindow.SpecialTreats;
        }
        private void InitialCommandActions()
        {
            ShowCooperativeSelectionWindow = new RelayCommand(ShowCooperativeWindowAction);
            GetPatinetData = new RelayCommand(GetPatientDataAction);
            SearchCustomerByIDNumber = new RelayCommand(SearchCusByIDNumAction);
            SearchCustomerByName = new RelayCommand(SearchCusByNameAction);
            SearchCustomerByBirthday = new RelayCommand(SearchCusByBirthAction);
            SearchCustomerByTel = new RelayCommand(SearchCustomerByTelAction);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ShowCommonInsSelectionWindowAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            PharmacistSelectionChanged = new RelayCommand(PharmacistChangedAction);
            GetDiseaseCodeById = new RelayCommand<string>(GetDiseaseCodeByIdAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            AdjustButtonClick = new RelayCommand(AdjustButtonClickAction);
            RegisterButtonClick = new RelayCommand(RegisterButtonClickAction);
            PrescribeButtonClick = new RelayCommand(PrescribeButtonClickAction);
            ClearButtonClick = new RelayCommand(ClearPrescription);
        }

        private void InitialPrescription()
        {
            CurrentPrescription = new Prescription();
            CurrentPrescription.InitialCurrentPrescription();
        }
        private void ClearPrescription()
        {
            InitialPrescription();
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            Messenger.Default.Register<Prescription>(this, "SelectedPrescription", GetSelectedPrescription);
            Messenger.Default.Register<Institution>(this, "SelectedInstitution", GetSelectedInstitution);
            Messenger.Default.Register<ProductStruct>(this, "SelectedProduct", GetSelectedProduct);
            Messenger.Default.Register<NotificationMessage>("DeleteMedicine", DeleteMedicine);
            Messenger.Default.Register<NotificationMessage>("AdjustDateChanged", AdjustDateChanged);
        }

        private void AdjustDateChanged(NotificationMessage adjustChange)
        {
            if (!adjustChange.Notification.Equals("AdjustDateChanged")) return;
            CheckDeclareStatus();
        }

        #endregion
        #region EventAction
        private void AdjustCaseSelectionChangedAction()
        {
            CheckDeclareStatus();
            if (CurrentPrescription.Treatment.AdjustCase != null &&
                CurrentPrescription.Treatment.AdjustCase.Id.Equals("0"))
            {
                NotPrescribe = false;
                CurrentPrescription.Treatment.Institution =
                    new Institution
                    {
                        Id = ViewModelMainWindow.CurrentPharmacy.Id,
                        Name = ViewModelMainWindow.CurrentPharmacy.Name,
                        FullName = ViewModelMainWindow.CurrentPharmacy.Id + ViewModelMainWindow.CurrentPharmacy.Name
                    };
                CurrentPrescription.Treatment.PrescriptionCase = null;
                CurrentPrescription.Treatment.TempMedicalNumber = string.Empty;
                CurrentPrescription.Treatment.Copayment = null;
                CurrentPrescription.Treatment.TreatDate = null;
                CurrentPrescription.Treatment.ChronicSeq = null;
                CurrentPrescription.Treatment.ChronicTotal = null;
                CurrentPrescription.Treatment.Division = null;
                CurrentPrescription.Treatment.MainDisease = null;
                CurrentPrescription.Treatment.SubDisease = null;
                CurrentPrescription.Treatment.SpecialTreat = null;
                CurrentPrescription.Treatment.PaymentCategory = null;
                SetMedicinesPaySelf();
            }
            else
            {
                NotPrescribe = true;
            }
        }

        private void SetMedicinesPaySelf()
        {
            var medList = CurrentPrescription.Medicines.Where(m => m is MedicineNHI || m is MedicineOTC).ToList();
            if (medList.Count > 0)
            {
                foreach (var m in medList)
                {
                    if(m.PaySelf) continue;
                    m.PaySelf = true;
                }
            }
        }

        private void GetDiseaseCodeByIdAction(string id)
        {
            var d = new DiseaseCode();
            MainWindow.ServerConnection.OpenConnection();
            d.GetDataByCodeId(id);
            MainWindow.ServerConnection.CloseConnection();
            if (string.IsNullOrEmpty(CurrentPrescription.Treatment.MainDisease.FullName))
            {
                if (string.IsNullOrEmpty(d.ID))
                {
                    MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
                }
                else
                {
                    CurrentPrescription.Treatment.MainDisease = d;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(d.ID))
                {
                    MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
                }
                else
                {
                    CurrentPrescription.Treatment.SubDisease = d;
                }
            }
        }
        private void GetSelectedProduct(ProductStruct selectedProduct)
        {
            CurrentPrescription.AddMedicineBySearch(selectedProduct.ID,SelectedMedicinesIndex);
            CurrentPrescription.CountPrescriptionPoint();
            if (SelectedMedicinesIndex == CurrentPrescription.Medicines.Count - 1)
                CurrentPrescription.Medicines.Add(new Medicine());
        }
        #endregion
        #region CommandActions
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            CurrentPrescription.Patient = receiveSelectedCustomer;
        }
        private void GetSelectedPrescription(Prescription receiveSelectedPrescription)
        {
            CurrentPrescription = receiveSelectedPrescription;
            //MainWindow.ServerConnection.OpenConnection();
            //CurrentPrescription.ConvertNHIandOTCPrescriptionMedicines();
            //MainWindow.ServerConnection.CloseConnection();
            CurrentPrescription.CountPrescriptionPoint();
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            CurrentPrescription.Treatment.Institution = receiveSelectedInstitution;
        }
        private void DeleteMedicine(NotificationMessage obj)
        {
            if(CurrentPrescription.Medicines.Count <= SelectedMedicinesIndex) return;
            var m = CurrentPrescription.Medicines[SelectedMedicinesIndex];
            if (m is MedicineNHI med && !string.IsNullOrEmpty(med.Source) ||
                m is MedicineOTC otc && !string.IsNullOrEmpty(otc.Source))
            {
                CurrentPrescription.Medicines.RemoveAt(SelectedMedicinesIndex);
                CurrentPrescription.CountPrescriptionPoint();
                if (CurrentPrescription.Medicines.Count == 0)
                {
                    CurrentPrescription.Medicines.Add(new Medicine());
                }
            }
                
        }
        #endregion
        #region GeneralFunctions
        private void CheckDeclareStatus()
        {
            if(!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.Id) && CurrentPrescription.Treatment.AdjustCase.Id.Equals("0"))
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
                    DeclareStatus = DateTime.Today.Date >= ((DateTime)adjust).Date ?
                        PrescriptionDeclareStatus.Adjust :
                        PrescriptionDeclareStatus.Register;
                }
            }
        }
        private void NormalAdjust()
        {
            CurrentPrescription.Id = CurrentPrescription.InsertPresription();
            CurrentPrescription.ProcessInventory();
            CurrentPrescription.ProcessEntry("調劑耗用", "PreMasId", CurrentPrescription.Id);
            CurrentPrescription.ProcessCopaymentCashFlow("部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("自費");
            CurrentPrescription.ProcessSelfPayCashFlow("押金");
        }
        private void CooperativeAdjust()
        {
            CurrentPrescription.Id = CurrentPrescription.InsertPresription();
            CurrentPrescription.InsertCooperAdjust();
            CurrentPrescription.ProcessCopaymentCashFlow("合作部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("合作自費");
            CurrentPrescription.ProcessSelfPayCashFlow("合作押金");
            CurrentPrescription.UpdateCooperativePrescriptionStatus();
        }
        private void ChronicAdjust()
        {
            CurrentPrescription.Id = CurrentPrescription.InsertPresription();
            CurrentPrescription.PredictResere();
            CurrentPrescription.DeleteReserve();
            CurrentPrescription.ProcessInventory();
            CurrentPrescription.ProcessEntry("調劑耗用", "PreMasId", CurrentPrescription.Id);
            CurrentPrescription.ProcessCopaymentCashFlow("部分負擔");
            CurrentPrescription.ProcessDepositCashFlow("自費");
            CurrentPrescription.ProcessSelfPayCashFlow("押金");
        }
        private void NormalRegister() { 
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder) {
                MedicinesSendSingdeWindow.MedicinesSendSingdeWindow medicinesSendSingdeWindow = new MedicinesSendSingdeWindow.MedicinesSendSingdeWindow(CurrentPrescription);
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn) {  
                    return;
                }
                    
            } 
            CurrentPrescription.InsertReserve(); 
        }
        
        private void ChronicRegister() {
            if (CurrentPrescription.PrescriptionStatus.IsSendOrder)
            {
                MedicinesSendSingdeWindow.MedicinesSendSingdeWindow medicinesSendSingdeWindow = new MedicinesSendSingdeWindow.MedicinesSendSingdeWindow(CurrentPrescription);
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                { 
                    return;
                }
            }
            CurrentPrescription.UpdateReserve();
        }
        private void CreateDailyUploadData()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.寫卡;
                CurrentPrescription.PrescriptionSign = HisApiFunction.WritePrescriptionData(CurrentPrescription);
                BusyContent = StringRes.產生每日上傳資料;
                if (CurrentPrescription.Card.IsGetMedicalNumber)
                    HisApiFunction.CreatDailyUploadData();
                else
                {
                    if (!CurrentPrescription.PrescriptionStatus.IsDeposit)
                    {
                        HisApiFunction.CreatErrorDailyUploadData();
                    }
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void CountMedicinePoint()
        {
            CurrentPrescription.CountPrescriptionPoint();
        }
        #endregion
    }
}
