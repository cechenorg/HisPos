using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionWindow
{
    public class CooperativeSelectionViewModel : ViewModelBase
    {
        #region Property
        private CollectionViewSource cooPreCollectionViewSource;
        private CollectionViewSource CooPreCollectionViewSource
        {
            get => cooPreCollectionViewSource;
            set
            {
                Set(() => CooPreCollectionViewSource, ref cooPreCollectionViewSource, value);
            }
        }

        private ICollectionView cooPreCollectionView;
        public ICollectionView CooPreCollectionView
        {
            get => cooPreCollectionView;
            private set
            {
                Set(() => CooPreCollectionView, ref cooPreCollectionView, value);
            }
        }

        private Prescriptions cooperativePrescriptions;
        private Prescriptions CooperativePrescriptions
        {
            get => cooperativePrescriptions;
            set
            {
                Set(() => CooperativePrescriptions, ref cooperativePrescriptions, value);
            }
        }

        private CooperativeViewHistories customerHistories;
        public CooperativeViewHistories CustomerHistories
        {
            get => customerHistories;
            private set
            {
                Set(() => CustomerHistories, ref customerHistories, value);
            }
        }

        private CooperativeViewHistory selectedHistory;
        public CooperativeViewHistory SelectedHistory
        {
            get => selectedHistory;
            set
            {
                Set(() => SelectedHistory, ref selectedHistory, value);
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

        private DateTime? startDate = DateTime.Today;
        public DateTime? StartDate 
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? endDate = DateTime.Today;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private string idNumber;
        // ReSharper disable once InconsistentNaming
        public string IDNumber
        {
            get => idNumber;
            set
            {
                Set(() => IDNumber, ref idNumber, value);
            }
        }

        private bool? isRead;
        public bool? IsRead
        {
            get => isRead;
            set
            {
                Set(() => IsRead, ref isRead, value);
            }
        }

        private bool? isNotRead;
        public bool? IsNotRead
        {
            get => isNotRead;
            set
            {
                Set(() => IsNotRead, ref isNotRead, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand StartDateChanged { get; set; }
        public RelayCommand EndDateChanged { get; set; }
        public RelayCommand IDNumberChanged { get; set; }
        public RelayCommand IsReadChecked { get; set; }
        public RelayCommand SelectionChanged { get; set; }
        public RelayCommand PrintMedBag { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        #endregion
        public CooperativeSelectionViewModel()
        {
            InitialCommandActions();
            Messenger.Default.Register<Prescriptions>(this, "CooperativePrescriptions", GetCooperativePrescription);
        }
        ~CooperativeSelectionViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        #region InitialFunctions
        private void InitialCommandActions()
        {
            StartDateChanged = new RelayCommand(ExecuteStartDateChanged);
            EndDateChanged = new RelayCommand(ExecuteEndDateChanged);
            IDNumberChanged = new RelayCommand(ExecuteIDNumberChanged);
            IsReadChecked = new RelayCommand(ExecuteIsReadChecked);
            SelectionChanged = new RelayCommand(ExecuteSelectionChanged);
            PrintMedBag = new RelayCommand(ExecutePrintMedBag);
            PrescriptionSelected = new RelayCommand(ExecutePrescriptionSelected);
        }
        #endregion
        #region CommandActions
        private void ExecuteStartDateChanged()
        {
            if (StartDate is null)
                CooPreCollectionViewSource.Filter -= FilterByStartDate;
            else
                CooPreCollectionViewSource.Filter += FilterByStartDate;
        }
        private void ExecuteEndDateChanged()
        {
            if (EndDate is null)
                CooPreCollectionViewSource.Filter -= FilterByEndDate;
            else
                CooPreCollectionViewSource.Filter += FilterByEndDate;
        }
        private void ExecuteIDNumberChanged()
        {
            if (string.IsNullOrEmpty(IDNumber))
                CooPreCollectionViewSource.Filter -= FilterByIDNumber;
            else
                CooPreCollectionViewSource.Filter += FilterByIDNumber;
        }
        private void ExecuteIsReadChecked()
        {
            CooPreCollectionViewSource.Filter += FilterByIsRead;
        }
        private void ExecuteSelectionChanged()
        {
            if (SelectedPrescription != null)
                CustomerHistories = new CooperativeViewHistories(SelectedPrescription.Patient.Id);
        }
        private void ExecutePrintMedBag()
        {
            SelectedPrescription.PrintMedBag();
        }
        private void ExecutePrescriptionSelected()
        {
            MainWindow.ServerConnection.OpenConnection();
            SelectedPrescription.Patient = SelectedPrescription.Patient.Check();
            SelectedPrescription.Treatment.MainDisease = SelectedPrescription.Treatment.MainDisease.GetDataByCodeId(SelectedPrescription.Treatment.MainDisease.Id);
            SelectedPrescription.Treatment.SubDisease = SelectedPrescription.Treatment.SubDisease.GetDataByCodeId(SelectedPrescription.Treatment.SubDisease.Id);
            Messenger.Default.Send(SelectedPrescription, "SelectedPrescription");
            Messenger.Default.Send(new NotificationMessage("CloseCooperativeSelection"));
            MainWindow.ServerConnection.CloseConnection();
        }
        #endregion
        #region Functions
        private void GetCooperativePrescription(Prescriptions receivePrescriptions)
        {
            CooperativePrescriptions = new Prescriptions();
            CooperativePrescriptions = receivePrescriptions;
            CooPreCollectionViewSource = new CollectionViewSource { Source = CooperativePrescriptions };
            CooPreCollectionView = CooPreCollectionViewSource.View;
            IsNotRead = true;
            IsRead = false;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            CooPreCollectionViewSource.Filter += FilterByStartDate;
            CooPreCollectionViewSource.Filter += FilterByEndDate;
            CooPreCollectionViewSource.Filter += FilterByIsRead;
        }
        #endregion
        #region Filter
        private void FilterByStartDate(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (StartDate is null)
                e.Accepted = true;
            else if (DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)StartDate) >= 0)
                e.Accepted = true;
            else
            {
                e.Accepted = false;
            }
        }
        private void FilterByEndDate(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (EndDate is null)
                e.Accepted = true;
            else if (DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)EndDate) <= 0)
                e.Accepted = true;
            else
            {
                e.Accepted = false;
            }
        }
        private void FilterByIDNumber(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (IDNumber is null || src.Patient.IDNumber.Contains(IDNumber))
                e.Accepted = true;
            else
            {
                e.Accepted = false;
            }
        }
        private void FilterByIsRead(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (IsRead != null && (src.PrescriptionStatus.IsRead && (bool)IsRead))
            {
                if(StartDate != null && DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)StartDate) < 0)
                    e.Accepted = false;
                else if (EndDate != null && DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)EndDate) > 0)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else if (IsNotRead != null && (!src.PrescriptionStatus.IsRead && (bool)IsNotRead))
            {
                if (StartDate != null && DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)StartDate) < 0)
                    e.Accepted = false;
                else if (EndDate != null && DateTime.Compare((DateTime)src.Treatment.TreatDate, (DateTime)EndDate) > 0)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        #endregion
    }
}
