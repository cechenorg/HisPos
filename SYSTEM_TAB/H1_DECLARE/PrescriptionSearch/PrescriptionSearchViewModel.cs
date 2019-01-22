using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.InstitutionSelectionWindow;
using MedicalPersonnel = His_Pos.NewClass.Person.MedicalPerson.MedicalPersonnel;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModel:ViewModelBase
    {
        #region Variables
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public Institutions Institutions { get; set; }
        public AdjustCases AdjustCases { get; set; }
        private Prescriptions searchPrescriptions;
        public Prescriptions SearchPrescriptions
        {
            get => searchPrescriptions;
            set
            {
                Set(() => SearchPrescriptions, ref searchPrescriptions, value);
            }
        }
        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private string patient;
        public string Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
            }
        }
        private MedicalPersonnel selectedSelectedPharmacist;
        public MedicalPersonnel SelectedPharmacist
        {
            get => selectedSelectedPharmacist;
            set
            {
                Set(() => SelectedPharmacist, ref selectedSelectedPharmacist, value);
            }
        }
        private AdjustCase selectedAdjustCase;
        public AdjustCase SelectedAdjustCase
        {
            get => selectedAdjustCase;
            set
            {
                Set(() => SelectedAdjustCase, ref selectedAdjustCase, value);
            }
        }
        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set
            {
                Set(() => SelectedInstitution, ref selectedInstitution, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand Search { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand ImportDeclareFile { get; set; }
        #endregion
        public PrescriptionSearchViewModel()
        {
            InitialVariables();
            InitialCommands();
        }
        #region InitialFunctions
        private void InitialVariables()
        {
            SearchPrescriptions = new Prescriptions();
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            Institutions = ViewModelMainWindow.Institutions;
            AdjustCases = ViewModelMainWindow.AdjustCases;
        }
        private void InitialCommands()
        {
            Search = new RelayCommand(SearchAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(GetInstitutionAction);
            ImportDeclareFile = new RelayCommand(ImportDeclareFileAction);
        }
        #endregion
        #region CommandActions
        private void SearchAction()
        {
            //依條件查詢對應處方
            SearchPrescriptions.GetSearchPrescriptions(StartDate,EndDate,Patient,SelectedAdjustCase,SelectedInstitution,SelectedPharmacist);
        }
        private void GetInstitutionAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString + "4", MessageType.WARNING);
                return;
            }
            SelectedInstitution = null;
            var result = Institutions.Where(i => i.Id.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    SelectedInstitution = result[0];
                    break;
                default:
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ImportDeclareFileAction()
        {
            //匯入申報檔
        }
        #endregion
        #region Functions
        #endregion
    }
}
