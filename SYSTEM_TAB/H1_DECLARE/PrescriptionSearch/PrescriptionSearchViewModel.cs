using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ImportDeclareXml;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using static His_Pos.NewClass.Prescription.ImportDeclareXml.ImportDeclareXml;
using MedicalPersonnel = His_Pos.NewClass.Person.MedicalPerson.MedicalPersonnel;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
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
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public Institutions Institutions { get; set; }
        public AdjustCases AdjustCases { get; set; }
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
        private DateTime? birth;
        public DateTime? Birth
        {
            get => birth;
            set
            {
                Set(() => Birth, ref birth, value);
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
        #endregion
        #region Commands
        public RelayCommand Search { get; set; }
        public RelayCommand ReserveSearch { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> CheckInsEmpty { get; set; }
        public RelayCommand ImportDeclareFileCommand { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand Clear { get; set; }
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
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            Institutions = ViewModelMainWindow.Institutions;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            StartDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            EndDate = DateTime.Today;
        }
        private void InitialCommands()
        {
            Search = new RelayCommand(SearchAction);
            ReserveSearch = new RelayCommand(ReserveSearchAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(GetInstitutionAction);
            ImportDeclareFileCommand = new RelayCommand(ImportDeclareFileAction);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            CheckInsEmpty = new RelayCommand<string>(CheckInsEmptyAction);
            Clear = new RelayCommand(ClearAction);
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionSearchViewModel) + "InsSelected", GetSelectedInstitution);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals(nameof(PrescriptionSearchViewModel)+"PrescriptionEdited"))
                    SearchAction();
            });
        }
        #endregion
        #region CommandActions
        private void SearchAction()
        {
            if (StartDate is null)
            {
                MessageWindow.ShowMessage(StringRes.StartDateEmpty,MessageType.WARNING);
                return;
            }
            if (EndDate is null)
                EndDate = DateTime.Today;
            if (DateTime.Compare((DateTime)StartDate, (DateTime)EndDate) > 0)
            {
                MessageWindow.ShowMessage(StringRes.StartDateOutOfRange, MessageType.WARNING);
                return;
            }
            var end = (DateTime) EndDate;
            var start = (DateTime) StartDate;
            var month = (end.Year - start.Year) * 12 + (end.Month - start.Month);
            if (month > 3)
            {
                MessageWindow.ShowMessage(StringRes.SearchDateOutOfRange, MessageType.WARNING);
                return;
            }
            SearchPrescriptions.Clear();
            //依條件查詢對應處方
            MainWindow.ServerConnection.OpenConnection();
            SearchPrescriptions.GetSearchPrescriptions(StartDate,EndDate,SelectedAdjustCase,SelectedInstitution,SelectedPharmacist);
            MainWindow.ServerConnection.CloseConnection();
            UpdateCollectionView();
            SetPrescriptionsSummary();
        }

        private void SetPrescriptionsSummary(bool reserve)
        {
            List<int> summary = new List<int>();
            summary = SearchPrescriptions.GetSummary(reserve);
            TotalCount = SearchPrescriptions.Count;
            ChronicCount = SearchPrescriptions.Count(p => p.AdjustCase.Id.Equals("2"));
            TotalPoint = summary[0];
            MedicinePoint = summary[1];
            MedicalServicePoint = summary[2];
            CopaymentPoint = summary[3];
            Profit = summary[4];
        }

        private void ReserveSearchAction()
        {
            SearchPrescriptions.Clear();
            //查詢預約慢箋
            SearchPrescriptions.GetReservePrescription(StartDate, EndDate, SelectedAdjustCase, SelectedInstitution, SelectedPharmacist);
            UpdateCollectionView();
            SetPrescriptionsSummary();
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
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search,ViewModelEnum.PrescriptionSearch);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ImportDeclareFileAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            ImportDeclareFile();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void CheckInsEmptyAction(string search)
        {
            if (string.IsNullOrEmpty(search))
                SelectedInstitution = null;
        }
        private void ShowPrescriptionEditWindowAction()
        {
            if(SelectedPrescription is null) return;
            EditedPrescription = SelectedPrescription;
            PrescriptionEditWindow.PrescriptionEditWindow prescriptionEdit;
            prescriptionEdit = SelectedPrescription.Source.Equals(PrescriptionSource.Normal) ? 
                new PrescriptionEditWindow.PrescriptionEditWindow(SelectedPrescription.GetPrescriptionByID(),ViewModelEnum.PrescriptionSearch) : 
                new PrescriptionEditWindow.PrescriptionEditWindow(SelectedPrescription.GetReservePrescriptionByID(), ViewModelEnum.PrescriptionSearch);
            prescriptionEdit.ShowDialog();
        }
        private void ClearAction()
        {
            SelectedInstitution = null;
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            EndDate = DateTime.Today;
            SelectedPharmacist = null;
            SelectedAdjustCase = null;
            Patient = string.Empty;
            Birth = null;
        }
        #endregion
        #region Functions
        private void UpdateCollectionView()
        {
            PrescriptionCollectionVS = new CollectionViewSource { Source = SearchPrescriptions };
            PrescriptionCollectionView = PrescriptionCollectionVS.View;
            PrescriptionCollectionView.MoveCurrentToFirst();
            SelectedPrescription = (PrescriptionSearchPreview)PrescriptionCollectionView.CurrentItem;
        }
        private void UpdateFilter()
        {
            if (PrescriptionCollectionVS is null) return;
            PrescriptionCollectionVS.Filter += FilterByPatient;
        }
        private void GetSelectedInstitution(Institution ins)
        {
            SelectedInstitution = ins;
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
            List<ImportDeclareXml.Ddata> ddatasCollection = new List<Ddata>(); 
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

                XmlNodeList ddatas = doc.GetElementsByTagName("ddata");
                XmlDocument data = new XmlDocument();
                foreach (XmlNode node in ddatas)
                {
                    data.LoadXml("<ddata>" + node.SelectSingleNode("dhead").InnerXml + node.SelectSingleNode("dbody").InnerXml + "</ddata>");
                    Ddata d = XmlService.Deserialize<ImportDeclareXml.Ddata>(data.InnerXml);
                    ddatasCollection.Add(d); 
                }
                PrescriptionDb.ImportDeclareXml(ddatasCollection, fileId);
                MessageWindow.ShowMessage("匯入申報檔完成!",MessageType.SUCCESS);
            }

        }
        #endregion
        #region Filters
        private void FilterByPatient(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchPreview src))
                e.Accepted = false;
            else if (string.IsNullOrEmpty(Patient) && Birth is null)
                e.Accepted = true;
            else if(!string.IsNullOrEmpty(Patient) && Birth is null)
            {
                if (src.Patient.Name.Contains(Patient) || src.Patient.IDNumber.Contains(Patient.ToUpper()))
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
            else if (string.IsNullOrEmpty(Patient) && Birth != null)
            {
                if (src.Patient.Birthday != null && DateTime.Compare((DateTime)src.Patient.Birthday,(DateTime)Birth) == 0)
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
            else if (!string.IsNullOrEmpty(Patient) && Birth != null)
            {
                if (src.Patient.Name.Contains(Patient) || src.Patient.IDNumber.Contains(Patient.ToUpper()))
                {
                    if (src.Patient.Birthday != null && DateTime.Compare((DateTime)src.Patient.Birthday, (DateTime)Birth) == 0)
                        e.Accepted = true;
                    else
                        e.Accepted = false;
                }
                else
                    e.Accepted = false;
            }
        }
        #endregion
    }
}
