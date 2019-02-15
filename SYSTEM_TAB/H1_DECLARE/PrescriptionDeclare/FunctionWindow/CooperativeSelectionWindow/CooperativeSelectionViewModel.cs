using System;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeSelectionWindow
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
                CooPreCollectionViewSource.Filter += Filter;
            }
        }

        private DateTime? endDate = DateTime.Today;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
                CooPreCollectionViewSource.Filter += Filter;
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
                CooPreCollectionViewSource.Filter += Filter;
            }
        }

        private bool isRead;
        public bool IsRead
        {
            get => isRead;
            set
            {
                Set(() => IsRead, ref isRead, value);
                CooPreCollectionViewSource.Filter += Filter;
            }
        }

        private bool isNotRead;
        public bool IsNotRead
        {
            get => isNotRead;
            set
            {
                Set(() => IsNotRead, ref isNotRead, value);
                CooPreCollectionViewSource.Filter += Filter;
            }
        }
        #endregion
        #region Commands
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
            SelectionChanged = new RelayCommand(SelectionChangedAction);
            PrintMedBag = new RelayCommand(PrintAction);
            PrescriptionSelected = new RelayCommand(PrescriptionSelectedAction);
        }
        #endregion
        #region CommandActions
        private void IDNumberChangedAction()
        {
            CooPreCollectionViewSource.Filter += Filter;
        }
        private void SelectionChangedAction()
        {
            if (SelectedPrescription is null) return;
            CustomerHistories = new CooperativeViewHistories(SelectedPrescription.Patient.IDNumber);
            CustomerHistories.Insert(0,new CooperativeViewHistory(SelectedPrescription));
        }
        private void PrintAction()
        {
            if(SelectedPrescription is null) return;
            SelectedPrescription.GetCompletePrescriptionData(false,true);
            SelectedPrescription.CountPrescriptionPoint();
            var medBagPrint = new ConfirmWindow("是否列印藥袋", "列印確認");
            if ((bool)medBagPrint.DialogResult)
            {
                var printBySingleMode = new MedBagSelectionWindow();
                var singleMode = (bool)printBySingleMode.ShowDialog();
                var receiptPrint = false;
                if (SelectedPrescription.PrescriptionPoint.AmountsPay > 0)
                {
                    var receiptResult = new ConfirmWindow(Resources.PrintReceipt, Resources.PrintConfirm);
                    if (receiptResult.DialogResult != null)
                        receiptPrint = (bool)receiptResult.DialogResult;
                }
                SelectedPrescription.PrintMedBag(singleMode);
                if(receiptPrint)
                    SelectedPrescription.PrintReceipt();
            }
        }
        private void PrescriptionSelectedAction()
        {
            if(SelectedPrescription is null) return;
            SelectedPrescription.GetCompletePrescriptionData(true,true);
            Messenger.Default.Send(SelectedPrescription, "SelectedPrescription");
            Messenger.Default.Send(new NotificationMessage("CloseCooperativeSelection"));
        }
        #endregion
        #region Functions
        private void GetCooperativePrescription(Prescriptions receivePrescriptions)
        {
            CooperativePrescriptions = new Prescriptions();
            CooperativePrescriptions = receivePrescriptions;
            CooPreCollectionViewSource = new CollectionViewSource { Source = CooperativePrescriptions };
            CooPreCollectionView = CooPreCollectionViewSource.View;
            IsRead = false;
            IsNotRead = true;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            cooPreCollectionViewSource.Filter += Filter;
        }
        #endregion
        #region Filter
        private void Filter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Prescription src))
                e.Accepted = false;
            else if (StartDate is null || DateTime.Compare((DateTime) src.Treatment.TreatDate, (DateTime) StartDate) >= 0)
            {
                if (EndDate is null || DateTime.Compare((DateTime) src.Treatment.TreatDate, (DateTime) EndDate) <= 0)
                {
                    if (string.IsNullOrEmpty(IDNumber) || src.Patient.IDNumber.Contains(IDNumber))
                    {
                        if (IsRead && IsNotRead)
                            e.Accepted = true;
                        else if (IsRead && !IsNotRead)
                        {
                            e.Accepted = src.PrescriptionStatus.IsRead;
                        }
                        else if(IsNotRead && !IsRead)
                        {
                            e.Accepted = !src.PrescriptionStatus.IsRead;
                        }
                        else
                        {
                            e.Accepted = false;
                        }
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = false;
                }
            }
            else
            {
                e.Accepted = false;
            }
        }
        #endregion
    }
}
