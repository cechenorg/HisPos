using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow
{
    public class CommonHospitalsViewModel:ViewModelBase
    {
        public RelayCommand<string> FocusUpDownCommand { get; set; }
        public RelayCommand InstitutionSelected { get; set; }
        private Institution selectedInstitution;
        public Institution SelectedInstitution {
            get => selectedInstitution;
            set
            {
                Set(() => SelectedInstitution, ref selectedInstitution, value);
            }
        }
        private CollectionViewSource commonHospitalsCollectionViewSource;

        private CollectionViewSource CommonHospitalsCollectionViewSource
        {
            get => commonHospitalsCollectionViewSource;
            set
            {
                Set(() => CommonHospitalsCollectionViewSource, ref commonHospitalsCollectionViewSource, value);
            }
        }

        private ICollectionView commonHospitalsCollectionView;
        public ICollectionView CommonHospitalsCollectionView
        {
            get => commonHospitalsCollectionView;
            private set
            {
                Set(() => CommonHospitalsCollectionView, ref commonHospitalsCollectionView, value);
            }
        }
        private Institutions commonHospitalsCollection;
        public Institutions CommonHospitalsCollection
        {
            get => commonHospitalsCollection;
            set
            {
                Set(() => CommonHospitalsCollection, ref commonHospitalsCollection, value);
            }
        }
        private ViewModelEnum viewModel { get; set; }
        public CommonHospitalsViewModel(ViewModelEnum viewModelEnum)
        {
            viewModel = viewModelEnum;
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            InstitutionSelected = new RelayCommand(InstitutionSelectedAction);
            CommonHospitalsCollection = new Institutions(false);
            CommonHospitalsCollection.GetCommon();
            CommonHospitalsCollectionViewSource = new CollectionViewSource { Source = CommonHospitalsCollection };
            CommonHospitalsCollectionView = CommonHospitalsCollectionViewSource.View;
            SelectedInstitution = (Institution)CommonHospitalsCollectionView.Cast<object>().First();
        }
        private void FocusUpDownAction(string direction)
        {
            if (CommonHospitalsCollection.Count > 0)
            {
                int maxIndex = CommonHospitalsCollection.Count - 1;

                switch (direction)
                {
                    case "UP":
                        if (CommonHospitalsCollectionView.CurrentPosition > 0)
                            CommonHospitalsCollectionView.MoveCurrentToPrevious();
                        break;
                    case "DOWN":
                        if (CommonHospitalsCollectionView.CurrentPosition < maxIndex)
                            CommonHospitalsCollectionView.MoveCurrentToNext();
                        break;
                }
                SelectedInstitution = (Institution)CommonHospitalsCollectionView.CurrentItem;
            }
        }
        private void InstitutionSelectedAction()
        {
            switch (viewModel)
            {
                case ViewModelEnum.PrescriptionDeclare:
                    Messenger.Default.Send(SelectedInstitution, nameof(PrescriptionDeclareViewModel) + "InsSelected");
                    break;
                case ViewModelEnum.PrescriptionEdit:
                    Messenger.Default.Send(SelectedInstitution, nameof(PrescriptionEditViewModel) + "InsSelected");
                    break;
            }
            Messenger.Default.Send(new NotificationMessage("CloseCommonHospitalsWindow"));
        }
    }
}
