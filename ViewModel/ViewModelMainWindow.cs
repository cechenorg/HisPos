using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.ViewModel
{
    public class ViewModelMainWindow : MainViewModel, IViewModelMainWindow
    {
        //this property is to show you can lock the tabs with a binding
        private bool _canMoveTabs;
        public bool CanMoveTabs
        {
            get { return _canMoveTabs; }
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }
        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;
        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }

        private string _cardReaderStatus;
        public string CardReaderStatus
        {
            get => _cardReaderStatus;
            set
            {
                _cardReaderStatus = value;
                OnPropertyChanged(nameof(CardReaderStatus));
            }
        }
        private string _samDcStatus;
        public string SamDcStatus
        {
            get => _samDcStatus;
            set
            {
                _samDcStatus = value;
                OnPropertyChanged(nameof(SamDcStatus));
            }
        }

        private string _hpcCardStatus;
        public string HpcCardStatus
        {
            get => _hpcCardStatus;
            set
            {
                _hpcCardStatus = value;
                OnPropertyChanged(nameof(HpcCardStatus));
            }
        }

        private bool _isConnectionOpened;

        public bool IsConnectionOpened
        {
            get => _isConnectionOpened;
            set
            {
                _isConnectionOpened = value;
            }
        }
        private bool _isIcCardValid;
        public bool IsIcCardValid
        {
            get => _isIcCardValid;
            set
            {
                _isIcCardValid = value;
            }
        }

        public bool IsHpcValid { get; set; } = false;
        public bool IsVerifySamDc { get; set; } = false;

        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);

            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            CanMoveTabs = true;
            ShowAddButton = false;
        }
    }
}
