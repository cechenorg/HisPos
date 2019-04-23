using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class; 
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ImportDeclareXml;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using static His_Pos.NewClass.Prescription.ImportDeclareXml.ImportDeclareXml;
using StringRes = His_Pos.Properties.Resources;
// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    public class PrescriptionSearchViewModel : TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
        public Institutions Institutions { get; set; }
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
        private string patientName;
        public string PatientName
        {
            get => patientName;
            set
            {
                Set(() => PatientName, ref patientName, value);
            }
        }
        private string patientIDNumber;
        public string PatientIDNumber
        {
            get => patientIDNumber;
            set
            {
                Set(() => PatientIDNumber, ref patientIDNumber, value);
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
        private int profit;
        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }
        private int medicineCount;
        public int MedicineCount
        {
            get => medicineCount;
            set
            {
                Set(() => MedicineCount, ref medicineCount, value);
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
        private string medicineID;
        public string MedicineID
        {
            get => medicineID;
            set
            {
                Set(() => MedicineID, ref medicineID, value);
            }
        }
        private string medicineName;
        public string MedicineName
        {
            get => medicineName;
            set
            {
                Set(() => MedicineName, ref medicineName, value);
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
        private Division selectedDivision;
        public Division SelectedDivision
        {
            get => selectedDivision;
            set
            {
                Set(() => SelectedDivision, ref selectedDivision, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand Search { get; set; }
        public RelayCommand ReserveSearch { get; set; }
        public RelayCommand ImportDeclareFileCommand { get; set; }
        public RelayCommand ExportPrescriptionCsvCommand { get; set; }
        public RelayCommand ExportMedicineCsvCommand { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> CheckInsEmpty { get; set; }
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
            SearchPrescriptions = new PrescriptionSearchPreviews();
            Institutions = ViewModelMainWindow.Institutions;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            Divisions = ViewModelMainWindow.Divisions;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }
        private void InitialCommands()
        {
            Search = new RelayCommand(SearchAction);
            ReserveSearch = new RelayCommand(ReserveSearchAction);
            ImportDeclareFileCommand = new RelayCommand(ImportDeclareFileAction);
            ExportPrescriptionCsvCommand = new RelayCommand(ExportPrescriptionCsvAction);
            ExportMedicineCsvCommand = new RelayCommand(ExportMedicineCsvAction);
            Clear = new RelayCommand(ClearAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(GetInstitutionAction);
            CheckInsEmpty = new RelayCommand<string>(CheckInsEmptyAction);

        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage>(nameof(PrescriptionSearchView) + "ShowPrescriptionEditWindow", ShowPrescriptionEditWindowAction);
        }
        #endregion
        #region CommandActions
        private void SearchAction()
        {
            if (!CheckCondition()) return;
            if(!CheckStartDate()) return;
            if (!CheckEndDate()) return;
            if(!CheckDateOutOfRange()) return;
            var previews = new PrescriptionSearchPreviews();
            SearchPrescriptions.Clear();
            MainWindow.ServerConnection.OpenConnection();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.處方查詢;
                //依條件查詢對應處方
                previews.GetSearchPrescriptions(StartDate, EndDate, PatientName?.Trim(),PatientIDNumber?.Trim(), PatientBirth, SelectedAdjustCase,MedicineID?.Trim(), MedicineName?.Trim(), SelectedInstitution,SelectedDivision);
                SearchPrescriptions = previews;
                SetPrescriptionsSummary(false);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MainWindow.ServerConnection.CloseConnection();
                UpdateCollectionView();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void SetPrescriptionsSummary(bool reserve)
        {
            var summary = SearchPrescriptions.GetSummary(reserve, MedicineID);
            TotalCount = SearchPrescriptions.Count;
            ChronicCount = SearchPrescriptions.Count(p => p.AdjustCase.ID.Equals("2"));
            TotalPoint = summary[0];
            MedicinePoint = summary[1];
            MedicalServicePoint = summary[2];
            CopaymentPoint = summary[3];
            Profit = summary[4];
            MedicineCount = summary[5];
        }

        private void ReserveSearchAction()
        {
            if (!CheckCondition()) return;
            if (!CheckStartDate()) return;
            if (!CheckEndDate()) return;
            if (!CheckDateOutOfRange()) return;
            var previews = new PrescriptionSearchPreviews();
            SearchPrescriptions.Clear();
            MainWindow.ServerConnection.OpenConnection();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.處方查詢;
                //依條件查詢對應處方
                previews.GetReservePrescription(StartDate, EndDate, PatientName, PatientIDNumber, PatientBirth, SelectedAdjustCase, MedicineID, MedicineName, SelectedInstitution,SelectedDivision);
                SearchPrescriptions = previews;
                SetPrescriptionsSummary(true);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MainWindow.ServerConnection.CloseConnection();
                UpdateCollectionView();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void ImportDeclareFileAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            ImportDeclareFile();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ShowPrescriptionEditWindowAction(NotificationMessage msg)
        {
            if(SelectedPrescription is null || !msg.Notification.Equals(nameof(PrescriptionSearchView) + "ShowPrescriptionEditWindow")) return;
            EditedPrescription = SelectedPrescription;
            MainWindow.ServerConnection.OpenConnection();
            var prescription = SelectedPrescription.Source.Equals(PrescriptionSource.Normal) ? 
                SelectedPrescription.GetPrescriptionByID() : SelectedPrescription.GetReservePrescriptionByID();
            MainWindow.ServerConnection.CloseConnection();
            prescription.Source = SelectedPrescription.Source;
            var pSource = SelectedPrescription.Source;
            var prescriptionEdit = new PrescriptionEditWindow.PrescriptionEditWindow(SelectedPrescription.ID, pSource);
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            prescriptionEdit.ShowDialog();
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }
        private void ClearAction()
        {
            StartDate = null;
            EndDate = null;
            SelectedAdjustCase = null;
            PatientName = string.Empty;
            PatientIDNumber = string.Empty;
            PatientBirth = null;
            MedicineID = string.Empty;
            MedicineName = string.Empty;
            SelectedInstitution = null;
            SelectedDivision = null;
            SearchPrescriptions.Clear();
            UpdateCollectionView();
            TotalCount = 0;
            ChronicCount = 0;
            TotalPoint = 0;
            MedicinePoint = 0;
            MedicalServicePoint = 0;
            CopaymentPoint = 0;
            Profit = 0;
            MedicineCount = 0;
        }

        private void Refresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals(nameof(PrescriptionSearchViewModel) + "PrescriptionEdited"))
                SearchAction();
            else if (msg.Notification.Equals(nameof(PrescriptionSearchViewModel) + "ReservePrescriptionEdited"))
            {
                ReserveSearchAction();
            }
        }
        private void GetInstitutionAction(string search)
        {
            SelectedInstitution = null;
            var result = Institutions.Where(i => i.ID.Contains(search) || i.Name.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    SelectedInstitution = result[0];
                    break;
                default:
                    Messenger.Default.Register<Institution>(this, nameof(PrescriptionSearchViewModel) + "InsSelected", GetSelectedInstitution);
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search, ViewModelEnum.PrescriptionSearch);
                    institutionSelectionWindow.ShowDialog();
                    Messenger.Default.Unregister<Institution>(this, nameof(PrescriptionSearchViewModel) + "InsSelected", GetSelectedInstitution);
                    break;
            }
        }
        private void CheckInsEmptyAction(string search)
        {
            if (string.IsNullOrEmpty(search))
                SelectedInstitution = null;
        }
        #endregion
        #region Functions
        private void ExportMedicineCsvAction() {
            List<int> idList = new List<int>();
            foreach (var a in SearchPrescriptions) {
                idList.Add(a.ID);
            }
            DataTable table = MedicineDb.GetPrescriptionMedicineSumById(idList); 
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "藥品統計存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") +  "藥品統計存檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("日期,藥品名稱,數量");
                        foreach (DataRow s in table.Rows)
                        {

                            file.WriteLine($"{s.Field<string>("ID")},{s.Field<string>("Name")},{s.Field<int>("TotalAmount")}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }

            }
        }
        private void ExportPrescriptionCsvAction() {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "處方存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "我是處方請存我|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + "處方存檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("調劑狀態,藥袋狀態,醫療院所,科別,病患姓名,就醫序號,身分證,生日,處方就醫日,處方調劑日,實際調劑日");
                        foreach (var s in SearchPrescriptions)
                        {
                            string insName = s.Institution is null ? "" : s.Institution.Name;
                            string divName = s.Division is null ? "" : s.Division.Name;
                            string s_adjust = s.IsAdjust == true ? "已調劑" : "未調劑";
                            file.WriteLine($"{s_adjust},{s.StoStatus},{insName},{divName},{s.Patient.Name},{s.MedicalNumber},{s.Patient.IDNumber},{((DateTime)s.Patient.Birthday).AddYears(-1911).ToString("yyy/MM/dd")},{((DateTime)s.TreatDate).AddYears(-1911).ToString("yyy/MM/dd")},{((DateTime)s.AdjustDate).AddYears(-1911).ToString("yyy/MM/dd")},{s.InsertDate}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                }
                catch (Exception ex) {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
                
            }
        }
        private void UpdateCollectionView()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions };
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            PrescriptionCollectionView.MoveCurrentToFirst();
            SelectedPrescription = (PrescriptionSearchPreview)PrescriptionCollectionView.CurrentItem;
        }
        private void ImportDeclareFile() {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "選擇申報檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Xml健保申報檔案|*.xml";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            List<Ddata> ddatasCollection = new List<Ddata>(); 
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName.Replace("\\" + fdlg.SafeFileName, "");
                Properties.Settings.Default.Save();
                doc.Load(fdlg.FileName);
                string fileId;
                string fileHead = doc.GetElementsByTagName("tdata")[0].InnerXml; 
                DataTable filetable = PrescriptionDb.CheckImportDeclareFileExist(fileHead);
                if (filetable.Rows.Count > 0)
                    fileId = filetable.Rows[0]["newId"].ToString();
                else {
                    MessageWindow.ShowMessage("此申報檔已經匯入~!", MessageType.ERROR);
                    return;
                }
                List<string> declareFiles = new List<string>();
                XmlNodeList ddatas = doc.GetElementsByTagName("ddata");
                XmlDocument data = new XmlDocument();
                foreach (XmlNode node in ddatas)
                { 
                    data.LoadXml("<ddata>" + node.SelectSingleNode("dhead").InnerXml + node.SelectSingleNode("dbody").InnerXml + "</ddata>");
                    Ddata d = XmlService.Deserialize<ImportDeclareXml.Ddata>(data.InnerXml);
                    declareFiles.Add(node.InnerXml);
                    ddatasCollection.Add(d); 
                }
                PrescriptionDb.ImportDeclareXml(ddatasCollection, declareFiles, fileId);
                MessageWindow.ShowMessage("匯入申報檔完成!",MessageType.SUCCESS);
            }
        }
        private bool CheckCondition()
        {
            if (StartDate is null && EndDate is null && string.IsNullOrEmpty(PatientName) && string.IsNullOrEmpty(PatientIDNumber) && PatientBirth is null && string.IsNullOrEmpty(MedicineID) && string.IsNullOrEmpty(MedicineName) && (SelectedInstitution is null || string.IsNullOrEmpty(SelectedInstitution.Name)) && SelectedDivision is null)
            {
                MessageWindow.ShowMessage("起始結束日期.病患姓名.身分證.生日.藥品代碼.藥品名稱.釋出院所或科別請至少擇一填寫", MessageType.WARNING);
                return false;
            }
            return true;
        }

        private bool CheckStartDate()
        {
            if (StartDate is null && EndDate != null)
            {
                MessageWindow.ShowMessage("請填寫起始日期", MessageType.WARNING);
                return false;
            }
            return true;
        }
        private bool CheckEndDate()
        {
            if (StartDate != null && EndDate is null)
            {
                MessageWindow.ShowMessage("請填寫結束日期", MessageType.WARNING);
                return false;
            }
            return true;
        }
        private bool CheckDateOutOfRange()
        {
            if (StartDate != null && EndDate != null)
            {
                if (DateTime.Compare((DateTime)StartDate, (DateTime)EndDate) > 0)
                {
                    MessageWindow.ShowMessage(StringRes.StartDateOutOfRange, MessageType.WARNING);
                    return false;
                }
            }
            return true;
        }
        private void GetSelectedInstitution(Institution ins)
        {
            SelectedInstitution = ins;
        }
        #endregion
    }
}
