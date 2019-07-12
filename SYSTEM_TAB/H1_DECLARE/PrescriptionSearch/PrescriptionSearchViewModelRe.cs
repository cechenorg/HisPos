using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModelRe : TabBase
    {
        public Collection<string> TimeIntervalTypes => new Collection<string> {"調劑日","登錄日","預約日"};
        public Collection<string> PatientConditions => new Collection<string> { "姓名", "身分證"};
        public Collection<string> MedicineConditions => new Collection<string> { "藥品代碼", "藥品名稱" };
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
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        private string selectedTimeIntervalType;
        public string SelectedTimeIntervalType
        {
            get => selectedTimeIntervalType;
            set
            {
                Set(() => SelectedTimeIntervalType, ref selectedTimeIntervalType, value);
            }
        }
        private string selectedPatientCondition;
        public string SelectedPatientCondition
        {
            get => selectedPatientCondition;
            set
            {
                Set(() => SelectedPatientCondition, ref selectedPatientCondition, value);
            }
        }
        private string patientCondition;
        public string PatientCondition
        {
            get => patientCondition;
            set
            {
                Set(() => PatientCondition, ref patientCondition, value);
            }
        }
        private string selectedMedicineCondition;
        public string SelectedMedicineCondition
        {
            get => selectedMedicineCondition;
            set
            {
                Set(() => SelectedMedicineCondition, ref selectedMedicineCondition, value);
            }
        }
        private string medicineCondition;
        public string MedicineCondition
        {
            get => medicineCondition;
            set
            {
                Set(() => MedicineCondition, ref medicineCondition, value);
            }
        }
        private DateTime? patientBirth;
        public DateTime? PatientBirth
        {
            get => patientBirth;
            set
            {
                Set(() => PatientBirth, ref patientBirth, value);
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
        private Division selectedDivision;
        public Division SelectedDivision
        {
            get => selectedDivision;
            set
            {
                Set(() => SelectedDivision, ref selectedDivision, value);
            }
        }
        private PrescriptionSearchPreview selectedPrescription;
        public PrescriptionSearchPreview SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
            }
        }
        private PrescriptionSearchPreview editedPrescription;
        public PrescriptionSearchPreview EditedPrescription
        {
            get => editedPrescription;
            set
            {
                Set(() => EditedPrescription, ref editedPrescription, value);
            }
        }
        public PrescriptionSearchInstitutions Institutions { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public Divisions Divisions { get; set; }
        private PrescriptionSearchPreviews searchPrescriptions;
        public PrescriptionSearchPreviews SearchPrescriptions
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
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private string selectedInstitutionCount;
        public string SelectedInstitutionCount
        {
            get => selectedInstitutionCount;
            set
            {
                Set(() => SelectedInstitutionCount, ref selectedInstitutionCount, value);
            }
        }
        public override TabBase getTab()
        {
            return this;
        }
        private BackgroundWorker worker;
        public RelayCommand FilterAdjustedInstitution { get; set; }
        public RelayCommand Search { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand GetNoBucklePrescriptions { get; set; }
        public PrescriptionSearchViewModelRe()
        {
            AdjustCases = new AdjustCases(false) { null };
            foreach (var adjust in ViewModelMainWindow.AdjustCases)
            {
                AdjustCases.Add(adjust);
            }
            Divisions = new Divisions { null };
            foreach (var division in ViewModelMainWindow.Divisions)
            {
                Divisions.Add(division);
            }
            InitCondition();
            FilterAdjustedInstitution = new RelayCommand(FilterAdjustedInstitutionAction);
            Search = new RelayCommand(SearchAction);
            Clear = new RelayCommand(ClearAction);
            GetNoBucklePrescriptions = new RelayCommand(GetNoBucklePrescriptionsAction);
        }
        private void SearchAction()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => { SearchByConditions(); };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                EndSearch();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ClearAction()
        {
            InitCondition();
        }

        private void EndSearch()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions};
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
        }

        private void SearchByConditions()
        {
            BusyContent = "處方查詢中...";
            var selectedIns = Institutions.Where(i => i.Selected);
            var insIDList = selectedIns.Select(i => i.ID).ToList();
            var conditionTypes = new Dictionary<string, string>
            {
                {"TimeInterval", SelectedTimeIntervalType},
                {"Patient", SelectedPatientCondition},
                {"Medicine", SelectedMedicineCondition}
            };
            var conditions = new Dictionary<string, string>
            {
                {"Patient", PatientCondition},
                {"Medicine", MedicineCondition}
            };
            var dates = new Dictionary<string, DateTime?>
            {
                {"sDate", StartDate}, {"eDate", EndDate}, {"PatientBirthday", PatientBirth}
            };
            SearchPrescriptions = new PrescriptionSearchPreviews();
            MainWindow.ServerConnection.OpenConnection();
            SearchPrescriptions.GetSearchPrescriptionsRe(conditionTypes, conditions, dates,  SelectedAdjustCase,insIDList, SelectedDivision);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void FilterAdjustedInstitutionAction()
        {
            var insFilter = new AdjustedInstitutionSelectionWindow.AdjustedInstitutionSelectionWindow(Institutions);
            var selectCount = Institutions.Count(i => i.Selected);
            if (selectCount <= 3)
            {
                SelectedInstitutionCount = string.Empty;
                foreach (var ins in Institutions.Where(i => i.Selected))
                {
                    SelectedInstitutionCount += ins.Name.Length > 10 ? $"{ins.Name.Substring(0, 10)}... " : $"{ins.Name} " ;
                }
            }
            else
            {
                SelectedInstitutionCount = "已選 " + Institutions.Count(i => i.Selected) + " 間";
            }
        }

        private void GetNoBucklePrescriptionsAction()
        {
            throw new NotImplementedException();
        }

        private void InitCondition()
        {
            SearchPrescriptions = new PrescriptionSearchPreviews();
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions };
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            SelectedTimeIntervalType = TimeIntervalTypes[0];
            SelectedPatientCondition = PatientConditions[0];
            SelectedMedicineCondition = MedicineConditions[0];
            Institutions = new PrescriptionSearchInstitutions();
            Institutions.GetAdjustedInstitutions();
            SelectedInstitutionCount = "已選 " + Institutions.Count(i => i.Selected) + " 間";
            SelectedAdjustCase = AdjustCases[0];
            SelectedDivision = Divisions[0];
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }
    }
}
