using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.SYSTEM_TAB.ADMIN_MANAGE.AdminFunction;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedFrequencyManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.AuthenticationManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.PharmacyManage;
using His_Pos.SYSTEM_TAB.H5_ATTEND.ClockIn;
using His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeAdjustReport;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeEntry;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport;
using His_Pos.SYSTEM_TAB.INDEX;

namespace His_Pos.ChromeTabViewModel
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

        private void PinTabCommandAction(TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            view.Refresh();
        }

        //To close a tab, we simply remove the viewmodel from the source collection.
        private void CloseTabCommandAction(TabBase vm)
        {
            switch (vm.TabName)
            {
                case nameof(FeatureItem.處方登錄):
                    break;
                case nameof(FeatureItem.處方查詢):
                    break;
                case nameof(FeatureItem.匯出申報檔):
                    break;
                case nameof(FeatureItem.商品查詢):
                    break;

                case nameof(FeatureItem.進退貨管理):
                    //if (ProductPurchase.ProductPurchaseView.Instance.backgroundWorker.IsBusy)
                    //{
                    //    MessageWindow message = new MessageWindow("正在儲存", MessageType.ERROR);
                    //    return;
                    //}
                    break;

                case nameof(FeatureItem.進退貨紀錄):
                    break;

                case nameof(FeatureItem.新增盤點):
                    break;

                case nameof(FeatureItem.庫存盤點紀錄):
                    break;

                case nameof(FeatureItem.商品類別管理):
                    break;

                case nameof(FeatureItem.供應商管理):
                    break;

                case nameof(FeatureItem.櫃位管理):
                    LocationManageView.Instance.SaveLocation();
                    break;

                case nameof(FeatureItem.員工管理):
                    break;
                case nameof(FeatureItem.顧客管理):
                    break;
                case nameof(FeatureItem.藥品頻率管理):
                    break;

                case nameof(FeatureItem.上下班打卡):
                    break;

                case nameof(FeatureItem.排班管理):
                    break;

                case nameof(FeatureItem.審核管理):
                    break;

                case nameof(FeatureItem.藥袋管理):
                    break;
                case nameof(FeatureItem.藥局管理):
                    break;
                case nameof(FeatureItem.庫存現值查詢):
                    break;
                case nameof(FeatureItem.進退貨報表查詢):
                    break;
                case nameof(FeatureItem.部分負擔自費報表):
                    break;
                case nameof(FeatureItem.合作診所藥品耗用):
                    break;
                case nameof(FeatureItem.系統函式):
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
                //系統函式
                case nameof(FeatureItem.系統函式):
                    newTab = new  AdminFunction() { TabName = "系統函式", Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                //每日作業
                case nameof(FeatureItem.每日作業):
                    newTab = new Index() { TabName = "每日作業", Icon = @"..\Images\Home.png", IsPinned = true };
                    break;

                //處方管理
                case nameof(FeatureItem.處方登錄):
                    newTab = new PrescriptionDeclareViewModel() { TabName = MainWindow.HisFeatures[0].Functions[0], Icon = MainWindow.HisFeatures[0].Icon };
                    break;

                case nameof(FeatureItem.處方查詢):
                    newTab = new PrescriptionSearchViewModel() { TabName = MainWindow.HisFeatures[0].Functions[1], Icon = MainWindow.HisFeatures[0].Icon };
                    break;

                case nameof(FeatureItem.藥品頻率管理):
                    newTab = new MedFrequencyManage() { TabName = MainWindow.HisFeatures[0].Functions[2], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                
                case nameof(FeatureItem.藥袋管理):
                    newTab = new MedBagManage() { TabName = MainWindow.HisFeatures[0].Functions[3], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.匯出申報檔):
                    newTab = new DeclareFileManageViewModel() { TabName = MainWindow.HisFeatures[0].Functions[4], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                //商品管理
                case nameof(FeatureItem.商品查詢):
                    newTab = new ProductManagementViewModel() { TabName = MainWindow.HisFeatures[1].Functions[0], Icon = MainWindow.HisFeatures[1].Icon };
                    break;

                case nameof(FeatureItem.進退貨管理):
                    newTab = (Application.Current.FindResource("ProductPurchaseReturnLocator") as ProductPurchaseReturnLocator).ProductPurchaseReturn;
                    break;

                case nameof(FeatureItem.進退貨紀錄):
                    newTab = new ProductPurchaseRecord() { TabName = MainWindow.HisFeatures[1].Functions[2], Icon = MainWindow.HisFeatures[1].Icon };
                    break;

                case nameof(FeatureItem.商品類別管理):
                    newTab = new ProductTypeManage() { TabName = MainWindow.HisFeatures[1].Functions[3], Icon = MainWindow.HisFeatures[0].Icon };
                    break;

                case nameof(FeatureItem.櫃位管理):
                    newTab = new LocationManage() { TabName = MainWindow.HisFeatures[1].Functions[4], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //盤點
                case nameof(FeatureItem.新增盤點):
                    newTab = new StockTaking() { TabName = MainWindow.HisFeatures[2].Functions[0], Icon = MainWindow.HisFeatures[2].Icon };
                    break;

                case nameof(FeatureItem.庫存盤點紀錄):
                    newTab = new StockTakingRecord() { TabName = MainWindow.HisFeatures[2].Functions[1], Icon = MainWindow.HisFeatures[2].Icon };
                    break;

                //基本資料管理
                case nameof(FeatureItem.供應商管理):
                    newTab = new ManufactoryManage() { TabName = MainWindow.HisFeatures[3].Functions[0], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.藥局管理):
                    newTab = new PharmacyManage() { TabName = MainWindow.HisFeatures[3].Functions[1], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.員工管理):
                    newTab = new EmployeeManage() { TabName = MainWindow.HisFeatures[3].Functions[2], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.審核管理):
                    newTab = new AuthenticationManage() { TabName = MainWindow.HisFeatures[3].Functions[3], Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                case nameof(FeatureItem.顧客管理):
                    newTab = new CustomerManageViewModel() { TabName = MainWindow.HisFeatures[3].Functions[4], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //打卡
                case nameof(FeatureItem.上下班打卡):
                    newTab = new ClockIn() { TabName = MainWindow.HisFeatures[4].Functions[0], Icon = MainWindow.HisFeatures[4].Icon };
                    break;

                case nameof(FeatureItem.排班管理):
                    newTab = new WorkScheduleManage() { TabName = MainWindow.HisFeatures[4].Functions[1], Icon = MainWindow.HisFeatures[4].Icon };
                    break;

                //報表
                case nameof(FeatureItem.庫存現值查詢):
                    newTab = new EntrySearchViewModel() { TabName = MainWindow.HisFeatures[5].Functions[0], Icon = MainWindow.HisFeatures[5].Icon };
                    break;
                case nameof(FeatureItem.進退貨報表查詢):
                    newTab = new PurchaseReturnReport() { TabName = MainWindow.HisFeatures[5].Functions[1], Icon = MainWindow.HisFeatures[5].Icon };
                    break;
                case nameof(FeatureItem.合作診所藥品耗用):
                    newTab = new CooperativeAdjustReport() { TabName = MainWindow.HisFeatures[5].Functions[2], Icon = MainWindow.HisFeatures[5].Icon };
                    break;
                case nameof(FeatureItem.部分負擔自費報表):
                    newTab = new CooperativeEntry() { TabName = MainWindow.HisFeatures[5].Functions[3], Icon = MainWindow.HisFeatures[5].Icon };
                    break;
                default:
                    return;
            }
            this.ItemCollection.Add(newTab.getTab());
            this.SelectedTab = this.ItemCollection[ItemCollection.Count - 1];
        }

        public bool IsTabOpened(string tabName)
        {
            const int MAX_OPENTAB = 1;
            int tabCount = 1;

            foreach (TabBase tab in ItemCollection)
            {
                if (tab.TabName == tabName)
                {
                    SelectedTab = tab;

                    switch (tabName)
                    {
                        case nameof(FeatureItem.處方登錄):
                            if (tabCount < MAX_OPENTAB)
                            {
                                tabCount++;
                                continue;
                            }
                            break;
                        case nameof(FeatureItem.商品查詢):
                            if (InventoryManagementView.Instance is null) break;

                            if (InventoryManagementView.DataChanged)
                            {
                                InventoryManagementView.Instance.MergingData();
                                InventoryManagementView.Instance.SearchData();
                            }
                            break;

                        case nameof(FeatureItem.進退貨紀錄):
                            if (ProductPurchaseRecordView.Instance is null) break;

                            if (ProductPurchaseRecordView.DataChanged)
                                ProductPurchaseRecordView.Instance.UpdateUi();

                            ProductPurchaseRecordView.Instance.PassValueSearchData();
                            break;

                        case nameof(FeatureItem.新增盤點):
                            if (StockTakingView.Instance is null) break;

                            if (StockTakingView.DataChanged)
                            {
                                StockTakingView.Instance.InitProduct();

                                StockTakingView.DataChanged = false;
                            }
                            break;

                        case nameof(FeatureItem.審核管理):
                            if (AuthenticationManageView.Instance is null) break;

                            if (AuthenticationManageView.DataChanged)
                            {
                                AuthenticationManageView.Instance.InitAuthRecord();
                                AuthenticationManageView.Instance.UpdateUi();

                                AuthenticationManageView.DataChanged = false;
                            }
                            break;

                        case nameof(FeatureItem.排班管理):
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