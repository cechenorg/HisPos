﻿using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePharmacy;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Declare.DeclarePreview;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using StringRes = His_Pos.Properties.Resources;

// ReSharper disable InconsistentNaming
namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    // ReSharper disable once ClassTooBig
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DeclareFileManageViewModel : TabBase
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

        private DateTime? declareDateStart;

        public DateTime? DeclareDateStart
        {
            get => declareDateStart;
            set
            {
                if (value != null)
                    Set(() => DeclareDateStart, ref declareDateStart, value);
            }
        }

        private DateTime? declareDateEnd;

        public DateTime? DeclareDateEnd
        {
            get => declareDateEnd;
            set
            {
                if (value != null)
                    Set(() => DeclareDateEnd, ref declareDateEnd, value);
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

        #endregion Variables

        #region Commands

        public RelayCommand GetPreviewPrescriptions { get; set; }
        public RelayCommand AdjustPharmacistSetting { get; set; }
        public RelayCommand AdjustPharmacistOfDay { get; set; }
        public RelayCommand AdjustPharmacistOfMonth { get; set; }
        public RelayCommand ShowPrescriptionEditWindow { get; set; }
        public RelayCommand SetDecFilePreViewSummary { get; set; }
        public RelayCommand CreateDeclareFileCommand { get; set; }
        public RelayCommand AddToEditListCommand { get; set; }

        #endregion Commands

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
            if (DeclarePharmacies.Count==0) 
            {
                return;  
            }
            SelectedPharmacy = DeclarePharmacies.SingleOrDefault(p => p.ID.Equals(ViewModelMainWindow.CurrentPharmacy.ID));
            DeclareFile = new DeclarePreviewOfMonth();
            DeclareDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            var decDate = (DateTime)DeclareDateStart;
            DeclareDateEnd = new DateTime(decDate.Year, decDate.Month, DateTime.DaysInMonth(decDate.Year, decDate.Month));
            GetPharmacistSchedule();
            DeclareFile.GetNotAdjustPrescriptionCount(DeclareDateStart, DeclareDateEnd, SelectedPharmacy.ID);
            var duplicatePrescriptionWindow = new DuplicatePrescriptionWindow.DuplicatePrescriptionWindow(decDate, (DateTime)DeclareDateEnd);
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

        #endregion Initial

        #region CommandActions

        private void GetPreviewPrescriptionsActions()
        {
            if (!CheckStartOrEndDayNull()) return;
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
            if (!CheckStartOrEndDayNull()) return;
            MainWindow.ServerConnection.OpenConnection();
            GetPrescriptions();
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckStartOrEndDayNull()
        {
            if (DeclareDateStart is null || DeclareDateEnd is null)
            {
                MessageWindow.ShowMessage("請填寫申報日期區間", MessageType.ERROR);
                return false;
            }
            return true;
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void AdjustPharmacistSettingAction()
        {
            if (!CheckStartOrEndDayNull()) return;
            var decDate = (DateTime)DeclareDateStart;
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
            if (DeclareDateStart is null || DeclareDateEnd is null)
            {
                MessageWindow.ShowMessage("尚未填寫申報日期區間。", MessageType.WARNING);
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
            Debug.Assert(DeclareDateStart != null, nameof(DeclareDateStart) + " != null");
            var decDate = (DateTime)DeclareDateStart;
            MainWindow.ServerConnection.OpenConnection();
            BusyContent = "處方排序中...";
            DeclareFile.DeclarePres.AdjustMedicalServiceAndSerialNumber();
            MainWindow.ServerConnection.CloseConnection();
            BusyContent = StringRes.產生申報資料;
            var decFile = new DeclareFile(DeclareFile, SelectedPharmacy.ID, (DateTime)DeclareDateStart, (DateTime)DeclareDateEnd);
            DeclareFile.CreateDeclareFile(decFile, (DateTime)DeclareDateStart, (DateTime)DeclareDateEnd);
        }

        private void AddToEditListAction()
        {
            SetDecFilePreViewSummaryAction();
            DeclareFile.SelectedDayPreview.CheckNotDeclareCount();
        }

        #endregion CommandActions

        private void GetPrescriptions()
        {
            Debug.Assert(DeclareDateStart != null, nameof(DeclareDateStart) + " != null");
            Debug.Assert(DeclareDateEnd != null, nameof(DeclareDateEnd) + " != null");
            var sDate = (DateTime)DeclareDateStart;
            var eDate = (DateTime)DeclareDateEnd;

            if (SelectedPharmacy==null) 
            {
                NewFunction.ShowMessageFromDispatcher("查無處方！", MessageType.ERROR);
                //MessageWindow.ShowMessage("查無處方！", MessageType.ERROR);
                return;
            }
            DeclareFile.GetSearchPrescriptions(sDate, eDate, SelectedPharmacy.ID);
            DeclareFile.SetSummary();
            DeclareFile.DeclareDate = (DateTime)DeclareDateStart;
        }

        private void PrescriptionEditedRefresh(NotificationMessage msg)
        {
            if (msg.Notification.Equals("PrescriptionEdited"))
                Refresh();
        }

        private void GetPharmacistSchedule()
        {
            Debug.Assert(DeclareDateStart != null, nameof(DeclareDateStart) + " != null");
            Debug.Assert(DeclareDateEnd != null, nameof(DeclareDateEnd) + " != null");
            var sDate = (DateTime)DeclareDateStart;
            var eDate = (DateTime)DeclareDateEnd;
            MainWindow.ServerConnection.OpenConnection();
            PharmacistSchedule = new PharmacistSchedule();
            PharmacistSchedule.GetPharmacistSchedule(sDate, eDate);
            PharmacistScheduleWithPrescriptionCount = new PharmacistSchedule();
            PharmacistScheduleWithPrescriptionCount.GetPharmacistScheduleWithCount(sDate, eDate);
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

        #endregion Functions
    }
}