using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.ControlMedicineDeclare;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

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

        #endregion Properties

        #region Commands

        public RelayCommand GetData { get; set; }
        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }

        #endregion Commands

        public ControlMedicineUsageViewModel()
        {
            InitSearchDate();
            GetData = new RelayCommand(GetDataAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            GetDataAction();
        }

        private void GetDataAction()
        {
            if (StartDate is null)
            {
                MessageWindow.ShowMessage("起始日期格式錯誤。", MessageType.ERROR);
                return;
            }
            if (EndDate is null)
            {
                MessageWindow.ShowMessage("結束日期格式錯誤。", MessageType.ERROR);
                return;
            }
            ControlMedicineDeclares = new ControlMedicineDeclares();
            ControlMedicineDeclares.GetUsageData((DateTime)StartDate, (DateTime)EndDate);
            ControlCollectionViewSource = new CollectionViewSource { Source = ControlMedicineDeclares };
            ControlCollectionView = ControlCollectionViewSource.View;
        }

        private void DateMouseDoubleClickAction(MaskedTextBox sender)
        {
            switch (sender.Name)
            {
                case "StartDate":
                    StartDate = DateTime.Today;
                    break;

                case "EndDate":
                    EndDate = DateTime.Today;
                    break;
            }
        }

        private void InitSearchDate()
        {
            StartDate = DateTime.Today.AddDays(-1);
            EndDate = DateTime.Today;
        }
    }
}