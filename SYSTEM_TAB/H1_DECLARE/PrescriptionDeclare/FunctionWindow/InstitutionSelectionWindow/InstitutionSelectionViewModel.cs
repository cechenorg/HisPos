using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

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

        private bool isEditing = true;

        public bool IsEditing
        {
            get => isEditing;
            private set
            {
                Set(() => IsEditing, ref isEditing, value);
            }
        }

        public RelayCommand SearchTextChanged { get; set; }
        public RelayCommand InstitutionSelected { get; set; }
        public RelayCommand<string> FocusUpDownCommand { get; set; }

        private Institution selectedInstitution;

        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set { Set(() => SelectedInstitution, ref selectedInstitution, value); }
        }

        public bool ShowDialog { get; private set; }

        private void ExecuteSearchTextChanged()
        {
            if (IsEditing)
            {
                IsEditing = false;
                InsCollectionViewSource.Filter += FilterBySearchText;
                switch (Institutions.Count)
                {
                    case 0:
                        MessageWindow.ShowMessage("查無此院所", MessageType.WARNING);
                        break;

                    default:
                        InsCollectionViewSource.View.MoveCurrentToFirst();
                        SelectedInstitution = (Institution)InsCollectionViewSource.View.CurrentItem;
                        break;
                }
            }
            else
            {
                ExecuteInstitutionSelected();
            }
        }

        private void ExecuteInstitutionSelected()
        {
            Messenger.Default.Send(SelectedInstitution, "GetSelectedInstitution");
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
            Institutions = ViewModelMainWindow.Institutions;
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            InstitutionSelected = new RelayCommand(ExecuteInstitutionSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            var resultList = Institutions.Where(i => i.FullName.Contains(searchText)).ToList();
            var resultCount = resultList.Count();
            switch (resultCount)
            {
                case 0:
                    ShowDialog = false;
                    MessageWindow.ShowMessage("查無此院所", MessageType.WARNING);
                    break;

                case 1:
                    ShowDialog = false;
                    SelectedInstitution = resultList[0];
                    ExecuteInstitutionSelected();
                    break;

                default:
                    ShowDialog = true;
                    break;
            }
            InsCollectionViewSource = new CollectionViewSource { Source = Institutions };
            InsCollectionView = InsCollectionViewSource.View;
            Search = searchText;
            ExecuteSearchTextChanged();
        }

        ~InstitutionSelectionViewModel()
        {
            Search = string.Empty;
            Messenger.Default.Unregister(this);
        }

        private void FocusUpDownAction(string direction)
        {
            if (!IsEditing && Institutions.Count > 0)
            {
                int maxIndex = Institutions.Count - 1;

                switch (direction)
                {
                    case "UP":
                        if (InsCollectionView.CurrentPosition > 0)
                            InsCollectionView.MoveCurrentToPrevious();
                        break;

                    case "DOWN":
                        if (InsCollectionView.CurrentPosition < maxIndex)
                            InsCollectionView.MoveCurrentToNext();
                        break;
                }
                SelectedInstitution = (Institution)InsCollectionView.CurrentItem;
            }
        }
    }
}