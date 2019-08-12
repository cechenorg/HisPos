﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.NewClass;
using His_Pos.NewClass.Person.Employee;
using His_Pos.Service; 

namespace His_Pos.FunctionWindow
{
    public class LoginWindowViewModel : ViewModelBase
    {
        #region ----- Define Command -----
        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand LeaveCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public string Account { get; set; }
        private bool isAccountValid = false;
        public bool IsAccountWrong {
            get { return isAccountValid; }
            set { Set(() => IsAccountWrong, ref isAccountValid, value); }
        }
        #endregion

        public LoginWindowViewModel()
        { 
            LoginCommand = new RelayCommand<object>(LoginAction);
            LeaveCommand = new RelayCommand(LeaveAction);
            CheckCsHis();
            if (!CheckSettingFileExist())
            {
                var verifyPharmacyWindow = new VerifyPharmacyWindow.VerifyPharmacyWindow();
            }
            else { 
                    ReadSettingFile(); 
            }
        }

        #region ----- Define Actions -----
        private void LoginAction(object sender)
        {
            MainWindow.ServerConnection.OpenConnection();
            Employee user = Employee.Login(Account, (sender as PasswordBox)?.Password);
            MainWindow.ServerConnection.CloseConnection();
            
            if (user != null)
            {
                //LoadingWindow loadingWindow = new LoadingWindow();
                //loadingWindow.GetNecessaryData(user);
                NewFunction.ExceptionLog(user.Name + " Login");
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Messenger.Default.Send(new NotificationMessage("CloseLogin"));
            }
            else
            {
                IsAccountWrong = true;
            }
        }

        private void LeaveAction()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseLogin"));
        }
        #endregion

        #region ----- Define Functions -----
        public static void ReadSettingFile() {
            var filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (var fileReader = new StreamReader(filePath))
            {
                var medReg = new Regex(@"M (.*)");
                var recReg = new Regex(@"Rc (.*)");
                var recRegWithForm = new Regex(@"Rc (.*)[$](.*)");
                var repReg = new Regex(@"Rp (.*)");
                var verifyKey = fileReader.ReadLine();
                verifyKey = verifyKey.Substring(2,verifyKey.Length-2);
                var xml = WebApi.GetPharmacyInfoByVerify(verifyKey);
                var PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
                var MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
                var PharmacyTel = xml.SelectSingleNode("CurrentPharmacyInfo/Telphone").InnerText;
                var PharmacyAddress = xml.SelectSingleNode("CurrentPharmacyInfo/Address").InnerText;
                var dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
                Properties.Settings.Default.SQL_local =
                    $"Data Source={dbtargetIp};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword";
                Properties.Settings.Default.SQL_global =
                    $"Data Source={dbtargetIp};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword";
                Properties.Settings.Default.SystemSerialNumber = verifyKey;
                var medBagPrinter = fileReader.ReadLine();
                var receiptPrinter = fileReader.ReadLine();
                var reportPrinter = fileReader.ReadLine();
                var comport = fileReader.ReadLine();
                var match = medReg.Match(medBagPrinter);
                Properties.Settings.Default.MedBagPrinter = match.Groups[1].Value;
                if (receiptPrinter.Contains("$"))
                {
                    match = recRegWithForm.Match(receiptPrinter);
                    Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;
                    Properties.Settings.Default.ReceiptForm = match.Groups[2].Value;
                }
                else
                {
                    match = recReg.Match(receiptPrinter);
                    Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;
                    Properties.Settings.Default.ReceiptForm = "點陣";
                }
                match = repReg.Match(reportPrinter);
                Properties.Settings.Default.ReportPrinter = match.Groups[1].Value;
                Properties.Settings.Default.ReaderComPort = string.IsNullOrEmpty(comport) ? "" : comport.Substring(4, comport.Length - 4);
                Properties.Settings.Default.Save();
            }
        }
        private static bool CheckSettingFileExist() { 
            if (!Directory.Exists("C:\\Program Files\\HISPOS"))
                Directory.CreateDirectory("C:\\Program Files\\HISPOS");  

            return File.Exists("C:\\Program Files\\HISPOS\\settings.singde"); 
        }
        #endregion
        private static void CheckCsHis()
        {
            try
            {
                var csHisSource = "C:\\NHI\\LIB\\CSHIS.dll";
                var csHisMed2Source = "C:\\NHI\\VPN\\CSHISMED2.dll";
                var csHisMed3Source = "C:\\NHI\\VPN\\CSHISMED3.dll";
                var csHisTarget = Environment.Is64BitOperatingSystem ? "C:\\Windows\\SysWOW64\\CSHIS.dll" : "C:\\Windows\\System32\\CSHIS.dll";
                var csHisMed2Target = Environment.Is64BitOperatingSystem ? "C:\\Windows\\SysWOW64\\CSHISMED2.dll" : "C:\\Windows\\System32\\CSHISMED2.dll";
                var csHisMed3Target = Environment.Is64BitOperatingSystem ? "C:\\Windows\\SysWOW64\\CSHISMED3.dll" : "C:\\Windows\\System32\\CSHISMED3.dll";
                CheckAndReplaceFile(csHisSource, csHisTarget);
                CheckAndReplaceFile(csHisMed2Source, csHisMed2Target);
                CheckAndReplaceFile(csHisMed3Source, csHisMed3Target);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void CheckAndReplaceFile(string sourceFile,string targetFile)
        {
            if (File.Exists(sourceFile))
            {
                if (File.Exists(targetFile))
                {
                    File.SetAttributes(targetFile, FileAttributes.Normal);
                    File.Delete(targetFile);
                }
                File.Copy(sourceFile, targetFile, true);
            }
        }
    }
}
