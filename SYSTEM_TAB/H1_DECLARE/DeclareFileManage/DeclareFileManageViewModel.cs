using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePharmacy;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Declare.DeclarePreview;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting;
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
        private int? startDay;
        public int? StartDay
        {
            get => startDay;
            set
            {
                Set(() => StartDay, ref startDay, value);
            }
        }
        private int? endDay;
        public int? EndDay
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
            StartDay = 1;
            EndDay = DateTime.DaysInMonth(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month);
            GetPharmacistSchedule();
            DeclareFile.GetNotAdjustPrescriptionCount((DateTime)DeclareDate, new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month, (int)EndDay), SelectedPharmacy.ID);
            EditedList = new DeclarePrescriptions();
        }
        private void InitialCommands()
        {
            GetPreviewPrescriptions = new RelayCommand(GetPreviewPrescriptionsActions);
            AdjustPharmacistSetting = new RelayCommand(AdjustPharmacistSettingAction);
            AdjustPharmacistOfDay = new RelayCommand(AdjustPharmacistOfDayAction,CheckDayIsOutOfRange);
            AdjustPharmacistOfMonth = new RelayCommand(AdjustPharmacistOfMonthAction,CheckMonthIsOutOfRange);
            ShowPrescriptionEditWindow = new RelayCommand(ShowPrescriptionEditWindowAction);
            SetDecFilePreViewSummary = new RelayCommand(SetDecFilePreViewSummaryAction);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFileAction);
            AddToEditListCommand = new RelayCommand(AddToEditListAction);
        }
        private bool CheckDayIsOutOfRange()
        {
            return DeclareFile.SelectedDayPreview != null && DeclareFile.SelectedDayPreview.IsAdjustOutOfRange;
        }

        private bool CheckMonthIsOutOfRange()
        {
            return DeclareFile.DeclarePreviews.Count(pre => pre.IsAdjustOutOfRange) > 0;
        }
        #endregion
        #region CommandActions
        private void GetPreviewPrescriptionsActions()
        {
            if (StartDay == null || EndDay == null)
            {
                MessageWindow.ShowMessage("請填寫查詢日期區間",MessageType.ERROR);
                return;
            }

            if (DeclareDate is null)
            {
                MessageWindow.ShowMessage("請填寫申報年月", MessageType.ERROR);
                return;
            }
            var worker = new BackgroundWorker();
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
        private void AdjustPharmacistSettingAction()
        {
            var adjustPharmacistWindow = new AdjustPharmacistWindow(new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month, 1));
            MainWindow.ServerConnection.OpenConnection();
            GetPharmacistSchedule();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void AdjustPharmacistOfDayAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "藥師調整處理中...";
                DeclareFile.SelectedDayPreview.PresOfDay.AdjustPharmacist(GetAdjustPharmacist(false));
                MainWindow.ServerConnection.OpenConnection();
                DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
                MainWindow.ServerConnection.CloseConnection();
                foreach (var pre in DeclareFile.DeclarePreviews)
                {
                    pre.CheckAdjustOutOfRange();
                }
                DeclareFile.SetSummary();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void AdjustPharmacistOfMonthAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "藥師調整處理中...";
                var adjustList = new DeclarePrescriptions();
                foreach (var pre in DeclareFile.DeclarePreviews.Where(pre => pre.IsAdjustOutOfRange))
                {
                    foreach (var dec in pre.PresOfDay)
                    {
                        adjustList.Add(dec);
                    }
                }
                adjustList.AdjustPharmacist(GetAdjustPharmacist(true));
                MainWindow.ServerConnection.OpenConnection();
                DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
                MainWindow.ServerConnection.CloseConnection();
                foreach (var pre in DeclareFile.DeclarePreviews)
                {
                    pre.CheckAdjustOutOfRange();
                }
                foreach (var pre in DeclareFile.DeclarePreviews.Where(pre => !pre.IsAdjustOutOfRange))
                {
                    foreach (var dec in pre.PresOfDay)
                    {
                        adjustList.Add(dec);
                    }
                }
                DeclareFile.SetSummary();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void ShowPrescriptionEditWindowAction()
        {
            if (DeclareFile.SelectedDayPreview.SelectedPrescription is null) return;
            MainWindow.ServerConnection.OpenConnection();
            var selected = new Prescription(PrescriptionDb.GetPrescriptionByID(DeclareFile.SelectedDayPreview.SelectedPrescription.ID).Rows[0], PrescriptionSource.Normal);
            MainWindow.ServerConnection.CloseConnection();
            var prescriptionEdit = new PrescriptionEditWindow(selected.Id);
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            prescriptionEdit.ShowDialog();
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }

        private void SetDecFilePreViewSummaryAction()
        {
            DeclareFile.SetSummary();
        }
        private void CreateDeclareFileAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.產生申報資料;
                MainWindow.ServerConnection.OpenConnection();
                DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
                DeclareFile.DeclarePres.SerializeFileContent();
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                var decFile = new DeclareFile(DeclareFile, SelectedPharmacy.ID);
                DeclareFile.CreateDeclareFile(decFile);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void AddToEditListAction()
        {
            SetDecFilePreViewSummaryAction();
            DeclareFile.SelectedDayPreview.CheckNotDeclareCount();
        }
        #endregion
        private void GetPrescriptions()
        {
            var end = new DateTime(((DateTime)DeclareDate).Year, ((DateTime)DeclareDate).Month+1, 1).AddDays(-1).Day;
            if (EndDay > end)
                EndDay = end;
            var sDate = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, (int)StartDay);
            var eDate = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, (int)EndDay);
            DeclareFile.GetSearchPrescriptions(sDate, eDate, SelectedPharmacy.ID);
            DeclareFile.SetSummary();
            DeclareFile.DeclareDate = (DateTime)DeclareDate;
        }
        private void Refresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals("PrescriptionEdited"))
            {
                MainWindow.ServerConnection.OpenConnection();
                GetPrescriptions();
                MainWindow.ServerConnection.CloseConnection();
            }
        }

        private void GetPharmacistSchedule()
        {
            var start = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, (int)StartDay);
            var last = DateTimeExtensions.GetDateTimeWithDay(DeclareDate, (int)EndDay);
            PharmacistSchedule = new PharmacistSchedule();
            PharmacistSchedule.GetPharmacistSchedule(start, last);
        }

        private List<PharmacistScheduleItem> GetAdjustPharmacist(bool month)
        {
            var pharmacistList = new List<PharmacistScheduleItem>();
            var schedule = new List<PharmacistScheduleItem>();
            if (month)
            {
                schedule = PharmacistSchedule.ToList();
                foreach (var s in schedule)
                {
                    pharmacistList.Add(s);
                }
            }
            else
            {
                schedule = PharmacistSchedule.Where(p => p.Date.Equals(DeclareFile.SelectedDayPreview.Date)).ToList();
                foreach (var s in schedule)
                {
                    pharmacistList.Add(s);
                }
            }
            return pharmacistList;
        }
        #endregion
    }
}
