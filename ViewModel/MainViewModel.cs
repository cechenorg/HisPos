using System.Collections.Generic;
using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using His_Pos.Class;
using System.Windows;
using System.Windows.Controls;
using His_Pos.H5_ATTEND.ClockIn;
using His_Pos.InventoryManagement;
using His_Pos.LocationManage;
using His_Pos.H4_BASIC_MANAGE.PharmacyManage;
using His_Pos.H4_BASIC_MANAGE.AuthenticationManage;
using His_Pos.H4_BASIC_MANAGE.MedBagManage;
using His_Pos.H5_ATTEND.WorkScheduleManage;
using His_Pos.H4_BASIC_MANAGE.MedFrequencyManage;
using His_Pos.H4_BASIC_MANAGE.EmployeeManage;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.IndexView;

namespace His_Pos.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //since we don't know what kind of objects are bound, so the sorting happens outside with the ReorderTabsCommand.
        public RelayCommand<TabReorder> ReorderTabsCommand { get; set; }
        public RelayCommand<object> AddTabCommand { get; set; }
        public RelayCommand<TabBase> CloseTabCommand { get; set; }
        public RelayCommand<TabBase> PinTabCommand { get; set; }
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

        public MainViewModel()
        {
            this.ItemCollection = new ObservableCollection<TabBase>();
            this.ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            this.ReorderTabsCommand = new RelayCommand<TabReorder>(ReorderTabsCommandAction);
            this.AddTabCommand = new RelayCommand<object>(AddTabCommandAction);
            this.CloseTabCommand = new RelayCommand<TabBase>(CloseTabCommandAction);
            PinTabCommand = new RelayCommand<TabBase>(PinTabCommandAction);
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
        void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void PinTabCommandAction(TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            view.Refresh();
        }

        //To close a tab, we simply remove the viewmodel from the source collection.
        private void CloseTabCommandAction(TabBase vm)
        {
            switch ( vm.TabName )
            {
                case nameof(FeatureItem.�B��n��):
                    break;
                case nameof(FeatureItem.���հ�):
                    break;
                case nameof(FeatureItem.�B��d��):
                    break;
                case nameof(FeatureItem.�ӫ~�d��):
                    break;
                case nameof(FeatureItem.�B�z��޲z):
                    //if (ProductPurchase.ProductPurchaseView.Instance.backgroundWorker.IsBusy)
                    //{
                    //    MessageWindow message = new MessageWindow("���b�x�s", MessageType.ERROR);
                    //    return;
                    //}
                    break;
                case nameof(FeatureItem.�B�z�����):
                    break;
                case nameof(FeatureItem.�s�W�L�I):
                    break;
                case nameof(FeatureItem.�w�s�L�I����):
                    break;
                case nameof(FeatureItem.�ӫ~���O�޲z):
                    break;
                case nameof(FeatureItem.�����Ӻ޲z):
                    break;
                case nameof(FeatureItem.�d��޲z):
                    LocationManageView.Instance.SaveLocation();
                    break;
                case nameof(FeatureItem.���u�޲z):
                    break;
                case nameof(FeatureItem.�ī~�W�v�޲z):
                    break;
                case nameof(FeatureItem.�W�U�Z���d):
                    break;
                case nameof(FeatureItem.�ƯZ�޲z):
                    break;
                case nameof(FeatureItem.�f�ֺ޲z):
                    break;
                case nameof(FeatureItem.�ĳU�޲z):
                    break;
                case nameof(FeatureItem.�ħ��޲z):
                    break;
                default:
                    return;
            }

            this.ItemCollection.Remove(vm);
        }
     
        //Adds a random tab
        public void AddTabCommandAction(object featureItem)
        {
            if (IsTabOpened(featureItem.ToString())) return;

            TabBase newTab;

            switch (featureItem.ToString())
            {
                //����
                case nameof(FeatureItem.����):
                    newTab = new Index() { TabName = "����", Icon = MainWindow.HisFeatures[0].Icon, IsPinned = true };
                    break;

                //�B��޲z
                case nameof(FeatureItem.�B��n��):
                    newTab = new PrescriptionDec2() { TabName = MainWindow.HisFeatures[0].Functions[0], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                case nameof(FeatureItem.�B��d��):
                    newTab = new PrescriptionInquire.PrescriptionInquire() { TabName = MainWindow.HisFeatures[0].Functions[1], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                case nameof(FeatureItem.�ī~�W�v�޲z):
                    newTab = new MedFrequencyManage() { TabName = MainWindow.HisFeatures[0].Functions[2], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.�ĳU�޲z):
                    newTab = new MedBagManage() { TabName = MainWindow.HisFeatures[0].Functions[3], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //�ӫ~�޲z
                case nameof(FeatureItem.�ӫ~�d��):
                    newTab = new InventoryManagement.InventoryManagement() { TabName = MainWindow.HisFeatures[1].Functions[0], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.�B�z��޲z):
                    newTab = new ProductPurchase.ProductPurchase() { TabName = MainWindow.HisFeatures[1].Functions[1], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.�B�z�����):
                    newTab = new ProductPurchaseRecord.ProductPurchaseRecord() { TabName = MainWindow.HisFeatures[1].Functions[2], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.�ӫ~���O�޲z):
                    newTab = new ProductTypeManage.ProductTypeManage() { TabName = MainWindow.HisFeatures[1].Functions[3], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                case nameof(FeatureItem.�d��޲z):
                    newTab = new LocationManage.LocationManage() { TabName = MainWindow.HisFeatures[1].Functions[4], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //�L�I
                case nameof(FeatureItem.�s�W�L�I):
                    newTab = new StockTaking.StockTaking() { TabName = MainWindow.HisFeatures[2].Functions[0], Icon = MainWindow.HisFeatures[2].Icon };
                    break;
                case nameof(FeatureItem.�w�s�L�I����):
                    newTab = new StockTakingRecord.StockTakingRecord() { TabName = MainWindow.HisFeatures[2].Functions[1], Icon = MainWindow.HisFeatures[2].Icon };
                    break;
                
                //�򥻸�ƺ޲z
                case nameof(FeatureItem.�����Ӻ޲z):
                    newTab = new ManufactoryManage.ManufactoryManage() { TabName = MainWindow.HisFeatures[3].Functions[0], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.�ħ��޲z):
                    newTab = new PharmacyManage() { TabName = MainWindow.HisFeatures[3].Functions[1], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.���u�޲z):
                    newTab = new EmployeeManage() { TabName = MainWindow.HisFeatures[3].Functions[2], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.�f�ֺ޲z):
                    newTab = new AuthenticationManage() { TabName = MainWindow.HisFeatures[3].Functions[3], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.�U�Ⱥ޲z):
                    newTab = new CustomerManage() { TabName = MainWindow.HisFeatures[3].Functions[4], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //���d
                case nameof(FeatureItem.�W�U�Z���d):
                    newTab = new ClockIn() { TabName = MainWindow.HisFeatures[4].Functions[0], Icon = MainWindow.HisFeatures[4].Icon };
                    break;
                case nameof(FeatureItem.�ƯZ�޲z):
                    newTab = new WorkScheduleManage() { TabName = MainWindow.HisFeatures[4].Functions[1], Icon = MainWindow.HisFeatures[4].Icon };
                    break;
                default:
                    return;
            }
            this.ItemCollection.Add(newTab.getTab());
            this.SelectedTab = this.ItemCollection[ItemCollection.Count - 1];
        }

        public bool IsTabOpened(string tabName)
        {
            const int MAX_OPENTAB = 2;
            int tabCount = 1;

            foreach (TabBase tab in ItemCollection)
            {
                if (tab.TabName == tabName)
                {
                    SelectedTab = tab;

                    switch (tabName)
                    {
                        case nameof(FeatureItem.�B��n��):
                            if (tabCount < MAX_OPENTAB)
                            {
                                tabCount++;
                                continue;
                            }
                            break;
                        case nameof(FeatureItem.�ӫ~�d��):
                            if (InventoryManagementView.Instance is null) break;

                            if (InventoryManagementView.DataChanged) {
                                InventoryManagementView.Instance.MergingData();
                                InventoryManagementView.Instance.SearchData();
                            }
                            break;
                        case nameof(FeatureItem.�B�z�����):
                            if (ProductPurchaseRecord.ProductPurchaseRecordView.Instance is null) break;

                            if (ProductPurchaseRecord.ProductPurchaseRecordView.DataChanged)
                                ProductPurchaseRecord.ProductPurchaseRecordView.Instance.UpdateUi();

                            ProductPurchaseRecord.ProductPurchaseRecordView.Instance.PassValueSearchData();
                            break;
                        case nameof(FeatureItem.�s�W�L�I):
                            if (StockTaking.StockTakingView.Instance is null) break;

                            if (StockTaking.StockTakingView.DataChanged)
                            {
                                StockTaking.StockTakingView.Instance.InitProduct();

                                StockTaking.StockTakingView.DataChanged = false;
                            }
                            break;
                        case nameof(FeatureItem.�f�ֺ޲z):
                            if (AuthenticationManageView.Instance is null) break;

                            if (AuthenticationManageView.DataChanged)
                            {
                                AuthenticationManageView.Instance.InitAuthRecord();
                                AuthenticationManageView.Instance.UpdateUi();

                                AuthenticationManageView.DataChanged = false;
                            }
                            break;
                        case nameof(FeatureItem.�ƯZ�޲z):
                            if (WorkScheduleManageView.Instance is null) break;

                            if (WorkScheduleManageView.DataChanged)
                            {
                                WorkScheduleManageView.Instance.InitCalendar();
                                WorkScheduleManageView.DataChanged = false;
                            }
                            break;
                    }

                    return true;
                }
            }
            
            return false;
        }
    }
}