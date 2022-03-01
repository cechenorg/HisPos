using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.AdjustedInstitutionSelectionWindow
{
    public class AdjustedInstitutionSelectionViewModel : ViewModelBase
    {
        private PrescriptionSearchInstitutions institutions;

        public PrescriptionSearchInstitutions Institutions
        {
            get => institutions;
            set { Set(() => Institutions, ref institutions, value); }
        }

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
                }
            }
        }

        private string search;

        public string Search
        {
            get => search;
            set
            {
                Set(() => Search, ref search, value);
                InsCollectionViewSource.Filter += FilterBySearchText;
            }
        }

        private bool isEditing = false;

        public bool IsEditing
        {
            get => isEditing;
            private set
            {
                Set(() => IsEditing, ref isEditing, value);
            }
        }

        public string SelectedCount
        {
            get => "已選 " + Institutions.Count(i => i.Selected) + " 間";
        }

        public RelayCommand<string> FocusUpDownCommand { get; set; }
        public RelayCommand SelectAll { get; set; }
        public RelayCommand CancelSelectAll { get; set; }
        public RelayCommand SelectedChanged { get; set; }

        public AdjustedInstitutionSelectionViewModel(PrescriptionSearchInstitutions adjustedInstitutions)
        {
            Institutions = adjustedInstitutions;
            InsCollectionViewSource = new CollectionViewSource { Source = Institutions };
            InsCollectionView = InsCollectionViewSource.View;
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            SelectAll = new RelayCommand(SelectAllAction);
            CancelSelectAll = new RelayCommand(CancelSelectAllAction);
            SelectedChanged = new RelayCommand(SelectedCountChangedAction);
        }

        private void SelectedCountChangedAction()
        {
            RaisePropertyChanged("SelectedCount");
        }

        private void SelectAllAction()
        {
            foreach (var i in Institutions)
            {
                i.Selected = true;
            }
        }

        private void CancelSelectAllAction()
        {
            foreach (var i in Institutions)
            {
                i.Selected = false;
            }
        }

        private void FocusUpDownAction(string direction)
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
        }

        private void FilterBySearchText(object sender, FilterEventArgs e)
        {
            if (!(e.Item is PrescriptionSearchInstitution src))
                e.Accepted = false;
            else if (src.FullName.Contains(Search))
                e.Accepted = true;
            else
            {
                e.Accepted = false;
            }
        }
    }
}