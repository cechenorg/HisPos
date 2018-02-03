using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using His_Pos.Class;
using His_Pos.ViewModel;
using MenuUserControl;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(User userLogin)
        {
            FeatureFactory();
            InitializeComponent();
            WindowState = WindowState.Normal;
            Width = SystemParameters.PrimaryScreenWidth * 0.833;
            Height = Width * 0.5625;
            CurrentUser = userLogin;
            InitializePosMenu();
            InitializeHisMenu();
            InitialUserBlock();
            StratClock();
            _openWindows = new List<DockingWindow>();
        }

        private void InitialUserBlock()
        {
            UserName.Content = CurrentUser.Name;
        }

        private void FeatureFactory()
        {
            HisFeatures.Add(new Feature( @"..\Images\PrescriptionIcon.png", Properties.Resources.hisPrescription, new string[] { Properties.Resources.hisPrescriptionDeclare, Properties.Resources.hisPrescriptionInquire, Properties.Resources.hisPrescriptionRevise }));

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
                HisFeature1.SetLabelText(HisFeatures[i].Title);
                HisFeature1.SetLabelImage(HisFeatures[i].Icon);
                SetFeaturesItem(HisFeature1, HisFeatures[i].Functions);
            }
        }
        private void MenuChange()
        {
            PosMenu.Visibility = (PosMenu.Visibility == Visibility.Visible)? Visibility.Collapsed : Visibility.Visible;
            HisMenu.Visibility = (HisMenu.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetFeaturesItem(UserControl1 features, string [] itemsName)
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
            if (!(sender is UserControl1 feature)) throw new ArgumentNullException(nameof(feature));
            feature.ListMenuTimer?.Start();
        }

        private void FeaturesItemMouseDown(object sender, RoutedEventArgs e)
        {
            var m = sender as MenuItem;
            Debug.Assert(m != null, nameof(m) + " != null");
            ((ViewModelMainWindow) DataContext).AddTabCommandAction(m.Name);
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
    }
}
