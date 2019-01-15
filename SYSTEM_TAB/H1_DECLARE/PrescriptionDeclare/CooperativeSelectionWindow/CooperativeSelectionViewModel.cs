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

        #region Command

        private RelayCommand startDateChanged;
        public RelayCommand StartDateChanged
        {
            get =>
                startDateChanged ??
                (startDateChanged = new RelayCommand(ExecuteStartDateChanged));
            set => startDateChanged = value;
        }
        private void ExecuteStartDateChanged()
        {
            if (StartDate is null)
                CooPreCollectionViewSource.Filter -= FilterByStartDate;
            else
                CooPreCollectionViewSource.Filter += FilterByStartDate;
        }

        private RelayCommand endDateChanged;
        public RelayCommand EndDateChanged
        {
            get =>
                endDateChanged ??
                (endDateChanged = new RelayCommand(ExecuteEndDateChanged));
            set => endDateChanged = value;
        }
        private void ExecuteEndDateChanged()
        {
            if (EndDate is null)
                CooPreCollectionViewSource.Filter -= FilterByEndDate;
            else
                CooPreCollectionViewSource.Filter += FilterByEndDate;
        }

        private RelayCommand idNumberChanged;
        public RelayCommand IDNumberChanged
        {
            get =>
                idNumberChanged ??
                (idNumberChanged = new RelayCommand(ExecuteIDNumberChanged));
            set => idNumberChanged = value;
        }
        private void ExecuteIDNumberChanged()
        {
            if (string.IsNullOrEmpty(IDNumber))
                CooPreCollectionViewSource.Filter -= FilterByIDNumber;
            else
                CooPreCollectionViewSource.Filter += FilterByIDNumber;
        }

        private RelayCommand isReadChecked;
        public RelayCommand IsReadChecked
        {
            get =>
                isReadChecked ??
                (isReadChecked = new RelayCommand(ExecuteIsReadChecked));
            set => isReadChecked = value;
        }
        private void ExecuteIsReadChecked()
        {
            CooPreCollectionViewSource.Filter += FilterByIsRead;
        }

        private RelayCommand selectionChanged;
        public RelayCommand SelectionChanged
        {
            get =>
                selectionChanged ??
                (selectionChanged = new RelayCommand(ExecuteSelectionChanged));
            set => selectionChanged = value;
        }
        private void ExecuteSelectionChanged()
        {
            if (SelectedPrescription != null)
                CustomerHistories = new CooperativeViewHistories(SelectedPrescription.Patient.Id);
        }

        private RelayCommand printMedBag;
        public RelayCommand PrintMedBag
        {
            get =>
                printMedBag ??
                (printMedBag = new RelayCommand(ExecutePrintMedBag));
            set => printMedBag = value;
        }
        private void ExecutePrintMedBag()
        {
            SelectedPrescription.PrintMedBag();
        }

        private RelayCommand<Window> prescriptionSelected;
        public RelayCommand<Window> PrescriptionSelected
        {
            get =>
                prescriptionSelected ??
                (prescriptionSelected = new RelayCommand<Window>(ExecutePrescriptionSelected));
            set => prescriptionSelected = value;
        }
        private void ExecutePrescriptionSelected(Window window)
        {
            MainWindow.ServerConnection.OpenConnection();
            SelectedPrescription.Patient = SelectedPrescription.Patient.Check(); 
            SelectedPrescription.Treatment.MainDisease = SelectedPrescription.Treatment.MainDisease.GetDataByCodeId();
            SelectedPrescription.Treatment.SubDisease = SelectedPrescription.Treatment.SubDisease.GetDataByCodeId();
            Messenger.Default.Send(SelectedPrescription, "SelectedPrescription");
            Messenger.Default.Send(new NotificationMessage("CloseCooperativeSelection"));
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send<Prescription>(SelectedPrescription, "SelectedPrescription");
            window?.Close();
        }
        #endregion

        public CooperativeSelectionViewModel()
        {
            PrescriptionSelected = new RelayCommand<Window>(ExecutePrescriptionSelected);
            Messenger.Default.Register<Prescriptions>(this, "CooperativePrescriptions", GetCooperativePrescription);
        }

        ~CooperativeSelectionViewModel()
        {
            Messenger.Default.Unregister(this);
        }

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

        #region Filter
        private void FilterByStartDate(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (StartDate is null)
                e.Accepted = true;
            else if (DateTime.Compare(src.Treatment.TreatDate, (DateTime)StartDate) >= 0)
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
            else if (DateTime.Compare(src.Treatment.TreatDate, (DateTime)EndDate) <= 0)
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
                if(StartDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)StartDate) < 0)
                    e.Accepted = false;
                else if (EndDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)EndDate) > 0)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else if (IsNotRead != null && (!src.PrescriptionStatus.IsRead && (bool)IsNotRead))
            {
                if (StartDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)StartDate) < 0)
                    e.Accepted = false;
                else if (EndDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)EndDate) > 0)
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
