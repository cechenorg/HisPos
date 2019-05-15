using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.PrescriptionRefactoring;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

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
        public Prescription CurrentPrescription { get; set; }
        #endregion

        #region Commands
        public RelayCommand GetCooperativePres { get; set; }
        public RelayCommand GetPatientData { get; set; }
        public RelayCommand GetCustomers { get; set; }
        public RelayCommand GetInstitution { get; set; }
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
            GetCustomers = new RelayCommand(GetCustomersAction);
            GetInstitution = new RelayCommand(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
        }

        #endregion

        #region CommandAction
        private void GetCooperativePresAction()
        {
            //查詢合作診所處方
        }

        private void GetPatientDataAction()
        {
            //取得病患資料(讀卡)
        }

        private void GetCustomersAction()
        {
            //顧客查詢
        }

        private void GetInstitutionAction()
        {
            //院所查詢
        }

        private void GetCommonInstitutionAction()
        {
            //常用院所查詢
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
                    if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEmptyOrEquals(parameters))
                        Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                    disease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (disease == null) return;
                    CurrentPrescription.Treatment.MainDisease = disease;
                    break;
                case "SecondDiagnosis":
                    if (string.IsNullOrEmpty(diseaseID) || CurrentPrescription.CheckDiseaseEmptyOrEquals(parameters))
                        Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                    disease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    if (disease == null) return;
                    CurrentPrescription.Treatment.SubDisease = disease;
                    break;
            }
        }
        #endregion
    }
}
