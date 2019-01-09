using System;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.Pharmacy;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl
{
    /// <summary>
    /// MyPharmacyControl.xaml 的互動邏輯
    /// </summary>
    public partial class MyPharmacyControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Struct -----
        public class MyPharmacy
        {
            public MyPharmacy(DataRow dataRow)
            {
                Name = dataRow["CURPHA_NAME"].ToString();
                Id = dataRow["CURPHA_ID"].ToString();
                Address = dataRow["CURPHA_ADDR"].ToString();
                Telephone = dataRow["CURPHA_TEL"].ToString();
                IsReaderNew = Boolean.Parse(dataRow["CURPHA_READERISNEW"].ToString());
                ReaderCom = Int16.Parse(dataRow["CURPHA_READERCOM"].ToString());
                VPN = dataRow["CURPHA_VPN"].ToString();
            }

            public string Name { get; set; }
            public string Id { get; set; }
            public string Address { get; set; }
            public string Telephone { get; set; }
            public bool IsReaderNew { get; set; }
            public int ReaderCom { get; set; }
            public string VPN { get; set; }
        }
        #endregion

        #region ----- Define Variable -----

        private MyPharmacy pharmacy;
        public MyPharmacy myPharmacy
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

            myPharmacy = PharmacyDb.GetMyPharmacy();
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
            myPharmacy = PharmacyDb.GetMyPharmacy();

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
                MessageWindow messageWindow = new MessageWindow("VPN 格式錯誤!", MessageType.ERROR);
                messageWindow.ShowDialog();

                return;
            }

            PharmacyDb.SetMyPharmacy(myPharmacy);

            ClearDataChangedStatus();
        }

        private bool IsVPNValid()
        {
            Regex VPNReg = new Regex(@"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}");
            Match match;

            match = VPNReg.Match(myPharmacy.VPN);

            return match.Success;
        }

        private void MyPharmacyControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            IsFirst = false;
        }

        private void VerifyHpcPin_Click(object sender, RoutedEventArgs e)
        {
            HisApiBase.VerifyHpcPin();
        }
    }
}
