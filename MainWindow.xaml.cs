using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChromeTabs;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Resource;
using His_Pos.Service;
using His_Pos.ViewModel;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow MainWindowInstance;
        public MainWindow(User userLogin)
        {
            FeatureFactory();
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Title = "藥健康 POS-HIS 系統";
            CurrentUser = userLogin;
            Instance = this;
            InitializePosMenu();
            InitializeHisMenu();
            InitialUserBlock();
            StratClock();
            _openWindows = new List<DockingWindow>();
            MainWindowInstance = this;
           
        }
        
        private void InitialUserBlock()
        {
            UserName.Content = CurrentUser.Name;
        }

        private void FeatureFactory()
        {
            HisFeatures.Add(new Feature( @"..\Images\PrescriptionIcon.png", Properties.Resources.hisPrescription,
                            new string[] { Properties.Resources.hisPrescriptionDeclare,Properties.Resources.PrescriptionDec2, Properties.Resources.hisPrescriptionInquire }));
            HisFeatures.Add(new Feature(@"..\Images\Truck_50px.png", Properties.Resources.StockManage,
                            new string[] { Properties.Resources.StockSearch, Properties.Resources.ProductPurchase, Properties.Resources.ProductPurchaseRecord }));
            HisFeatures.Add(new Feature(@"..\Images\StockTaking.png", Properties.Resources.StockTaking,
                            new string[] { Properties.Resources.NewStockTaking, Properties.Resources.StockTakingRecord }));
            HisFeatures.Add(new Feature(@"..\Images\Management.png", Properties.Resources.DataManagement,
                            new string[] { Properties.Resources.ProductTypeManage, Properties.Resources.LocationManage, Properties.Resources.ManufactoryManage, Properties.Resources.PharmacyManage, Properties.Resources.EmployeeManage, Properties.Resources.MedFrequencyManage }));
            HisFeatures.Add(new Feature(@"..\Images\ClockIn.png", Properties.Resources.Attend,
                            new string[] { Properties.Resources.ClockIn, Properties.Resources.Leave, Properties.Resources.WorkScheduleManage }));
        }
        
        private void InitializePosMenu()
        {
            for (int i = 0; i < PosFeatures.Count; i++)
            {
                PosFeature1.SetLabelText(PosFeatures[i].Title);
                PosFeature1.SetLabelImage(PosFeatures[i].Icon);
                SetFeaturesItem(PosFeature1, PosFeatures[i].Functions);
            }
        }
        private void InitializeHisMenu()
        {
            for (int i = 0; i < HisFeatures.Count; i++)
            {
                (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelText(HisFeatures[i].Title);
                (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelImage(HisFeatures[i].Icon);
                SetFeaturesItem((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem), HisFeatures[i].Functions);
            }
        }
        private void MenuChange()
        {
            PosMenu.Visibility = (PosMenu.Visibility == Visibility.Visible)? Visibility.Collapsed : Visibility.Visible;
            HisMenu.Visibility = (HisMenu.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetFeaturesItem(MenuListItem features, string [] itemsName)
        {
            if (features == null || itemsName == null)
                throw new ArgumentNullException(nameof(itemsName));
        
            foreach (var t in itemsName)
            {
                var newItem = new MenuItem();
                features.NewMenuItem(t,newItem, FeaturesItemMouseDown);
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

        private void MenuSwitchMouseDown(object sender, MouseButtonEventArgs e)
        {
            var menuLabel = sender as Label;

            if (menuLabel is null) return;

            switch (menuLabel.Content)
            {
                case "P O S":
                    menuLabel.Foreground = Brushes.Orange;
                    HisSwitch.Foreground = Brushes.DimGray;
                    MenuChange();
                    break;
                case "H I S":
                    menuLabel.Foreground = Brushes.LightSkyBlue;
                    PosSwitch.Foreground = Brushes.DimGray;
                    MenuChange();
                    break;
            }
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
            if (Tabs.SelectedItem is null) return;

            ((ViewModelMainWindow)DataContext).AddTabCommandAction(((TabBase)Tabs.SelectedItem).TabName);
        }
    }
}
