using System;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.PrescriptionRefactoring;
// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CooperativePrescriptionWindow
{
    public class CooperativePrescriptionViewModel : ViewModelBase
    {
        #region Variables
        private Prescriptions cooperativePres { get; set; }
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
        #endregion

        public CooperativePrescriptionViewModel()
        {
            InitPrescriptions();
        }

        private void InitPrescriptions()
        {
            cooperativePres = new Prescriptions();
            var getCooperativePresWorker = new BackgroundWorker();
            getCooperativePresWorker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.取得合作處方;
                XmlOfPrescriptions.GetFile();
                cooperativePres.GetCooperative(DateTime.Today.AddDays(-10), DateTime.Today);
            };
            getCooperativePresWorker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            getCooperativePresWorker.RunWorkerAsync();
        }
    }
}
