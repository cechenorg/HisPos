using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.ADMIN_MANAGE.AdminFunction;
using His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch;
using His_Pos.SYSTEM_TAB.H11_CLOSING.Closing;
using His_Pos.SYSTEM_TAB.H11_CLOSING.ClossingCashSelect;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.MedBagManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.AuthenticationManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage;
using His_Pos.SYSTEM_TAB.H5_ATTEND.ClockInSearch;
using His_Pos.SYSTEM_TAB.H5_ATTEND.AddClockIn;
using His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.TodayCashStockEntryReport;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.InstitutionDeclarePointReport;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement;
using His_Pos.SYSTEM_TAB.H9_SYSTEMTUTORIAL.Tutorial;
using His_Pos.SYSTEM_TAB.INDEX;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ActivityManage;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport;

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

                case nameof(FeatureItem.額外收支):
                    break;

                case nameof(FeatureItem.會計科目):
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
                    ConfirmWindow confirmWindow = new ConfirmWindow("關閉視窗後盤點單不會儲存 是否關閉?", "關閉新增盤點確認");
                    if (!(bool)confirmWindow.DialogResult)
                        return;
                    break;

                case nameof(FeatureItem.庫存盤點紀錄):
                    break;

                case nameof(FeatureItem.商品類別管理):
                    break;

                case nameof(FeatureItem.供應商管理):
                    break;

                case nameof(FeatureItem.櫃位管理):
                    //LocationManageView.Instance.SaveLocation();
                    break;

                case nameof(FeatureItem.員工管理):
                    break;

                case nameof(FeatureItem.顧客管理):
                    break;

                case nameof(FeatureItem.藥品頻率管理):
                    break;

                case nameof(FeatureItem.審核管理):
                    break;

                case nameof(FeatureItem.藥局管理):
                    break;

                case nameof(FeatureItem.庫存現值報表):
                    break;

                case nameof(FeatureItem.進退貨報表):
                    break;

                case nameof(FeatureItem.合作診所藥品耗用):
                    break;

                case nameof(FeatureItem.管制藥品簿冊申報):
                    break;

                case nameof(FeatureItem.會計總帳報表):
                    break;
                case nameof(FeatureItem.每日總帳報表):
                    break;
                case nameof(FeatureItem.系統函式):
                    break;

                case nameof(FeatureItem.申報院所點數總表):
                    break;

                case nameof(FeatureItem.盤點計畫):
                    break;

                case nameof(FeatureItem.藥袋查詢):
                    break;

                case nameof(FeatureItem.藥健康網頁):
                    break;

                case nameof(FeatureItem.促銷管理):
                    break;

                case nameof(FeatureItem.銷售紀錄):
                    break;

                case nameof(FeatureItem.結帳作業):
                    break;

                case nameof(FeatureItem.損益報表):
                    break;

                case nameof(FeatureItem.資產負債表):
                    break;
                case nameof(FeatureItem.沖帳作業):
                    break;

                case nameof(FeatureItem.關班作業):
                    break;
                case nameof(FeatureItem.關班帳務查詢):
                    break;

                // add by shani
                case nameof(FeatureItem.上下班打卡):
                    break;
                // add by shani
                case nameof(FeatureItem.排班管理):
                    break;
                // add by shani
                case nameof(FeatureItem.打卡記錄查詢):
                    break;
                case nameof(FeatureItem.舊每日總帳報表):
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

                case nameof(FeatureItem.匯出申報檔):
                    newTab = new DeclareFileManageViewModel() { TabName = MainWindow.HisFeatures[0].Functions[2], Icon = MainWindow.HisFeatures[0].Icon };
                    break;

                case nameof(FeatureItem.額外收支):
                    newTab = new AdditionalCashFlowManageViewModel() { TabName = MainWindow.HisFeatures[2].Functions[0], Icon = MainWindow.HisFeatures[2].Icon };
                    break;

                //銷售作業
                case nameof(FeatureItem.結帳作業):
                    newTab = new ProductTransactionViewModel() { TabName = MainWindow.HisFeatures[1].Functions[0], Icon = MainWindow.HisFeatures[1].Icon };
                    break;

                case nameof(FeatureItem.銷售紀錄):
                    newTab = new ProductTransactionRecordViewModel() { TabName = MainWindow.HisFeatures[1].Functions[1], Icon = MainWindow.HisFeatures[1].Icon };
                    break;

                case nameof(FeatureItem.促銷管理):
                    newTab = new ActivityManageViewModel() { TabName = MainWindow.HisFeatures[1].Functions[2], Icon = MainWindow.HisFeatures[1].Icon };
                    break;

                //商品管理
                case nameof(FeatureItem.商品查詢):
                    newTab = new ProductManagementViewModel() { TabName = MainWindow.HisFeatures[3].Functions[0], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.藥袋查詢):
                    newTab = new MedBagViewModel() { TabName = MainWindow.HisFeatures[3].Functions[1], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.進退貨管理):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).ProductPurchaseReturn;
                    break;

                case nameof(FeatureItem.進退貨紀錄):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).ProductPurchaseRecord;
                    break;

                case nameof(FeatureItem.商品類別管理):
                    newTab = new ProductTypeManageViewModel() { TabName = MainWindow.HisFeatures[3].Functions[4], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                case nameof(FeatureItem.櫃位管理):
                    newTab = new LocationManage() { TabName = MainWindow.HisFeatures[3].Functions[5], Icon = MainWindow.HisFeatures[3].Icon };
                    break;

                //盤點
                case nameof(FeatureItem.新增盤點):
                    newTab = new StockTakingViewModel() { TabName = MainWindow.HisFeatures[4].Functions[0], Icon = MainWindow.HisFeatures[4].Icon };
                    break;

                case nameof(FeatureItem.庫存盤點紀錄):
                    newTab = new StockTakingRecord() { TabName = MainWindow.HisFeatures[4].Functions[1], Icon = MainWindow.HisFeatures[4].Icon };
                    break;

                case nameof(FeatureItem.盤點計畫):
                    newTab = new StockTakingPlanViewModel() { TabName = MainWindow.HisFeatures[4].Functions[2], Icon = MainWindow.HisFeatures[4].Icon };
                    break;

                //基本資料管理
                case nameof(FeatureItem.供應商管理):
                    newTab = new ManufactoryManageViewModel() { TabName = MainWindow.HisFeatures[5].Functions[0], Icon = MainWindow.HisFeatures[5].Icon };
                    break;

                case nameof(FeatureItem.員工管理):
                    newTab = new EmployeeManageViewModel() { TabName = MainWindow.HisFeatures[5].Functions[2], Icon = MainWindow.HisFeatures[5].Icon };
                    break;

                case nameof(FeatureItem.審核管理):
                    newTab = new AuthenticationManage() { TabName = MainWindow.HisFeatures[5].Functions[3], Icon = MainWindow.HisFeatures[5].Icon };
                    break;

                case nameof(FeatureItem.顧客管理):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).CustomerManageView;
                    break;

                //打卡 add by SHANI
                case nameof(FeatureItem.上下班打卡): //add by shani
                    newTab = new AddClockInViewModel() { TabName = MainWindow.HisFeatures[6].Functions[0], Icon = MainWindow.HisFeatures[6].Icon };
                    break;
                case nameof(FeatureItem.排班管理):
                    newTab = new WorkScheduleManage() { TabName = MainWindow.HisFeatures[6].Functions[1], Icon = MainWindow.HisFeatures[6].Icon };
                    break;
                case nameof(FeatureItem.打卡記錄查詢):
                    newTab = new ClockInSearchViewModel() { TabName = MainWindow.HisFeatures[6].Functions[2], Icon = MainWindow.HisFeatures[6].Icon };
                    break;

                //報表

                case nameof(FeatureItem.庫存現值報表):
                    newTab = new EntrySearchViewModel() { TabName = MainWindow.HisFeatures[7].Functions[0], Icon = MainWindow.HisFeatures[7].Icon };
                    break;

                case nameof(FeatureItem.進退貨報表):
                    newTab = new PurchaseReturnReportViewModel() { TabName = MainWindow.HisFeatures[7].Functions[1], Icon = MainWindow.HisFeatures[7].Icon };
                    break;

                case nameof(FeatureItem.管制藥品簿冊申報):
                    newTab = new ControlMedicineDeclareViewModel() { TabName = MainWindow.HisFeatures[7].Functions[2], Icon = MainWindow.HisFeatures[7].Icon };
                    break;

                case nameof(FeatureItem.會計總帳報表):
                    newTab = new CashStockEntryReportViewModel() { TabName = MainWindow.HisFeatures[7].Functions[3], Icon = MainWindow.HisFeatures[7].Icon };
                    break;
                case nameof(FeatureItem.舊每日總帳報表):
                    newTab = new TodayCashStockEntryReportViewModel() { TabName = MainWindow.HisFeatures[7].Functions[4], Icon = MainWindow.HisFeatures[7].Icon };
                    break;
                case nameof(FeatureItem.每日總帳報表):
                    newTab = new NewTodayCashStockEntryReportViewModel() { TabName = MainWindow.HisFeatures[7].Functions[5], Icon = MainWindow.HisFeatures[7].Icon };
                    break;

                //會計報表
                case nameof(FeatureItem.申報院所點數總表):
                    newTab = new InstitutionDeclarePointReportViewModel() { TabName = MainWindow.HisFeatures[8].Functions[0], Icon = MainWindow.HisFeatures[8].Icon };
                    break;

                case nameof(FeatureItem.損益報表):
                    newTab = new NewIncomeStatementViewModel() { TabName = MainWindow.HisFeatures[8].Functions[1], Icon = MainWindow.HisFeatures[8].Icon };
                    break;

                case nameof(FeatureItem.資產負債表):
                    newTab = new BalanceSheetViewModel() { TabName = MainWindow.HisFeatures[8].Functions[2], Icon = MainWindow.HisFeatures[8].Icon };
                    break;
                case nameof(FeatureItem.沖帳作業):
                    newTab = new StrikeManageViewModel() { TabName = MainWindow.HisFeatures[8].Functions[3], Icon = MainWindow.HisFeatures[8].Icon };
                    break;

                case nameof(FeatureItem.會計科目):
                    newTab = new AccountsManageViewModel() { TabName = MainWindow.HisFeatures[8].Functions[4], Icon = MainWindow.HisFeatures[8].Icon };
                    break;

                //系統教學
                case nameof(FeatureItem.系統教學文件):
                    newTab = new TutorialViewModel() { TabName = MainWindow.HisFeatures[10].Functions[0], Icon = MainWindow.HisFeatures[10].Icon };
                    break;
                //公司網站
                /* case nameof(FeatureItem.藥健康網頁):
                     newTab = new CompanyWebViewModel() { TabName = MainWindow.HisFeatures[11].Functions[0], Icon = MainWindow.HisFeatures[11].Icon };
                     break;*/

                //系統函式
                case nameof(FeatureItem.系統函式):
                    newTab = new AdminFunction() { TabName = "系統函式", Icon = MainWindow.HisFeatures[9].Icon };
                    break;
                //關班作業
                case nameof(FeatureItem.關班作業):
                    newTab = new ClosingWorkViewModel() { TabName = MainWindow.HisFeatures[11].Functions[0], Icon = MainWindow.HisFeatures[11].Icon };
                    break;
                //關班帳務查詢
                case nameof(FeatureItem.關班帳務查詢):
                    newTab = new ClosingCashSelectViwModel() { TabName = MainWindow.HisFeatures[11].Functions[1], Icon = MainWindow.HisFeatures[11].Icon };
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

                        case nameof(FeatureItem.審核管理):
                            if (AuthenticationManageView.Instance is null) break;

                            if (AuthenticationManageView.DataChanged)
                            {
                                AuthenticationManageView.Instance.InitAuthRecord();
                                AuthenticationManageView.Instance.UpdateUi();

                                AuthenticationManageView.DataChanged = false;
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