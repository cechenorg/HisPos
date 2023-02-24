using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Prescription.ICCard.Upload;
using His_Pos.NewClass.Prescription.ImportDeclareXml;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.WareHouse;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModel : TabBase
    {
        #region Properties

        public Collection<string> TimeIntervalTypes => new Collection<string> { "調劑日", "登錄日" };
        public Collection<string> PatientConditions => new Collection<string> { "姓名", "身分證" };
        public Collection<string> MedicineConditions => new Collection<string> { "藥品代碼", "藥品名稱" };
        private PrescriptionType searchType { get; set; }
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
                if (!selectedTimeIntervalType.Equals("調劑日"))
                {
                    FilterNoBuckle = false;
                    FilterNotAdjust = false;
                    FilterNoGetCard = false;
                }
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
                {
                    PrescriptionCollectionVS.Filter += NoBuckleFilter;
                    SetPrescriptionsSummary();
                }
            }
        }

        private bool filterNotAdjust;

        public bool FilterNotAdjust
        {
            get => filterNotAdjust;
            set
            {
                Set(() => FilterNotAdjust, ref filterNotAdjust, value);
                if (PrescriptionCollectionVS != null)
                {
                    PrescriptionCollectionVS.Filter += NotAdjustFilter;
                    SetPrescriptionsSummary();
                }
            }
        }

        private bool filterNoGetCard;

        public bool FilterNoGetCard
        {
            get => filterNoGetCard;
            set
            {
                Set(() => FilterNoGetCard, ref filterNoGetCard, value);
                if (PrescriptionCollectionVS != null)
                {
                    PrescriptionCollectionVS.Filter += NoGetCardFilter;
                    SetPrescriptionsSummary();
                }
            }
        }

        public bool IsNotBuckle = false;

        #endregion Properties

        #region Commands

        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }
        public RelayCommand FilterAdjustedInstitution { get; set; }
        public RelayCommand Search { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand ShowPrescriptionEdit { get; set; }
        public RelayCommand ExportPrescriptionCsv { get; set; }
        public RelayCommand ExportMedicineCsv { get; set; }
        public RelayCommand SearchNotBuckleCommad { get; set; }
        public RelayCommand ImportDeclareFileCommand { get; set; }
        public RelayCommand GetIcData2Commad { get; set; }
        #endregion Commands

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
            ImportDeclareFileCommand = new RelayCommand(ImportDeclareFileAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            Search = new RelayCommand(SearchAction);
            Clear = new RelayCommand(ClearAction);
            FilterAdjustedInstitution = new RelayCommand(FilterAdjustedInstitutionAction);
            ShowPrescriptionEdit = new RelayCommand(ShowPrescriptionEditAction);
            ExportPrescriptionCsv = new RelayCommand(ExportPrescriptionCsvAction);
            ExportMedicineCsv = new RelayCommand(ExportMedicineCsvAction);
            SearchNotBuckleCommad = new RelayCommand(SearchNotBuckleAction);
            GetIcData2Commad = new RelayCommand(GetIcData2Action);
        }

        private void ImportDeclareFileAction()
        {

            var precriptionList = new List<ImportDeclareXml.Ddata>();
            var innerXmlList = new List<string>();
           

            List<ImportDeclareXml.Ddata> users;


            XmlDocument doc = new XmlDocument();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//該值確定是否可以選擇多個檔案
            dialog.Title = "請選擇資料夾";
            dialog.Filter = "所有檔案(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                doc.Load(file);
            }
            else {
                return;
            }
            
            
            XmlNodeReader xmlNodeReader = new XmlNodeReader(doc);
            var nodes = doc.SelectNodes("pharmacy//ddata");
           
            foreach (XmlNode node in nodes)
            {
                string innerXml = $"{node.InnerXml}";
                  
                var dictPreMas = new His_Pos.NewClass.Prescription.ImportDeclareXml.ImportDeclareXml.Ddata();
                dictPreMas.D1 = node.SelectSingleNode("dhead/d1")?.InnerText ?? "";
                dictPreMas.D2 = node.SelectSingleNode("dhead/d2")?.InnerText ?? "";
                dictPreMas.D3 = node.SelectSingleNode("dhead/d3")?.InnerText ?? "";
                dictPreMas.D4 = node.SelectSingleNode("dhead/d4")?.InnerText ?? "";
                dictPreMas.D5 = node.SelectSingleNode("dhead/d5")?.InnerText ?? "";
                dictPreMas.D6 = node.SelectSingleNode("dhead/d6")?.InnerText ?? "";
                dictPreMas.D7 = node.SelectSingleNode("dhead/d7")?.InnerText ?? "";
                dictPreMas.D8 = node.SelectSingleNode("dhead/d8")?.InnerText ?? "";
                dictPreMas.D9 = node.SelectSingleNode("dhead/d9")?.InnerText ?? "";


                dictPreMas.D13 = node.SelectSingleNode("dhead/d13")?.InnerText ?? "";
                dictPreMas.D14 = node.SelectSingleNode("dhead/d14")?.InnerText ?? "";
                dictPreMas.D15 = node.SelectSingleNode("dhead/d15")?.InnerText ?? "";
                dictPreMas.D16 = node.SelectSingleNode("dhead/d16")?.InnerText ?? "";
                dictPreMas.D17 = node.SelectSingleNode("dhead/d17")?.InnerText ?? "";
                dictPreMas.D18 = node.SelectSingleNode("dhead/d18")?.InnerText ?? "";

                dictPreMas.D20 = node.SelectSingleNode("dhead/d20")?.InnerText ?? "";
                dictPreMas.D21 = node.SelectSingleNode("dhead/d21")?.InnerText ?? "";
                dictPreMas.D22 = node.SelectSingleNode("dhead/d22").InnerText ?? "";
                dictPreMas.D23 = node.SelectSingleNode("dhead/d23")?.InnerText ?? "";
                dictPreMas.D24 = node.SelectSingleNode("dhead/d24")?.InnerText ?? "";
                dictPreMas.D25 = node.SelectSingleNode("dhead/d25")?.InnerText ?? "";
                dictPreMas.D26 = node.SelectSingleNode("dhead/d26")?.InnerText ?? "";

                dictPreMas.D30 = node.SelectSingleNode("dbody/d30")?.InnerText ?? "";
                dictPreMas.D31 = node.SelectSingleNode("dbody/d31")?.InnerText ?? "";
                dictPreMas.D32 = node.SelectSingleNode("dbody/d32")?.InnerText ?? "";
                dictPreMas.D33 = node.SelectSingleNode("dbody/d33")?.InnerText ?? "";



                dictPreMas.D35 = node.SelectSingleNode("dbody/d35")?.InnerText ?? "";
                dictPreMas.D36 = node.SelectSingleNode("dbody/d36")?.InnerText ?? "";
                dictPreMas.D37 = node.SelectSingleNode("dbody/d37")?.InnerText ?? "";
                dictPreMas.D38 = node.SelectSingleNode("dbody/d38")?.InnerText ?? "";
                dictPreMas.D43 = node.SelectSingleNode("dbody/d43")?.InnerText ?? "";
                dictPreMas.D44 = node.SelectSingleNode("dbody/d44")?.InnerText ?? "";
                dictPreMas.Pdatas = new List<ImportDeclareXml.Pdata>();
                var nodess = node.SelectNodes("dbody/pdata");
                foreach (XmlNode nodesss in nodess) {
                    var dictPreDetail = new His_Pos.NewClass.Prescription.ImportDeclareXml.ImportDeclareXml.Pdata();
                    dictPreDetail.P1= nodesss.SelectSingleNode("p1")?.InnerText ?? "";
                    dictPreDetail.P2 = nodesss.SelectSingleNode("p2")?.InnerText ?? "";
                    dictPreDetail.P3 = nodesss.SelectSingleNode("p3")?.InnerText ?? "";
                    dictPreDetail.P4 = nodesss.SelectSingleNode("p4")?.InnerText ?? "";
                    dictPreDetail.P5 = nodesss.SelectSingleNode("p5")?.InnerText ?? "";
                    dictPreDetail.P6 = nodesss.SelectSingleNode("p6")?.InnerText ?? "";
                    dictPreDetail.P7 = nodesss.SelectSingleNode("p7")?.InnerText ?? "";
                    dictPreDetail.P8 = nodesss.SelectSingleNode("p8")?.InnerText ?? "";
                    dictPreDetail.P9 = nodesss.SelectSingleNode("p9")?.InnerText ?? "";
                    dictPreDetail.P10 = nodesss.SelectSingleNode("p10")?.InnerText ?? "";
                    dictPreDetail.P11 = nodesss.SelectSingleNode("p11")?.InnerText ?? "";
                    dictPreDetail.P12 = nodesss.SelectSingleNode("p12")?.InnerText ?? "";
                    dictPreDetail.P13 = nodesss.SelectSingleNode("p13")?.InnerText ?? "";
                    dictPreDetail.P15 = nodesss.SelectSingleNode("p15")?.InnerText ?? "";

                    dictPreMas.Pdatas.Add(dictPreDetail);
                }
                innerXmlList.Add(innerXml);
                precriptionList.Add(dictPreMas); 
            }
             

            MainWindow.ServerConnection.OpenConnection();

            //判斷申報檔重複
            DataTable currentPreMas = PrescriptionDb.GetPrescriptionForImportXml();



            int totalPrescriptionCount = precriptionList.Count;
            foreach (DataRow currentPrescription in currentPreMas.Rows)
            {
                string cusIDnumber = currentPrescription["Cus_IDNumber"].ToString();
                DateTime treatmentDate = currentPrescription.Field<DateTime>("PreMas_TreatmentDate");
                string divisionID = currentPrescription["PreMas_DivisionID"].ToString();


                for (int k = 0; k < precriptionList.Count; k++)
                {
                    int year = Convert.ToInt32(precriptionList[k].D14.Substring(0, 3)) + 1911;
                    int month = Convert.ToInt32(precriptionList[k].D14.Substring(3, 2));
                    int day = Convert.ToInt32(precriptionList[k].D14.Substring(5, 2));

                    DateTime tempTreatmentDate = new DateTime(year, month, day);

                    if (precriptionList[k].D3 == cusIDnumber && precriptionList[k].D13 == divisionID &&
                        tempTreatmentDate == treatmentDate)
                    {
                        precriptionList.Remove(precriptionList[k--]); 
                    }
                } 
            }
            int notRepeatPrescriptionCount = precriptionList.Count;

            if (precriptionList.Count == 0)
            {
                MessageWindow.ShowMessage("此申報檔全部處方重複!", MessageType.ERROR);
                MainWindow.ServerConnection.CloseConnection();
                return;
            }


            PrescriptionDb.ImportDeclareXml(precriptionList, innerXmlList, "1");
            //int j = 300;
            //for (int ii = 0; ii < dict.Count; ii += 300)
            //{
            //    List<ImportDeclareXml.Ddata> cList = new List<ImportDeclareXml.Ddata>();
            //    cList = dict.Take(j).Skip(ii).ToList();
            //    List<string> cList2 = new List<string>();
            //    cList2 = dict2.Take(j).Skip(ii).ToList();

            //    j += 300; 
            //    PrescriptionDb.ImportDeclareXml(dict, dict2, "1"); 
            //}
            
            MainWindow.ServerConnection.CloseConnection();

            MessageWindow.ShowMessage($@"欲匯入{totalPrescriptionCount}張處方箋
            重複處方張數:{totalPrescriptionCount - notRepeatPrescriptionCount}
            成功匯入處方張數:{notRepeatPrescriptionCount}", MessageType.SUCCESS);
        }

        #endregion InitFunctions

        #region CommandAction

        private void DateMouseDoubleClickAction(MaskedTextBox sender)
        {
            switch (sender.Name)
            {
                case "StartDate":
                    StartDate = DateTime.Today;
                    break;

                case "EndDate":
                    EndDate = DateTime.Today;
                    break;
            }
        }

        private void SearchAction()
        {
            IsNotBuckle = false;
            if (CheckSearchConditionsEmpty())
            {
                MessageWindow.ShowMessage("請至少填寫一個查詢條件", MessageType.WARNING);
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

        private void SearchNotBuckleAction()
        {
            IsNotBuckle = true;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => { SearchNotBuckle(); };
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
            
            var insFilter = new AdjustedInstitutionSelectionWindow.AdjustedInstitutionSelectionWindow(Institutions );
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

            if(Institutions.IsNeedReSearch)
                SearchAction();
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
                file.WriteLine("調劑狀態,藥袋狀態,醫療院所,科別,病患姓名,就醫序號,身分證,生日,處方就醫日,處方調劑日,實際調劑日,登錄日,藥品費,特殊材料費,藥事服務費,部分負擔,總點數");
                foreach (var s in SearchPrescriptions)
                {
                    var insName = s.Institution is null ? "" : s.Institution.Name;
                    var divName = s.Division is null ? "" : s.Division.Name;
                    var sAdjust = s.IsAdjust ? "已調劑" : "未調劑";
                    var adjDate = DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(s.AdjustDate);
                    var treatDate = s.TreatDate is null ? "" : ((DateTime)s.TreatDate).AddYears(-1911).ToString("yyy/MM/dd");
                    //Debug.Assert(s.Patient.Birthday != null, "s.Patient.Birthday != null");
                    if (s.Patient.Birthday == null) {
                        s.Patient.Birthday = DateTime.Now.AddYears(100);
                    }

                    file.WriteLine($"{sAdjust},{s.StoStatus},{insName}," +
                            $"{divName},{s.Patient.Name},{s.MedicalNumber},{s.Patient.IDNumber}," +
                            $"{((DateTime)s.Patient.Birthday).AddYears(-1911):yyy/MM/dd}," +
                            $"{treatDate}," + $"{adjDate},{s.InsertDate},{s.RegisterDate},{s.MedicinePoint},{s.SpecialMaterialPoint},{s.MedicalServicePoint},{s.CopaymentPoint},{s.TotalPoint}");
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
                var wareHouses = new WareHouses(WareHouseDb.Init());
                foreach (var w in wareHouses)
                {
                    file.WriteLine("庫名," + w.Name);
                    file.WriteLine("商品代碼,藥品中文名稱,藥品英文名稱,上次進價,健保價,庫存,調劑量,扣庫量");
                    var table = MedicineDb.GetPrescriptionMedicineSumById(idList, w.ID);
                    foreach (DataRow s in table.Rows)
                    {
                        file.WriteLine($@"{s.Field<string>("Pro_ID")},{s.Field<string>("cName")},{s.Field<string>("eName")},{s.Field<int>("Inv_LastPrice")}, {s.Field<int>("Med_Price")},{s.Field<double>("Inv_Inventory")},{s.Field<int>("TotalAmount")},{s.Field<int>("BuckleAmount")}");
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

        #endregion CommandAction

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
            MainWindow.ServerConnection.CloseConnection();
            searchType = SelectedTimeIntervalType.Equals("預約日") ? PrescriptionType.ChronicReserve : PrescriptionType.Normal;
        }

        private void SearchNotBuckle()
        {
            BusyContent = "處方查詢中...";
            SearchPrescriptions = new PrescriptionSearchPreviews();
            MainWindow.ServerConnection.OpenConnection();
            SearchPrescriptions.GetNoBucklePrescriptions();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void EndSearch()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions };
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            TotalCount = SearchPrescriptions.Count;
            ChronicCount = SearchPrescriptions.Count(p => p.AdjustCase.ID.Equals("2"));
            if (PrescriptionCollectionVS != null && searchType.Equals(PrescriptionType.Normal))
            {
                PrescriptionCollectionVS.Filter += NoBuckleFilter;
                PrescriptionCollectionVS.Filter += NotAdjustFilter;
            }
            if (EditedPrescription != null && SearchPrescriptions.SingleOrDefault(p => p.ID.Equals(EditedPrescription)) != null)
            {
                PrescriptionCollectionView.MoveCurrentTo(SearchPrescriptions.SingleOrDefault(p => p.ID.Equals(EditedPrescription)));
            }
            SetPrescriptionsSummary();
        }

        private void Resort()
        {
            switch (SelectedTimeIntervalType)
            {
                case "調劑日":
                    PrescriptionCollectionVS.SortDescriptions.Add(new SortDescription("InsertDate", ListSortDirection.Descending));
                    PrescriptionCollectionVS.View.Refresh();
                    break;

                case "登錄日":
                    PrescriptionCollectionVS.SortDescriptions.Add(new SortDescription("RegisterDate", ListSortDirection.Descending));
                    PrescriptionCollectionVS.View.Refresh();
                    break;

                default:
                    PrescriptionCollectionVS.SortDescriptions.Add(new SortDescription("AdjustDate", ListSortDirection.Descending));
                    PrescriptionCollectionVS.View.Refresh();
                    break;
            }
        }

        private void Refresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals("PrescriptionEdited"))
            {
                if (IsNotBuckle)
                {
                    SearchNotBuckleAction();
                }
                else
                {
                    SearchAction();
                }
            }
        }

        private void SetPrescriptionsSummary()
        {
            var summary = searchType.Equals(PrescriptionType.ChronicReserve) ? GetReserveSummary() : GetSummary();
            TotalCount = PrescriptionCollectionView.Cast<object>().Count();
            ChronicCount = PrescriptionCollectionView.Cast<object>()
                .Count(p => ((PrescriptionSearchPreview)p).AdjustCase.ID.Equals("2"));
            MedicinePoint = summary[0];
            CopaymentPoint = summary[1];
            MedicalServicePoint = summary[2];
            ApplyPoint = summary[3];
            TotalPoint = summary[4];
            Resort();
        }

        private void NoBuckleFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchPreview src))
                e.Accepted = false;
            else
            {
                e.Accepted = (!FilterNoBuckle || src.NoBuckleStatus != null && src.NoBuckleStatus.Equals(1)) &&
                             (!FilterNoGetCard || src.IsDeposit);
            }
        }

        private void NotAdjustFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchPreview src))
                e.Accepted = false;
            else
            {
                e.Accepted = (FilterNotAdjust || src.IsAdjust) &&
                             (!FilterNoBuckle || src.NoBuckleStatus != null && src.NoBuckleStatus.Equals(1));
            }
        }

        private void NoGetCardFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchPreview src))
                e.Accepted = false;
            else
            {
                e.Accepted = !FilterNoGetCard || src.IsDeposit &&
                             (!FilterNoBuckle || src.NoBuckleStatus != null && src.NoBuckleStatus.Equals(1));
            }
        }

        public List<int> GetSummary()
        {
            var presID = (from object p in PrescriptionCollectionView select ((PrescriptionSearchPreview)p).ID).ToList();
            var table = PrescriptionDb.GetSearchPrescriptionsSummary(presID);
            return (from DataColumn c in table.Rows[0].Table.Columns select table.Rows[0].Field<int>(c.ColumnName)).ToList();
        }

        public List<int> GetReserveSummary()
        {
            var presID = (from object p in PrescriptionCollectionView select ((PrescriptionSearchPreview)p).ID).ToList();
            var table = PrescriptionDb.GetSearchReservesSummary(presID);
            return (from DataColumn c in table.Rows[0].Table.Columns select table.Rows[0].Field<int>(c.ColumnName)).ToList();
        }

        private void GetIcData2Action()
        {
            #region 2.0版本
            try
            {
                DataTable tbPre = PrescriptionDb.GetPrescriptionByID(SelectedPrescription.ID);
                Prescription p = new Prescription();

                if (tbPre != null && tbPre.Rows.Count > 0)
                {
                    bool isAdjust = Convert.ToBoolean(tbPre.Rows[0]["IsAdjust"]);
                    if(!isAdjust)
                    {
                        MessageWindow.ShowMessage("此筆處方未調劑\r\n無法匯出2.0XML檔案", MessageType.ERROR);
                        return;
                    }
                }

                foreach (DataRow dr in tbPre.Rows)
                {
                    p = new Prescription(dr, searchType);
                }

                if(string.IsNullOrEmpty(p.TreatmentCode))
                {
                    MessageWindow.ShowMessage("此筆處方無就醫識別碼\r\n無法匯出2.0XML檔案", MessageType.ERROR);
                    return;
                }

                p = GetIcCardData(p);

                var uploadData2 = HisApiFunction.GetIcData2(p, false);

                SaveFileDialog diag = new SaveFileDialog();
                diag.FileName = string.Format("{0}_{1}.xml", ViewModelMainWindow.CurrentPharmacy.ID, DateTime.Now.ToString("yyyyMMddHHmmss"));
                diag.Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*";
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(diag.FileName, uploadData2, Encoding.GetEncoding(950));
                }
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage(e.Message, MessageType.ERROR);
            }
            #endregion
        }
        private Prescription GetIcCardData(Prescription p)
        {
            p.Card.MedicalNumberData = new SeqNumber();
            List<string> listSign = new List<string>();
            string xml = PrescriptionDb.GetSignXml(p.ID);

            DataSet ds = CXmlToDataSet(xml);
            SeqNumber seq = new SeqNumber();
            foreach (DataTable tb in ds.Tables)
            {
                if(tb.Columns.Contains("A79"))
                {
                    foreach(DataRow dr in tb.Rows)
                    {
                        listSign.Add(Convert.ToString(dr["A79"]));
                    }
                }
                
                if (tb.Columns.Contains("A11"))//M02卡片號碼
                {
                    p.Card.CardNumber = Convert.ToString(tb.Rows[0]["A11"]);
                }
                if (tb.Columns.Contains("A12"))//M03身分證
                {
                    p.Card.IDNumber = Convert.ToString(tb.Rows[0]["A12"]);
                }
                if (tb.Columns.Contains("A13"))//M04生日
                {
                    p.Card.Birthday = DateTimeExtensions.TWDateStringToDateTime(Convert.ToString(tb.Rows[0]["A13"]));
                }
                if (tb.Columns.Contains("A14"))//M05醫療院所
                {
                    seq.InstitutionId = Convert.ToString(tb.Rows[0]["A14"]);
                }
                if (tb.Columns.Contains("A16"))//M05醫療院所
                {
                    seq.SamId = Convert.ToString(tb.Rows[0]["A16"]);
                }
                if (tb.Columns.Contains("A22"))//M14醫療院所
                {
                    seq.SecuritySignature = Convert.ToString(tb.Rows[0]["A22"]);
                }
            }

            p.Card.MedicalNumberData = seq;
            p.PrescriptionSign = listSign;
            return p;
        }
        public static DataSet CXmlToDataSet(string xmlStr)
        {
            DataSet ds = new DataSet();
            if (!string.IsNullOrEmpty(xmlStr))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    StrStream = new StringReader(xmlStr);
                    Xmlrdr = new XmlTextReader(StrStream);
                    ds.ReadXml(Xmlrdr);
                    return ds;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    //释放资源
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}