using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
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
using StringRes = His_Pos.Properties.Resources;

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
        #endregion
        #region Commands
        public RelayCommand GetPreviewPrescriptions { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SetDecFilePreViewSummary { get; set; }
        public RelayCommand CreateDeclareFileCommand { get; set; }
        #endregion
        public DeclareFileManageViewModel()
        {
            InitialVariables();
            InitialCommands();
            RegisterMessengers();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.取得歷史處方;
                GetPrescriptions();
            };
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        #region Functions
        #region Initial
        private void InitialVariables()
        {
            DecFilePreViews = new DeclareFilePreviews();
            DecEnd = DateTime.Today;
            DecStart = new DateTime(((DateTime)DecEnd).Year, ((DateTime)DecEnd).Month, 1).AddMonths(-1);
            MedicalPersonnels = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = ViewModelMainWindow.AdjustCases;
            Institutions = ViewModelMainWindow.Institutions;
        }
        private void InitialCommands()
        {
            GetPreviewPrescriptions = new RelayCommand(GetPreviewPrescriptionsActions);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SetDecFilePreViewSummary = new RelayCommand(SetDecFilePreViewSummaryAction);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFileAction);
        }

        private void GetPreviewPrescriptionsActions()
        {
            GetPrescriptions();
        }
        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals(nameof(DeclareFileManageViewModel) + "PrescriptionEdited"))
                    GetPrescriptions();
            });
        }
        #endregion
        #region CommandActions
        private void ShowPrescriptionEditWindowAction()
        {
            if (SelectedFile.SelectedPrescription is null) return;
            MainWindow.ServerConnection.OpenConnection();
            Prescription selected =
                new Prescription(PrescriptionDb.GetPrescriptionByID(SelectedFile.SelectedPrescription.ID).Rows[0],
                    PrescriptionSource.Normal);
            MainWindow.ServerConnection.CloseConnection();
            PrescriptionEditWindow prescriptionEdit = new PrescriptionEditWindow(selected, ViewModelEnum.PrescriptionSearch);
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
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.產生申報資料;
                SelectedFile.DeclarePrescriptions.SerializeFileContent();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                //IsBusy = false;
                //var decFile = new DeclareFile(SelectedFile);
                ////if (SelectedFile.CheckFileExist())
                ////{
                ////    ConfirmWindow confirm = new ConfirmWindow("此申報年月已存在申報檔，是否覆蓋?", "檔案存在", true);
                ////    if (!(bool)confirm.DialogResult)
                ////        return;
                ////}
                //SelectedFile.CreateDeclareFile(decFile);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        #endregion
        private void GetPrescriptions()
        {
            var prescriptions = new DeclarePrescriptions();
            prescriptions.GetSearchPrescriptions((DateTime)DecStart, (DateTime)DecEnd);
            DecFilePreViews.Clear();
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
