using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using His_Pos.Service;

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

        private PrescriptionSearchInstitutions originInstitutions;

        public PrescriptionSearchInstitutions OriginInstitutions
        {
            get => originInstitutions;
            set { Set(() => OriginInstitutions, ref originInstitutions, value); }
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

        private bool _isSelectedAll = true;

        public bool IsSelectedAll
        {
            get => _isSelectedAll;
            set
            {
                Set(() => IsSelectedAll, ref _isSelectedAll, value);

                if (value)
                {
                    SelectAllAction();
                }
                else
                {
                    CancelSelectAllAction();
                }
            }
        }

        public string SelectedCount
        {
            get => "已選 " + Institutions.Count(i => i.Selected) + " 間";
        }
        private Window _window;
        public RelayCommand<string> FocusUpDownCommand { get; set; }
        public RelayCommand SelectAll { get; set; }
        public RelayCommand CancelSelectAll { get; set; }
        public RelayCommand SelectedChanged { get; set; }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public AdjustedInstitutionSelectionViewModel(PrescriptionSearchInstitutions adjustedInstitutions, Window window)
        {
            _window = window;

            OriginInstitutions = adjustedInstitutions;
            Institutions = adjustedInstitutions.DeepCloneViaJson();
            InsCollectionViewSource = new CollectionViewSource { Source = Institutions };
            InsCollectionView = InsCollectionViewSource.View;
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            SelectAll = new RelayCommand(SelectAllAction);
            CancelSelectAll = new RelayCommand(CancelSelectAllAction);
            SelectedChanged = new RelayCommand(SelectedCountChangedAction);

            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction);
        }

        private void SubmitAction()
        {
          
            for (int i = 0; i < OriginInstitutions.Count; i++)
            {
                OriginInstitutions[i].Selected = Institutions.Single(_ => _.ID == OriginInstitutions[i].ID).Selected;
            }

            _window.Close();
        }

        private void CancelAction()
        {
            _window.Close();
        }

        private void SelectedCountChangedAction()
        {
            RaisePropertyChanged("SelectedCount");
        }

        private void SelectAllAction()
        {
            if (string.IsNullOrEmpty(Search))
            {
                foreach (var i in Institutions)
                {
                    i.Selected = true;
                }
            }
            else
            {
                foreach (var i in Institutions.Where(_ => _.Name.Contains(Search)))
                {
                    i.Selected = true;
                }
            }
           

            SelectedCountChangedAction();
        }

        private void CancelSelectAllAction()
        {
            if (string.IsNullOrEmpty(Search))
            {
                foreach (var i in Institutions)
                {
                    i.Selected = false;
                }
            }
            else
            {
                foreach (var i in Institutions.Where(_ => _.Name.Contains(Search)))
                {
                    i.Selected = false;
                }
            }

            SelectedCountChangedAction();
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