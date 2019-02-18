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
            IsGetIcCard = false;
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
            Medicines = null;/// MedicineDb.GetDeclareMedicineByMasId(row["HISDECMAS_ID"].ToString());
            ChronicSequence = row["HISDECMAS_CONTINUOUSNUM"].ToString();
            ChronicTotal = row["HISDECMAS_CONTINUOUSTOTAL"].ToString();
            OriginalMedicalNumber = row["HISDECMAS_OLDNUMDER"].ToString();
            DataColumnCollection columns = row.Table.Columns;
            if (columns.Contains("IS_GETCARD") && !string.IsNullOrEmpty(row["IS_GETCARD"].ToString()))
            {
                IsGetIcCard = (bool)row["IS_GETCARD"];
            }
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
        public Prescription(XmlDocument xml)
        {
            Customer = new Customer(xml);
            Pharmacy = new Pharmacy.Pharmacy(xml);
            Treatment = new Treatment(xml);
            Medicines = new ObservableCollection<AbstractClass.Product>();
            foreach (XmlNode xmlNode in xml.SelectNodes("DeclareXml/DeclareXmlDocument/case/orders/item")){
                if(xmlNode.Attributes["id"].Value.Length < 10)
                    Medicines.Add(new PrescriptionOTC(xmlNode));
                else
                    Medicines.Add(new DeclareMedicine(xmlNode));
            }
            ChronicSequence = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/continous_prescription").Attributes["count"].Value;
            ChronicTotal = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/continous_prescription").Attributes["total"].Value;

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

        private Pharmacy.Pharmacy _pharmacy;

        public Pharmacy.Pharmacy Pharmacy
        {
            get => _pharmacy;
            set
            {
                _pharmacy = value;
                OnPropertyChanged(nameof(Pharmacy));
            }
        } //藥局

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
        private bool _isDeposit;
        public bool IsDeposit
        {
            get => _isDeposit;
            set
            {
                _isDeposit = value;
                OnPropertyChanged(nameof(IsDeposit));
            }
        }//是否押金
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

        private string _originalMedicalNumber;

        public string OriginalMedicalNumber
        {
            get => _originalMedicalNumber;
            set
            {
                _originalMedicalNumber = value;
                OnPropertyChanged(nameof(OriginalMedicalNumber));
            }
        } //D43原處方就醫序號

        private int _copaymentPoint;

        public int CopaymentPoint
        {
            get => _copaymentPoint;
            set
            {
                _copaymentPoint = value;
                OnPropertyChanged(nameof(CopaymentPoint));
            }
        }//D17部分負擔點數

        private double _medicinePoint;

        public double MedicinePoint
        {
            get => _medicinePoint;
            set
            {
                _medicinePoint = value;
                OnPropertyChanged(nameof(MedicinePoint));
            }
        }//D33用藥明細點數

        public ErrorList EList = new ErrorList();
        private bool adjustCaseNull = false;
        public bool Declare { get; set; }
        private List<Error> _errorMessage;
        public List<Error> CheckPrescriptionData()
        {
            _errorMessage = new List<Error>();
            CheckReleaseInstitution();
            CheckTreatmentCase();
            CheckPatientInfo();
            CheckAdjustDateAndTreatmentDate();
            CheckMedicalPersonnel();
            CheckChronicTimes();
            CheckMedicalNumber();
            CheckCopayment();
            CheckDivision();
            CheckDiseaseCodes();
            CheckPaymentCategory();
            return _errorMessage;
        }

        private void CheckMedicalPersonnel()
        {
            if (Pharmacy.MedicalPersonnel is null)
                AddError("0", "請選擇調劑藥師");
            else
            {
                if (string.IsNullOrEmpty(Pharmacy.MedicalPersonnel.IcNumber))
                    AddError("0", "請選擇調劑藥師");
            }
        }

        private void CheckPatientInfo()
        {
            if(string.IsNullOrEmpty(Customer.Name))
                AddError("0","病患姓名未填寫");
            //Customer.IcCard.CheckIcNumber(Customer.IcCard.IdNumber);
            CheckBirthDay();
        }

        /*
        * 確認就醫序號D7
        */

        private void CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(Customer.IcCard.MedicalNumber))
            {
                AddError("0", "就醫序號未填寫");
            }
            else if (Treatment.AdjustCase.ID.Equals("D"))
            {
                Customer.IcCard.MedicalNumber = "N";
            }
            else if (Treatment.AdjustCase.ID.Equals("2"))
            {
                if (int.Parse(ChronicSequence) > 1)
                {
                    var tmpMedicalNumber = Customer.IcCard.MedicalNumber;
                    OriginalMedicalNumber = tmpMedicalNumber;
                    Customer.IcCard.MedicalNumber = "IC0" + ChronicSequence;
                }
            }
            else if(!Customer.IcCard.MedicalNumber.Contains("IC") && Customer.IcCard.MedicalNumber != "N")
            {
                var medicalNumberReg = new Regex(@"\d+");
                if (!medicalNumberReg.IsMatch(Customer.IcCard.MedicalNumber) || Customer.IcCard.MedicalNumber.Length != 4)
                    AddError("0", "就醫序號輸入格式錯誤，須為4位數字，不足補0(如:0001)");
            }
        }

        /*
         * 判斷調劑案件為藥是居家照護及協助戒菸計畫
         */
        private bool CheckHomeCareAndSmokingCessation()
        {
            if (Treatment.AdjustCase is null)
                return false;
            if (!string.IsNullOrEmpty(Treatment.AdjustCase.ID))
                return Treatment.AdjustCase.ID.StartsWith("D") || Treatment.AdjustCase.ID.StartsWith("5");
           
            AddError("0", "未選擇調劑案件");
            return false;
        }

        /*
         * 確認釋出院所D21
         */

        private void CheckReleaseInstitution()
        {
            if (Treatment.MedicalInfo.Hospital is null || string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Id))
            {
                AddError("0", "未選擇釋出院所");
                return;
            }

            if (!Treatment.MedicalInfo.Hospital.Id.Equals("N")) return;
            if (CheckHomeCareAndSmokingCessation() == false)
                AddError("0", "非藥事居家照護(調劑案件:D).協助辦理門診戒菸計畫(調劑案件:5)者，釋出院所不可為\"N\"");
            else
            {
                ///Treatment.MedicalInfo.Hospital = MainWindow.Institutions.SingleOrDefault(h=>h.Id.Equals("N")).DeepCloneViaJson();
            }
        }

        /*
         * 確認就醫科別D13
         */

        private void CheckDivision()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.Hospital.Division.ID)) return;
            if (CheckHomeCareAndSmokingCessation()) return;
            AddError("0", "未選擇就醫科別");
        }

        /*
         *確認處方案件D22
         */

        private void CheckTreatmentCase()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.TreatmentCase.ID)) return;
            if (!CheckHomeCareAndSmokingCessation())
                AddError("0", "請選擇處方案件");
        }

        /*
         * 確認國際疾病代碼D8.D9
         */

        private void CheckDiseaseCodes()
        {
            if (!string.IsNullOrEmpty(Treatment.MedicalInfo.MainDiseaseCode.Id)) return;

            if (!Treatment.AdjustCase.ID.Equals("D"))
                AddError("0", "未填寫主要診斷代碼");
        }

        /*
         * 確認給付類別D5
         */

        private void CheckPaymentCategory()
        {
            if (Treatment.PaymentCategory is null) return;
            if (!string.IsNullOrEmpty(Treatment.PaymentCategory.ID)) return;

            if (!Treatment.AdjustCase.ID.Equals("D") || !Treatment.AdjustCase.ID.Equals("2"))
                AddError("0", "未選擇給付類別");
        }

        /*
         * 確認部分負擔代碼D15
         */

        private void CheckCopayment()
        {
            if (string.IsNullOrEmpty(Treatment.Copayment.Id))
                AddError("0", "未選擇部分負擔");

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
            if (!Treatment.AdjustCase.ID.Equals("2")) return;
            if (string.IsNullOrEmpty(ChronicSequence) || string.IsNullOrEmpty(ChronicTotal))
            {
                AddError("0", "未填寫領藥次數(可調劑次數 - 領藥次數)");
            }
        }

        private void CheckBirthDay()
        {
            if (DateTime.Compare(Customer.Birthday, DateTime.Now) > 0)
            {
                AddError("0", "生日不可超過現在時間");
            }
        }

        private void CheckAdjustDateAndTreatmentDate()
        {
            if (DateTime.Compare(Treatment.TreatmentDate, Treatment.AdjustDate) > 0)
            {
                AddError("0", "就醫日期不可大於調劑日期");
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