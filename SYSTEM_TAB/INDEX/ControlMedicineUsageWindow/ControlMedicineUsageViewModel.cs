using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Medicine.ControlMedicineDeclare;

namespace His_Pos.SYSTEM_TAB.INDEX.ControlMedicineUsageWindow
{
    public class ControlMedicineUsageViewModel : ViewModelBase
    {
        #region Properties

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private CollectionViewSource controlCollectionViewSource;
        private CollectionViewSource ControlCollectionViewSource
        {
            get => controlCollectionViewSource;
            set
            {
                Set(() => ControlCollectionViewSource, ref controlCollectionViewSource, value);
            }
        }

        private ICollectionView controlCollectionView;
        public ICollectionView ControlCollectionView
        {
            get => controlCollectionView;
            private set
            {
                Set(() => ControlCollectionView, ref controlCollectionView, value);
            }
        }


        private ControlMedicineDeclares controlMedicineDeclares;
        public ControlMedicineDeclares ControlMedicineDeclares
        {
            get { return controlMedicineDeclares; }
            set
            {
                Set(() => ControlMedicineDeclares, ref controlMedicineDeclares, value);
            }
        }

        private ControlMedicineDeclare selectedItem;
        public ControlMedicineDeclare SelectedItem
        {
            get { return selectedItem; }
            set
            {
                Set(() => SelectedItem, ref selectedItem, value);
            }
        }
        #endregion

        #region Commands

        public RelayCommand GetData { get; set; }
        

        #endregion
        public ControlMedicineUsageViewModel()
        {
            InitSearchDate();
            GetData = new RelayCommand(GetDataAction);
            GetDataAction();
        }

        private void GetDataAction()
        {
            ControlMedicineDeclares = new ControlMedicineDeclares();
            ControlMedicineDeclares.GetUsageData((DateTime)StartDate, (DateTime)EndDate);
            ControlCollectionViewSource = new CollectionViewSource {Source = ControlMedicineDeclares};
            ControlCollectionView = ControlCollectionViewSource.View;
        }

        private void InitSearchDate()
        {
            StartDate = DateTime.Today.AddDays(-1);
            EndDate = DateTime.Today;
        }
    }
}
