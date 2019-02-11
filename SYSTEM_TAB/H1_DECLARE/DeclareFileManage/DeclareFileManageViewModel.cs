using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Declare.DeclareFilePreview;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
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
            private set
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
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
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
            DecEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DecStart = new DateTime(((DateTime)DecEnd).Year, ((DateTime)DecEnd).Month, 1).AddMonths(-3);
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            Institutions = ViewModelMainWindow.Institutions;
        }
        private void InitialCommands()
        {
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
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
        #endregion
        #endregion


    }
}
