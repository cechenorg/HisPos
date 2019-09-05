﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.WareHouse;
using His_Pos.Properties;
using His_Pos.Service;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModel : TabBase
    {
        #region Properties

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
                RaisePropertyChanged("NoBuckleFilterEnable");
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
                if (string.IsNullOrEmpty(value.ID))
                    value = null;
                Set(() => SelectedAdjustCase, ref selectedAdjustCase, value);
            }
        }
        private Division selectedDivision;
        public Division SelectedDivision
        {
            get => selectedDivision;
            set
            {
                if (string.IsNullOrEmpty(value.ID))
                    value = null;
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
        private int? editedPrescription;
        public int? EditedPrescription
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
        private int totalCount;
        public int TotalCount
        {
            get => totalCount;
            set
            {
                Set(() => TotalCount, ref totalCount, value);
            }
        }
        private int chronicCount;
        public int ChronicCount
        {
            get => chronicCount;
            set
            {
                Set(() => ChronicCount, ref chronicCount, value);
            }
        }
        private int totalPoint;
        public int TotalPoint
        {
            get => totalPoint;
            set
            {
                Set(() => TotalPoint, ref totalPoint, value);
            }
        }
        private int medicinePoint;
        public int MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }
        private int medicalServicePoint;
        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }
        private int copaymentPoint;
        public int CopaymentPoint
        {
            get => copaymentPoint;
            set
            {
                Set(() => CopaymentPoint, ref copaymentPoint, value);
            }
        }
        private int applyPoint;
        public int ApplyPoint
        {
            get => applyPoint;
            set
            {
                Set(() => ApplyPoint, ref applyPoint, value);
            }
        }

        public bool NoBuckleFilterEnable => SelectedTimeIntervalType.Equals("調劑日");

        private bool filterNoBuckle;
        public bool FilterNoBuckle
        {
            get => filterNoBuckle;
            set
            {
                Set(() => FilterNoBuckle, ref filterNoBuckle, value);
                if (PrescriptionCollectionVS != null)
                    PrescriptionCollectionVS.Filter += NoBuckleFilter;
            }
        }
        #endregion
        #region Commands

        public RelayCommand FilterAdjustedInstitution { get; set; }
        public RelayCommand Search { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand ShowPrescriptionEdit { get; set; }
        public RelayCommand ExportPrescriptionCsv { get; set; }
        public RelayCommand ExportMedicineCsv { get; set; }
        #endregion
        public PrescriptionSearchViewModel()
        {
            InitProperties();
            InitCondition();
            InitCommand();
        }

        #region InitFunctions
        private void InitProperties()
        {
            AdjustCases = new AdjustCases(false) { new AdjustCase() };
            foreach (var adjust in ViewModelMainWindow.AdjustCases)
            {
                AdjustCases.Add(adjust);
            }
            Divisions = new Divisions { new Division() };
            foreach (var division in ViewModelMainWindow.Divisions)
            {
                Divisions.Add(division);
            }
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
            TotalCount = 0;
            ChronicCount = 0;
            TotalPoint = 0;
            MedicinePoint = 0;
            MedicalServicePoint = 0;
            CopaymentPoint = 0;
            ApplyPoint = 0;
            MedicineCondition = string.Empty;
            PatientCondition = string.Empty;
            PatientBirth = null;
        }

        private void InitCommand()
        {
            Search = new RelayCommand(SearchAction);
            Clear = new RelayCommand(ClearAction);
            FilterAdjustedInstitution = new RelayCommand(FilterAdjustedInstitutionAction);
            ShowPrescriptionEdit = new RelayCommand(ShowPrescriptionEditAction);
            ExportPrescriptionCsv = new RelayCommand(ExportPrescriptionCsvAction);
            ExportMedicineCsv = new RelayCommand(ExportMedicineCsvAction);
        }
        #endregion

        #region CommandAction

        private void SearchAction()
        {
            if (CheckSearchConditionsEmpty())
            {
                MessageWindow.ShowMessage("請至少填寫一個查詢條件",MessageType.WARNING);
                return;
            }
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

        private bool CheckSearchConditionsEmpty()
        {
            return (StartDate is null || EndDate is null) && string.IsNullOrEmpty(PatientCondition) &&
                   PatientBirth is null && string.IsNullOrEmpty(MedicineCondition) && string.IsNullOrEmpty(SelectedAdjustCase?.ID) &&
                   string.IsNullOrEmpty(SelectedDivision?.ID);
        }

        private void ClearAction()
        {
            InitCondition();
            StartDate = null;
            EndDate = null;
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
                    SelectedInstitutionCount += ins.Name.Length > 10 ? $"{ins.Name.Substring(0, 10)}... " : $"{ins.Name} ";
                }
            }
            else
            {
                SelectedInstitutionCount = "已選 " + Institutions.Count(i => i.Selected) + " 間";
            }
        }

        private void ShowPrescriptionEditAction()
        {
            if (SelectedPrescription is null) return;
            EditedPrescription = SelectedPrescription.ID;
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            PrescriptionService.ShowPrescriptionEditWindow(SelectedPrescription.ID, SelectedPrescription.Type);
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }

        private void ExportPrescriptionCsvAction()
        {
            var fileDialog = CreatePrescriptionExportFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.DeclareXmlPath = fileDialog.FileName;
                Settings.Default.Save();
                StartExportPrescriptionCsv(fileDialog);
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
            }
        }

        private void StartExportPrescriptionCsv(SaveFileDialog fileDialog)
        {
            using (var file = new StreamWriter(fileDialog.FileName, false, Encoding.UTF8))
            {
                file.WriteLine("調劑狀態,藥袋狀態,醫療院所,科別,病患姓名,就醫序號,身分證,生日,處方就醫日,處方調劑日,實際調劑日,登錄日");
                foreach (var s in SearchPrescriptions)
                {
                    var insName = s.Institution is null ? "" : s.Institution.Name;
                    var divName = s.Division is null ? "" : s.Division.Name;
                    var sAdjust = s.IsAdjust ? "已調劑" : "未調劑";
                    var adjDate = DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(s.AdjustDate);
                    var treatDate = s.TreatDate is null ? "" : ((DateTime)s.TreatDate).AddYears(-1911).ToString("yyy/MM/dd");
                    Debug.Assert(s.Patient.Birthday != null, "s.Patient.Birthday != null");
                    file.WriteLine($"{sAdjust},{s.StoStatus},{insName}," +
                            $"{divName},{s.Patient.Name},{s.MedicalNumber},{s.Patient.IDNumber}," +
                            $"{((DateTime)s.Patient.Birthday).AddYears(-1911):yyy/MM/dd}," +
                            $"{treatDate}," + $"{adjDate},{s.InsertDate},{s.RegisterDate}");
                }
                file.Close();
                file.Dispose();
            }
        }

        private SaveFileDialog CreatePrescriptionExportFileDialog()
        {
            return new SaveFileDialog
            {
                Title = Resources.處方存檔,
                InitialDirectory = string.IsNullOrEmpty(Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath,
                Filter = Resources.PrescriptionFileType,
                FileName = DateTime.Today.ToString("yyyyMMdd") + Resources.處方存檔,
                FilterIndex = 2,
                RestoreDirectory = true
            };
        }

        private void ExportMedicineCsvAction()
        {
            var idList = CreatePrescriptionIDList();
            var fileDialog = CreateMedicineExportFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.DeclareXmlPath = fileDialog.FileName;
                Settings.Default.Save();
                try
                {
                    StartExportMedicineCsv(fileDialog, idList);
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }

            }
        }

        private void StartExportMedicineCsv(SaveFileDialog fileDialog, List<int> idList)
        {
            using (var file = new StreamWriter(fileDialog.FileName, false, Encoding.UTF8))
            {
                var wareHouses = WareHouses.GetWareHouses();
                foreach (var w in wareHouses)
                {
                    file.WriteLine("庫名," + w.Name);
                    file.WriteLine("商品代碼,藥品中文名稱,藥品英文名稱,上次進價,健保價,庫存,調劑量,扣庫量");
                    var table = MedicineDb.GetPrescriptionMedicineSumById(idList, w.ID);
                    foreach (DataRow s in table.Rows)
                    {
                        file.WriteLine($@"{s.Field<string>("Pro_ID")},{s.Field<string>("cName")},{s.Field<string>("eName")},{s.Field<int>("Pro_LastPrice")}, {s.Field<int>("Med_Price")},{s.Field<double>("Inv_Inventory")},{s.Field<int>("TotalAmount")},{s.Field<int>("BuckleAmount")}");
                    }
                    file.WriteLine();
                    file.WriteLine();
                    file.WriteLine();
                }
                file.Close();
                file.Dispose();
            }
        }

        private SaveFileDialog CreateMedicineExportFileDialog()
        {
            return new SaveFileDialog
            {
                Title = Resources.藥品統計存檔,
                InitialDirectory = string.IsNullOrEmpty(Settings.Default.DeclareXmlPath) ? @"c:\" : Settings.Default.DeclareXmlPath,
                Filter = Resources.CSVFileType,
                FileName = DateTime.Today.ToString("yyyyMMdd") + Resources.藥品統計存檔,
                FilterIndex = 2,
                RestoreDirectory = true
            };
        }

        private List<int> CreatePrescriptionIDList()
        {
            var idList = new List<int>();
            foreach (var a in SearchPrescriptions)
            {
                idList.Add(a.ID);
            }
            return idList;
        }

        #endregion

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
            SearchPrescriptions.GetSearchPrescriptionsRe(conditionTypes, conditions, dates, SelectedAdjustCase, insIDList, SelectedDivision);
            SetPrescriptionsSummary();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void EndSearch()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions};
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            TotalCount = SearchPrescriptions.Count;
            ChronicCount = SearchPrescriptions.Count(p => p.AdjustCase.ID.Equals("2"));
            if (PrescriptionCollectionVS != null)
                PrescriptionCollectionVS.Filter += NoBuckleFilter;
            if (EditedPrescription != null && SearchPrescriptions.SingleOrDefault(p => p.ID.Equals(EditedPrescription)) != null)
            {
                PrescriptionCollectionView.MoveCurrentTo(SearchPrescriptions.SingleOrDefault(p => p.ID.Equals(EditedPrescription)));
            }
        }

        private void Refresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals("PrescriptionEdited"))
                SearchAction();
        }

        private void SetPrescriptionsSummary()
        {
            var summary = SearchPrescriptions.GetSummary();
            TotalCount = SearchPrescriptions.Count;
            ChronicCount = SearchPrescriptions.Count(p => p.AdjustCase.ID.Equals("2"));
            MedicinePoint = summary[0];
            CopaymentPoint = summary[1];
            MedicalServicePoint = summary[2];
            ApplyPoint = summary[3];
            TotalPoint = summary[4];
        }

        private void NoBuckleFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchPreview src))
                e.Accepted = false;
            else
            {
                if (FilterNoBuckle)
                {
                    if (src.NoBuckleStatus != null && src.NoBuckleStatus.Equals(1))
                        e.Accepted = true;
                    else
                        e.Accepted = false;
                }
                else
                    e.Accepted = true;
            }
        }
    }
}
