using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
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
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using Resources = His_Pos.Properties.Resources;
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
        #endregion
        #region Commands
        public RelayCommand<TextBox> GetCustomers { get; set; }
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
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
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    BusyContent = Resources.讀取健保卡;
                    MainWindow.ServerConnection.OpenConnection();
                    CurrentPrescription.ReadCard();
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
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void GetCustomersAction(TextBox condition)
        {
            //顧客查詢
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

        private void GetInstitutionAction(string insID)
        {
            //院所查詢
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var institutionSelectionWindow = new InstitutionSelectionWindow(insID);
        }

        private void GetCommonInstitutionAction()
        {
            //常用院所查詢
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            CurrentPrescription.Institution = new Institution();
            var commonInsSelectionWindow = new CommonHospitalsWindow();
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
            Messenger.Default.Register<NotificationMessage<Prescription>>("CustomerPrescriptionSelected", GetCustomerPrescription);
            var cusPreWindow = new CustomerPrescriptionWindow(CurrentPrescription.Patient, CurrentPrescription.Card);
            //Messenger.Default.Register<CustomPrescriptionStruct>(this, "PrescriptionSelected", GetSelectedPrescription);
            //Messenger.Default.Register<NotificationMessage<Prescription>>("CooperativePrescriptionSelected", GetCooperativePrescription);
            //var cusPreSelectWindow = new CusPreSelectWindow(CurrentPrescription.Patient.ID, CurrentPrescription.Patient.IDNumber, CurrentPrescription.Card);
        }
        #endregion
    }
}
