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
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using His_Pos.NewClass.Person.Employee;

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
               
                case nameof(FeatureItem.進退貨管理): 
                    ProductPurchaseReturnViewModel PurchaseViewModel 
                        = (ProductPurchaseReturnViewModel)ItemCollection.FirstOrDefault(s => s.TabName == nameof(FeatureItem.進退貨管理));//每個功能只會同時開啟一個tab，所以tab[0]
                    PurchaseViewModel.NormalViewModel.SearchString = string.Empty;//關閉進退貨管理清空搜尋條件
                    break;
                case nameof(FeatureItem.新增盤點):
                    ConfirmWindow confirmWindow = new ConfirmWindow("關閉視窗後盤點單不會儲存 是否關閉?", "關閉新增盤點確認");
                    if (!(bool)confirmWindow.DialogResult)
                    {
                        ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
                        view.Refresh();
                        return;
                    }
                    break;
                case nameof(FeatureItem.進退貨紀錄):
                case nameof(FeatureItem.處方登錄):
                case nameof(FeatureItem.處方查詢):
                case nameof(FeatureItem.匯出申報檔):
                case nameof(FeatureItem.額外收支):
                case nameof(FeatureItem.立帳作業):
                case nameof(FeatureItem.商品查詢):
                case nameof(FeatureItem.庫存盤點紀錄):
                case nameof(FeatureItem.商品類別管理):
                case nameof(FeatureItem.供應商管理):
                case nameof(FeatureItem.櫃位管理):
                case nameof(FeatureItem.員工管理):
                case nameof(FeatureItem.顧客管理):
                case nameof(FeatureItem.藥品頻率管理):
                case nameof(FeatureItem.審核管理):
                case nameof(FeatureItem.藥局管理):
                case nameof(FeatureItem.庫存現值報表):
                case nameof(FeatureItem.進退貨報表):
                case nameof(FeatureItem.合作診所藥品耗用):
                case nameof(FeatureItem.管制藥品簿冊申報):
                case nameof(FeatureItem.會計總帳報表):
                case nameof(FeatureItem.每日總帳報表):
                case nameof(FeatureItem.系統函式):
                case nameof(FeatureItem.申報院所點數總表):
                case nameof(FeatureItem.盤點計畫):
                case nameof(FeatureItem.藥袋查詢):
                case nameof(FeatureItem.藥健康網頁):
                case nameof(FeatureItem.促銷管理):
                case nameof(FeatureItem.銷售紀錄):
                case nameof(FeatureItem.商品訂購網站):
                case nameof(FeatureItem.結帳作業):
                case nameof(FeatureItem.損益報表):
                case nameof(FeatureItem.新損益報表):
                case nameof(FeatureItem.資產負債表):
                case nameof(FeatureItem.沖帳作業):
                case nameof(FeatureItem.關班作業):
                case nameof(FeatureItem.關班帳務查詢):
                case nameof(FeatureItem.上下班打卡):
                case nameof(FeatureItem.排班管理):
                case nameof(FeatureItem.打卡記錄查詢):
                case nameof(FeatureItem.舊每日總帳報表):
                case nameof(FeatureItem.系統教學文件):
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

            IEmployeeService employeeService = new EmployeeService(new EmployeeDb());

            switch (featureItem.ToString())
            {
                //每日作業
                case nameof(FeatureItem.每日作業):
                    newTab = new Index() { TabName = "每日作業", Icon = @"..\Images\Home.png", IsPinned = true };
                    break;

                //處方管理
                case nameof(FeatureItem.處方登錄):
                    newTab = new PrescriptionDeclareViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.處方作業)).Icon };
                    break;

                case nameof(FeatureItem.處方查詢):
                    newTab = new PrescriptionSearchViewModel() {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.處方作業)).Icon };
                    break;

                case nameof(FeatureItem.匯出申報檔):
                    newTab = new DeclareFileManageViewModel() { 
                     Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.處方作業)).Icon };
                    break;

                case nameof(FeatureItem.藥袋查詢):
                    newTab = new MedBagViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.處方作業)).Icon };
                    break;
                     
                //銷售作業
                case nameof(FeatureItem.結帳作業):
                    newTab = new ProductTransactionViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.銷售作業)).Icon };
                    break;

                case nameof(FeatureItem.銷售紀錄):
                    newTab = new ProductTransactionRecordViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.銷售作業)).Icon };
                    break;
                case nameof(FeatureItem.商品訂購網站):
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = ViewModelMainWindow.SingdeWebURI,
                        UseShellExecute = true
                    });
                    return;
                //藥局管理
                case nameof(FeatureItem.顧客管理):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).CustomerManageView;
                    newTab.Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon; 
                    break;
                case nameof(FeatureItem.供應商管理):
                    newTab = new ManufactoryManageViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon };
                    break;

                case nameof(FeatureItem.員工管理):
                    newTab = new EmployeeManageViewModel(employeeService) { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon };
                    break;

                case nameof(FeatureItem.額外收支):
                    newTab = new AdditionalCashFlowManageViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon };
                    break;
                     
                case nameof(FeatureItem.關班作業):
                    newTab = new ClosingWorkViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon };
                    break; 

                case nameof(FeatureItem.關班帳務查詢):
                    newTab = new ClosingCashSelectViwModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.藥局管理)).Icon };
                    break;
                     
                //庫存管理 
                case nameof(FeatureItem.商品查詢):
                    newTab = new ProductManagementViewModel() {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon };
                    break;

                case nameof(FeatureItem.進退貨管理):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).ProductPurchaseReturn;
                    newTab.Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon;
                    break;

                case nameof(FeatureItem.進退貨紀錄):
                    newTab = (Application.Current.FindResource("Locator") as ViewModelLocator).ProductPurchaseRecord;
                    newTab.Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon;
                    break;

                case nameof(FeatureItem.櫃位管理):
                    newTab = new LocationManage() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon };
                    break;

                case nameof(FeatureItem.新增盤點):
                    newTab = new StockTakingViewModel() {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon
                    };
                    break;

                case nameof(FeatureItem.庫存盤點紀錄):
                    newTab = new StockTakingRecord() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon };
                    break;

                case nameof(FeatureItem.盤點計畫):
                    newTab = new StockTakingPlanViewModel() {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存管理)).Icon };
                    break;

                //庫存報表
                case nameof(FeatureItem.庫存現值報表):
                    newTab = new EntrySearchViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存報表)).Icon };
                    break;

                case nameof(FeatureItem.進退貨報表):
                    newTab = new PurchaseReturnReportViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存報表)).Icon };
                    break;

                case nameof(FeatureItem.管制藥品簿冊申報):
                    newTab = new ControlMedicineDeclareViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.庫存報表)).Icon };
                    break;

                //會計作業
                case nameof(FeatureItem.沖帳作業):
                    newTab = new StrikeManageViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計作業)).Icon };
                    break;

                case nameof(FeatureItem.立帳作業):
                    newTab = new AccountsManageViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計作業)).Icon };
                    break;

                //會計報表 
                case nameof(FeatureItem.每日總帳報表):
                    newTab = new NewTodayCashStockEntryReportViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon };
                    break;

                case nameof(FeatureItem.會計總帳報表):
                    newTab = new CashStockEntryReportViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon };
                    break;
                case nameof(FeatureItem.申報院所點數總表):
                    newTab = new InstitutionDeclarePointReportViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon };
                    break;

                case nameof(FeatureItem.損益報表):
                    newTab = new NewIncomeStatementViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon };
                    break;
                case nameof(FeatureItem.新損益報表):
                    newTab = new NewIncomeStatementViewModel()
                    {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon
                    };
                    break;

                case nameof(FeatureItem.資產負債表):
                    newTab = new BalanceSheetViewModel() { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.會計報表)).Icon };
                    break;

                //出勤管理
                case nameof(FeatureItem.上下班打卡):   
                    newTab = new AddClockInViewModel(employeeService) {
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.出勤管理)).Icon };
                    break;

                case nameof(FeatureItem.打卡記錄查詢):
                    newTab = new ClockInSearchViewModel(employeeService) { 
                        Icon = MainWindow.HisFeatures.Single(_ => _.Title == nameof(FeatureTab.出勤管理)).Icon };
                    break;

                //other

                case nameof(FeatureItem.商品類別管理):
                    newTab = new ProductTypeManageViewModel() { Icon = MainWindow.HisFeatures[3].Icon };
                    break;
                      
                case nameof(FeatureItem.促銷管理):
                    newTab = new ActivityManageViewModel() { Icon = MainWindow.HisFeatures[1].Icon };
                    break; 
              
                case nameof(FeatureItem.審核管理):
                    newTab = new AuthenticationManage() {  Icon = MainWindow.HisFeatures[5].Icon };
                    break;
                     
                case nameof(FeatureItem.排班管理):
                    newTab = new WorkScheduleManage() { Icon = MainWindow.HisFeatures[6].Icon };
                    break;
                      
                case nameof(FeatureItem.系統教學文件):
                    newTab = new TutorialViewModel() { Icon = MainWindow.HisFeatures[10].Icon };
                    break; 
                case nameof(FeatureItem.系統函式):
                    newTab = new AdminFunction() { Icon = MainWindow.HisFeatures[9].Icon };
                    break;
               
                default:
                    return;
            }

            newTab.TabName = featureItem.ToString();
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