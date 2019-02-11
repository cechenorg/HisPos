using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.SYSTEM_TAB.SETTINGS.SettingControl;

namespace His_Pos.SYSTEM_TAB.SETTINGS
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

        private PrinterControl printerControl;
        private DatabaseControl databaseControl;
        private MyPharmacyControl myPharmacyControl;

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

            InitSettingDatas();
            InitControls();

            CurrentStack = MyPharmacyStack;

            UpdateContentControl();
            LabelVersion.Content = "系統版本  " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void InitControls()
        {
            printerControl = new PrinterControl();
            databaseControl = new DatabaseControl();
            myPharmacyControl = new MyPharmacyControl();
        }

        private void InitSettingDatas()
        {
            Regex localReg = new Regex(@"L (Data Source=[0-9.]*,[0-9]*;Persist Security Info=True;User ID=[a-zA-Z0-9]*;Password=[a-zA-Z0-9]*)");
            Regex globalReg = new Regex(@"G (Data Source=[0-9.]*,[0-9]*;Persist Security Info=True;User ID=[a-zA-Z0-9]*;Password=[a-zA-Z0-9]*)");

            Regex medReg = new Regex(@"M (.*)");
            Regex recReg = new Regex(@"Rc (.*)");
            Regex repReg = new Regex(@"Rp (.*)");

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
              // string newLine = fileReader.ReadLine();
              // Match match = localReg.Match(newLine);
              // Properties.Settings.Default.SQL_local = match.Groups[1].Value;
              //
              // newLine = fileReader.ReadLine();
              // match = globalReg.Match(newLine);
              // Properties.Settings.Default.SQL_global = match.Groups[1].Value;

                string newLine = fileReader.ReadLine();
                Match match = medReg.Match(newLine);
                Properties.Settings.Default.MedBagPrinter = match.Groups[1].Value;

                newLine = fileReader.ReadLine();
                match = recReg.Match(newLine);
                Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;

                newLine = fileReader.ReadLine();
                match = repReg.Match(newLine);
                Properties.Settings.Default.ReportPrinter = match.Groups[1].Value;

                Properties.Settings.Default.Save();
            }
        }

        #region ----- Update View -----
        private void UpdateContentControl()
        {
            SelectTab();

            SettingTabs settingTab = (SettingTabs) Int16.Parse(CurrentStack.Tag.ToString());

            switch (settingTab)
            {
                case SettingTabs.MyPharmacy:
                    myPharmacyControl.ResetPharmacy();
                    CurrentControl = myPharmacyControl;
                    break;
                case SettingTabs.Printer:
                    printerControl.ResetPrinter();
                    CurrentControl = printerControl;
                    break;
                case SettingTabs.Database:
                    databaseControl.ResetConnection();
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
