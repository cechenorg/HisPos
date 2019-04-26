using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Declare.DeclareFilePreview;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Declare.DeclarePreviewOfDay;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using MaterialDesignThemes.Wpf;
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
        //private CollectionViewSource decFilePreViewSource;
        //private CollectionViewSource DecFilePreViewSource
        //{
        //    get => decFilePreViewSource;
        //    set
        //    {
        //        Set(() => DecFilePreViewSource, ref decFilePreViewSource, value);
        //    }
        //}
        //private ICollectionView decFilePreViewCollectionView;
        //public ICollectionView DecFilePreViewCollectionView
        //{
        //    get => decFilePreViewCollectionView;
        //    private set
        //    {
        //        Set(() => DecFilePreViewCollectionView, ref decFilePreViewCollectionView, value);
        //    }
        //}

        //private DeclareFilePreviews decFilePreViews;
        //public DeclareFilePreviews DecFilePreViews
        //{
        //    get => decFilePreViews;
        //    private set
        //    {
        //        Set(() => DecFilePreViews, ref decFilePreViews, value);
        //    }
        //}
        //private DeclareFilePreview selectedPreview;
        //public DeclareFilePreview SelectedPreview
        //{
        //    get => selectedPreview;
        //    set
        //    {
        //        Set(() => SelectedPreview, ref selectedPreview, value);
        //    }
        //}
        private DeclarePreviewOfMonth declareFile;
        public DeclarePreviewOfMonth DeclareFile
        {
            get => declareFile;
            set
            {
                Set(() => DeclareFile, ref declareFile, value);
            }
        }
        private DateTime? declareDate;
        public DateTime? DeclareDate
        {
            get => declareDate;
            set
            {
                if(value != null)

                Set(() => DeclareDate, ref declareDate, value);
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
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        private int startDay;
        public int StartDay
        {
            get => startDay;
            set
            {
                Set(() => StartDay, ref startDay, value);
            }
        }
        private int endDay;
        public int EndDay
        {
            get => endDay;
            set
            {
                Set(() => EndDay, ref endDay, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand GetPreviewPrescriptions { get; set; }
        public RelayCommand AdjustPharmacistSetting { get; set; }
        public RelayCommand AdjustPharmacistOfDay { get; set; }
        public RelayCommand AdjustPharmacistOfMonth { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SetDecFilePreViewSummary { get; set; }
        public RelayCommand CreateDeclareFileCommand { get; set; }
        #endregion
        public DeclareFileManageViewModel()
        {
            InitialVariables();
            InitialCommands();
            RegisterMessengers();
            GetPreviewPrescriptionsActions();
        }
        #region Functions
        #region Initial
        private void InitialVariables()
        {
            DeclareFile = new DeclarePreviewOfMonth();
            //DecFilePreViews = new DeclareFilePreviews();
            DeclareDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month,1).AddMonths(-1);
            StartDay = 1;
            EndDay = DateTime.DaysInMonth(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month);
        }
        private void InitialCommands()
        {
            GetPreviewPrescriptions = new RelayCommand(GetPreviewPrescriptionsActions);
            AdjustPharmacistSetting = new RelayCommand(AdjustPharmacistSettingAction);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SetDecFilePreViewSummary = new RelayCommand(SetDecFilePreViewSummaryAction);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFileAction);
        }

        private void GetPreviewPrescriptionsActions()
        {
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
        private void AdjustPharmacistSettingAction()
        {
            var adjustPharmacistWindow = new AdjustPharmacistWindow(new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month, 1));
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
            //if (SelectedPreview.SelectedPrescription is null) return;
            MainWindow.ServerConnection.OpenConnection();
            //var selected = new Prescription(PrescriptionDb.GetPrescriptionByID(SelectedPreview.SelectedPrescription.ID).Rows[0], PrescriptionSource.Normal);
            MainWindow.ServerConnection.CloseConnection();
            //var prescriptionEdit = new PrescriptionEditWindow(selected.Id);
            //prescriptionEdit.ShowDialog();
        }
        private void SetDecFilePreViewSummaryAction()
        {
            //var currentPosition = DecFilePreViewCollectionView.CurrentPosition;
            //DecFilePreViews[DecFilePreViewCollectionView.CurrentPosition].SetSummary();
            //DecFilePreViewSource = new CollectionViewSource { Source = DecFilePreViews };
            //DecFilePreViewCollectionView = DecFilePreViewSource.View;
            //DecFilePreViewCollectionView.MoveCurrentToPosition(currentPosition);
        }
        private void CreateDeclareFileAction()
        {
            //if(SelectedPreview is null) return;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.產生申報資料;
                MainWindow.ServerConnection.OpenConnection();
                //SelectedPreview.DeclarePrescriptions.SerializeFileContent();
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                //var decFile = new DeclareFile(SelectedPreview,SelectedPreview.);
                //if (SelectedPreview.CheckFileExist())
                //{
                //    ConfirmWindow confirm = new ConfirmWindow("此申報年月已存在申報檔，是否覆蓋?", "檔案存在", true);
                //    if (!(bool)confirm.DialogResult)
                //        return;
                //}
                //SelectedPreview.CreateDeclareFile(decFile);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        #endregion
        private void GetPrescriptions()
        {
            var end = new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month+1, 1).AddDays(-1).Day;
            if (EndDay > end)
                EndDay = end;
            var sDate = new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month,StartDay);
            var eDate = new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month, EndDay);
            DeclareFile.GetSearchPrescriptions(sDate, eDate);
            DeclareFile.SetSummary();
            //DecFilePreViews.Clear();
            //foreach (var decs in prescriptions.GroupBy(p=>p.PharmacyID).Select(grp => grp.ToList()).ToList())
            //{
            //    foreach (var pres in decs.GroupBy(p=>p.AdjustDate.Month).Select(grp => grp.ToList()).ToList())
            //    {
            //        var decFile = new DeclareFilePreview();
            //        decFile.DeclarePrescriptions.AddPrescriptions(pres);
            //        decFile.SetSummary();
            //        //DecFilePreViews.Add(decFile);
            //    }
            //}
            //DecFilePreViewSource = new CollectionViewSource { Source = DecFilePreViews };
            //DecFilePreViewCollectionView = DecFilePreViewSource.View;
            //if (DecFilePreViewCollectionView.Cast<DeclareFilePreview>().ToList().Count > 0)
            //{
            //    DecFilePreViewCollectionView.MoveCurrentToFirst();
            //    SelectedPreview = DecFilePreViewCollectionView.CurrentItem.Cast<DeclareFilePreview>();
            //}
        }

        #endregion
    }
}
