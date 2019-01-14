using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
            set { cooPreCollectionViewSource = value; RaisePropertyChanged(() => CooPreCollectionViewSource); }
        }

        private ICollectionView cooPreCollectionView;
        public ICollectionView CooPreCollectionView
        {
            get => cooPreCollectionView;
            set { cooPreCollectionView = value; RaisePropertyChanged(() => CooPreCollectionView); }
        }

        private Prescriptions cooperativePrescriptions;
        private Prescriptions CooperativePrescriptions
        {
            get => cooperativePrescriptions;
            set { cooperativePrescriptions = value; RaisePropertyChanged(() => CooperativePrescriptions); }
        }

        private CooperativeViewHistories customerHistories;
        public CooperativeViewHistories CustomerHistories
        {
            get => customerHistories;
            private set { customerHistories = value; RaisePropertyChanged(() => CustomerHistories); }
        }

        private CooperativeViewHistory selectedHistory;
        public CooperativeViewHistory SelectedHistory
        {
            get => selectedHistory;
            set { selectedHistory = value; RaisePropertyChanged(() => SelectedHistory); }
        }

        private Prescription selectedPrescription;
        public Prescription SelectedPrescription
        {
            get => selectedPrescription;
            set { selectedPrescription = value; RaisePropertyChanged(() => SelectedPrescription); }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set { startDate = value; RaisePropertyChanged(() => StartDate); }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set { endDate = value; RaisePropertyChanged(() => EndDate); }
        }

        private string idNumber;
        public string IDNumber
        {
            get => idNumber;
            set { idNumber = value; RaisePropertyChanged(() => IDNumber); }
        }

        private bool? isRead;
        public bool? IsRead
        {
            get => isRead;
            set { isRead = value; RaisePropertyChanged(() => IsRead); }
        }

        private bool? isNotRead;
        public bool? IsNotRead
        {
            get => isNotRead;
            set { isNotRead = value; RaisePropertyChanged(() => IsNotRead); }
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

        private RelayCommand endDateChagned;
        public RelayCommand EndDateChanged
        {
            get =>
                endDateChagned ??
                (endDateChagned = new RelayCommand(ExecuteEndDateChanged));
            set => endDateChagned = value;
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
            window?.Close();
        }
        #endregion

        public CooperativeSelectionViewModel()
        {
            PrescriptionSelected = new RelayCommand<Window>(ExecutePrescriptionSelected);
            CooperativePrescriptions = new Prescriptions();
            CooperativePrescriptions.GetCooperativePrescriptions();
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
            else if (IsRead != null && (src.IsRead && (bool)IsRead))
            {
                if(StartDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)StartDate) < 0)
                    e.Accepted = false;
                else if (EndDate != null && DateTime.Compare(src.Treatment.TreatDate, (DateTime)EndDate) > 0)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else if (IsNotRead != null && (!src.IsRead && (bool)IsNotRead))
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
