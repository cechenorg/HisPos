using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.MedicineRefactoring;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using System.Linq;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.CooperativeInstitution;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public enum PrescriptionType
    {
        Normal = 0,
        Cooperative = 1,
        Orthopedics = 2
    }

    public enum PrescriptionSource
    {
        Normal = 0,
        Register = 1
    }

    public class Prescription : ObservableObject
    {
        public Prescription()
        {
            Institution = new Institution();
            Division = new Division();
            Pharmacist = new MedicalPersonnel();
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            AdjustCase = new AdjustCase();
            PrescriptionCase = new PrescriptionCase();
            Copayment = new Copayment();
            PaymentCategory = new PaymentCategory();
            SpecialTreat = new SpecialTreat();
        }

        public Prescription(DataRow r)
        {
            PrescriptionPoint = new PrescriptionPoint(r);
            PrescriptionStatus = new PrescriptionStatus(r);
        }

        public Prescription(OrthopedicsPrescription c)
        {
            #region CooPreVariable
            var prescription = c.DeclareXmlDocument.Prescription;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            var customer = prescription.CustomerProfile.Customer;
            var birthYear = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1911 : int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(5, 2));
            #endregion 
            Type = PrescriptionType.Orthopedics;
            SourceId = c.CooperativePrescriptionId;
            Remark = customer.Remark;
            PrescriptionStatus.IsVIP = Remark.EndsWith("Y");
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            #region InitTreatment
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            for (int i = 0; i < diseaseCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MainDisease.ID = diseases[i].Code;
                        break;
                    case 1:
                        SubDisease.ID = diseases[i].Code;
                        break;
                }
            }
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            Copayment = new Copayment();
            if (!string.IsNullOrEmpty(insurance.CopaymentCode))
            {
                switch (insurance.CopaymentCode)
                {
                    case "003":
                    case "004":
                    case "007":
                    case "009":
                    case "I22":
                    case "001":
                    case "002":
                    case "005":
                    case "006":
                    case "008":
                    case "902":
                    case "903":
                    case "906":
                    case "907":
                        Copayment = VM.GetCopayment(insurance.CopaymentCode);
                        break;
                }
            }
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode;

            TreatDate = Convert.ToDateTime(c.InsertDate);
            AdjustDate = DateTime.Today;
            PaymentCategory = VM.GetPaymentCategory("4");
            SpecialTreat = new SpecialTreat();
            #endregion
            Patient = new Customer(customer,birthYear,birthMonth,birthDay);
            Card = new IcCard();
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead?.Equals("D") ?? false;
            foreach (var m in prescription.MedicineOrder.Item)
            {
                Medicines.Add(new Medicine(m));
            }
        }

        public Prescription(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool IsRead)
        {
            #region CooPreVariable
            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            var birthYear = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1911 : int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(5, 2));
            #endregion
            Type = PrescriptionType.Cooperative;
            SourceId = sourceId;
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Card = new IcCard();
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            for (var i = 0; i < diseaseCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MainDisease.ID = diseases[i].Code;
                        break;
                    case 1:
                        SubDisease.ID = diseases[i].Code;
                        break;
                }
            }
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            Copayment = new Copayment();
            if (!string.IsNullOrEmpty(insurance.CopaymentCode))
            {
                switch (insurance.CopaymentCode)
                {
                    case "003":
                    case "004":
                    case "007":
                    case "009":
                    case "I22":
                    case "001":
                    case "002":
                    case "005":
                    case "006":
                    case "008":
                    case "902":
                    case "903":
                    case "906":
                    case "907":
                        Copayment = VM.GetCopayment(insurance.CopaymentCode);
                        break;
                }
            }
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.Insurance.IcErrorCode;

            TreatDate = treatDate.Date;
            AdjustDate = DateTime.Today;
            PaymentCategory = VM.GetPaymentCategory("4");
            SpecialTreat = new SpecialTreat();
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = IsRead;
            foreach (var m in prescription.MedicineOrder.Item)
            {
                Medicines.Add(new Medicine(m));
            }
        }
        #region Properties
        public string SourceId { get; set; }
        public string Remark { get; set; }
        public Customer Patient { get; set; }
        public IcCard Card { get; set; }
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceCode { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int? DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public Medicines Medicines { get; set; }
        public PrescriptionType Type { get; set; }
        public PrescriptionSource Source { get; set; }
        private Institution institution;//釋出院所 D21
        public Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
                CheckTypeByInstitution();
            }
        }

        private Division division;//就醫科別 D13
        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
            }
        }

        private MedicalPersonnel pharmacist;//醫事人員代號 D25
        public MedicalPersonnel Pharmacist
        {
            get => pharmacist;
            set
            {
                Set(() => Pharmacist, ref pharmacist, value);
            }
        }
        public string MedicalNumber { get; set; } //就醫序號 D7

        private DateTime? treatDate;//就醫日期 D7
        public DateTime? TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
            }
        }

        private DateTime? adjustDate;//調劑日期 D23
        public DateTime? AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
            }
        }

        private DiseaseCode mainDisease;//主診斷代碼(國際疾病分類碼1) D8
        public DiseaseCode MainDisease
        {
            get => mainDisease;
            set
            {
                Set(() => MainDisease, ref mainDisease, value);
            }
        }

        private DiseaseCode subDisease;//副診斷代碼(國際疾病分類碼2) D9
        public DiseaseCode SubDisease
        {
            get => subDisease;
            set
            {
                Set(() => SubDisease, ref subDisease, value);
            }
        }

        private int? chronicTotal;//連續處方可調劑次數 D36
        public int? ChronicTotal
        {
            get => chronicTotal;
            set
            {
                Set(() => ChronicTotal, ref chronicTotal, value);
            }
        }

        private int? chronicSeq;
        public int? ChronicSeq
        {
            get => chronicSeq;
            set
            {
                Set(() => ChronicSeq, ref chronicSeq, value);

            }
        }//連續處方箋調劑序號 D35

        private AdjustCase adjustCase;//調劑案件 D1
        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private PrescriptionCase prescriptionCase;//原處方服務機構之案件分類  D22
        public PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                Set(() => PrescriptionCase, ref prescriptionCase, value);
            }
        }

        private Copayment copayment;//部分負擔代碼  D15
        public Copayment Copayment
        {
            get => copayment;
            set
            {
                Set(() => Copayment, ref copayment, value);
            }
        }

        private PaymentCategory paymentCategory;//給付類別 D5
        public PaymentCategory PaymentCategory
        {
            get => paymentCategory;
            set
            {
                Set(() => PaymentCategory, ref paymentCategory, value);
            }
        }

        public string OriginalMedicalNumber { get; set; } //原處方就醫序號 D43

        private SpecialTreat specialTreat;//特定治療代碼 D26
        public SpecialTreat SpecialTreat
        {
            get => specialTreat;
            set
            {
                Set(() => SpecialTreat, ref specialTreat, value);
            }
        }
        private string tempMedicalNumber;

        public string TempMedicalNumber
        {
            get => tempMedicalNumber;
            set
            {
                Set(() => TempMedicalNumber, ref tempMedicalNumber, value);
            }
        }
        #endregion

        public bool CheckDiseaseEquals(List<string> parameters)
        {
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (elementName.Equals("MainDiagnosis"))
            {
                return diseaseID.Equals(MainDisease.FullName);
            }
            return diseaseID.Equals(SubDisease.FullName);
        }

        private void CheckTypeByInstitution()
        {
            if (Institution != null && !string.IsNullOrEmpty(Institution.ID) && Institution.CheckCooperative())
            {
                if (Institution.CheckIsOrthopedics())
                {
                    Type = PrescriptionType.Orthopedics;
                    PrescriptionStatus.IsBuckle = false;
                }
                else
                {
                    Type = PrescriptionType.Cooperative;
                    var clinic = VM.CooperativeClinicSettings.Single(c => c.CooperavieClinic.ID.Equals(Institution.ID));
                    PrescriptionStatus.IsBuckle = clinic.IsBuckle;
                }
            }
            else//非合作診所
            {
                Type = PrescriptionType.Normal;
                PrescriptionStatus.IsBuckle = true;
            }
        }

        public void GetMedicines()
        {
            throw new NotImplementedException();
        }
    }
}
