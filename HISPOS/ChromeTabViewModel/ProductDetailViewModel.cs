using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.OTCControl;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.ChromeTabViewModel
{
    public class ProductDetailViewModel : ViewModelBase
    {
        public RelayCommand<TabReorder> ReorderTabsCommand { get; set; }
        public RelayCommand<string[]> AddTabCommand { get; set; }
        public RelayCommand<TabBase> CloseTabCommand { get; set; }
        public ObservableCollection<TabBase> ItemCollection { get; set; }

        //This is the current selected tab, if you change it, the tab is selected in the tab control.
        private TabBase _selectedTab;

        public TabBase SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    Set(() => SelectedTab, ref _selectedTab, value);
                }
            }
        }

        private bool _canAddTabs;

        public bool CanAddTabs
        {
            get { return _canAddTabs; }
            set
            {
                if (_canAddTabs != value)
                {
                    Set(() => CanAddTabs, ref _canAddTabs, value);
                    AddTabCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ProductDetailViewModel()
        {
            this.ItemCollection = new ObservableCollection<TabBase>();
            this.ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            this.ReorderTabsCommand = new RelayCommand<TabReorder>(ReorderTabsCommandAction);
            this.AddTabCommand = new RelayCommand<string[]>(AddTabCommandAction);
            this.CloseTabCommand = new RelayCommand<TabBase>(CloseTabCommandAction);
            CanAddTabs = true;
        }

        protected virtual void ReorderTabsCommandAction(TabReorder reorder)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            int from = reorder.FromIndex;
            int to = reorder.ToIndex;
            var tabCollection = view.Cast<TabBase>().ToList();//Get the ordered collection of our tab control
            tabCollection[from].TabNumber = tabCollection[to].TabNumber; //Set the new index of our dragged tab

            if (to > from)
            {
                for (int i = from + 1; i <= to; i++)
                {
                    tabCollection[i].TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
                }
            }
            else if (from > to)//when we decrement the tab index
            {
                for (int i = to; i < from; i++)
                {
                    tabCollection[i].TabNumber++;//When we decrement the tab index, we need to increment all other tabs.
                }
            }

            view.Refresh();//Refresh the view to force the sort description to do its work.
        }

        //We need to set the TabNumber property on the viewmodels when the item source changes to keep it in sync.
        private void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TabBase tab in e.NewItems)
                {
                    if (this.ItemCollection.Count > 1)
                    {
                        //If the new tab don't have an existing number, we increment one to add it to the end.
                        if (tab.TabNumber == 0)
                            tab.TabNumber = this.ItemCollection.OrderBy(x => x.TabNumber).LastOrDefault().TabNumber + 1;
                    }
                }
            }
            else
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
                view.Refresh();
                var tabCollection = view.Cast<TabBase>().ToList();
                foreach (var item in tabCollection)
                    item.TabNumber = tabCollection.IndexOf(item);
            }
        }

        private void CloseTabCommandAction(TabBase vm)
        {
            this.ItemCollection.Remove(vm);

            if (ItemCollection.Count == 0 && ProductDetail.Instance != null)
            {
                ProductDetail.Instance.Close();
                ProductDetail.Instance = null;
            }
        }

        public void AddTabCommandAction(string[] newProduct)
        {
            TabBase newTab;

            foreach (TabBase tab in ItemCollection)
            {
                if (tab.TabName == newProduct[0])
                {
                    SelectedTab = tab;
                    return;
                }
            }

            DataTable dataTable = ProductDetailDB.GetProductTypeByID(newProduct[0]);

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            ProductTypeEnum type = (ProductTypeEnum)dataTable.Rows[0].Field<int>("Pro_TypeID");

            switch (type)
            {
                case ProductTypeEnum.NHIMedicine:
                    newTab = new MedicineControlViewModel(newProduct[0], type, newProduct[1]) { TabName = newProduct[0], Icon = "/Images/BlueDot.png" };
                    break;

                case ProductTypeEnum.OTCMedicine:
                    newTab = new OTCControlViewModel(newProduct[0], type, newProduct[1]) { TabName = newProduct[0], Icon = "/Images/BlueDot.png" };
                    break;

                case ProductTypeEnum.SpecialMedicine:
                    newTab = new MedicineControlViewModel(newProduct[0], type, newProduct[1]) { TabName = newProduct[0], Icon = "/Images/BlueDot.png" };
                    break;

                default:
                    return;
            }

            ItemCollection.Add(newTab.getTab());
            SelectedTab = ItemCollection[ItemCollection.Count - 1];
        }
    }
}