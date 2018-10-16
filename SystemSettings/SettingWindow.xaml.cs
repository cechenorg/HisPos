using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.SystemSettings.SettingControl;

namespace His_Pos.SystemSettings
{
    /// <summary>
    /// SettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SettingWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        private StackPanel CurrentStack { get; set; }
        private UserControl currentControl;

        public UserControl CurrentControl
        {
            get { return currentControl; }
            set
            {
                currentControl = value;
                NotifyPropertyChanged("CurrentControl");
            }
        }

        private PrinterControl printerControl = new PrinterControl();
        private DatabaseControl databaseControl = new DatabaseControl();
        private MyPharmacyControl myPharmacyControl = new MyPharmacyControl();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public SettingWindow()
        {
            InitializeComponent();

            DataContext = this;

            CurrentStack = MyPharmacyStack;

            UpdateContentControl();
        }

        #region ----- Update View -----
        private void UpdateContentControl()
        {
            SelectTab();

            SettingTabs settingTab = (SettingTabs) Int16.Parse(CurrentStack.Tag.ToString());

            switch (settingTab)
            {
                case SettingTabs.MyPharmacy:
                    CurrentControl = myPharmacyControl;
                    break;
                case SettingTabs.Printer:
                    CurrentControl = printerControl;
                    break;
                case SettingTabs.Database:
                    CurrentControl = databaseControl;
                    break;
            }
        }
        
        private void SelectTab()
        {
            CurrentStack.Background = Brushes.LightGray;

            Label label = CurrentStack.Children.OfType<Label>().ToList()[0];
            label.Foreground = Brushes.DimGray;
        }

        private void ClearSelectTab()
        {
            CurrentStack.Background = Brushes.Transparent;

            Label label = CurrentStack.Children.OfType<Label>().ToList()[0];
            label.Foreground = Brushes.DarkGray;
        }
        #endregion

        private void SettingTab_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is null) return;

            StackPanel stackPanel = sender as StackPanel;

            if (CurrentStack != null)
                ClearSelectTab();

            CurrentStack = stackPanel;

            UpdateContentControl();
        }
    }
}
