using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.PrescriptionRefactoring;
using His_Pos.NewClass.PrescriptionRefactoring.Cooperative;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow;
using Resources = His_Pos.Properties.Resources;
// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow
{
    public class CooperativePrescriptionViewModel : ViewModelBase
    {
        #region Variables
        private CooperativePreBases cooperativePres { get; set; }
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
        private CooperativePreBase selectedPrescription;
        public CooperativePreBase SelectedPrescription
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
        public RelayCommand PrintMedBag { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public RelayCommand Refresh { get; set; }
        #endregion
        public CooperativePrescriptionViewModel()
        {
            InitPrescriptions();
            InitCommands();
        }

        private void InitPrescriptions()
        {
            cooperativePres = new CooperativePreBases();
            var getCooperativePresWorker = new BackgroundWorker();
            getCooperativePresWorker.DoWork += (o, ea) =>
            {
                BusyContent = Resources.取得合作處方;
                MainWindow.ServerConnection.OpenConnection();
                cooperativePres.GetCooperative(DateTime.Today.AddDays(-10), DateTime.Today);
                MainWindow.ServerConnection.CloseConnection();
            };
            getCooperativePresWorker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                CooPreCollectionViewSource = new CollectionViewSource { Source = cooperativePres };
                CooPreCollectionView = CooPreCollectionViewSource.View;
                cooPreCollectionViewSource.Filter += Filter;
                IsRead = false;
                IsNotRead = true;
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;
            };
            IsBusy = true;
            getCooperativePresWorker.RunWorkerAsync();
        }

        private void InitCommands()
        {
            PrintMedBag = new RelayCommand(PrintAction);
            PrescriptionSelected = new RelayCommand(PrescriptionSelectedAction);
            Refresh = new RelayCommand(RefreshAction);
        }

        #region Actions
        private void PrintAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            SelectedPrescription?.Print();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void PrescriptionSelectedAction()
        {
            throw new NotImplementedException();
        }

        private void RefreshAction()
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Filter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is CooperativePreBase src))
                e.Accepted = false;
            else if (StartDate is null || DateTime.Compare(src.TreatDate, (DateTime)StartDate) >= 0)
            {
                if (EndDate is null || DateTime.Compare(src.TreatDate, (DateTime)EndDate) <= 0)
                {
                    if (string.IsNullOrEmpty(IDNumber) || src.Patient.IDNumber.Contains(IDNumber))
                    {
                        if (IsRead && IsNotRead)
                            e.Accepted = true;
                        else if (IsRead && !IsNotRead)
                            e.Accepted = src.IsRead;
                        else if (IsNotRead && !IsRead)
                            e.Accepted = !src.IsRead;
                        else
                            e.Accepted = false;
                    }
                    else
                        e.Accepted = false;
                }
                else
                    e.Accepted = false;
            }
            else
                e.Accepted = false;
        }
    }
}
