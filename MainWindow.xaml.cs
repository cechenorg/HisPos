using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        public static DataTable MedicineDataTable = new DataTable();
        public static User CurrentUser = new User();
        private List<DockingWindow> _openWindows;
        public static DataView View;
        public MainWindow(User userLogin)
        {
            InitializeComponent();
            WindowState = WindowState.Normal;
            Width = SystemParameters.PrimaryScreenWidth * 0.833;
            Height = Width * 0.5625;
            InitializePaymentBlock();
            CurrentUser = userLogin;
            InitializeMenu(HisHided);
            _openWindows = new List<DockingWindow>();
        }

        private void InitializeMenu(bool hisHided)
        {
            if (hisHided)
                InitializePosMenu();
            else
                InitializeHisMenu();
        }

        private void InitializePosMenu()
        {
            HisMenu.Visibility = Visibility.Collapsed;
            PosMenu.Visibility = Visibility.Visible;
            _posFeaturesItemsList = new List<string[]> { _posFeatures1Item};
            _posFeaturesList = new List<UserControl1> { PosFeature1};
            for (int i = 0; i < 1; i++) {
                _posFeaturesList[i].SetLabelText(_posFeatures[i]);
                _posFeaturesList[i].SetLabelImage(_posFeaturesIcon[i]);
                SetFeaturesItem(_posFeaturesList[i], _posFeaturesItemsList[i]);
            }
            UpdateLayout();
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
        private void InitializeHisMenu()
        {
            PosMenu.Visibility = Visibility.Collapsed;
            HisMenu.Visibility = Visibility.Visible;
            HisFeaturesItemsList = new List<string[]> { HisFeatures1Item };
            HisFeaturesList = new List<UserControl1> { HisFeature1 };
            for (int i = 0; i < 1; i++)
            {
                HisFeaturesList[i].SetLabelText(HisFeatures[i]);
                HisFeaturesList[i].SetLabelImage(HisFeaturesIcon[i]);
                SetFeaturesItem(HisFeaturesList[i], HisFeaturesItemsList[i]);
            }
            UpdateLayout();
        }
        private void InitializePaymentBlock()
        {
            InputMethod.SetIsInputMethodEnabled(Barcode, false);
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
            switch (m.Name)
            {
                case "處方登錄":
                    ((ViewModelMainWindow) DataContext).AddTabCommandAction("處方登錄");
                    break;
                case "處方查詢":
                    ((ViewModelMainWindow) DataContext).AddTabCommandAction("處方查詢");
                    break;
                case "處方修改":
                    ((ViewModelMainWindow)DataContext).AddTabCommandAction("處方修改");
                    break;
            }
        }

        private void MenuSwitchMouseDown(object sender, MouseButtonEventArgs e)
        {
            var menuLabel = sender as Label;
            if (menuLabel != null && menuLabel.Content.Equals("P O S"))
            {
                menuLabel.Foreground = Brushes.Orange;
                HisSwitch.Foreground = Brushes.DimGray;
                InitializeMenu(true);
            }
            else if(menuLabel != null && menuLabel.Content.Equals("H I S"))
            {
                menuLabel.Foreground = Brushes.LightSkyBlue;
                PosSwitch.Foreground = Brushes.DimGray;
                InitializeMenu(false);
            }
            InitializeComponent();
            UpdateLayout();
        }
    }
}
