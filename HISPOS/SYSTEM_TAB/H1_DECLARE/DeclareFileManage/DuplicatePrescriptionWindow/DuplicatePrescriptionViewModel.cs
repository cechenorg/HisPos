using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.DuplicatePrescription;
using System;
using System.ComponentModel;
using System.Windows.Data;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.DuplicatePrescriptionWindow
{
    public class DuplicatePrescriptionViewModel : ViewModelBase
    {
        private DuplicatePrescriptions prescriptions;

        public DuplicatePrescriptions Prescriptions
        {
            get => prescriptions; set
            {
                Set(() => Prescriptions, ref prescriptions, value);
            }
        }

        private CollectionViewSource presCollectionViewSource;

        public CollectionViewSource PresCollectionViewSource
        {
            get => presCollectionViewSource;
            set
            {
                Set(() => PresCollectionViewSource, ref presCollectionViewSource, value);
            }
        }

        private ICollectionView presCollectionView;

        public ICollectionView PresCollectionView
        {
            get => presCollectionView;
            set
            {
                Set(() => PresCollectionView, ref presCollectionView, value);
            }
        }

        private string title;

        public string Title
        {
            get => title;
            set
            {
                Set(() => Title, ref title, value);
            }
        }

        private bool showDialog;

        public bool ShowDialog
        {
            get => showDialog;
            set
            {
                Set(() => ShowDialog, ref showDialog, value);
            }
        }

        public DuplicatePrescriptionViewModel()
        {
        }

        public DuplicatePrescriptionViewModel(DateTime startDate, DateTime endDate)
        {
            Title = VM.CurrentPharmacy.Name + " " + (startDate.Year - 1911) + "年" + startDate.Month + "月 重複處方";
            MainWindow.ServerConnection.OpenConnection();
            Prescriptions = new DuplicatePrescriptions();
            Prescriptions.GetData(startDate, endDate);
            MainWindow.ServerConnection.CloseConnection();
            ShowDialog = Prescriptions.Count > 0;
            PresCollectionViewSource = new CollectionViewSource { Source = Prescriptions };
            PresCollectionView = PresCollectionViewSource.View;
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("PatientData");
            PresCollectionView.GroupDescriptions.Add(groupDescription);
        }
    }
}