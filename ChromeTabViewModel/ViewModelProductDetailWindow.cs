using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.ViewModel
{
    public class ViewModelProductDetailWindow : ProductDetailViewModel, IViewModelProdcutDetailWindow
    {
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

        public ViewModelProductDetailWindow()
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
