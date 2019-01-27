using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow
{
    public class InstitutionSelectionViewModel : ViewModelBase
    {
        public Institutions Institutions { get; set; }
        private CollectionViewSource insCollectionViewSource;
        private CollectionViewSource InsCollectionViewSource
        {
            get => insCollectionViewSource;
            set
            {
                Set(() => InsCollectionViewSource, ref insCollectionViewSource, value);
            }
        }
        private ICollectionView insCollectionView;
        public ICollectionView InsCollectionView
        {
            get => insCollectionView;
            private set
            {
                Set(() => InsCollectionView, ref insCollectionView, value);
                if (!insCollectionView.IsEmpty)
                {
                    InsCollectionViewSource.View.MoveCurrentToFirst();
                    SelectedInstitution = (Institution)InsCollectionViewSource.View.CurrentItem;
                }
            }
        }
        private string search;
        public string Search
        {
            get => search;
            set { Set(() => Search, ref search, value); }
        }
        public RelayCommand SearchTextChanged { get; set; }
        public RelayCommand InstitutionSelected { get; set; }
        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set { Set(() => SelectedInstitution, ref selectedInstitution, value); }
        }
        private void ExecuteSearchTextChanged()
        {
            if(Search.Length < 4) return;
            InsCollectionViewSource.Filter += FilterBySearchText;
        }
        private void ExecuteInstitutionSelected()
        {
            Messenger.Default.Send(SelectedInstitution, "SelectedInstitution");
            Messenger.Default.Send(new NotificationMessage("CloseInstitutionSelection"));
        }
        private void FilterBySearchText(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Institution src))
                e.Accepted = false;
            else if (src.FullName.Contains(Search))
                e.Accepted = true;
            else
            {
                e.Accepted = false;
            }
        }
        public InstitutionSelectionViewModel(string searchText)
        {
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            InstitutionSelected = new RelayCommand(ExecuteInstitutionSelected);
            Search = searchText;
            Institutions = ViewModelMainWindow.Institutions;
            InsCollectionViewSource = new CollectionViewSource { Source = Institutions };
            InsCollectionView = InsCollectionViewSource.View;
            InsCollectionViewSource.Filter += FilterBySearchText;
        }
    }
}
