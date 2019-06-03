using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
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
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.CooperativeInstitution;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions;
using His_Pos.NewClass.PrescriptionRefactoring.Service;
using His_Pos.NewClass.Product.Medicine.MedBag;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Medicine = His_Pos.NewClass.MedicineRefactoring.Medicine;
using Medicines = His_Pos.NewClass.MedicineRefactoring.Medicines;
using Resources = His_Pos.Properties.Resources; 


namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public enum PrescriptionType
    {
        Normal = 0,
        Cooperative = 1,
        Orthopedics = 2
    }

    public class Prescription : ObservableObject,ICloneable
    {
        public Prescription()
        {
            Medicines = new Medicines();
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
            Patient = new Customer();
            Card = new IcCard();
        }

        public Prescription(DataRow r,ChronicType type)
        {
            ID = r.Field<int>("ID");
            Patient = Customer.GetCustomerByCusId(r.Field<int>("CustomerID"));
            Institution = VM.GetInstitution(r.Field<string>("InstitutionID"));
            Division = VM.GetDivision(r.Field<string>("DivisionID"));
            AdjustDate = r.Field<DateTime>("AdjustDate");
            TreatDate = r.Field<DateTime?>("TreatmentDate");
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicSequence").ToString()))
                ChronicSeq = int.Parse(r.Field<byte>("ChronicSequence").ToString());
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicTotal").ToString()))
                ChronicTotal = int.Parse(r.Field<byte>("ChronicTotal").ToString());
            if (!string.IsNullOrEmpty(r.Field<string>("MainDiseaseID")))
                MainDisease = DiseaseCode.GetDiseaseCodeByID(r.Field<string>("MainDiseaseID"));
            if (!string.IsNullOrEmpty(r.Field<string>("SecondDiseaseID")))
                SubDisease = DiseaseCode.GetDiseaseCodeByID(r.Field<string>("SecondDiseaseID"));
            AdjustCase = VM.GetAdjustCase(r.Field<string>("AdjustCaseID"));
            Copayment = VM.GetCopayment(r.Field<string>("CopaymentID"));
            PrescriptionCase = VM.GetPrescriptionCases(r.Field<string>("PrescriptionCaseID"));
            PaymentCategory = VM.GetPaymentCategory(r.Field<string>("PaymentCategoryID"));
            PrescriptionPoint = new PrescriptionPoint(r,type);
            PrescriptionStatus = new PrescriptionStatus(r);
            switch (type)
            {
                case ChronicType.Register:
                    Medicines = new Medicines();
                    Medicines.GetDataByPrescriptionId(ID);
                    break;
                case ChronicType.Reserve:
                    Medicines = new Medicines();
                    Medicines.GetDataByReserveId(ID);
                    break;
            }
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
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            #region InitTreatment
            Card = new IcCard();
            SpecialTreat = new SpecialTreat();
            AdjustDate = DateTime.Today;
            TreatDate = Convert.ToDateTime(c.InsertDate);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            PaymentCategory = VM.GetPaymentCategory("4");
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            OrthopedicsGetDisease(diseases);
            GetCopayment(insurance.CopaymentCode);
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
            #endregion
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead?.Equals("D") ?? false;
            Medicines = new Medicines();
            Medicines.GetDataByOrthopedicsPrescription(prescription.MedicineOrder.Item);
        }

        public Prescription(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead)
        {
            #region CooPreVariable
            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            var cusBirth = customer.Birth.Trim();
            int birthYear = 0, birthMonth = 0, birthDay = 0;
            if (cusBirth.Length >= 7)
            {
                birthYear = string.IsNullOrEmpty(cusBirth) ? 1911 : int.Parse(cusBirth.Substring(0, 3)) + 1911;
                birthMonth = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(3, 2));
                birthDay = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(5, 2));
            }
            #endregion
            Type = PrescriptionType.Cooperative;
            SourceId = sourceId;
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Card = new IcCard();
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            PaymentCategory = VM.GetPaymentCategory("4");
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            TreatDate = treatDate.Date;
            AdjustDate = DateTime.Today;
            SpecialTreat = new SpecialTreat();
            CooperativeGetDisease(diseases);
            GetCopayment(insurance.CopaymentCode);
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber;
                TempMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                TempMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.Insurance.IcErrorCode;
            var cooperativeSetting = VM.CooperativeClinicSettings.Single(s => s.CooperavieClinic.ID.Equals(Institution.ID));
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = isRead;
            PrescriptionStatus.IsBuckle = !(VM.CooperativeClinicSettings.GetWareHouseByPrescription(Institution, AdjustCase.ID) is null);
            Medicines = new Medicines();
            Medicines.GetDataByCooperativePrescription(prescription.MedicineOrder.Item, PrescriptionStatus.IsBuckle);
        }
        #region Properties
        public int ID { get; set; }
        public string SourceId { get; set; }
        public string Remark { get; set; }
        public IcCard Card { get; set; }
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceCode { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public Medicines Medicines { get; set; }
        public PrescriptionType Type { get; set; }

        private Customer patient;
        public Customer Patient
        {
            get => patient;
            set { Set(() => Patient, ref patient, value); }
        }
        private Institution institution;//釋出院所 D21
        public Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
                if (institution == null) return;
                CheckTypeByInstitution();
                Medicines.CheckWareHouse(institution);
            }
        }

        private Division division;//就醫科別 D13
        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
                if(AdjustCase is null) return;
                if ((!AdjustCase.ID.Equals("1") && !AdjustCase.ID.Equals("3"))) return;
                switch (division.ID)
                {
                    case "40":
                        PrescriptionCase = VM.GetPrescriptionCases("19");
                        break;
                    default:
                        PrescriptionCase = VM.GetPrescriptionCases("09");
                        break;
                }
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
                if (chronicSeq == null || !(chronicSeq > 0)) return;
                AdjustCase = VM.GetAdjustCase("2");
                if (ChronicSeq >= 2)
                {
                    MedicalNumber = "IC0" + chronicSeq;
                }
            }
        }//連續處方箋調劑序號 D35

        private AdjustCase adjustCase;//調劑案件 D1
        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
                if (adjustCase == null) return;
                switch (adjustCase.ID)
                {
                    case "4":
                        Copayment = VM.GetCopayment("009");
                        break;
                    case "1":
                    case "3":
                        PrescriptionCase = VM.GetPrescriptionCases("09");
                        PaymentCategory = VM.GetPaymentCategory("04");
                        break;
                    case "2":
                        Copayment = VM.GetCopayment("I22");
                        PrescriptionCase = VM.GetPrescriptionCases("04");
                        PaymentCategory = null;
                        break;
                    case "5":
                        PrescriptionCase = VM.GetPrescriptionCases("B7");
                        TempMedicalNumber = "IC07";
                        Copayment = VM.GetCopayment("Z00");
                        MainWindow.ServerConnection.OpenConnection();
                        MainDisease = DiseaseCode.GetDiseaseCodeByID("F17200");
                        MainWindow.ServerConnection.CloseConnection();
                        break;
                }
            }
        }

        private PrescriptionCase prescriptionCase;//原處方服務機構之案件分類  D22
        public PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                Set(() => PrescriptionCase, ref prescriptionCase, value);
                if (PrescriptionCase == null) return;
                switch (PrescriptionCase.ID)
                {
                    case "007"://山地離島就醫/戒菸免收
                    case "11"://牙醫一般
                    case "12"://牙醫急診
                    case "13"://牙醫門診
                    case "14"://牙醫資源不足方案
                    case "15"://牙周統合照護
                    case "16"://牙醫特殊專案
                    case "19"://牙醫其他專案
                    case "C1"://論病計酬
                        Copayment = VM.GetCopayment("I22");
                        break;
                }
            }
        }

        private Copayment copayment;//部分負擔代碼  D15
        public Copayment Copayment
        {
            get => copayment;
            set
            {
                Set(() => Copayment, ref copayment, value);
                if (Copayment != null)
                {
                    CountCopaymentPoint();
                }
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

        private Medicine selectedMedicine;
        public Medicine SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = true;
            }
        }
        #endregion

        public bool CheckDiseaseEquals(List<string> parameters)
        {
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            return diseaseID.Equals(elementName.Equals("MainDiagnosis") ? MainDisease.FullName : SubDisease.FullName);
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
                    PrescriptionStatus.IsBuckle = !(VM.CooperativeClinicSettings.GetWareHouseByPrescription(Institution, AdjustCase.ID) is null);
                }
            }
            else//非合作診所
            {
                Type = PrescriptionType.Normal;
                PrescriptionStatus.IsBuckle = true;
            }

            if (Medicines == null) return;
            foreach (var m in Medicines)
            {
                m.IsBuckle = PrescriptionStatus.IsBuckle;
            }
        }

        public void UpdateCooperativePrescriptionIsRead()
        {
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(SourceId);
        }

        private void GetCopayment(string copID)
        {
            Copayment = new Copayment();
            if (string.IsNullOrEmpty(copID)) return;
            switch (copID)
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
                    Copayment = VM.GetCopayment(copID);
                    break;
            }
        }
        private void OrthopedicsGetDisease(IReadOnlyList<Item> diseases)
        {
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            for (var i = 0; i < diseaseCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MainDisease.ID = diseases[i].Code;
                        MainDisease.GetData();
                        break;
                    case 1:
                        SubDisease.ID = diseases[i].Code;
                        SubDisease.GetData();
                        break;
                }
            }
        }
        private void CooperativeGetDisease(IReadOnlyList<CooperativePrescription.Item> diseases)
        {
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            for (var i = 0; i < diseaseCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MainDisease.ID = diseases[i].Code;
                        MainDisease.GetData();
                        break;
                    case 1:
                        SubDisease.ID = diseases[i].Code;
                        SubDisease.GetData();
                        break;
                }
            }
        }

        public void CountPrescriptionPoint(bool countSelfPay)
        {
            if (!AdjustCase.ID.Equals("0"))
            {
                PrescriptionPoint.MedicinePoint = Medicines.CountMedicinePoint();
                PrescriptionPoint.SpecialMaterialPoint = Medicines.CountSpecialMedicinePoint();
                CheckCopayment();
                if (Patient.Birthday != null)
                {
                    CheckMedicalServiceData();//確認藥事服務資料
                    var details = SetPrescriptionDetail();//產生藥品資料
                    PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
                }
                PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                               PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
                PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
            }
            if (countSelfPay)
                PrescriptionPoint.AmountSelfPay = Medicines.CountSelfPay();
            PrescriptionPoint.AmountsPay = PrescriptionPoint.CopaymentPoint + PrescriptionPoint.AmountSelfPay;
            PrescriptionPoint.ActualReceive = PrescriptionPoint.AmountsPay;
        }

        private void CheckCopayment()
        {
            if (AdjustCase.ID.Equals("2") || ChronicSeq != null && ChronicSeq > 0 || ChronicTotal != null && ChronicTotal > 0)
                Copayment = VM.GetCopayment("I22");
            if (!CheckFreeCopayment())
                Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
            else
            {
                if (Copayment != null && Copayment.Id.Equals("I21") && PrescriptionPoint.MedicinePoint > 100)
                    Copayment = VM.GetCopayment("I20");
            }
            if (Copayment != null && !CheckFreeCopayment())
                PrescriptionPoint.CopaymentPoint = CountCopaymentPoint();
            else
                PrescriptionPoint.CopaymentPoint = 0;
        }

        private bool CheckFreeCopayment()
        {
            if (Copayment is null) return false;
            //006.001~009(除006).801.802.901.902.903.904
            switch (Copayment.Id)
            {
                case "006"://勞保被人因職業傷害或疾病門診者
                case "001"://重大傷病
                case "002"://分娩
                case "003"://低收入戶
                case "004"://榮民
                case "005"://結核病患至指定之醫療院所就醫者
                case "007"://山地離島就醫/戒菸免收
                case "008"://經離島醫院診所轉至台灣本門及急救者
                case "009"://其他免負擔
                case "I22"://免收
                    return true;
            }
            return false;
        }

        private int CountCopaymentPoint()
        {
            var point = PrescriptionPoint.MedicinePoint;
            var copPoint = 0;
            if (point <= 100)
                copPoint = 0;
            if (point > 100 && point <= 200)
                copPoint = 20;
            if (point >= 201 && point <= 300)
                copPoint = 40;
            if (point >= 301 && point <= 400)
                copPoint = 60;
            if (point >= 401 && point <= 500)
                copPoint = 80;
            if (point >= 501 && point <= 600)
                copPoint = 100;
            if (point >= 601 && point <= 700)
                copPoint = 120;
            if (point >= 701 && point <= 800)
                copPoint = 140;
            if (point >= 801 && point <= 900)
                copPoint = 160;
            if (point >= 901 && point <= 1000)
                copPoint = 180;
            else
                copPoint = 200;
            if (!CheckFreeCopayment())
                return copPoint;
            if (AdjustCase.ID.Equals("5") && (Copayment.Id.Equals("003") || Copayment.Id.Equals("007") ||  Copayment.Id.Equals("907")))
            {
                if(copPoint > 0)
                    PrescriptionPoint.AdministrativeAssistanceCopaymentPoint = copPoint;
            }
            //003、004、005、006、901、902、903、904、906、907
            else if (Copayment.Id.Equals("003") || Copayment.Id.Equals("004") || Copayment.Id.Equals("005") || Copayment.Id.Equals("006") || Copayment.Id.Equals("901") || Copayment.Id.Equals("902") || Copayment.Id.Equals("903") || Copayment.Id.Equals("904") || Copayment.Id.Equals("906") || Copayment.Id.Equals("907"))
            {
                if (copPoint > 0)
                    PrescriptionPoint.AdministrativeAssistanceCopaymentPoint = copPoint;
            }
            return 0;
        }

        private void CheckMedicalServiceData()
        {
            if (MedicineDays >= 28)
            {
                MedicalServiceCode = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 69;
            }
            else if (MedicineDays > 7 && MedicineDays < 14)
            {
                MedicalServiceCode = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
            else if (MedicineDays >= 14 && MedicineDays < 28)
            {
                MedicalServiceCode = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 59;
            }
            else
            {
                MedicalServiceCode = "05202B";//一般處方給付(7天以內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
        }

        private List<Pdata> SetPrescriptionDetail()
        {
            var details = new List<Pdata>();
            var serialNumber = 1;
            foreach (var med in Medicines.GetDeclare())
            {
                details.Add(new Pdata(med, serialNumber.ToString()));
                serialNumber++;
            }
            details.AddRange(Medicines.Where(m => m.PaySelf).Select(med => new Pdata(med, string.Empty)));
            if (AdjustCase.ID.Equals("0") || CheckOnlyBloodGlucoseTestStrip()) return details;
            var medicalService = new Pdata(PDataType.Service, MedicalServiceCode, Patient.CheckAgePercentage(), 1);
            details.Add(medicalService);
            MedicineDays = Medicines.CountMedicineDays();//計算最大給藥日份
            if (!AdjustCase.ID.Equals("1") && !AdjustCase.ID.Equals("3")) return details;
            var dailyPrice = CheckIfSimpleFormDeclare();
            if (dailyPrice <= 0) return details;
            foreach (var d in details)
            {
                if (!d.P1.Equals("1")) continue;
                d.P1 = "4";
                d.P8 = $"{0.00:0000000.00}";
                d.P9 = "00000000";
            }
            var simpleForm = new Pdata(PDataType.SimpleForm, dailyPrice.ToString(), 100, MedicineDays);
            details.Add(simpleForm);
            return details;
        }

        private bool CheckOnlyBloodGlucoseTestStrip()
        {
            if (Medicines.Count != 1) return false;
            if (!Medicines[0].CheckIsBloodGlucoseTestStrip()) return false;
            MedicalServiceCode = null;
            PrescriptionPoint.MedicalServicePoint = 0;
            return true;
        }

        private int CheckIfSimpleFormDeclare()
        {
            if (Patient.Birthday is null) return 0;
            if (MedicineDays > 3)
            {
                if (AdjustCase.ID.Equals("3"))
                    AdjustCase = VM.GetAdjustCase("1");
                return 0;
            }
            var medicinePoint = Medicines.Where(m => !m.PaySelf).Sum(med => med.NHIPrice * med.Amount);
            var medFormCount = Medicines.CountOralLiquidAgent();//口服液劑(原瓶包裝)數量
            var dailyPrice = CountDayPayAmount(Patient.CountAge(), medFormCount);//計算日劑藥費金額
            if (dailyPrice * MedicineDays < medicinePoint)
            {
                if (AdjustCase.ID.Equals("3"))
                    AdjustCase = VM.GetAdjustCase("1");
                return 0;
            }
            AdjustCase = VM.GetAdjustCase("3");
            PrescriptionPoint.MedicinePoint = dailyPrice * MedicineDays;
            return dailyPrice;
        }

        private int CountDayPayAmount(int cusAge, int medFormCount)
        {
            const int ma1 = 22, ma2 = 31, ma3 = 37, ma4 = 41;
            if (cusAge <= 12 && medFormCount == 1) return ma2;
            if (cusAge <= 12 && medFormCount == 2) return ma3;
            if (cusAge <= 12 && medFormCount >= 3) return ma4;
            return ma1;
        }

        #region PrintFunctions
        public void PrintMedBag(bool singleMode)
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new MedBagMedicines(Medicines, singleMode);
            if (singleMode)
            {
                foreach (var m in medBagMedicines)
                {
                    rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReportSingle.rdlc";
                    rptViewer.ProcessingMode = ProcessingMode.Local;
                    var parameters = PrescriptionService.CreateSingleMedBagParameter(m,this);
                    rptViewer.LocalReport.SetParameters(parameters);
                    rptViewer.LocalReport.DataSources.Clear();
                    rptViewer.LocalReport.Refresh();
                    MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
                    {
                        ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                    }));
                }
            }
            else
            {
                var json = JsonConvert.SerializeObject(medBagMedicines);
                var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
                rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReport.rdlc";
                rptViewer.ProcessingMode = ProcessingMode.Local;
                var parameters = PrescriptionService.CreateMultiMedBagParameter(this);
                rptViewer.LocalReport.SetParameters(parameters);
                rptViewer.LocalReport.DataSources.Clear();
                var rd = new ReportDataSource("DataSet1", dataTable);
                rptViewer.LocalReport.DataSources.Add(rd);
                rptViewer.LocalReport.Refresh();
                MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                }));
            }
        }
        public void PrintReceipt()
        {
            try
            {
                var rptViewer = new ReportViewer();
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt.rdlc";
                rptViewer.ProcessingMode = ProcessingMode.Local;
                var parameters = PrescriptionService.CreateReceiptParameters(this);
                rptViewer.LocalReport.SetParameters(parameters);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.Refresh();
                MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintReceipt(rptViewer);
                }));
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage(Resources.列印錯誤 + e.Message, MessageType.WARNING);
            }
        }
        public void PrintDepositSheet()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.ReportPath = @"RDLC\DepositSheet.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateDepositSheetParameters(this);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
            MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintDeposit(rptViewer);
            }));
        }
        
        #endregion

        public void PrintMedBagAndReceipt()
        {
            var medBagPrint = new ConfirmWindow(Resources.藥袋列印確認, Resources.列印確認, true);
            Debug.Assert(medBagPrint.DialogResult != null, "medBagPrint.DialogResult != null");
            if (!(bool)medBagPrint.DialogResult) return;
            var printBySingleMode = new MedBagSelectionWindow();
            var singleMode = (bool)printBySingleMode.ShowDialog();
            var receiptPrint = false;
            if (PrescriptionPoint.AmountsPay > 0)
            {
                var receiptResult = new ConfirmWindow(Resources.收據列印確認, Resources.列印確認, true);
                if (receiptResult.DialogResult != null)
                    receiptPrint = (bool)receiptResult.DialogResult;
            }
            PrintMedBag(singleMode);
            if (receiptPrint)
                PrintReceipt();
        }

        public object Clone()
        {
            var clone = new Prescription
            {
                Type = Type,
                Patient = Patient.DeepCloneViaJson(),
                Institution = Institution.DeepCloneViaJson(),
                Division = Division.DeepCloneViaJson(),
                Pharmacist = Pharmacist.DeepCloneViaJson(),
                TempMedicalNumber = TempMedicalNumber,
                TreatDate = TreatDate,
                AdjustDate = AdjustDate,
                MainDisease = MainDisease.DeepCloneViaJson(),
                SubDisease = SubDisease.DeepCloneViaJson(),
                ChronicSeq = ChronicSeq,
                ChronicTotal = ChronicTotal,
                AdjustCase = AdjustCase.DeepCloneViaJson(),
                PrescriptionCase = PrescriptionCase.DeepCloneViaJson(),
                Copayment = Copayment.DeepCloneViaJson(),
                PaymentCategory = PaymentCategory.DeepCloneViaJson(),
                SpecialTreat = SpecialTreat.DeepCloneViaJson(),
                Medicines = Medicines
            };
            return clone;
        }

        public void ReadCard()
        {
            var success = Card.GetBasicData();
            if (success)
            {
                var cus = new Customer(Card);
                Patient = cus;
                var customers = Patient.Check();
                if (customers.Count == 0)
                    Patient.InsertData();
                else
                    Patient = customers[0];
            }
        }

        public void AddMedicine(string medicineID)
        {
            var paySelf = AdjustCase.ID.Equals("0");
            int? selectedMedicinesIndex = null;
            if (SelectedMedicine != null)
                selectedMedicinesIndex = Medicines.IndexOf(SelectedMedicine);
            Medicines.AddMedicine(medicineID, paySelf, selectedMedicinesIndex);
        }
    }
}
