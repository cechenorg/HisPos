using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
            Customer = new Customer();
            Pharmacy = new Pharmacy();
            Treatment = new Treatment();
            Medicines = new ObservableCollection<DeclareMedicine>();
        }
     
        public Prescription(Customer customer, Pharmacy pharmacy, Treatment treatment, ObservableCollection<DeclareMedicine> medicines)
        {
            Customer = customer;
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }

        public Prescription(DataRow row)
        {
            Customer = new Customer(row);
            Pharmacy = new Pharmacy(row);
            Treatment = new Treatment(row);
            Medicines = MedicineDb.GetDeclareMedicineByMasId(row["HISDECMAS_ID"].ToString());
        }
        public Prescription(XmlNode xml)
        {
            Customer = new Customer(xml);
            Customer.Id = CustomerDb.CheckCustomerExist(Customer);
            Pharmacy = new Pharmacy(xml);
            Treatment = new Treatment(xml);
            ChronicSequence = xml.SelectSingleNode("d35") == null ? null : xml.SelectSingleNode("d35").InnerText;
            ChronicTotal = xml.SelectSingleNode("d36") == null ? null : xml.SelectSingleNode("d36").InnerText;
            OriginalMedicalNumber = xml.SelectSingleNode("d43") == null ? null : xml.SelectSingleNode("d43").InnerText;
        }
        public Customer Customer { get; set; }
        public Pharmacy Pharmacy { get; set; } //藥局
        public Treatment Treatment { get; set; } //在醫院拿到的資料
        public string ChronicSequence { get; set; }//D35連續處方箋調劑序號
        public string ChronicTotal { get; set; }//D36連續處方可調劑次數
        public ObservableCollection<DeclareMedicine> Medicines { get; set; }
        public ObservableCollection<CustomerHistory.CustomerHistory> CustomerHistories { get; set; }
        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
        public string ErrorMessage = "";
        private bool adjustCaseNull = false;

        public string CheckPrescriptionData()
        {
            ErrorMessage = string.Empty;
            Customer.IcCard.CheckIcNumber(Customer.IcCard.IcNumber);
            Customer.CheckBirthDay(Customer.Birthday);
            CheckReleaseInstitution();
            CheckDivision();
            CheckDoctor();
            CheckMedicalNumber();
            CheckTreatmentCase();
            CheckTreatDate();
            CheckAdjustDate();
            CheckDiseaseCodes();
            CheckPaymentCategory();
            CheckCopayment();
            CheckChronicTimes();
            Pharmacy = MainWindow.CurrentUser.Pharmacy;
            Treatment.MedicalPersonId = MainWindow.CurrentUser.IcNumber;
            Customer.Id = "1";
            return ErrorMessage;
        }

        /*
        * 確認就醫序號D7
        */

        private void CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(Customer.IcCard.MedicalNumber))
            {
                ErrorMessage += "就醫序號未填寫\n";
            }
            if (!string.IsNullOrEmpty(ChronicSequence))
            {
                if (int.Parse(ChronicSequence) > 1)
                    Customer.IcCard.MedicalNumber = "IC0" + ChronicSequence;
            }
            if (CheckHomeCareAndSmokingCessation())
                Customer.IcCard.MedicalNumber = "N";
            if (!Customer.IcCard.MedicalNumber.Contains("IC") && Customer.IcCard.MedicalNumber != "N")
            {
                Regex medicalNumberReg = new Regex(@"\d+");
                if (!medicalNumberReg.IsMatch(Customer.IcCard.MedicalNumber))
                    ErrorMessage += "就醫序號輸入格式錯誤\n";
            }
        }

        /*
         * 判斷調劑案件為藥是居家照護及協助戒菸計畫
         */

        private bool CheckHomeCareAndSmokingCessation()
        {
            if (string.IsNullOrEmpty(Treatment.AdjustCase.Id))
            {
                MessageWindow m = new MessageWindow("未選擇調劑案件", MessageType.ERROR);
                if (!adjustCaseNull)
                {
                    adjustCaseNull = true;
                    m.Show();
                }
            }
            return Treatment.AdjustCase.Id.StartsWith("D") || Treatment.AdjustCase.Id.StartsWith("5");
        }

        /*
         * 確認釋出院所D21
         */

        private void CheckReleaseInstitution()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Id))
                ErrorMessage += "未選擇釋出院所";
            if (Treatment.MedicalInfo.Hospital.Id.Equals("N"))
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    ErrorMessage += "非藥事居家照護(調劑案件:D).協助辦理門診戒菸計畫(調劑案件:5)者，釋出院所不可為\"N\"";
            }
        }

        /*
         * 確認就醫科別D13
         */

        private void CheckDivision()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Division.Id))
            {
                if (!CheckHomeCareAndSmokingCessation())
                    ErrorMessage += "未選擇就醫科別\n";
            }
        }

        /*
         * 確認診治醫生D24
         */

        private void CheckDoctor()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Doctor.Id))
            {
                if (!CheckHomeCareAndSmokingCessation())
                    ErrorMessage += "未填寫診治醫師代號\n";
            }
        }

        /*
         *確認處方案件D22
         */

        private void CheckTreatmentCase()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.TreatmentCase.Id))
            {
                if (!CheckHomeCareAndSmokingCessation())
                {
                    ErrorMessage += "請選擇處方案件\n";
                }
            }
        }

        /*
         * 確認就醫日期D14
         */

        private void CheckTreatDate()
        {
            if (string.IsNullOrEmpty(Treatment.TreatDateStr))
            {
                if (!Treatment.AdjustCase.Id.Equals("D"))
                    ErrorMessage += "未選擇就醫日期\n";
            }
        }

        /*
         * 確認調劑日期D23
         */

        private void CheckAdjustDate()
        {
            if (string.IsNullOrEmpty(Treatment.AdjustDateStr))
                ErrorMessage += "未選擇調劑日期,如為藥事居家照護請選擇訪視日期\n";
        }

        /*
         * 確認國際疾病代碼D8.D9
         */

        private void CheckDiseaseCodes()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.MainDiseaseCode.Id))
            {
                if (!Treatment.AdjustCase.Id.Equals("D"))
                    ErrorMessage += "未填寫主要診斷代碼\n";
            }
        }

        /*
         * 確認給付類別D5
         */

        private void CheckPaymentCategory()
        {
            if (string.IsNullOrEmpty(Treatment.PaymentCategory.Id))
            {
                if (!Treatment.AdjustCase.Id.Equals("D"))
                    ErrorMessage += "未選擇給付類別\n";
            }
        }

        /*
         * 確認部分負擔代碼D15
         */

        private void CheckCopayment()
        {
            if (string.IsNullOrEmpty(Treatment.Copayment.Id))
                ErrorMessage += "未選擇部分負擔\n";
            if (Treatment.Copayment.Id.Equals("903"))
            {
                var newBornBirth = DateTimeExtensions.ToUsDate(Customer.IcCard.IcMarks.NewbornsData.Birthday);
                var newBornAge = DateTime.Now - newBornBirth;
                CheckNewBornAge(newBornAge);
            }
        }

        /*
         * 確認新生兒就醫
         */

        private void CheckNewBornAge(TimeSpan newBornAge)
        {
            if (newBornAge.Days > 60)
                ErrorMessage += "新生兒依附註記方式就醫者新生兒年齡應小於60日\n";
        }

        /*
         * 確認慢箋領藥次數D35.36
         */

        private void CheckChronicTimes()
        {
            if (!Treatment.AdjustCase.Id.Equals("2")) return;
            if (string.IsNullOrEmpty(ChronicSequence) || string.IsNullOrEmpty(ChronicTotal))
                ErrorMessage += "未填寫領藥次數(調劑序號/可調劑次數)\n";
        }
    }
}