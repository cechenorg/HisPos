using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl
{
    /// <summary>
    /// PrinterControl.xaml 的互動邏輯
    /// </summary>
    public partial class PrinterControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        public Collection<string> Printers { get; set; } = new Collection<string>();

        private string medBagPrinter;
        public string MedBagPrinter
        {
            get { return medBagPrinter; }
            set
            {
                medBagPrinter = value;
                NotifyPropertyChanged("MedBagPrinter");
            }
        }

        private string receiptPrinter;
        public string ReceiptPrinter
        {
            get { return receiptPrinter; }
            set
            {
                receiptPrinter = value;
                NotifyPropertyChanged("ReceiptPrinter");
            }
        }

        private string reportPrinter;
        public string ReportPrinter
        {
            get { return reportPrinter; }
            set
            {
                reportPrinter = value;
                NotifyPropertyChanged("ReportPrinter");
            }
        }

        public bool IsDataChanged { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public PrinterControl()
        {
            InitializeComponent();

            DataContext = this;

            InitPrinters();
            GetSavedPrinter();
        }

        private void GetSavedPrinter()
        {
            MedBagPrinter = Properties.Settings.Default.MedBagPrinter;
            ReceiptPrinter = Properties.Settings.Default.ReceiptPrinter;
            ReportPrinter = Properties.Settings.Default.ReportPrinter;
        }

        private void InitPrinters()
        {
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                Printers.Add(PrinterSettings.InstalledPrinters[i]);
            }
        }

        #region ----- Data Changed -----
        private void DataHasChanged()
        {
            IsDataChanged = true;

            UpdateDataChangedUi();
        }

        private void UpdateDataChangedUi()
        {
            if (IsDataChanged)
            {
                ChangedLabel.Content = "已修改";
                ChangedLabel.Foreground = Brushes.Red;

                CancelBtn.IsEnabled = true;
                ConfirmBtn.IsEnabled = true;
            }
            else
            {
                ChangedLabel.Content = "未修改";
                ChangedLabel.Foreground = Brushes.DimGray;

                CancelBtn.IsEnabled = false;
                ConfirmBtn.IsEnabled = false;
            }
        }

        private void ClearDataChangedStatus()
        {
            IsDataChanged = false;

            UpdateDataChangedUi();
        }

        #endregion

        private void Printer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataHasChanged();
        }

        private void CancelChange_Click(object sender, RoutedEventArgs e)
        {
            ResetPrinter();
        }

        public void ResetPrinter()
        {
            GetSavedPrinter();

            ClearDataChangedStatus();
        }

        public void ConfirmChange_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MedBagPrinter = MedBagPrinter;
            Properties.Settings.Default.ReceiptPrinter = ReceiptPrinter;
            Properties.Settings.Default.ReportPrinter = ReportPrinter;

            Properties.Settings.Default.Save();

            SavePrinterToFile();

            ClearDataChangedStatus();
        }

        private void SavePrinterToFile()
        {
            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine();
            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);

                fileWriter.WriteLine("M " + Properties.Settings.Default.MedBagPrinter);
                fileWriter.WriteLine("Rc " + Properties.Settings.Default.ReceiptPrinter);
                fileWriter.WriteLine("Rp " + Properties.Settings.Default.ReportPrinter);
            }
        }
    }
}
