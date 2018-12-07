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
            get => _canMoveTabs;
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
            get => _showAddButton;
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
                Set(() => CardReaderStatus, ref _cardReaderStatus, value);
            }
        }
        private string _samDcStatus;
        public string SamDcStatus
        {
            get => _samDcStatus;
            set
            {
                _samDcStatus = value;
                Set(() => SamDcStatus, ref _samDcStatus, value);
            }
        }

        private string _hpcCardStatus;
        public string HpcCardStatus
        {
            get => _hpcCardStatus;
            set
            {
                _hpcCardStatus = value;
                Set(() => HpcCardStatus, ref _hpcCardStatus, value);
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
        private bool _isHpcValid;

        public bool IsHpcValid
        {
            get => _isHpcValid;
            set
            {
                Set(() => IsHpcValid, ref _isHpcValid, value);
            }
        }

        private bool _isVerifySamDc;

        public bool IsVerifySamDc
        {
            get => _isVerifySamDc;
            set
            {
                Set(() => IsVerifySamDc, ref _isVerifySamDc, value);
            }
        }

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
