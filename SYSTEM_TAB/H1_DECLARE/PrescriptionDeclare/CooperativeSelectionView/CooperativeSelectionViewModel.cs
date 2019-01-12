using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionView
{
    public class CooperativeSelectionViewModel : ViewModelBase
    {
        #region Property
        private CollectionViewSource cooPreCollectionViewSource;
        public CollectionViewSource CooPreCollectionViewSource
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
        public Prescriptions CooperativePrescriptions
        {
            get => cooperativePrescriptions;
            set { cooperativePrescriptions = value; RaisePropertyChanged(() => CooperativePrescriptions); }
        }

        private CooperativeViewHistories customerHistories;
        public CooperativeViewHistories CustomerHistories
        {
            get => customerHistories;
            set { customerHistories = value; RaisePropertyChanged(() => CustomerHistories); }
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

        private bool isRead;
        public bool IsRead
        {
            get => isRead;
            set { isRead = value; RaisePropertyChanged(() => IsRead); }
        }

        private bool isNotRead;
        public bool IsNotRead
        {
            get => isNotRead;
            set { isNotRead = value; RaisePropertyChanged(() => IsNotRead); }
        }
        #endregion
        #region Command
        private RelayCommand startDateChangedCommand;
        public RelayCommand StartDateChangedCommand
        {
            get =>
                startDateChangedCommand ??
                (startDateChangedCommand = new RelayCommand(ExcuteStartDateChangedCommand));
            set => startDateChangedCommand = value;
        }

        private RelayCommand endDateChagnedCommand;
        public RelayCommand EndDateChangedCommand
        {
            get =>
                endDateChagnedCommand ??
                (endDateChagnedCommand = new RelayCommand(ExcuteEndDateChangedCommand));
            set => endDateChagnedCommand = value;
        }

        private RelayCommand idNumberChangedCommand;
        public RelayCommand IDNumberChangedCommand
        {
            get =>
                idNumberChangedCommand ??
                (idNumberChangedCommand = new RelayCommand(ExcuteIDNumberChangedCommand));
            set => idNumberChangedCommand = value;
        }

        private RelayCommand isReadCheckedCommand;
        public RelayCommand IsReadCheckedCommand
        {
            get =>
                isReadCheckedCommand ??
                (isReadCheckedCommand = new RelayCommand(ExcuteIsReadChekedCommand));
            set => isReadCheckedCommand = value;
        }

        private RelayCommand<Window> prescriptionSelectedCommand;
        public RelayCommand<Window> PrescriptionSelectedCommand
        {
            get =>
                prescriptionSelectedCommand ??
                (prescriptionSelectedCommand = new RelayCommand<Window>(ExcutePrescriptionSelectedCommand));
            set => prescriptionSelectedCommand = value;
        }

        private RelayCommand selectionChangedCommand;
        public RelayCommand SelectionChangedCommand
        {
            get =>
                selectionChangedCommand ??
                (selectionChangedCommand = new RelayCommand(ExcuteSelectionChangedCommand));
            set => selectionChangedCommand = value;
        }

        private void ExcuteStartDateChangedCommand()
        {
            if(StartDate is null)
                CooPreCollectionViewSource.Filter -= FilterByStartDate;
            else
                CooPreCollectionViewSource.Filter += FilterByStartDate;
        }

        private void ExcuteEndDateChangedCommand()
        {
            if (EndDate is null)
                CooPreCollectionViewSource.Filter -= FilterByEndDate;
            else
                CooPreCollectionViewSource.Filter += FilterByEndDate;
        }

        private void ExcuteIDNumberChangedCommand()
        {
            if (string.IsNullOrEmpty(IDNumber))
                CooPreCollectionViewSource.Filter -= FilterByIDNumber;
            else
                CooPreCollectionViewSource.Filter += FilterByIDNumber;
        }

        private void ExcuteIsReadChekedCommand()
        {
            CooPreCollectionViewSource.Filter += FilterByIsRead;
        }
        private void ExcuteSelectionChangedCommand()
        {
            if(SelectedPrescription != null)
            CustomerHistories = new CooperativeViewHistories(SelectedPrescription.Patient.Id);
        }
        private void ExcutePrescriptionSelectedCommand(Window window)
        {
            window?.Close();
        }

        
        #endregion
        public CooperativeSelectionViewModel()
        {
            PrescriptionSelectedCommand = new RelayCommand<Window>(ExcutePrescriptionSelectedCommand);
            CooperativePrescriptions = new Prescriptions();
            CooperativePrescriptions.GetCooperativePrescriptions();
            CooPreCollectionViewSource = new CollectionViewSource { Source = CooperativePrescriptions };
            CooPreCollectionView = CooPreCollectionViewSource.View;
            IsNotRead = true;
            IsRead = false;
        }

        private void FilterByStartDate(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (DateTime.Compare(src.Treatment.TreatDate, (DateTime)StartDate) > 0)
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
            else if (DateTime.Compare(src.Treatment.TreatDate, (DateTime)EndDate) < 0)
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
            else if (src.IsRead && IsRead)
            {
                e.Accepted = true;
            }
            else if (!src.IsRead && IsNotRead)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
    }
}
