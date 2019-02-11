using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.Xml;
using System.IO;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.FunctionWindow.VerifyPharmacyWindow
{
    public class VerifyPharmacyViewModel : ViewModelBase
    {
        #region Var
        private string dbtargetIp;
        public bool isverify;
        public bool Isverify
        {
            get { return isverify; }
            set { Set(() => Isverify, ref isverify, value); }
        }
        public string verifyNumber;
        public string VerifyNumber {
            get { return verifyNumber; } 
            set { Set(() => VerifyNumber, ref verifyNumber, value); }
        }
        public string pharmacyName;
        public string PharmacyName
        {
            get { return pharmacyName; }
            set { Set(() => PharmacyName, ref pharmacyName, value); }
        }
        public string medicalNum;
        public string MedicalNum
        {
            get { return medicalNum; }
            set { Set(() => MedicalNum, ref medicalNum, value); }
        }
        public string pharmacyTel;
        public string PharmacyTel
        {
            get { return pharmacyTel; }
            set { Set(() => PharmacyTel, ref pharmacyTel, value); }
        }
        public string pharmacyAddress;
        public string PharmacyAddress
        {
            get { return pharmacyAddress; }
            set { Set(() => PharmacyAddress, ref pharmacyAddress, value); }
        }
        #endregion
        #region Command
        public RelayCommand VerifyCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        #endregion
        public VerifyPharmacyViewModel() {
            VerifyCommand = new RelayCommand(VerifyAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            Isverify = false;
        } 
        #region Action
        private void VerifyAction() {
            if (string.IsNullOrEmpty(VerifyNumber)) {
                MessageWindow.ShowMessage("請輸入認證碼",Class.MessageType.WARNING);
                return;
            }
            XmlDocument xml = WebApi.GetPharmacyInfoByVerify(VerifyNumber);
            if (string.IsNullOrEmpty(xml.InnerText)) {
                MessageWindow.ShowMessage("找不到認證藥局 請聯絡工程師", Class.MessageType.WARNING);
                return;
            }
            PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
            MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
            PharmacyTel = xml.SelectSingleNode("CurrentPharmacyInfo/Telphone").InnerText;
            PharmacyAddress = xml.SelectSingleNode("CurrentPharmacyInfo/Address").InnerText;
            dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
            Isverify = true;
        }
        private void SubmitAction() {
              
            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";
            File.Create(filePath).Dispose(); 
            using (TextWriter fileWriter = new StreamWriter(filePath))
            {
                fileWriter.WriteLine("V " + VerifyNumber);
                fileWriter.WriteLine("M ");
                fileWriter.WriteLine("Rc ");
                fileWriter.WriteLine("Rp ");
            }
            Properties.Settings.Default.SQL_local =
                string.Format("Data Source={0};Persist Security Info=True;User ID=singde;Password=city1234", dbtargetIp);
            Properties.Settings.Default.SQL_global =
               string.Format("Data Source={0};Persist Security Info=True;User ID=singde;Password=city1234", dbtargetIp);
            Properties.Settings.Default.SystemSerialNumber = VerifyNumber;
            Properties.Settings.Default.MedBagPrinter = "";
            Properties.Settings.Default.ReceiptPrinter = "";
            Properties.Settings.Default.ReportPrinter = ""; 
            Properties.Settings.Default.Save();

            Pharmacy p = new Pharmacy();
            p.Id = MedicalNum;
            p.Name = PharmacyName;
            p.Tel = PharmacyTel;
            p.Address = PharmacyTel;
            p.InsertPharmacy();
            Messenger.Default.Send(new NotificationMessage("CloseVerifyPharmacyWindow")); 
        }
        #endregion
    }
}
