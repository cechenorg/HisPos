using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public RelayCommand SubmmitCommand { get; set; }
        #endregion
        public VerifyPharmacyViewModel() {
            Isverify = false;
        } 
        #region Action
        private void VerifyAction() {
            if (string.IsNullOrEmpty(VerifyNumber)) {
                MessageWindow.ShowMessage("請輸入認證碼",Class.MessageType.WARNING);
                return;
            }
            XmlDocument xml = WebApi.GetPharmacyInfoByVerify(VerifyNumber);
            PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
            MedicalNum = xml.SelectSingleNode("CurrentPharmacyInfo/MedicalNum").InnerText;
            PharmacyTel = xml.SelectSingleNode("CurrentPharmacyInfo/Telphone").InnerText;
            PharmacyAddress = xml.SelectSingleNode("CurrentPharmacyInfo/Address").InnerText;
            dbtargetIp = xml.SelectSingleNode("CurrentPharmacyInfo/DbTargetIp").InnerText;
            Isverify = true;
        }
        #endregion
    }
}
