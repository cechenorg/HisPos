using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclareFilePreview;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    public class DeclareFileManageViewModel:TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
        private CollectionViewSource decFilePreViewSource;
        private CollectionViewSource DecFilePreViewSource
        {
            get => decFilePreViewSource;
            set
            {
                Set(() => DecFilePreViewSource, ref decFilePreViewSource, value);
            }
        }
        private ICollectionView decFilePreViewCollectionView;
        public ICollectionView DecFilePreViewCollectionView
        {
            get => decFilePreViewCollectionView;
            private set
            {
                Set(() => DecFilePreViewCollectionView, ref decFilePreViewCollectionView, value);
            }
        }

        private DeclareFilePreviews decFilePreViews;

        public DeclareFilePreviews DecFilePreViews
        {
            get => decFilePreViews;
            private set
            {
                Set(() => DecFilePreViews, ref decFilePreViews, value);
            }
        }
        private DeclareFilePreview selectedFile;
        public DeclareFilePreview SelectedFile
        {
            get => selectedFile;
            set
            {
                Set(() => SelectedFile, ref selectedFile, value);
            }
        }
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public Institutions Institutions { get; set; }
        private DateTime? decStart;
        public DateTime? DecStart
        {
            get => decStart;
            set
            {
                Set(() => DecStart, ref decStart, value);
            }
        }
        private DateTime? decEnd;
        public DateTime? DecEnd
        {
            get => decEnd;
            set
            {
                Set(() => DecEnd, ref decEnd, value);
            }
        }

        private DeclarePrescription selectedPrescription;
        public DeclarePrescription SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand GetPreviewPrescriptions { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SetDecFilePreViewSummary { get; set; }
        public RelayCommand CreateDeclareFileCommand { get; set; }
        #endregion
        public DeclareFileManageViewModel()
        {
            InitialVariables();
            InitialCommands();
        }
        #region Functions
        #region Initial
        private void InitialVariables()
        {
            DecFilePreViews = new DeclareFilePreviews();
            DecEnd = DateTime.Today;
            DecStart = new DateTime(((DateTime)DecEnd).Year, ((DateTime)DecEnd).Month, 1).AddMonths(-3);
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            Institutions = ViewModelMainWindow.Institutions;
            GetPrescriptions();
        }
        private void InitialCommands()
        {
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            GetPreviewPrescriptions = new RelayCommand(GetPreviewPrescriptionsActions);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SetDecFilePreViewSummary = new RelayCommand(SetDecFilePreViewSummaryAction);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFileAction);
        }

        private void GetPreviewPrescriptionsActions()
        {
            GetPrescriptions();
        }
        #endregion
        #region CommandActions
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(Resources.ShortSearchString + "4", MessageType.WARNING);
                return;
            }
            var result = Institutions.Where(i => i.Id.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    SelectedFile.SelectedInstitution = result[0];
                    break;
                default:
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void ShowPrescriptionEditWindowAction()
        {
            if (SelectedPrescription is null) return;
            Prescription selected =
                new Prescription(PrescriptionDb.GetPrescriptionByID(SelectedPrescription.ID).Rows[0],
                    PrescriptionSource.Normal);
            selected.GetCompletePrescriptionData(true);
            PrescriptionEditWindow prescriptionEdit = new PrescriptionEditWindow(selected);
            prescriptionEdit.ShowDialog();
        }
        private void SetDecFilePreViewSummaryAction()
        {
            var currentPosition = DecFilePreViewCollectionView.CurrentPosition;
            DecFilePreViews[DecFilePreViewCollectionView.CurrentPosition].SetSummary();
            DecFilePreViewSource = new CollectionViewSource { Source = DecFilePreViews };
            DecFilePreViewCollectionView = DecFilePreViewSource.View;
            DecFilePreViewCollectionView.MoveCurrentToPosition(currentPosition);
        }
        private void CreateDeclareFileAction()
        {
            if(SelectedFile is null) return;
            var decFile = new DeclareFile(SelectedFile);
            if (SelectedFile.CheckFileExist())
            {
                ConfirmWindow confirm = new ConfirmWindow("此申報年月已存在申報檔，是否覆蓋?", "檔案存在");
                if (!(bool)confirm.DialogResult)
                    return;
            }
            SelectedFile.CreateDeclareFile(decFile);
        }
        #endregion
        private void GetPrescriptions()
        {
            var prescriptions = new DeclarePrescriptions();
            prescriptions.GetSearchPrescriptions((DateTime)DecStart, (DateTime)DecEnd);
            foreach (var decs in prescriptions.GroupBy(p=>p.PharmacyID).Select(grp => grp.ToList()).ToList())
            {
                foreach (var pres in decs.GroupBy(p=>p.AdjustDate.Month).Select(grp => grp.ToList()).ToList())
                {
                    var decFile = new DeclareFilePreview();
                    decFile.DeclarePrescriptions.AddPrescriptions(pres);
                    decFile.SetSummary();
                    DecFilePreViews.Add(decFile);
                }
            }
            DecFilePreViewSource = new CollectionViewSource { Source = DecFilePreViews };
            DecFilePreViewCollectionView = DecFilePreViewSource.View;
            if (DecFilePreViewCollectionView.Cast<DeclareFilePreview>().ToList().Count > 0)
            {
                DecFilePreViewCollectionView.MoveCurrentToFirst();
                SelectedFile = DecFilePreViewCollectionView.CurrentItem.Cast<DeclareFilePreview>();
            }
        }

        #endregion
    }
}
