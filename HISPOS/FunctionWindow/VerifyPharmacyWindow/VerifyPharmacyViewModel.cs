using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.IO;
using System.Xml;

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

        public string VerifyNumber
        {
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

        #endregion Var

        #region Command

        public RelayCommand VerifyCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }

        #endregion Command

        public VerifyPharmacyViewModel()
        {
            VerifyCommand = new RelayCommand(VerifyAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            Isverify = false;
        }

        #region Action

        private void VerifyAction()
        {
            if (string.IsNullOrEmpty(VerifyNumber))
            {
                MessageWindow.ShowMessage("請輸入認證碼", NewClass.MessageType.WARNING);
                return;
            }
            XmlDocument xml = WebApi.GetPharmacyInfoByVerify(VerifyNumber);
            if (string.IsNullOrEmpty(xml.InnerText))
            {
                MessageWindow.ShowMessage("找不到認證藥局 請聯絡工程師", NewClass.MessageType.WARNING);
                return;
            }
            PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
            MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
            PharmacyTel = xml.SelectSingleNode("CurrentPharmacyInfo/Telphone").InnerText;
            PharmacyAddress = xml.SelectSingleNode("CurrentPharmacyInfo/Address").InnerText;
            dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
            Isverify = true;
        }

        private void SubmitAction()
        {
            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";
            File.Create(filePath).Dispose();
            using (TextWriter fileWriter = new StreamWriter(filePath))
            {
                fileWriter.WriteLine("V " + VerifyNumber);
                fileWriter.WriteLine("M ");
                fileWriter.WriteLine("Rc $點陣");
                fileWriter.WriteLine("Rp ");
                fileWriter.WriteLine("Com ");
            }
            Properties.Settings.Default.SQL_local =
                string.Format("Data Source={0};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword", dbtargetIp);
            Properties.Settings.Default.SQL_global =
               string.Format("Data Source={0};Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword", dbtargetIp);
            Properties.Settings.Default.SystemSerialNumber = VerifyNumber;
            Properties.Settings.Default.MedBagPrinter = "";
            Properties.Settings.Default.ReceiptPrinter = "";
            Properties.Settings.Default.ReportPrinter = "";
            Properties.Settings.Default.ReaderComPort = "";
            Properties.Settings.Default.ReceiptForm = "點陣";
            Properties.Settings.Default.Save();

            Pharmacy p = new Pharmacy();
            p.ID = MedicalNum;
            p.Name = PharmacyName;
            p.Tel = PharmacyTel;
            p.Address = PharmacyAddress;
            p.InsertPharmacy();
            Messenger.Default.Send(new NotificationMessage("CloseVerifyPharmacyWindow"));
        }

        #endregion Action
    }
}