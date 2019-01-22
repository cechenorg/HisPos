using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.InstitutionSelectionWindow;
using MedicalPersonnel = His_Pos.NewClass.Person.MedicalPerson.MedicalPersonnel;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModel : TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
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
        private CollectionViewSource prescriptionCollectionVS;
        private CollectionViewSource PrescriptionCollectionVS
        {
            get => prescriptionCollectionVS;
            set
            {
                Set(() => PrescriptionCollectionVS, ref prescriptionCollectionVS, value);
            }
        }

        private ICollectionView prescriptionCollectionView;
        public ICollectionView PrescriptionCollectionView
        {
            get => prescriptionCollectionView;
            set
            {
                Set(() => PrescriptionCollectionView, ref prescriptionCollectionView, value);
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
                UpdateFilter();
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
        private Prescription selectedPrescription;
        public Prescription SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand Search { get; set; }
        public RelayCommand ReserveSearch { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand ImportDeclareFile { get; set; }
        #endregion
        public PrescriptionSearchViewModel()
        {
            InitialVariables();
            InitialCommands();
            RegisterMessengers();
        }
        ~PrescriptionSearchViewModel()
        {
            Messenger.Default.Unregister(this);
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
            ReserveSearch = new RelayCommand(ReserveSearchAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(GetInstitutionAction);
            ImportDeclareFile = new RelayCommand(ImportDeclareFileAction);
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<Institution>(this, "SelectedInstitution", GetSelectedInstitution);
        }
        #endregion
        #region CommandActions
        private void SearchAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            //依條件查詢對應處方
            SearchPrescriptions.GetSearchPrescriptions(StartDate,EndDate,SelectedAdjustCase,SelectedInstitution,SelectedPharmacist);
            MainWindow.ServerConnection.CloseConnection();
            UpdateCollectionView();
        }

        private void ReserveSearchAction()
        {
            //查詢預約慢箋
            SearchPrescriptions.GetReservePrescription();
            UpdateCollectionView();
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
        private void UpdateCollectionView()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions };
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            PrescriptionCollectionVS.Filter += FilterByPatient;
            if (PrescriptionCollectionView.IsEmpty) return;
            PrescriptionCollectionView.MoveCurrentToFirst();
            SelectedPrescription = (Prescription)PrescriptionCollectionView.CurrentItem;
        }
        private void UpdateFilter()
        {
            if (PrescriptionCollectionVS is null) return;
            if (string.IsNullOrEmpty(patient))
                PrescriptionCollectionVS.Filter -= FilterByPatient;
            else
                PrescriptionCollectionVS.Filter += FilterByPatient;
        }
        private void GetSelectedInstitution(Institution ins)
        {
            SelectedInstitution = ins;
        }
        #endregion
        #region Filters
        private void FilterByPatient(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (string.IsNullOrEmpty(Patient))
                e.Accepted = true;
            else
            {
                if (src.Patient.Name.Contains(Patient) || src.Patient.IDNumber.Contains(Patient))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }
        #endregion
    }
}
