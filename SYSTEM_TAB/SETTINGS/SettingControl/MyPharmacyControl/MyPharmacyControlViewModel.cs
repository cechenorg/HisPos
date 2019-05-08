﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.MyPharmacyControl
{
    public class MyPharmacyControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        public RelayCommand VerifyHPCPinCommand { get; set; }
        public RelayCommand VerifySAMDCCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isBusy;
        private string busyContent;
        public Pharmacy myPharmacy;
        private bool isDataChanged;

        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }
        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
        }
        public Pharmacy MyPharmacy
        {
            get => myPharmacy;
            set { Set(() => MyPharmacy, ref myPharmacy, value); }
        }
        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        public MyPharmacyControlViewModel()
        {
            InitMyPharmacy();
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ConfirmChangeAction()
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
            Properties.Settings.Default.ReaderComPort = myPharmacy.ReaderCom.ToString();
            Properties.Settings.Default.Save();

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine();
            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);
                fileWriter.WriteLine("Com " + Properties.Settings.Default.ReaderComPort);
            }

            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            InitMyPharmacy();
            IsDataChanged = false;
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        private void VerifyHPCPinAction()
        {
            HisApiFunction.VerifyHpcPin();
        }
        private void VerifySAMDCAction()
        {
            var verify = false;
            var bw = new BackgroundWorker();
            bw.DoWork += (o, ea) =>
            {
                BusyContent = Properties.Resources.認證安全模組;
                verify = HisApiFunction.VerifySamDc();
            };
            bw.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MessageWindow.ShowMessage(verify ? Properties.Resources.認證成功 : Properties.Resources.認證失敗, verify ? MessageType.SUCCESS : MessageType.ERROR);
            };
            IsBusy = true;
            bw.RunWorkerAsync();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsPharmacyDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsPharmacyDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            VerifyHPCPinCommand = new RelayCommand(VerifyHPCPinAction);
            VerifySAMDCCommand = new RelayCommand(VerifySAMDCAction);
        }
        private bool IsPharmacyDataChanged()
        {
            return IsDataChanged;
        }
        private void InitMyPharmacy()
        {
            MainWindow.ServerConnection.OpenConnection();
            MyPharmacy = Pharmacy.GetCurrentPharmacy();
            MainWindow.ServerConnection.CloseConnection();
        }
        private bool IsVPNValid()
        {
            Regex VPNReg = new Regex(@"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}");
            Match match;

            match = VPNReg.Match(myPharmacy.VpnIp);

            return match.Success;
        }
        #endregion
    }
}
