using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl
{
    /// <summary>
    /// MyPharmacyControl.xaml 的互動邏輯
    /// </summary>
    public partial class MyPharmacyControl : UserControl, INotifyPropertyChanged
    {
       
        #region ----- Define Variable -----

        private Pharmacy pharmacy;
        public Pharmacy myPharmacy
        {
            get { return pharmacy; }
            set
            {
                pharmacy = value;
                NotifyPropertyChanged("myPharmacy");
            }
        }

        public bool IsDataChanged { get; set; } = false;
        public bool IsFirst { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public MyPharmacyControl()
        {
            InitializeComponent();

            MainWindow.ServerConnection.OpenConnection();
            myPharmacy = Pharmacy.GetCurrentPharmacy();
            MainWindow.ServerConnection.CloseConnection();
            DataContext = this;
        }

        #region ----- Data Changed -----
        private void DataHasChanged()
        {
            if(IsFirst) return;

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

        private void Pharmacy_OnDataChanged(object sender, EventArgs e)
        {
            DataHasChanged();
        }

        public void ResetPharmacy()
        {
            MainWindow.ServerConnection.OpenConnection();
            myPharmacy = Pharmacy.GetCurrentPharmacy();
            MainWindow.ServerConnection.CloseConnection();
            ClearDataChangedStatus();
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ResetPharmacy();
        }

        private void ConfirmBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsVPNValid())
            {
                MessageWindow.ShowMessage("VPN 格式錯誤!", MessageType.ERROR);
                
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            myPharmacy.SetPharmacy();
            MainWindow.ServerConnection.CloseConnection();
            WebApi.UpdatePharmacyMedicalNum(myPharmacy.ID);
            ClearDataChangedStatus();
            Properties.Settings.Default.ReaderComPort = myPharmacy.ReaderCom.ToString();
            Properties.Settings.Default.Save();
            SavePrinterToFile();
        }

        private bool IsVPNValid()
        {
            Regex VPNReg = new Regex(@"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}");
            Match match;

            match = VPNReg.Match(myPharmacy.VpnIp);

            return match.Success;
        }

        private void MyPharmacyControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            IsFirst = false;
        }

        private void VerifyHpcPin_Click(object sender, RoutedEventArgs e)
        {
            HisApiFunction.VerifyHpcPin();
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
                fileWriter.WriteLine("Com " + Properties.Settings.Default.ReaderComPort);
            }
        }
    }
}
