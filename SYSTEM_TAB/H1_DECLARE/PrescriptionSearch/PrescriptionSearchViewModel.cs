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
        public RelayCommand ImportDeclareFileCommand { get; set; }

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
            ImportDeclareFileCommand = new RelayCommand(ImportDeclareFileAction);
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<Institution>(this, "SelectedInstitution", GetSelectedInstitution);
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
            //依條件查詢對應處方
            MainWindow.ServerConnection.OpenConnection();
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
            MainWindow.ServerConnection.OpenConnection();
            ImportDeclareFile();
            MainWindow.ServerConnection.CloseConnection();
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
        private void ImportDeclareFile() {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "選擇申報檔";
            fdlg.InitialDirectory = @"c:\";   //@是取消转义字符的意思
            fdlg.Filter = "Xml健保申報檔案|*.xml";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            List<ImportDeclareXml.Ddata> ddatasCollection = new List<Ddata>(); 
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
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
