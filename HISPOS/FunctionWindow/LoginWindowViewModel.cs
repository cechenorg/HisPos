using DTO.WebService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.NewClass.Encrypt.AES;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Pharmacy;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Xml;

namespace His_Pos.FunctionWindow
{
    public class LoginWindowViewModel : ViewModelBase
    {
        #region ----- Define Command -----

        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand LeaveCommand { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        public string Account { get; set; }
        private bool isAccountValid = false;

        public bool IsAccountWrong
        {
            get { return isAccountValid; }
            set { Set(() => IsAccountWrong, ref isAccountValid, value); }
        }
        public static string _MedicalNum { get; set; }
        public bool _IsCanLogIn { get; set; }

        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion ----- Define Variables -----

        private readonly IEmployeeService _employeeService;
        public LoginWindowViewModel()
        {
            _employeeService = new EmployeeService(new EmployeeDb());

            LoginCommand = new RelayCommand<object>(LoginAction);
            LeaveCommand = new RelayCommand(LeaveAction);
            CheckCsHis();
            if (!CheckSettingFileExist())
            {
                var verifyPharmacyWindow = new VerifyPharmacyWindow.VerifyPharmacyWindow();
            }
            else
            {
                ReadSettingFile();
            }
            ExecSQLCmd();
            _IsCanLogIn = IsValidityPeriod();
            
            if (!_IsCanLogIn)
            {
                MessageWindow.ShowMessage("用戶已過期，請聯絡杏德", Class.MessageType.WARNING);
            }
        }

        #region ----- Define Actions -----

        private void LoginAction(object sender)
        {
            bool isEnable = false;
            Employee loginEmployee = _employeeService.Login(Account, (sender as PasswordBox)?.Password);
            if (loginEmployee != null) 
            {
                ViewModelMainWindow.CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
                isEnable = EmployeeDb.CheckEmployeeIsEnable(loginEmployee.ID);
            }
            if ((_IsCanLogIn && isEnable) || (loginEmployee != null && loginEmployee.Authority == DomainModel.Enum.Authority.Admin))
            {
                //LoadingWindow loadingWindow = new LoadingWindow();
                //loadingWindow.GetNecessaryData(user);
                NewFunction.ExceptionLog(loginEmployee.Name + " Login");
                MainWindow mainWindow = new MainWindow(loginEmployee);
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

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----
        private bool IsValidityPeriod()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                HISPOSWebApiService hisposWebApiService = new HISPOSWebApiService();
                try
                {
                    PharmacyDTO serverPharmacyInfo = hisposWebApiService.GetServerPharmacyInfo(_MedicalNum);
                    if (serverPharmacyInfo != null)
                    {
                        if (serverPharmacyInfo.PHAMAS_VALIDUSEDATE != null)
                        {
                            var encrypt = AESEncrypt.AESEncryptBase64(serverPharmacyInfo.PHAMAS_VALIDUSEDATE.ToString("yyyy-MM-dd"), serverPharmacyInfo.PHAMAS_MEDICALNUM);
                            PharmacyDBService.SetPharmacyValidityPeriod(encrypt);
                        }
                    }
                }
                catch
                {

                }
            }
            Pharmacy pharmacy = Pharmacy.GetCurrentPharmacy();
            if (pharmacy != null)
            {
                DateTime validDate = new DateTime();
                if (pharmacy.CurPha_PeriodDate != null)
                {
                    string decryptString = AESEncrypt.AESDecryptBase64(pharmacy.CurPha_PeriodDate, pharmacy.ID);
                    if (DateTime.TryParse(decryptString, out validDate))
                    {
                        if (DateTime.Compare(validDate, DateTime.Today) >= 0)
                            return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public static void ReadSettingFile()
        {
            var filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (var fileReader = new StreamReader(filePath))
            {
                var medReg = new Regex(@"M (.*)");
                var recReg = new Regex(@"Rc (.*)");
                var recRegWithForm = new Regex(@"Rc (.*)[$](.*)");
                var repReg = new Regex(@"Rp (.*)");
                var verifyKey = fileReader.ReadLine();
                verifyKey = verifyKey.Substring(2, verifyKey.Length - 2);
                XmlDocument xml = new XmlDocument();
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    xml = WebApi.GetPharmacyInfoByVerify(verifyKey);
                }
                
                /*
                var xml = WebApi.GetPharmacyInfoByVerify(verifyKey);
                var PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
                var MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
                var PharmacyTel = xml.SelectSingleNode("CurrentPharmacyInfo/Telphone").InnerText;
                var PharmacyAddress = xml.SelectSingleNode("CurrentPharmacyInfo/Address").InnerText;
                var dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
                */
                var dbtargetIp = "127.0.0.1";
                if (xml != null && !string.IsNullOrEmpty(xml.InnerText))
                {
                    dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
                    _MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
                }
                else
                {
                    Pharmacy pharmacy = Pharmacy.GetCurrentPharmacy();
                    _MedicalNum = pharmacy.ID;
                }
                
                Properties.Settings.Default.SQL_local =
                    $"Data Source={dbtargetIp};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword;"; 
                Properties.Settings.Default.SQL_global =
                    $"Data Source={dbtargetIp};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword;";
                Properties.Settings.Default.SystemSerialNumber = verifyKey;

                 

                var medBagPrinter = fileReader.ReadLine();
                var receiptPrinter = fileReader.ReadLine();
                var reportPrinter = fileReader.ReadLine();
                var comport = fileReader.ReadLine();
                var icomport = fileReader.ReadLine();
                var inumber = fileReader.ReadLine();
                var ichk = fileReader.ReadLine();
                var inumS = fileReader.ReadLine();
                var inumC = fileReader.ReadLine();
                var inumE = fileReader.ReadLine();
                var pP = fileReader.ReadLine();
                var rpf = fileReader.ReadLine();
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
                Properties.Settings.Default.InvoiceComPort = string.IsNullOrEmpty(icomport) ? "" : icomport.Substring(5, icomport.Length - 5);
                Properties.Settings.Default.InvoiceNumber = string.IsNullOrEmpty(inumber) ? "" : inumber.Substring(5, inumber.Length - 5);
                Properties.Settings.Default.InvoiceCheck = string.IsNullOrEmpty(ichk) ? "" : ichk.Substring(5, ichk.Length - 5);
                Properties.Settings.Default.InvoiceNumberStart = string.IsNullOrEmpty(inumS) ? "" : inumS.Substring(6, inumS.Length - 6);
                Properties.Settings.Default.InvoiceNumberCount = string.IsNullOrEmpty(inumC) ? "" : inumC.Substring(6, inumC.Length - 6);
                Properties.Settings.Default.InvoiceNumberEng = string.IsNullOrEmpty(inumE) ? "" : inumE.Substring(6, inumE.Length - 6);

                if (pP != null)
                {
                    bool flag = Boolean.TryParse(pP.Substring(3, pP.Length - 3), out flag);
                    Properties.Settings.Default.PrePrint = flag.ToString();//string.IsNullOrEmpty(pP) ? "" : pP.Substring(3, pP.Length - 3);
                }
                
                Properties.Settings.Default.ReportFormat = string.IsNullOrEmpty(rpf) ? "" : rpf.Substring(4);
                Properties.Settings.Default.Save();
            }
        }

        public static string ReadSettingFilePharmacyName()
        {
            var filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (var fileReader = new StreamReader(filePath))
            {
                var medReg = new Regex(@"M (.*)");
                var recReg = new Regex(@"Rc (.*)");
                var recRegWithForm = new Regex(@"Rc (.*)[$](.*)");
                var repReg = new Regex(@"Rp (.*)");
                var verifyKey = fileReader.ReadLine();
                verifyKey = verifyKey.Substring(2, verifyKey.Length - 2);
                var xml = WebApi.GetPharmacyInfoByVerify(verifyKey);
                var PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
                return PharmacyName;
            }
        }

        private static bool CheckSettingFileExist()
        {
            if (!Directory.Exists("C:\\Program Files\\HISPOS"))
                Directory.CreateDirectory("C:\\Program Files\\HISPOS");

            return File.Exists("C:\\Program Files\\HISPOS\\settings.singde");
        }

        #endregion ----- Define Functions -----

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

        private static void CheckAndReplaceFile(string sourceFile, string targetFile)
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

        private void ExecSQLCmd()
        {
            string path = @"C:\Program Files\HISPOS\SQLPackage\";

            try
            {
                string pgVersion = Version;//目前程式更新版號
                string sqlVersion = PharmacyDBService.GetSysemVersion();//目前資料庫更新版號
                if (string.Compare(pgVersion, sqlVersion) == 0)
                    return;

                string[] files = Directory.GetFiles(@"SQLPackage");
                Array.Sort(files);
                if (files != null && files.Length > 0)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    foreach (string file in files)
                    {
                        string filePath = file;
                        string outputFilePath = string.Format(@"C:\Program Files\HISPOS\{0}", file);
                        string searchString = "DB_Local";
                        string replacementString = Properties.Settings.Default.SystemSerialNumber;
                        try
                        {
                            string fileVersion = file.Replace(@"SQLPackage\", string.Empty).Replace(".sql", string.Empty).Trim();
                            if (string.Compare(pgVersion, fileVersion) <= 0 && string.Compare(sqlVersion, fileVersion) < 0)//取得比目前程式版本小且比資料庫版號大的差異檔
                            {
                                string sqlContent = File.ReadAllText(filePath);
                                string modifiedSqlContent = sqlContent.Replace(searchString, replacementString);
                                File.WriteAllText(outputFilePath, modifiedSqlContent);
                            } 
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    string[] updFiles = Directory.GetFiles(path);
                    Array.Sort(updFiles);

                    Process process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    foreach (string file in updFiles)
                    {
                        string cmdText = string.Format(@"sqlcmd.exe -U sa -P ""123456a@"" -S ""127.0.0.1, 1433"" -f 65001 -d {0} -i ""{1}""",
                                Properties.Settings.Default.SystemSerialNumber,
                                file);
                        process.StandardInput.WriteLine(cmdText);
                    }
                    process.StandardInput.WriteLine("exit");
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Directory.Exists(path))
                        Directory.Delete(path, true);

                PharmacyDBService.SetSysemVersion(Version);
            }
        }
    }
}