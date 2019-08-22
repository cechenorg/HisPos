using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport.InstitutionDeclarePoint;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePharmacy;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Declare.DeclarePreview;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting;
using StringRes = His_Pos.Properties.Resources;

// ReSharper disable InconsistentNaming
namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    // ReSharper disable once ClassTooBig
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DeclareFileManageViewModel:TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
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
        private int startDay => 1;
        private int endDay;
        public int EndDay
        {
            get => endDay;
            set
            {
                Set(() => EndDay, ref endDay, value);
            }
        }

        private DeclarePharmacies declarePharmacies;

        public DeclarePharmacies DeclarePharmacies
        {
            get => declarePharmacies;
            set
            {
                Set(() => DeclarePharmacies, ref declarePharmacies, value);
            }
        }
        private PharmacistSchedule pharmacistScheduleWithPrescriptionCount;
        public PharmacistSchedule PharmacistScheduleWithPrescriptionCount
        {
            get => pharmacistScheduleWithPrescriptionCount;
            set
            {
                Set(() => PharmacistScheduleWithPrescriptionCount, ref pharmacistScheduleWithPrescriptionCount, value);
            }
        }

        private PharmacistSchedule pharmacistSchedule;
        public PharmacistSchedule PharmacistSchedule
        {
            get => pharmacistSchedule;
            set
            {
                Set(() => PharmacistSchedule, ref pharmacistSchedule, value);
            }
        }

        private DeclarePharmacy selectedPharmacy;

        public DeclarePharmacy SelectedPharmacy
        {
            get => selectedPharmacy;
            set
            {
                Set(() => SelectedPharmacy, ref selectedPharmacy, value);
            }
        }
        private DeclarePrescriptions editedList;

        public DeclarePrescriptions EditedList
        {
            get => editedList;
            set
            {
                Set(() => EditedList, ref editedList, value);
            }
        }
        private BackgroundWorker worker;
        #endregion
        #region Commands
        public RelayCommand GetPreviewPrescriptions { get; set; }
        public RelayCommand AdjustPharmacistSetting { get; set; }
        public RelayCommand AdjustPharmacistOfDay { get; set; }
        public RelayCommand AdjustPharmacistOfMonth { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SetDecFilePreViewSummary { get; set; }
        public RelayCommand CreateDeclareFileCommand { get; set; }
        public RelayCommand AddToEditListCommand { get; set; }
        #endregion
        public DeclareFileManageViewModel()
        {
            InitialVariables();
            InitialCommands();
            GetPreviewPrescriptionsActions();
        }
        #region Functions
        #region Initial
        private void InitialVariables()
        {
            DeclarePharmacies = new DeclarePharmacies();
            SelectedPharmacy = DeclarePharmacies.SingleOrDefault(p => p.ID.Equals(ViewModelMainWindow.CurrentPharmacy.ID));
            DeclareFile = new DeclarePreviewOfMonth();
            DeclareDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month,1).AddMonths(-1);
            var decDate = (DateTime)DeclareDate;
            EndDay = DateTime.DaysInMonth(decDate.Year, decDate.Month);
            var endDate = new DateTime(decDate.Year, decDate.Month, EndDay);
            GetPharmacistSchedule();
            DeclareFile.GetNotAdjustPrescriptionCount(DeclareDate, endDate, SelectedPharmacy.ID);
            var duplicatePrescriptionWindow = new DuplicatePrescriptionWindow.DuplicatePrescriptionWindow(decDate, endDate);
            if (duplicatePrescriptionWindow.ShowDialog)
                duplicatePrescriptionWindow.Show();
            EditedList = new DeclarePrescriptions();
        }
        private void InitialCommands()
        {
            GetPreviewPrescriptions = new RelayCommand(GetPreviewPrescriptionsActions);
            AdjustPharmacistSetting = new RelayCommand(AdjustPharmacistSettingAction);
            AdjustPharmacistOfDay = new RelayCommand(AdjustPharmacistOfDayAction);
            AdjustPharmacistOfMonth = new RelayCommand(AdjustPharmacistOfMonthAction);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SetDecFilePreViewSummary = new RelayCommand(SetDecFilePreViewSummaryAction);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFileAction);
            AddToEditListCommand = new RelayCommand(AddToEditListAction);
        }
        #endregion
        #region CommandActions
        private void GetPreviewPrescriptionsActions()
        {
            if(!CheckStartOrEndDayNull()) return;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.取得歷史處方;
                GetDeclarePrescriptions();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void GetDeclarePrescriptions()
        {
            MainWindow.ServerConnection.OpenConnection();
            GetPrescriptions();
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckStartOrEndDayNull()
        {
            if (DeclareDate is null)
            {
                MessageWindow.ShowMessage("請填寫申報年月", MessageType.ERROR);
                return false;
            }
            return true;
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void AdjustPharmacistSettingAction()
        {
            if (DeclareDate is null)
            {
                MessageWindow.ShowMessage("尚未填寫申報年月。",MessageType.WARNING);
                return;
            }
            var decDate = (DateTime)DeclareDate;
            var adjustPharmacistWindow = new AdjustPharmacistWindow(new DateTime(decDate.Year, decDate.Month, 1));
            MainWindow.ServerConnection.OpenConnection();
            GetPharmacistSchedule();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void AdjustPharmacistOfDayAction()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "藥師調整處理中...";
                StartAdjustPharmacistsOfDay();
            };
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void StartAdjustPharmacistsOfDay()
        {
            var pharmacists = GetAdjustPharmacistsOfDay();
            if (pharmacists.Count == 0)
            {
                MessageWindow.ShowMessage("尚未設定本日藥師", MessageType.WARNING);
                return;
            }
            DeclareFile.SelectedDayPreview.PresOfDay.AdjustPharmacist(pharmacists);
            MainWindow.ServerConnection.OpenConnection();
            DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
            DeclareFile.SelectedDayPreview.CheckAdjustOutOfRange();
            BusyContent = StringRes.取得歷史處方;
            GetPrescriptions();
            MainWindow.ServerConnection.CloseConnection();
        }

        private List<PharmacistScheduleItem> GetAdjustPharmacistsOfDay()
        {
            var schedule = PharmacistSchedule.Where(p => p.Date.Equals(DeclareFile.SelectedDayPreview.Date)).ToList();
            return schedule.ToList();
        }

        private void AdjustPharmacistOfMonthAction()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "藥師調整處理中...";
                StartAdjustPharmacistsOfMonth();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                Refresh();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void StartAdjustPharmacistsOfMonth()
        {
            var adjustList = CreateAdjustList();
            var pharmacistList = GetAdjustPharmacistsOfMonth();
            if (pharmacistList.Count == 0)
            {
                MessageWindow.ShowMessage("尚未設定本月藥師", MessageType.WARNING);
                return;
            }
            adjustList.AdjustPharmacist(pharmacistList);
            MainWindow.ServerConnection.OpenConnection();
            DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
            MainWindow.ServerConnection.CloseConnection();
            foreach (var pre in DeclareFile.DeclarePreviews)
            {
                pre.CheckAdjustOutOfRange();
            }
            DeclareFile.SetSummary();
        }

        private List<PharmacistScheduleItem> GetAdjustPharmacistsOfMonth()
        {
            var schedule = PharmacistSchedule.ToList();
            return schedule.ToList();
        }

        private DeclarePrescriptions CreateAdjustList()
        {
            var adjustList = new DeclarePrescriptions();
            foreach (var pre in DeclareFile.DeclarePreviews)
            {
                foreach (var dec in pre.PresOfDay)
                {
                    adjustList.Add(dec);
                }
            }
            return adjustList;
        }

        private void ShowPrescriptionEditWindowAction()
        {
            if (DeclareFile.SelectedDayPreview.SelectedPrescription is null) return;
            Messenger.Default.Register<NotificationMessage>(this, PrescriptionEditedRefresh);
            PrescriptionService.ShowPrescriptionEditWindow(DeclareFile.SelectedDayPreview.SelectedPrescription.ID);
            Messenger.Default.Unregister<NotificationMessage>(this, PrescriptionEditedRefresh);
        }

        private void SetDecFilePreViewSummaryAction()
        {
            DeclareFile.SetSummary();
        }

        private void CreateDeclareFileAction()
        {
            if (DeclareDate is null)
            {
                MessageWindow.ShowMessage("尚未填寫申報年月。", MessageType.WARNING);
                return;
            }
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => { CreateDeclareFile(); };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                Refresh();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void CreateDeclareFile()
        {
            Debug.Assert(DeclareDate != null, nameof(DeclareDate) + " != null");
            var decDate = (DateTime)DeclareDate;
            MainWindow.ServerConnection.OpenConnection();
            BusyContent = "處方排序中...";
            DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
            MainWindow.ServerConnection.CloseConnection();
            BusyContent = StringRes.產生申報資料;
            var decFile = new DeclareFile(DeclareFile, SelectedPharmacy.ID);
            DeclareFile.CreateDeclareFile(decFile, decDate);
        }

        private void AddToEditListAction()
        {
            SetDecFilePreViewSummaryAction();
            DeclareFile.SelectedDayPreview.CheckNotDeclareCount();
        }
        #endregion
        private void GetPrescriptions()
        {
            Debug.Assert(DeclareDate != null, nameof(DeclareDate) + " != null");
            var decDate = (DateTime) DeclareDate;
            var end = DateTime.DaysInMonth(decDate.Year, decDate.Month - 1);
            if (EndDay > end) EndDay = end;
            var sDate = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, startDay);
            var eDate = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, EndDay);
            DeclareFile.GetSearchPrescriptions(sDate, eDate, SelectedPharmacy.ID);
            DeclareFile.SetSummary();
            DeclareFile.DeclareDate = (DateTime)DeclareDate;
        }
        private void PrescriptionEditedRefresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals("PrescriptionEdited"))
            {
                Refresh();
            }
        }

        private void GetPharmacistSchedule()
        {
            var start = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, startDay);
            var last = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, EndDay);
            MainWindow.ServerConnection.OpenConnection();
            PharmacistSchedule = new PharmacistSchedule();
            PharmacistSchedule.GetPharmacistSchedule(start, last);
            PharmacistScheduleWithPrescriptionCount = new PharmacistSchedule();
            PharmacistScheduleWithPrescriptionCount.GetPharmacistScheduleWithCount(start, last);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void Refresh()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = StringRes.取得歷史處方;
                GetPrescriptions();
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        #endregion
    }
}
