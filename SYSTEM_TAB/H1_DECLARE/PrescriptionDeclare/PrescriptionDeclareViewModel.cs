﻿using System;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CustomerSelectionWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

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
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
        public Usages Usages { get; set; }
        public Positions Positions { get; set; }
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
        private string tempMedicalNumber;
        public string TempMedicalNumber
        {
            get => tempMedicalNumber;
            set
            {
                if (tempMedicalNumber != value)
                {
                    Set(() => TempMedicalNumber, ref tempMedicalNumber, value);
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
        public int SelectedMedicinesIndex { get; set; }
        #endregion
        #region Commands
        public RelayCommand ShowCooperativeSelectionWindow { get; set; }
        public RelayCommand ShowCustomerSelectionWindow { get; set; }
        // ReSharper disable once InconsistentNaming
        public RelayCommand SearchCustomerByIDNumber { get; set; }
        public RelayCommand SearchCustomerByName { get; set; }
        public RelayCommand SearchCustomerByBirthday { get; set; }
        public RelayCommand SearchCustomerByTel { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> GetDiseaseCodeById { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand ShowCommonInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand<string> EditMedicine { get; set; }
        public RelayCommand AdjustButtonClick { get; set; }
        public RelayCommand RegisterButtonClick { get; set; }
        public RelayCommand PrescribeButtonClick { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            CurrentPrescription = new Prescription();
            SelectedMedicinesIndex = 0;
            DeclareStatus = PrescriptionDeclareStatus.Adjust;
            InitializeVariables();
            CurrentPrescription.Medicines.Add(new Medicine());
        }

        ~PrescriptionDeclareViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        #region CommandActions
        private void ExecuteShowCooperativeWindow()
        {
            var cooperativePrescriptions = new Prescriptions();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得合作診所處方...";
                cooperativePrescriptions.GetCooperativePrescriptions(MainWindow.CurrentPharmacy.Id, DateTime.Today, DateTime.Today);
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
        private void ExecuteShowCustomerWindow()
        {
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow();
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private void ExecuteSearchCustomerByIDNumber()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.IDNumber, 3);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private void ExecuteSearchCustomerByName()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Name)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Name, 2);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private void ExecuteSearchCustomerByBirthday()
        {
            if (CurrentPrescription.Patient.Birthday is null) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(DateTimeExtensions.ConvertToTaiwanCalender((DateTime)CurrentPrescription.Patient.Birthday, false), 1);
            customerSelectionWindow.ShowDialog();
            if (((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer != null)
                CurrentPrescription.Patient = ((CustomerSelectionViewModel)customerSelectionWindow.DataContext).SelectedCustomer;
        }
        private void ExecuteSearchCustomerByTel()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Patient.Tel)) return;
            var customerSelectionWindow = new CustomerSelectionWindow.CustomerSelectionWindow(CurrentPrescription.Patient.Tel, 4);
            customerSelectionWindow.ShowDialog();
        }
        private void ExecuteShowInstitutionSelectionWindow(string search)
        {
            if (search.Length < 4) return;
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
        private void ExecuteShowCommonInstitutionSelectionWindow()
        {
            
        }
        private void ExecuteAddMedicine(string medicineID)
        {
            if (medicineID.Length < 5) return;
            FunctionWindow.AddProductWindow.AddMedicineWindow medicineWindow = null;
            medicineWindow = new FunctionWindow.AddProductWindow.AddMedicineWindow(medicineID);
            medicineWindow.ShowDialog();
        }
        private void ExecuteAdjustButtonClick()
        {
            var error = CurrentPrescription.CheckPrescriptionRule();
            if (!string.IsNullOrEmpty(error))
            {
                MessageWindow.ShowMessage(error,MessageType.ERROR);
            }
            else
            {
                CurrentPrescription.Id = CurrentPrescription.InsertPresription();
                CurrentPrescription.ProcessInventory();
                CurrentPrescription.ProcessEntry();
                CurrentPrescription.ProcessCashFlow();
            }
        }
        private void ExecuteRegisterButtonClick()
        {

        }
        private void ExecutePrescribeButtonClick()
        {

        }
        #endregion
        #region InitialFunctions
        private void InitializeVariables()
        {
            NotPrescribe = true;
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
        }
        private void InitialItemsSources()
        {
            Institutions = ViewModelMainWindow.Institutions;
            Divisions = ViewModelMainWindow.Divisions;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            PaymentCategories = ViewModelMainWindow.PaymentCategories;
            PrescriptionCases = ViewModelMainWindow.PrescriptionCases;
            Copayments = ViewModelMainWindow.Copayments;
            SpecialTreats = ViewModelMainWindow.SpecialTreats;
            Usages = ViewModelMainWindow.Usages;
            Positions = ViewModelMainWindow.Positions;
        }
        private void InitialCommandActions()
        {
            ShowCooperativeSelectionWindow = new RelayCommand(ExecuteShowCooperativeWindow);
            ShowCustomerSelectionWindow = new RelayCommand(ExecuteShowCustomerWindow);
            SearchCustomerByIDNumber = new RelayCommand(ExecuteSearchCustomerByIDNumber);
            SearchCustomerByName = new RelayCommand(ExecuteSearchCustomerByName);
            SearchCustomerByBirthday = new RelayCommand(ExecuteSearchCustomerByBirthday);
            SearchCustomerByTel = new RelayCommand(ExecuteSearchCustomerByTel);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ExecuteShowCommonInstitutionSelectionWindow);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ExecuteShowInstitutionSelectionWindow);
            GetDiseaseCodeById = new RelayCommand<string>(ExecuteGetDiseaseCodeById);
            AdjustCaseSelectionChanged = new RelayCommand(ExecuteAdjustCaseSelectionChanged);
            AddMedicine = new RelayCommand<string>(ExecuteAddMedicine);
            AdjustButtonClick = new RelayCommand(ExecuteAdjustButtonClick);
            RegisterButtonClick = new RelayCommand(ExecuteRegisterButtonClick);
            PrescribeButtonClick = new RelayCommand(ExecutePrescribeButtonClick);
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            Messenger.Default.Register<Prescription>(this, "SelectedPrescription", GetSelectedPrescription);
            Messenger.Default.Register<Institution>(this, "SelectedInstitution", GetSelectedInstitution);
            Messenger.Default.Register<ProductStruct>(this, "SelectedProduct", GetSelectedProduct);
        }
        #endregion
        #region EventAction
        private void ExecuteAdjustCaseSelectionChanged()
        {
            CheckDeclareStatus();
            if (CurrentPrescription.Treatment.AdjustCase != null &&
                CurrentPrescription.Treatment.AdjustCase.Id.Equals("0"))
            {
                NotPrescribe = false;
            }
            else
            {
                NotPrescribe = true;
            }
        }
        private void ExecuteGetDiseaseCodeById(string id)
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Treatment.MainDisease.FullName))
            {
                var tmpDisease = CurrentPrescription.Treatment.MainDisease.GetDataByCodeId(id);
                if (tmpDisease is null || string.IsNullOrEmpty(tmpDisease.Id))
                {
                    MessageWindow.ShowMessage("查無疾病代碼", MessageType.WARNING);
                }
                else
                {
                    CurrentPrescription.Treatment.MainDisease = tmpDisease;
                }
            }
            else
            {
                var tmpDisease = CurrentPrescription.Treatment.SubDisease.GetDataByCodeId(id);
                if (tmpDisease is null || string.IsNullOrEmpty(tmpDisease.Id))
                {
                    MessageWindow.ShowMessage("查無疾病代碼", MessageType.WARNING);
                }
                else
                {
                    CurrentPrescription.Treatment.SubDisease = tmpDisease;
                }
            }
        }

        private void GetSelectedProduct(ProductStruct selectedProduct)
        {
              CurrentPrescription.AddMedicineBySearch(selectedProduct.ID,SelectedMedicinesIndex);
            
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
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            CurrentPrescription.Treatment.Institution = receiveSelectedInstitution;
        }
        #endregion
        #region GeneralFunctions
        private void CheckDeclareStatus()
        {
            if(CurrentPrescription.Treatment.AdjustCase.Id.Equals("0"))
                DeclareStatus = PrescriptionDeclareStatus.Prescribe;
            else if (DateTime.Compare(CurrentPrescription.Treatment.AdjustDate.Date, DateTime.Today.Date) > 0)
                DeclareStatus = PrescriptionDeclareStatus.Register;
            else
            {
                DeclareStatus = PrescriptionDeclareStatus.Adjust;
            }
        }
        #endregion
    }
}
