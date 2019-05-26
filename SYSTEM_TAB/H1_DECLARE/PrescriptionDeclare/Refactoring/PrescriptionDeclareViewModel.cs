using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.PrescriptionRefactoring;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring
{
    public class PrescriptionDeclareViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region ItemsSource
        public Divisions Divisions { get; set; }
        public MedicalPersonnels MedicalPersonnels { get; set; }
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
        #region Variables
        private Prescription currentPrescription;
        public Prescription CurrentPrescription
        {
            get => currentPrescription;
            set
            {
                Set(() => CurrentPrescription, ref currentPrescription, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand<TextBox> GetCustomers { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> DiseaseCodeTextChanged { get; set; }
        #endregion
        public PrescriptionDeclareViewModel()
        {
            Init();
        }

        #region InitFunctions
        private void Init()
        {
            InitItemsSource();
            InitVariables();
            InitCommands();
        }

        private void InitItemsSource()
        {
            Divisions = VM.Divisions;
            MedicalPersonnels = new MedicalPersonnels(MedicalPersonnelInitType.Enable);
            AdjustCases = VM.AdjustCases;
            PaymentCategories = VM.PaymentCategories;
            PrescriptionCases = VM.PrescriptionCases;
            Copayments = VM.Copayments;
            SpecialTreats = VM.SpecialTreats;
            MainWindow.ServerConnection.OpenConnection();
            MedicineSets = new MedicineSets();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void InitVariables()
        {
            CurrentPrescription = new Prescription();
        }

        private void InitCommands()
        {
            GetCooperativePres = new RelayCommand(GetCooperativePresAction);
            GetPatientData = new RelayCommand(GetPatientDataAction);
            GetCustomers = new RelayCommand<TextBox>(GetCustomersAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
        }

        #endregion

        #region CommandAction
        private void GetCooperativePresAction()
        {
            //查詢合作診所處方
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cooPresWindow = new CooperativePrescriptionWindow();
        }

        private void GetCustomerPrescription(NotificationMessage<Prescription> receiveMsg)
        {
            Messenger.Default.Unregister<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            CurrentPrescription = receiveMsg.Content;
        }

        private void GetPatientDataAction()
        {
            //取得病患資料(讀卡)
        }

        private void GetCustomersAction(TextBox condition)
        {
            //顧客查詢
            Messenger.Default.Register<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
            switch (condition.Name)
            {
                case "PatientBirthday" when CurrentPrescription.Patient.Birthday is null:
                    MessageWindow.ShowMessage("查詢生日不可為空", MessageType.WARNING);
                    break;
                case "PatientBirthday":
                    //var c = new CustomerSelectionWindow(DateTimeExtensions.NullableDateToTWCalender(CurrentPrescription.Patient.Birthday, false), 1);
                    break;
                case "PatientName" when string.IsNullOrEmpty(CurrentPrescription.Patient.Name):
                    MessageWindow.ShowMessage("查詢姓名不可為空", MessageType.WARNING);
                    break;
                case "PatientName":
                    //c = new CustomerSelectionWindow(CurrentPrescription.Patient.Name, 2);
                    break;
                case "PatientIDNumber" when string.IsNullOrEmpty(CurrentPrescription.Patient.IDNumber):
                    MessageWindow.ShowMessage("查詢身分證不可為空", MessageType.WARNING);
                    break;
                case "PatientIDNumber":
                    //c = new CustomerSelectionWindow(CurrentPrescription.Patient.IDNumber, 3);
                    break;
                case "PatientTel" when string.IsNullOrEmpty(CurrentPrescription.Patient.Tel):
                    MessageWindow.ShowMessage("查詢電話不可為空", MessageType.WARNING);
                    break;
                case "PatientTel":
                    //c = new CustomerSelectionWindow(CurrentPrescription.Patient.Name, 4);
                    break;
            }
        }

        private void GetInstitutionAction(string insID)
        {
            //院所查詢
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionDeclareViewModel) + "InsSelected", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var institutionSelectionWindow = new InstitutionSelectionWindow(insID, ViewModelEnum.PrescriptionDeclare);
        }

        private void GetCommonInstitutionAction()
        {
            //常用院所查詢
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionDeclareViewModel) + "InsSelected", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var commonInsSelectionWindow = new CommonHospitalsWindow(ViewModelEnum.PrescriptionDeclare);
        }
        private void GetDiseaseCodeAction(object o)
        {
            var parameters = o.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            DiseaseCode disease;
            //診斷碼查詢
            switch (elementName)
            {
                case "MainDiagnosis":
                    if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEquals(parameters))
                    {
                        Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                        return;
                    }
                    disease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (disease == null) return;
                    CurrentPrescription.MainDisease = disease;
                    break;
                case "SecondDiagnosis":
                    if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEquals(parameters))
                    {
                        Messenger.Default.Send(new NotificationMessage(this, "FocusChronicTotal"));
                        return;
                    }
                    disease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (disease == null) return;
                    CurrentPrescription.SubDisease = disease;
                    break;
            }
        }
        #endregion

        #region MessengerFunctions
        private void GetSelectedCustomer(Customer receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<Customer>(this, "SelectedCustomer", GetSelectedCustomer);
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
            Messenger.Default.Unregister<Institution>(this, nameof(PrescriptionDeclareViewModel) + "InsSelected", GetSelectedInstitution);
            CurrentPrescription.Institution = receiveSelectedInstitution;
        }
        #endregion

        #region Functions
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
            //Messenger.Default.Register<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            //Messenger.Default.Register<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            //var cusPreSelectWindow = new CusPreSelectWindow(CurrentPrescription.Patient.ID, CurrentPrescription.Patient.IDNumber, CurrentPrescription.Card);
        }
        #endregion
    }
}
