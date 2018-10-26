using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChromeTabs;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Authority;
using His_Pos.Class.Person;
using His_Pos.Class.Pharmacy;
using His_Pos.Class.Product;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.HisApi;
using His_Pos.Resource;
using His_Pos.Service;
using His_Pos.SystemSettings;
using His_Pos.ViewModel;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;
using Pharmacy = His_Pos.Class.Pharmacy.Pharmacy;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public static string CardReaderStatus;
        public static Pharmacy CurrentPharmacy;
        public static MainWindow MainWindowInstance;
        public MainWindow(User userLogin)
        {
            FeatureFactory();
            InitializeComponent();
            WindowState = WindowState.Maximized;
            CurrentUser = userLogin;
            Instance = this;
            InitializeMenu();
            InitialUserBlock();
            StratClock();
            _openWindows = new List<DockingWindow>();
            MainWindowInstance = this;
            CurrentPharmacy = new Pharmacy("5932012975", "杏昌藥局", "330桃園市桃園區中正路1100號", "03-3573268");
            AddNewTab("首頁");
        }
        
        private void InitialUserBlock()
        {
            UserName.Content = CurrentUser.Name;
        }

        private void FeatureFactory()
        {
            HisFeatures.Add(new Feature( @"..\Images\PrescriptionIcon.png", Properties.Resources.hisPrescription,
                            new string[] { Properties.Resources.hisPrescriptionDeclare, Properties.Resources.hisPrescriptionInquire, Properties.Resources.MedFrequencyManage, Properties.Resources.MedBagManage }));

            HisFeatures.Add(new Feature(@"..\Images\Truck_50px.png", Properties.Resources.StockManage,
                            new string[] { Properties.Resources.StockSearch, Properties.Resources.ProductPurchase, Properties.Resources.ProductPurchaseRecord, Properties.Resources.ProductTypeManage, Properties.Resources.LocationManage }));

            HisFeatures.Add(new Feature(@"..\Images\StockTaking.png", Properties.Resources.StockTaking,
                            new string[] { Properties.Resources.NewStockTaking, Properties.Resources.StockTakingRecord }));

            HisFeatures.Add(new Feature(@"..\Images\Management.png", Properties.Resources.DataManagement,
                            new string[] { Properties.Resources.ManufactoryManage, Properties.Resources.PharmacyManage,
                                           Properties.Resources.EmployeeManage, Properties.Resources.AuthenticationManage, Properties.Resources.CustomerManage}));

            HisFeatures.Add(new Feature(@"..\Images\ClockIn.png", Properties.Resources.Attend,
                            new string[] { Properties.Resources.ClockIn, Properties.Resources.WorkScheduleManage }));

            HisFeatures.Add(new Feature(@"..\Images\DeclareFile.png", Properties.Resources.DeclareFile,
                new string[] { Properties.Resources.DeclareFileExport }));
            HisFeatures.Add(new Feature(@"..\Images\StockTaking.png", Properties.Resources.ReportSystem,
              new string[] { Properties.Resources.EntrySearch }));
        }
        
        private void InitializeMenu()
        {
            
            for (int i = 0; i < HisFeatures.Count; i++)
            {
               
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelText(HisFeatures[i].Title);
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelImage(HisFeatures[i].Icon);
                    SetFeaturesItem((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem), HisFeatures[i].Functions);
                if ((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem)._count != 0)
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).Visibility = Visibility.Visible;
                
            }
        }

        private void SetFeaturesItem(MenuListItem features, string [] itemsName)
        {
            if (features == null || itemsName == null)
                throw new ArgumentNullException(nameof(itemsName));

            Collection<string> tabAuth = AuthorityDb.GetTabAuthByGroupId(CurrentUser.Authority.AuthorityValue);
            foreach (var t in itemsName)
            {
                if (tabAuth.Count(tab => tab == t) != 0)
                {
                    var newItem = new MenuItem();
                    features.NewMenuItem(t, newItem, FeaturesItemMouseDown);
                }
            }
        }

        private void FeatureMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is MenuListItem feature)) throw new ArgumentNullException(nameof(feature));
            feature.ListMenuTimer?.Start();
        }

        private void FeaturesItemMouseDown(object sender, RoutedEventArgs e)
        {
            var m = sender as MenuItem;
            Debug.Assert(m != null, nameof(m) + " != null");
            AddNewTab(m.Name);
        }

        public void AddNewTab(string tabName)
        {
            ((ViewModelMainWindow)DataContext).AddTabCommandAction(tabName);
            this.Focus();
        }
        
        private void TickEvent(Object sender, EventArgs e)
        {
            SystemTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }
        /*
         *啟動處方登錄時間Timer
         */
        private void StratClock()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += TickEvent;
            timer.Start();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Tabs.SelectedItem is null) return;

            //((ViewModelMainWindow)DataContext).AddTabCommandAction(((TabBase)Tabs.SelectedItem).TabName);
        }

        private void Shortcut_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is null) return;

            Label shortcut = sender as Label;

            switch (shortcut.Content.ToString())
            {
                case "調劑":
                    AddNewTab(Properties.Resources.hisPrescriptionDeclare);
                    break;
                case "交易":
                    break;
            }
        }
        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            if (e.TabItem != null && e.Model is TabBase viewModel)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //var d = new DeclareDb();
            //d.StartDailyUpload();
            ProductDb.UpdateDailyStockValue();
            Application.Current.Shutdown();
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var t1 = new Thread(VerifySam);
            //t1.Start();
        }

        private void VerifySam()
        {
            var res = HisApiBase.csVerifySAMDC();
            switch (res)
            {
                case 0:
                    CardReaderStatus = "無任何錯誤";
                    break;
                case 4000:
                    CardReaderStatus = "讀卡機逾時";
                    break;
                case 4012:
                    CardReaderStatus = "未置入安全模組檔";
                    break;
                case 4032:
                    CardReaderStatus = "所插入非安全模組檔";
                    break;
                case 4051:
                    CardReaderStatus = "安全模組與 IDC 認證失敗";
                    break;
                case 4061:
                    CardReaderStatus = "網路連線失敗";
                    break;
                case 6005:
                    CardReaderStatus = "安全模組檔的外部認證失敗";
                    break;
                case 6006:
                    CardReaderStatus = "IDC 的外部認證失敗";
                    break;
                case 6007:
                    CardReaderStatus = "安全模組檔的內部認證失敗";
                    break;
                case 6008:
                    CardReaderStatus = "寫入讀卡機日期時間失敗";
                    break;
            }
        }
    }
}
