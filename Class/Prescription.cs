using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using JetBrains.Annotations;

namespace His_Pos.Class
{
    public class Prescription : INotifyPropertyChanged
    {
        public Prescription()
        {
            Customer = new Customer();
            Pharmacy = new Pharmacy.Pharmacy();
            Treatment = new Treatment();
            Medicines = new ObservableCollection<AbstractClass.Product>();
            EList = new ErrorList();
        }
     
        public Prescription(Customer customer, Pharmacy.Pharmacy pharmacy, Treatment treatment, ObservableCollection<AbstractClass.Product> medicines)
        {
            Customer = customer;
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }

        public Prescription(DataRow row)
        {
            var errorList = new ErrorList();
            Customer = new Customer(row,"fromXml");
            Pharmacy = new Pharmacy.Pharmacy(row);
            Treatment = new Treatment(row);
            Medicines = MedicineDb.GetDeclareMedicineByMasId(row["HISDECMAS_ID"].ToString());
            ChronicSequence = row["HISDECMAS_CONTINUOUSNUM"].ToString();
            ChronicTotal = row["HISDECMAS_CONTINUOUSTOTAL"].ToString();
        }

        public Prescription(XmlNode xml)
        {
            Customer = new Customer(xml);
            Pharmacy = new Pharmacy.Pharmacy(xml);
            Treatment = new Treatment(xml);
            ChronicSequence = xml.SelectSingleNode("d35")?.InnerText;
            ChronicTotal = xml.SelectSingleNode("d36")?.InnerText;
            OriginalMedicalNumber = xml.SelectSingleNode("d43")?.InnerText;
        }

        public Prescription(DeclareFileDdata d)
        {
            Customer = new Customer(d);
            Pharmacy = new Pharmacy.Pharmacy(d);
            Treatment = new Treatment(d);
            ChronicSequence = !string.IsNullOrEmpty(d.Dbody.D35) ? d.Dbody.D35 : string.Empty;
            ChronicTotal = !string.IsNullOrEmpty(d.Dbody.D36) ? d.Dbody.D36 : string.Empty;
            OriginalMedicalNumber = !string.IsNullOrEmpty(d.Dbody.D43) ? d.Dbody.D43 : string.Empty;
        }

        private Customer _customer;

        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public Pharmacy.Pharmacy Pharmacy { get; set; } //藥局
        private Treatment _treatment;

        public Treatment Treatment
        {
            get => _treatment;
            set
            {
                _treatment = value;
                OnPropertyChanged(nameof(Treatment));
            }
        } //在醫院拿到的資料

        public string MedicalRecordId = "";
        public string ChronicSequence { get; set; }//D35連續處方箋調劑序號
        public string ChronicTotal { get; set; }//D36連續處方可調劑次數
        private bool _isGetIcCard;
        public bool IsGetIcCard //健保卡是否讀取成功
        {
            get => _isGetIcCard;
            set
            {
                _isGetIcCard = value;
                OnPropertyChanged(nameof(IsGetIcCard));
            }
        }

        private ObservableCollection<AbstractClass.Product> _medicines;

        public ObservableCollection<AbstractClass.Product> Medicines
        {
            get => _medicines;
            set
            {
                _medicines = value;
                OnPropertyChanged(nameof(Medicines));
            }
        }

        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
        public ErrorList EList = new ErrorList();
        private bool adjustCaseNull = false;
        public bool Declare { get; set; }
        private List<Error> _errorMessage;
        public List<Error> CheckPrescriptionData()
        {
            _errorMessage = new List<Error>();
            CheckPatientInfo();
            CheckReleaseInstitution();
            CheckDivision();
            CheckMedicalNumber();
            CheckTreatmentCase();
            CheckDiseaseCodes();
            CheckPaymentCategory();
            CheckCopayment();
            CheckChronicTimes();
            Pharmacy = MainWindow.CurrentPharmacy;
            Pharmacy.MedicalPersonnel.IcNumber = MainWindow.CurrentUser.IcNumber;
            return _errorMessage;
        }

        private void CheckPatientInfo()
        {
            if(string.IsNullOrEmpty(Customer.Name))
                AddError("0","病患姓名未填寫");
            //Customer.IcCard.CheckIcNumber(Customer.IcCard.IcNumber);
            CheckBirthDay(Customer.Birthday);
        }

        /*
        * 確認就醫序號D7
        */

        private void CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(Customer.IcCard.MedicalNumber))
            {
                AddError("0", "就醫序號未填寫");
                return;
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
                    AddError("0", "就醫序號輸入格式錯誤");
            }
        }

        /*
         * 判斷調劑案件為藥是居家照護及協助戒菸計畫
         */
        private bool CheckHomeCareAndSmokingCessation()
        {
            if (!string.IsNullOrEmpty(Treatment.AdjustCase.Id))
                return Treatment.AdjustCase.Id.StartsWith("D") || Treatment.AdjustCase.Id.StartsWith("5");
           
            AddError("0", "未選擇調劑案件");
            return false;
        }

        /*
         * 確認釋出院所D21
         */

        private void CheckReleaseInstitution()
        {
            if (string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Id))
            {
                AddError("0", "未選擇釋出院所");
                return;
            }

            if (!Treatment.MedicalInfo.Hospital.Id.Equals("N")) return;

            if (CheckHomeCareAndSmokingCessation() == false)
                AddError("0", "非藥事居家照護(調劑案件:D).協助辦理門診戒菸計畫(調劑案件:5)者，釋出院所不可為\"N\"");
        }

        /*
         * 確認就醫科別D13
         */

        private void CheckDivision()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Division.Id)) return;

            if (CheckHomeCareAndSmokingCessation()) return;
            AddError("0", "未選擇就醫科別");

        }

        /*
         *確認處方案件D22
         */

        private void CheckTreatmentCase()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.TreatmentCase.Id)) return;

            if (!CheckHomeCareAndSmokingCessation())
                AddError("0", "請選擇處方案件");
        }

        /*
         * 確認國際疾病代碼D8.D9
         */

        private void CheckDiseaseCodes()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.MainDiseaseCode.Id)) return;

            if (!Treatment.AdjustCase.Id.Equals("D"))
                AddError("0", "未填寫主要診斷代碼");
        }

        /*
         * 確認給付類別D5
         */

        private void CheckPaymentCategory()
        {
            if (Treatment.PaymentCategory is null) return;
            if (!string.IsNullOrEmpty(Treatment.PaymentCategory.Id)) return;

            if (!Treatment.AdjustCase.Id.Equals("D"))
                AddError("0", "未選擇給付類別");
        }

        /*
         * 確認部分負擔代碼D15
         */

        private void CheckCopayment()
        {
            if (string.IsNullOrEmpty(Treatment.Copayment.Id))
                AddError("0", "未選擇部分負擔");

            if (!Treatment.Copayment.Id.Equals("903")) return;
            
            var newBornAge = DateTime.Now - Customer.IcCard.IcMarks.NewbornsData.Birthday;
            CheckNewBornAge(newBornAge);
        }

        /*
         * 確認新生兒就醫
         */

        private void CheckNewBornAge(TimeSpan newBornAge)
        {
            if (newBornAge.Days > 60)
                AddError("0", "新生兒依附註記方式就醫者新生兒年齡應小於60日");
        }

        /*
         * 確認慢箋領藥次數D35.36
         */

        private void CheckChronicTimes()
        {
            if (!Treatment.AdjustCase.Id.Equals("2")) return;
            if (string.IsNullOrEmpty(ChronicSequence) || string.IsNullOrEmpty(ChronicTotal))
            {
                AddError("0", "未填寫領藥次數(調劑序號/可調劑次數)");
            }
        }

        public void CheckBirthDay(DateTime customerBirthday)
        {
            if (customerBirthday >= DateTime.Now)
            {
                AddError("0", "生日不可超過現在時間");
            }
        }

        private void AddError(string id, string content)
        {
            var e = new Error
            {
                Id = id,
                Content = content
            };
            _errorMessage.Add(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}