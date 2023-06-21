using DomainModel.Enum;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Medicine.MedBag;
using His_Pos.NewClass.Medicine.MedicineSet;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.Service;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using Employee = His_Pos.NewClass.Person.Employee.Employee;
using Medicines = His_Pos.NewClass.Medicine.Base.Medicines;
using Resources = His_Pos.Properties.Resources;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

// ReSharper disable ClassTooBig

namespace His_Pos.NewClass.Prescription
{
    public enum PrescriptionType
    {
        Normal = 0,
        XmlOfPrescription = 1,
        Cooperative = 2,
        ChronicRegister = 3,
        ChronicReserve = 4
    }

    public class Prescription : ObservableObject, ICloneable
    {
        #region Constructors

        public Prescription()
        {
            Medicines = new Medicines();
            Institution = new Institution();
            Division = new Division();
            Pharmacist = new Employee();
            MainDisease = new DiseaseCode();
            SubDisease = new DiseaseCode();
            AdjustCase = new AdjustCase();
            PrescriptionCase = new PrescriptionCase();
            Copayment = new Copayment();
            PaymentCategory = new PaymentCategory();
            SpecialTreat = new SpecialTreat();
            Patient = new Customer();
            PrescriptionPoint = new PrescriptionPoint();
            PrescriptionStatus = new PrescriptionStatus();
            Type = new PrescriptionType();
        }

        public Prescription(DataRow r, PrescriptionType type)
        {
            if (type.Equals(PrescriptionType.ChronicReserve))
            {
                ID = 0;
                SourceId = r.Field<int>("ID").ToString();
            }
            else
            {
                ID = r.Field<int>("ID");
            }
            Patient = Customer.GetCustomerByCusId(r.Field<int>("CustomerID"));
            Institution = VM.GetInstitution(r.Field<string>("InstitutionID"));
            Division = VM.GetDivision(r.Field<string>("DivisionID"));
            Pharmacist = VM.CurrentPharmacy.AllEmployees.SingleOrDefault(p => p.IDNumber.Equals(r.Field<string>("Emp_IDNumber")));
            AdjustDate = r.Field<DateTime>("AdjustDate");
            AdjustDay = Convert.ToDateTime(AdjustDate).ToString("dd");
            AdjustMonth = Convert.ToDateTime(AdjustDate).ToString("MM");
            TreatDate = r.Field<DateTime?>("TreatmentDate");
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicSequence").ToString()))
                ChronicSeq = r.Field<byte>("ChronicSequence");
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicTotal").ToString()))
                ChronicTotal = r.Field<byte>("ChronicTotal");
            if (!string.IsNullOrEmpty(r.Field<string>("MainDiseaseID")))
                MainDisease = DiseaseCode.GetDiseaseCodeByID(r.Field<string>("MainDiseaseID"));
            if (!string.IsNullOrEmpty(r.Field<string>("SecondDiseaseID")))
                SubDisease = DiseaseCode.GetDiseaseCodeByID(r.Field<string>("SecondDiseaseID"));
            AdjustCase = VM.GetAdjustCase(r.Field<string>("AdjustCaseID"));
            Copayment = VM.GetCopayment(r.Field<string>("CopaymentID"));
            PrescriptionCase = VM.GetPrescriptionCases(r.Field<string>("PrescriptionCaseID"));
            PaymentCategory = VM.GetPaymentCategory(r.Field<string>("PaymentCategoryID"));
            SpecialTreat = VM.GetSpecialTreat(r.Field<string>("SpecialTreatID"));
            PrescriptionPoint = new PrescriptionPoint(r, type);
            PrescriptionStatus = new PrescriptionStatus(r, type);
            MedicalNumber = string.IsNullOrEmpty(r.Field<string>("MedicalNumber")) ? r.Field<string>("MedicalNumber") : r.Field<string>("MedicalNumber").Trim();
            OriginalMedicalNumber = string.IsNullOrEmpty(r.Field<string>("OldMedicalNumber")) ? r.Field<string>("OldMedicalNumber") : r.Field<string>("OldMedicalNumber").Trim();
            if (AdjustCase.ID.Equals("2"))
            {
                TempMedicalNumber = ChronicSeq == 1 ? MedicalNumber : OriginalMedicalNumber;
            }
            else
                TempMedicalNumber = MedicalNumber;
            switch (type)
            {
                case PrescriptionType.ChronicReserve:
                    Medicines = new Medicines();
                    Medicines.GetDataByReserveId(int.Parse(SourceId));
                    PrescriptionStatus.OrderStatus = "備藥狀態:";
                    switch (r.Field<string>("MedPrepareStatus"))
                    {
                        case "N":
                            PrescriptionStatus.OrderStatus += "未處理";
                            break;

                        case "D":
                            PrescriptionStatus.OrderStatus += "已備藥";
                            break;

                        default:
                            PrescriptionStatus.OrderStatus += "不備藥";
                            break;
                    }
                    PrescriptionStatus.ReserveSend = PrescriptionStatus.OrderStatus.Contains("已備藥");
                    OrderContent = PrescriptionStatus.OrderStatus;
                    if (!string.IsNullOrEmpty(r.Field<string>("StoreOrderID")))
                        OrderContent += " 單號:" + r.Field<string>("StoreOrderID");
                    break;

                case PrescriptionType.ChronicRegister:
                    Medicines = new Medicines();
                    Medicines.GetDataByPrescriptionId(ID);
                    PrescriptionStatus.OrderStatus = "訂單狀態:";
                    switch (r.Field<string>("StoOrd_Status"))
                    {
                        case "W":
                            PrescriptionStatus.OrderStatus += "等待確認";
                            break;

                        case "P":
                            PrescriptionStatus.OrderStatus += "等待收貨";
                            break;

                        case "D":
                            PrescriptionStatus.OrderStatus += "已收貨";
                            break;

                        case "S":
                            PrescriptionStatus.OrderStatus += "訂單做廢";
                            break;

                        default:
                            PrescriptionStatus.OrderStatus += "無訂單";
                            break;
                    }
                    PrescriptionStatus.ReserveSend = PrescriptionStatus.OrderStatus.Contains("已備藥");
                    OrderContent = PrescriptionStatus.OrderStatus;
                    if (!string.IsNullOrEmpty(r.Field<string>("StoreOrderID")))
                    {
                        OrderID = r.Field<string>("StoreOrderID");
                        OrderContent += " 單號:" + OrderID;
                    }
                    break;

                default:
                    Medicines = new Medicines();
                    Medicines.GetDataByPrescriptionId(ID);
                    break;
            }
            if (type.Equals(PrescriptionType.ChronicReserve))
            {
                AdjustDate = null;
            }
            Type = type;
            MedicineDays = Medicines.CountMedicineDays();

            Card = new IcCard();
            Card.Name = Patient.Name;
            if (r.Table.Columns.Contains("PreMas_CardNo"))
            {
                Card.CardNumber = Convert.ToString(r["PreMas_CardNo"]);
            }
            if (r.Table.Columns.Contains("PreMas_OrigTreatmentDT"))
            {
                Card.TreatDateTime = Convert.ToString(r["PreMas_OrigTreatmentDT"]);
            }
            if (r.Table.Columns.Contains("PreMas_MedIDCode1"))
            {
                OrigTreatmentCode = Convert.ToString(r["PreMas_MedIDCode1"]);
            }
            if (r.Table.Columns.Contains("PreMas_MedIDCode2"))
            {
                TreatmentCode = Convert.ToString(r["PreMas_MedIDCode2"]);
            }
            if(r.Table.Columns.Contains("PreMas_SecuritySign"))
            {
                SecuritySignature = Convert.ToString(r["PreMas_SecuritySign"]);
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
            #endregion CooPreVariable

            Type = PrescriptionType.Cooperative;

            SourceId = c.CooperativePrescriptionId;
            Remark = customer.Remark;
            PrescriptionStatus.IsVIP = Remark.EndsWith("Y");
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);

            #region InitTreatment

            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber.Trim();
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                MedicalNumber = insurance.MedicalNumber.Trim();
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            SpecialTreat = new SpecialTreat();
            AdjustDate = DateTime.Today;
            TreatDate = Convert.ToDateTime(c.InsertDate);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            PaymentCategory = VM.GetPaymentCategory("4");
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            OrthopedicsGetDisease(diseases);
            GetCopayment(insurance.CopaymentCode);
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode.Trim();

            #endregion InitTreatment

            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead?.Equals("D") ?? false;
            Medicines = new Medicines();
            AdjustDay = Convert.ToDateTime(AdjustDate).ToString("dd");
            AdjustMonth = Convert.ToDateTime(AdjustDate).ToString("MM");
            Medicines.GetDataByOrthopedicsPrescription(prescription.MedicineOrder.Item, WareHouse?.ID, IsBuckle, AdjustDate);
        }

        [SuppressMessage("ReSharper", "TooManyDependencies")]
        public Prescription(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead ,bool isPrint)
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

            #endregion CooPreVariable

            Type = PrescriptionType.XmlOfPrescription;
            SourceId = sourceId;
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber.Trim();
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                MedicalNumber = insurance.MedicalNumber.Trim();
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            PaymentCategory = VM.GetPaymentCategory("4");
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            TreatDate = treatDate.Date;
            AdjustDate = DateTime.Today;
            AdjustDay = Convert.ToDateTime(AdjustDate).ToString("dd");
            AdjustMonth = Convert.ToDateTime(AdjustDate).ToString("MM");
            AdjustYear = Convert.ToDateTime(AdjustDate).ToString("yyyy");

            SpecialTreat = new SpecialTreat();
            CooperativeGetDisease(diseases);
            GetCopayment(insurance.CopaymentCode);
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.Insurance.IcErrorCode.Trim();
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = isRead;
            PrescriptionStatus.IsPrint = isPrint;
            Medicines = new Medicines();
            Medicines.GetDataByCooperativePrescription(prescription.MedicineOrder.Item, WareHouse?.ID, IsBuckle, AdjustDate);
        }

        #endregion Constructors

        #region Properties

        public int ID { get; set; }
        public string SourceId { get; set; }
        public string Remark { get; set; }
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceCode { get; set; } //藥事服務代碼
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public Medicines Medicines { get; set; }
        public PrescriptionType Type { get; set; }
        public IcCard Card { get; set; }
        private Customer patient;

        public Customer Patient
        {
            get => patient;
            set { Set(() => Patient, ref patient, value); }
        }

        private string treatmentCode;
        public string TreatmentCode//就醫識別碼
        {
            get => treatmentCode;
            set { Set(() => TreatmentCode, ref treatmentCode, value); }
        }

        private string origTreatmentCode;
        public string OrigTreatmentCode//原就醫識別碼
        {
            get => origTreatmentCode;
            set { Set(() => OrigTreatmentCode, ref origTreatmentCode, value); }
        }

        private string origTreatmentDT;

        public string OrigTreatmentDT//原就醫日期
        {
            get => origTreatmentDT;
            set { Set(() => OrigTreatmentDT, ref origTreatmentDT, value); }
        }

        private string securitySignature;
        public string SecuritySignature
        {
            get => securitySignature;
            set { Set(() => SecuritySignature, ref securitySignature, value); }
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
                CheckEnableDivisions();
                CheckCopaymentRule();
            }
        }

        private Division division;//就醫科別 D13

        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
                CheckVariableByDivision();
            }
        }

        private Employee pharmacist;//醫事人員代號 D25

        public Employee Pharmacist
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
                CheckCopaymentRule();
                CountCopaymentPoint();
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

        private string adjustDay;//調劑日期 D23

        public string AdjustDay
        {
            get => adjustDay;
            set
            {
                Set(() => AdjustDay, ref adjustDay, value);
            }
        }


        private string adjustMonth;//調劑日期 D23

        public string AdjustMonth
        {
            get => adjustMonth;
            set
            {
                Set(() => AdjustMonth, ref adjustMonth, value);
            }
        }

        private string adjustYear;//調劑日期 D23

        public string AdjustYear
        {
            get => adjustYear;
            set
            {
                Set(() => AdjustYear, ref adjustYear, value);
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
                CheckVariableByChronicSequence();
            }
        }//連續處方箋調劑序號 D35

        private AdjustCase adjustCase;//調劑案件 D1

        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                if (adjustCase != null && value != null)
                {
                    if ((value.IsChronic() && !adjustCase.IsChronic()) || (!value.IsChronic() && adjustCase.IsChronic()))
                        IsBuckle = WareHouse != null;
                }
                Set(() => AdjustCase, ref adjustCase, value);
                if (adjustCase == null || Medicines is null) return;
                MedicineDays = Medicines.CountMedicineDays();
                CheckVariableByAdjustCase();
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
                    case "C5"://法定傳染隔離案件
                        Copayment = VM.GetCopayment("914");
                        PaymentCategory = VM.GetPaymentCategory("W");
                        SpecialTreat = VM.GetSpecialTreat("EE");
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
                    PrescriptionPoint.CopaymentPoint = CheckNotFreeCopayment() ? CountCopaymentPoint() : 0;
                    if (Type.Equals(PrescriptionType.Cooperative))
                        PrescriptionPoint.CopaymentPointPayable =
                            PrescriptionStatus.IsVIP ? 0 : PrescriptionPoint.CopaymentPoint;
                    else
                    {
                        PrescriptionPoint.CopaymentPointPayable = PrescriptionPoint.CopaymentPoint;
                    }
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

        private Medicine.Base.Medicine selectedMedicine;

        public Medicine.Base.Medicine SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (selectedMedicine != null)
                    selectedMedicine.IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    selectedMedicine.IsSelected = true;
            }
        }

        public WareHouse.WareHouse WareHouse => VM.CooperativeClinicSettings.GetWareHouseByPrescription(Institution, AdjustCase?.ID);
        public bool IsPrescribe => Medicines != null && Medicines.Count(m => m.PaySelf) == Medicines.Count && Medicines.Count > 0;
        private bool isBuckle = true;

        public bool IsBuckle
        {
            get => isBuckle;
            set
            {
                Set(() => IsBuckle, ref isBuckle, value);
                if (Medicines is null || !Medicines.Any()) return;
                MainWindow.ServerConnection.OpenConnection();
                switch (Type)
                {
                    case PrescriptionType.ChronicReserve:
                        Medicines.Update(IsBuckle, int.Parse(SourceId), Type);
                        break;

                    default:
                        if (ID == 0)
                            Medicines.Update(IsBuckle, ID, Type, AdjustDate, WareHouse?.ID);
                        else
                            Medicines.Update(IsBuckle, ID, Type);
                        break;
                }
                MainWindow.ServerConnection.CloseConnection();
            }
        }

        public int DeclareFileID { get; }
        public int WriteCardSuccess { get; set; }
        private List<Pdata> Details { get; set; }
        public DateTime? InsertTime { get; set; }

        private string orderContent;

        public string OrderContent
        {
            get => orderContent;
            set
            {
                Set(() => OrderContent, ref orderContent, value);
            }
        }

        private string orderID;

        public string OrderID
        {
            get => orderID;
            set
            {
                Set(() => OrderID, ref orderID, value);
            }
        }

        #endregion Properties

        public bool CheckDiseaseEquals(List<string> parameters)
        {
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            return diseaseID.Equals(elementName.Equals("MainDiagnosis") ? MainDisease?.FullName : SubDisease?.FullName);
        }

        private void CheckTypeByInstitution()
        {
            if (Institution != null && Institution.CheckCooperative())
                CheckIsOrthopedics();
            else
                Type = PrescriptionType.Normal;
        }

        private void CheckEnableDivisions()
        {
            if (string.IsNullOrEmpty(Institution.ID)) return;
            var table = InstitutionDb.GetEnableDivisions(Institution.ID);
            if (table.Rows.Count <= 0) return;
            var divListString = table.Rows[0].Field<string>("Divisions");
            if (string.IsNullOrEmpty(divListString)) return;
            var divisions = new List<string>();
            if (divListString.Contains(','))
                divisions = divListString.Split(',').ToList();
            else
                divisions.Add(divListString);
            if (divisions.Count == 1)
                Division = VM.GetDivision(divisions[0]);
        }

        private void CheckIsOrthopedics()
        {
            Type = Institution.CheckIsOrthopedics() ? PrescriptionType.Cooperative : PrescriptionType.XmlOfPrescription;
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

        public void CountPrescriptionPoint()
        {
            PrescriptionPoint.MedicinePoint = Medicines.CountMedicinePoint();
            PrescriptionPoint.SpecialMaterialPoint = Medicines.CountSpecialMedicinePoint();
            if (!AdjustCase.ID.Equals("0"))
            {
                CheckCopaymentRule();
                if (Patient.Birthday != null)
                {
                    SetMedicalService();//確認藥事服務資料
                }
                PrescriptionPoint.CountTotal();
                PrescriptionPoint.CountApply();
            }
        }
        private void GetCopayment()
        {
            if (CheckIsChronic() && MedicineDays >= 28)
                Copayment = VM.GetCopayment("I22");
            if (!CheckFreeCopayment())
                Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
        }

        private bool CheckIsChronic()
        {
            if (AdjustCase is null) return false;
            return AdjustCase.ID.Equals("2") || (ChronicSeq != null && ChronicSeq > 0 && ChronicTotal != null && ChronicTotal > 0);
        }

        private bool CheckNotFreeCopayment()
        {
            return Copayment != null && !CheckFreeCopayment();
        }

        private bool CheckFreeCopayment()
        {
            if (Copayment is null) return false;
            //006.001~009(除006).801.802.901.902.903.904
            switch (Copayment.Id)
            {
                case "001"://重大傷病
                case "002"://分娩
                case "003"://低收入戶
                case "004"://榮民
                case "005"://結核病患至指定之醫療院所就醫者
                case "006"://勞保被人因職業傷害或疾病門診者
                case "007"://山地離島就醫/戒菸免收
                case "008"://經離島醫院診所轉至台灣本門及急救者
                case "009"://其他免負擔
                case "801"://HMO
                case "802"://蘭綠計畫
                case "901"://多氯聯苯中毒之油症患者
                case "902"://三歲以下
                case "903"://新生兒
                case "904"://行政協助愛滋病
                case "905"://三氯氰胺汙染奶製品
                case "906"://替代役全民健康
                case "914"://法定傳染病
                case "I22"://免收
                    return true;
            }
            return false;
        }

        public int CountCopaymentPoint()
        {
            DateTime date = TreatDate is null ? DateTime.Today : Convert.ToDateTime(TreatDate);
            bool isChronic = false;
            if (AdjustCase != null)
            {
                isChronic = (AdjustCase.ID == "2" && MedicineDays >= 28) ? true : false;
            }

            if (!CheckFreeCopayment())
            {
                if (DateTime.Compare(date, new DateTime(2023, 7, 1)) < 0)//2023-07-01使用舊制
                {
                    return PrescriptionPoint.GetCopaymentValueOld(Institution.LevelType, isChronic);
                }
                else
                {
                    return PrescriptionPoint.GetCopaymentValue(Institution.LevelType, isChronic);
                } 
            }
            if (CheckIsAdministrativeAssistanceCopayment())
            {
                if (PrescriptionPoint.CopaymentValue > 0)
                    PrescriptionPoint.AdministrativeAssistanceCopaymentPoint = PrescriptionPoint.CopaymentValue;

                int copaymentValue = 0;
                if (DateTime.Compare(date, new DateTime(2023, 7, 1)) < 0)//2023-07-01使用舊制
                {
                    copaymentValue = PrescriptionPoint.GetCopaymentValueOld(Institution.LevelType, isChronic);
                }
                else
                {
                    copaymentValue = PrescriptionPoint.GetCopaymentValue(Institution.LevelType, isChronic);
                }

                if (copaymentValue > 0)
                    PrescriptionPoint.AdministrativeAssistanceCopaymentPoint = copaymentValue;
            }
            return 0;
        }

        private bool CheckIsAdministrativeAssistanceCopayment()
        {
            if (AdjustCase != null && AdjustCase.ID.Equals("5"))
                return Copayment.Id.Equals("003") || Copayment.Id.Equals("007") || Copayment.Id.Equals("907");
            switch (Copayment.Id)
            {
                case "003":
                case "004":
                case "005":
                case "006":
                case "901":
                case "902":
                case "903":
                case "904":
                case "905":
                case "906":
                    return true;

                default:
                    return false;
            }
        }

        private void SetMedicalService()
        {
            switch (MedicineDays)
            {
                case int n when n >= 28:
                    MedicalServiceCode = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                    PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05210B;
                    break;

                case int n when n >= 14 && n < 28:
                    MedicalServiceCode = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                    PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05206B;
                    break;

                case int n when n >= 7 && n < 14:
                    MedicalServiceCode = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                    PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05223B;
                    break;

                default:
                    MedicalServiceCode = "05202B";//一般處方給付(7天以內)
                    PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05202B;
                    break;
            }
        }

        private void SetPrescriptionDetail()
        {
            Details = new List<Pdata>();
            CreateMedicinesDetail();
            if (IsPrescribe || CheckOnlyBloodGlucoseTestStrip()) return;
            MedicineDays = Medicines.CountMedicineDays();//計算最大給藥日份
            var medicalService = new Pdata(PDataType.Service, MedicalServiceCode, Patient.CheckAgePercentage(), 1, (DateTime)AdjustDate);
            Details.Add(medicalService);
            if (CheckNotNormalPrescription()) return;
            var dailyPrice = CheckIfSimpleFormDeclare();
            if (dailyPrice <= 0) return;
            CreateSimpleFormDetail(dailyPrice);
        }

        private void CreateSimpleFormDetail(int dailyPrice)
        {
            foreach (var d in Details)
            {
                if (!d.P1.Equals("1")) continue;
                d.P1 = "4";
                d.P8 = $"{0.00:0000000.00}";
                d.P9 = "00000000";
            }
            var simpleForm = new Pdata(PDataType.SimpleForm, dailyPrice.ToString(), 100, MedicineDays, (DateTime)AdjustDate);
            Details.Add(simpleForm);
        }

        private void CreateMedicinesDetail()
        {
            var serialNumber = 1;
            foreach (var med in Medicines.GetDeclare())
            {
                Details.Add(new Pdata(med, serialNumber.ToString(), (DateTime)AdjustDate));
                serialNumber++;
            }
            Details.AddRange(Medicines.Where(m => m.PaySelf).Select(med => new Pdata(med, string.Empty, (DateTime)AdjustDate)));
        }

        private bool CheckNotNormalPrescription()
        {
            return !AdjustCase.ID.Equals("1") && !AdjustCase.ID.Equals("3");
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
            if (MedicineDays <= 3) return CountDailyPrice();
            if (AdjustCase.CheckIsSimpleForm())
                AdjustCase = VM.GetAdjustCase("1");
            return 0;
        }

        private int CountDailyPrice()
        {
            var dailyPrice = CountDayPayAmount();//計算日劑藥費金額
            if (dailyPrice * MedicineDays < PrescriptionPoint.MedicinePoint)
            {
                if (AdjustCase.CheckIsSimpleForm())
                    AdjustCase = VM.GetAdjustCase("1");
                return 0;
            }
            AdjustCase = VM.GetAdjustCase("3");
            PrescriptionPoint.MedicinePoint = dailyPrice * MedicineDays;
            return dailyPrice;
        }

        private int CountDayPayAmount()
        {
            const int ma1 = 22, ma2 = 31, ma3 = 37, ma4 = 41;
            var oralLiquidCount = Medicines.CountOralLiquidAgent();//口服液劑(原瓶包裝)數量
            var age = Patient.CountAge();
            if (IsChild(age) && oralLiquidCount == 1) return ma2;
            if (IsChild(age) && oralLiquidCount == 2) return ma3;
            if (IsChild(age) && oralLiquidCount >= 3) return ma4;
            return ma1;
        }

        private bool IsChild(int age)
        {
            return age <= 12;
        }

        #region PrintFunctions
        public void PrintMedBagSingleModeByCE()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new MedBagMedicines(Medicines, true);
            var medDays = Medicines.CountMedicineDays();
            for (int i = 1; i <= medBagMedicines.Count; i++)
            {
                SetSingleModeReportByCEViewer(rptViewer, medBagMedicines[i - 1], $"{i}/{medBagMedicines.Count}", medDays);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBagCE(rptViewer);
                });
            }
        }
        public void PrintMedBagSingleMode()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new MedBagMedicines(Medicines, true);
            var medDays = Medicines.CountMedicineDays();
            for (int i = 1; i <= medBagMedicines.Count; i++)
            {
                SetSingleModeReportViewer(rptViewer, medBagMedicines[i - 1], $"{i}/{medBagMedicines.Count}", medDays);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                });
            }
        }

        public void PrintMedBagMultiMode()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            SetMultiModeReportViewer(rptViewer);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
            });
        }

        public void PrintReceipt()
        {
            try
            {
                var rptViewer = new ReportViewer();
                rptViewer.LocalReport.DataSources.Clear();
                SetReceiptReportViewer(rptViewer);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintReceipt(rptViewer);
                });
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
            SetDepositReportViewer(rptViewer);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintDeposit(rptViewer);
            });
        }

        #region ReportViewerSettingFunctions

        private void SetSingleModeReportViewer(ReportViewer rptViewer, MedBagMedicine m, string orderNumber, int medDays)
        {
            rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReportSingle.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateSingleMedBagParameter(m, this, orderNumber, medDays);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }

        private void SetMultiModeReportViewer(ReportViewer rptViewer)
        {
            var medBagMedicines = new MedBagMedicines(Medicines, false);
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
        }
        private void SetSingleModeReportByCEViewer(ReportViewer rptViewer, MedBagMedicine m, string orderNumber, int medDays)
        {
            rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReportSingleByCE.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateSingleMedBagParameter(m, this, orderNumber, medDays);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }
        private void SetReceiptReportViewer(ReportViewer rptViewer)
        {
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt_A5.rdlc";
                    break;

                default:
                    rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt.rdlc";
                    break;
            }
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateReceiptParameters(this);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }

        private void SetDepositReportViewer(ReportViewer rptViewer)
        {
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    rptViewer.LocalReport.ReportPath = @"RDLC\DepositSheet_A5.rdlc";
                    break;

                default:
                    rptViewer.LocalReport.ReportPath = @"RDLC\DepositSheet.rdlc";
                    break;
            }
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateDepositSheetParameters(this);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }

        #endregion ReportViewerSettingFunctions

        #endregion PrintFunctions

        public void AddMedicine(string medicineID)
        {
            IsBuckle = WareHouse != null;
            var paySelf = AdjustCase.CheckIsPrescribe();
            int? selectedMedicinesIndex = null;
            if (SelectedMedicine != null)
            {
                selectedMedicinesIndex = Medicines.IndexOf(SelectedMedicine);
            }
            Medicines.AddMedicine(medicineID, paySelf, selectedMedicinesIndex, WareHouse?.ID, AdjustDate);
        }

        public void Init()
        {
            Division = null;
            SpecialTreat = null;
            TreatDate = DateTime.Today;
            AdjustDate = DateTime.Today;
            AdjustDay = Convert.ToDateTime(AdjustDate).ToString("dd");
            AdjustMonth = Convert.ToDateTime(AdjustDate).ToString("MM");
            AdjustYear = Convert.ToDateTime(AdjustDate).ToString("yyyy");
            AdjustCase = VM.GetAdjustCase("1");
            PrescriptionCase = VM.GetPrescriptionCases("09");
            Copayment = VM.GetCopayment("I21");
            PaymentCategory = VM.GetPaymentCategory("4");
        }

        private void CheckVariableByAdjustCase()
        {
            switch (adjustCase.ID)
            {
                case "D":
                    Copayment = VM.GetCopayment("009");
                    break;

                case "1":
                case "3":
                    SetNormalVariables();
                    break;

                case "2":
                    SetChronicVariables();
                    break;

                case "5":
                    SetQuitSmokeVariables();
                    break;
            }
            SetInstitutionToCurrentPharmacy();
        }

        private void SetNormalVariables()
        {
            if (PrescriptionCase.ID.Equals("C5")) return;
            if (Division != null && !string.IsNullOrEmpty(Division.ID))
                PrescriptionCase = Division.ID.Equals("40") ? VM.GetPrescriptionCases("19") : VM.GetPrescriptionCases("09");
            else
                PrescriptionCase = VM.GetPrescriptionCases("09");
            PaymentCategory = VM.GetPaymentCategory("4");
        }

        private void SetChronicVariables()
        {
            CheckCopaymentRule();
            PrescriptionCase = VM.GetPrescriptionCases("04");
            PaymentCategory = null;
        }

        private void SetQuitSmokeVariables()
        {
            PrescriptionCase = VM.GetPrescriptionCases("B7");
            TempMedicalNumber = "IC07";
            Copayment = VM.GetCopayment("Z00");
            MainWindow.ServerConnection.OpenConnection();
            MainDisease = DiseaseCode.GetDiseaseCodeByID("F17200");
            MainWindow.ServerConnection.CloseConnection();
        }

        public void DeleteMedicine()
        {
            IsBuckle = WareHouse != null;
            Medicines.Remove(SelectedMedicine);
            Medicines.ReOrder();
        }

        private void CheckVariableByDivision()
        {
            if (AdjustCase is null) return;
            if (!(AdjustCase.ID.Equals("1") || AdjustCase.ID.Equals("3"))) return;
            switch (division?.ID)
            {
                case "40":
                    PrescriptionCase = VM.GetPrescriptionCases("19");
                    Copayment = VM.GetCopayment("I22");
                    break;

                default:
                    PrescriptionCase = VM.GetPrescriptionCases("09");
                    break;
            }
        }

        private void CheckVariableByChronicSequence()
        {
            if (!IsChronicSeqValid()) return;
            AdjustCase = VM.GetAdjustCase("2");
            if (ChronicSeq >= 2)
                MedicalNumber = "IC0" + chronicSeq;
        }

        private bool IsChronicSeqValid()
        {
            return ChronicSeq != null && ChronicSeq > 0;
        }

        //public object PrintClone()
        //{
        //    var clone = new Prescription
        //    {
        //        Patient = (Customer)Patient.Clone(),
        //        Institution = VM.GetInstitution(Institution?.ID),
        //        Division = (Division)Division.Clone(),
        //        Pharmacist = Pharmacist.DeepCloneViaJson(),
        //        TempMedicalNumber = TempMedicalNumber,
        //        TreatDate = TreatDate,
        //        AdjustDate = AdjustDate,
        //        MainDisease = MainDisease.DeepCloneViaJson(),
        //        SubDisease = SubDisease?.DeepCloneViaJson(),
        //        ChronicSeq = ChronicSeq,
        //        ChronicTotal = ChronicTotal,
        //        AdjustCase = VM.GetAdjustCase(AdjustCase?.ID),
        //        PrescriptionCase = VM.GetPrescriptionCases(PrescriptionCase?.ID),
        //        Copayment = VM.GetCopayment(Copayment?.Id),
        //        PaymentCategory = VM.GetPaymentCategory(PaymentCategory?.ID),
        //        SpecialTreat = VM.GetSpecialTreat(SpecialTreat?.ID),
        //        PrescriptionPoint = PrescriptionPoint.DeepCloneViaJson(),
        //        PrescriptionStatus = PrescriptionStatus.DeepCloneViaJson(),
        //        InsertTime = InsertTime,
        //        Type = Type,
        //        OrderContent = OrderContent,
        //        OrderID = OrderID,
        //        AdjustDay = AdjustDay,
        //        AdjustYear = AdjustYear,
        //        AdjustMonth = AdjustMonth,
        //        Medicines = new Medicines()
        //    };
        //    //var clone = new Prescription();
        //    foreach (var m in Medicines)
        //    {
        //        switch (m)
        //        {
        //            case MedicineNHI _:
        //                clone.Medicines.Add((MedicineNHI)m.Clone());
        //                break;

        //            case MedicineSpecialMaterial _:
        //                clone.Medicines.Add((MedicineSpecialMaterial)m.Clone());
        //                break;

        //            case MedicineOTC _:
        //                clone.Medicines.Add((MedicineOTC)m.Clone());
        //                break;

        //            default:
        //                clone.Medicines.Add((MedicineVirtual)m.Clone());
        //                break;
        //        }
        //    }
        //    return clone;
        //}

        public object Clone()
        {
            var clone = new Prescription
            {
                Patient = (Customer)Patient.Clone(),
                Institution = VM.GetInstitution(Institution?.ID),
                Division = VM.GetDivision(Division?.ID),
                Pharmacist = Pharmacist.DeepCloneViaJson(),
                TempMedicalNumber = TempMedicalNumber,
                TreatDate = TreatDate,
                AdjustDate = AdjustDate,
                MainDisease = MainDisease.DeepCloneViaJson(),
                SubDisease = SubDisease?.DeepCloneViaJson(),
                ChronicSeq = ChronicSeq,
                ChronicTotal = ChronicTotal,
                AdjustCase = VM.GetAdjustCase(AdjustCase?.ID),
                PrescriptionCase = VM.GetPrescriptionCases(PrescriptionCase?.ID),
                Copayment = VM.GetCopayment(Copayment?.Id),
                PaymentCategory = VM.GetPaymentCategory(PaymentCategory?.ID),
                SpecialTreat = VM.GetSpecialTreat(SpecialTreat?.ID),
                PrescriptionPoint = PrescriptionPoint.DeepCloneViaJson(),
                PrescriptionStatus = PrescriptionStatus.DeepCloneViaJson(),
                InsertTime = InsertTime,
                Type = Type,
                OrderContent = OrderContent,
                OrderID = OrderID,
                AdjustDay = AdjustDay,
                AdjustYear = AdjustYear,
                AdjustMonth = AdjustMonth,
                Medicines = new Medicines()
            };
            foreach (var m in Medicines)
            {
                switch (m)
                {
                    case MedicineNHI _:
                        clone.Medicines.Add((MedicineNHI)m.Clone());
                        break;

                    case MedicineSpecialMaterial _:
                        clone.Medicines.Add((MedicineSpecialMaterial)m.Clone());
                        break;

                    case MedicineOTC _:
                        clone.Medicines.Add((MedicineOTC)m.Clone());
                        break;

                    default:
                        clone.Medicines.Add((MedicineVirtual)m.Clone());
                        break;
                }
            }
            return clone;
        }

        public void SetPrescribeAdjustCase()
        {
            AdjustCase = VM.GetAdjustCase("0");
        }

        private void SetInstitutionToCurrentPharmacy()
        {
            if (IsPrescribe || AdjustCase.CheckIsPrescribe())
            {
                if (Institution is null || string.IsNullOrEmpty(Institution.ID))
                    Institution = VM.GetInstitution(VM.CurrentPharmacy.ID);
            }
            else
            {
                if (Institution != null)
                {
                    if (Institution.ID.Equals(VM.CurrentPharmacy.ID))
                        Institution = new Institution();
                }
            }
        }

        public string CheckPrescriptionRule(bool noCard)
        {
            var errorMsg = string.Empty;
            errorMsg += CheckInstitution();
            errorMsg += CheckAdjustCase();
            errorMsg += CheckPharmacist();
            errorMsg += CheckMedicalNumber(noCard);
            errorMsg += CheckCopayment();
            errorMsg += CheckDivision();
            errorMsg += CheckPaymentCategory();
            errorMsg += CheckDiseaseCode();
            errorMsg += CheckChronicTimes();
            errorMsg += CheckPrescriptionCase();
            return errorMsg;
        }

        public void UpdateMedicines()
        {
            IsBuckle = WareHouse != null;
        }

        public void SetNormalAdjustStatus()
        {
            PrescriptionStatus.SetNormalAdjustStatus();
        }

        public void SetPrescribeAdjustStatus()
        {
            PrescriptionStatus.SetPrescribeStatus();
        }

        public void SetErrorAdjustStatus()
        {
            PrescriptionStatus.SetErrorAdjustStatus();
        }

        public void SetDepositAdjustStatus()
        {
            PrescriptionStatus.SetDepositAdjustStatus();
        }

        public bool InsertDb()
        {
            return ID == 0 ? InsertPrescription() : Update();
        }

        public bool InsertPrescription()
        {
            CreateDeclareFileContent();//產生申報資料
            var resultTable = PrescriptionDb.InsertPrescriptionByType(this, Details);

            while (NewFunction.CheckTransaction(resultTable))
            {
                var retry = new ConfirmWindow("處方登錄異常，是否重試?", "登錄異常", true);
                Debug.Assert(retry.DialogResult != null, "retry.DialogResult != null");
                if ((bool)retry.DialogResult)
                    resultTable = PrescriptionDb.InsertPrescriptionByType(this, Details);
                else
                {
                    return false;
                }
            }

            ID = resultTable.Rows[0].Field<int>("DecMasId");

            PrescriptionDb.InsertStoOrdPrescriptionID(this.ID);

            return true;
        }

        public bool Update()
        {
            switch (Type)
            {
                default:
                    CreateDeclareFileContent();//產生申報資料
                    var resultTable = PrescriptionDb.UpdatePrescriptionByType(this, Details);
                    while (NewFunction.CheckTransaction(resultTable))
                    {
                        var retry = new ConfirmWindow("處方登錄異常，是否重試?", "登錄異常", true);
                        Debug.Assert(retry.DialogResult != null, "retry.DialogResult != null");
                        if ((bool)retry.DialogResult)
                            resultTable = PrescriptionDb.UpdatePrescriptionByType(this, Details);
                        else
                        {
                            return false;
                        }
                    }
                    break;

                case PrescriptionType.ChronicReserve:
                    PrescriptionDb.UpdateReserve(this, Details);
                    break;
            }
            return true;
        }

        private void SetValue()
        {
            Institution.UpdateUsedTime();
            MedicineDays = Medicines.CountMedicineDays();
            CheckMedicalServiceData();//確認藥事服務資料
            PrescriptionPoint.Count(Details);
            SetPrescribeValue();
        }

        #region CheckMedicalServiceFunctions

        private void CheckMedicalServiceData()
        {
            if (IsPrescribe) return;
            if (SetMedicalService28Days()) return;
            if (SetMedicalServiceBetween14And28Days()) return;
            if (SetMedicalServiceBetween7And14Days()) return;
            SetMedicalServiceLessThan7Days();
        }

        private bool SetMedicalService28Days()
        {
            if (!CheckMedicineDays28()) return false;
            MedicalServiceCode = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
            PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05210B;
            return true;
        }

        private bool SetMedicalServiceBetween14And28Days()
        {
            if (!CheckMedicineDaysBetween14And28()) return false;
            MedicalServiceCode = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
            PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05206B;
            return true;
        }

        private bool SetMedicalServiceBetween7And14Days()
        {
            if (!CheckMedicineDaysBetween7And14()) return false;
            MedicalServiceCode = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
            PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05223B;
            return true;
        }

        private void SetMedicalServiceLessThan7Days()
        {
            MedicalServiceCode = "05202B";//一般處方給付(7天以內)
            PrescriptionPoint.MedicalServicePoint = (int)ServicePoint.CODE_05202B;
        }

        private bool CheckMedicineDaysBetween14And28()
        {
            return MedicineDays >= 14 && MedicineDays < 28;
        }

        private bool CheckMedicineDaysBetween7And14()
        {
            return MedicineDays > 7 && MedicineDays < 14;
        }

        private bool CheckMedicineDays28()
        {
            return MedicineDays >= 28;
        }

        #endregion CheckMedicalServiceFunctions

        private void SetPrescribeValue()
        {
            if (IsPrescribe)
            {
                AdjustCase = VM.GetAdjustCase("0").DeepCloneViaJson();
                MedicalServiceCode = string.Empty;
                PrescriptionPoint.MedicalServicePoint = 0;
                PrescriptionPoint.MedicinePoint = 0;
                PrescriptionPoint.TotalPoint = 0;
                PrescriptionPoint.ApplyPoint = 0;
                PrescriptionPoint.SpecialMaterialPoint = 0;
            }
        }

        private void CreateDeclareFileContent()
        {
            if (IsPrescribe) return;
            var notPrescribeMedicines = Details.Where(p => !p.P1.Equals("0"));
            var medDeclare = notPrescribeMedicines.ToList();
            var d = new Ddata(this, medDeclare);
            DeclareContent = d.SerializeObjectToXDocument();
            d.Dbody.Pdata = Details;
        }

        private string CheckInstitution()
        {
            if (CheckIsHomeCare() || CheckIsQuitSmoking())
            {
                Institution = new Institution { ID = "N", Name = string.Empty };
                return string.Empty;
            }
            if (string.IsNullOrEmpty(Institution?.ID))
            {
                return Resources.InstitutionError;
            }

            return VM.GetInstitution(Institution.ID) is null ? Resources.InstitutionError : string.Empty;
        }

        private bool CheckIsQuitSmoking()
        {
            return !string.IsNullOrEmpty(AdjustCase?.ID) && AdjustCase.ID.Equals("5");
        }

        private bool CheckIsHomeCare()
        {
            return !string.IsNullOrEmpty(AdjustCase?.ID) && AdjustCase.ID.Equals("D");
        }

        private string CheckAdjustCase()
        {
            return string.IsNullOrEmpty(AdjustCase?.ID) ? Resources.AdjustCaseError : string.Empty;
        }

        private string CheckPrescriptionCase()
        {
            var homeCareAndQuitSmoke = CheckIsHomeCare() || CheckIsQuitSmoking();
            if (!homeCareAndQuitSmoke && string.IsNullOrEmpty(PrescriptionCase?.ID))
                return Resources.PrescriptionCaseError;
            if (Division != null && Division.ID.Equals("40") && AdjustCase != null && (AdjustCase.ID.Equals("1") || AdjustCase.ID.Equals("3")))
                PrescriptionCase = VM.GetPrescriptionCases("19");
            return string.Empty;
        }

        private string CheckPharmacist()
        {
            return string.IsNullOrEmpty(Pharmacist?.IDNumber) ? Resources.尚未選擇藥師 : string.Empty;
        }

        private string CheckCopayment()
        {
            if (Copayment is null)
            {
                return Resources.CopaymentError;
            }

            if (!CheckIsHomeCare())
            {
                return string.IsNullOrEmpty(Copayment?.Id) ? Resources.CopaymentError : string.Empty;
            }

            Copayment = VM.GetCopayment("009");
            return string.Empty;
        }

        private string CheckDivision()
        {
            if (Division != null && !string.IsNullOrEmpty(Division.ID))
            {
                return string.Empty;
            }
            if (CheckIsHomeCare() || CheckIsQuitSmoking())
            {
                return string.Empty;
            }
            return Resources.DivisionError;
        }

        private string CheckPaymentCategory()
        {
            if (!(PaymentCategory is null))
            {
                return string.Empty;
            }
            bool isChronic = ChronicSeq != null || AdjustCase.IsChronic();
            if (CheckIsHomeCare() || isChronic)
            {
                return string.Empty;
            }
            return Resources.PaymentCategoryError;
        }

        private string CheckDiseaseCode()
        {
            if (string.IsNullOrEmpty(MainDisease?.ID))
            {
                return CheckIsHomeCare() ? string.Empty : Resources.DiseaseCodeError;
            }

            return string.Empty;
        }

        [SuppressMessage("ReSharper", "FlagArgument")]
        private string CheckMedicalNumber(bool noCard)
        {
            if (noCard)
            {
                return SetMedicalNumberWithoutICCard();
            }

            var medicalNumberEmpty = CheckMedicalNumberEmpty();
            if (!string.IsNullOrEmpty(medicalNumberEmpty))
            {
                return medicalNumberEmpty;
            }

            SetMedicalNumber();
            return string.Empty;
        }

        private string SetMedicalNumberWithoutICCard()
        {
            if (string.IsNullOrEmpty(TempMedicalNumber)) return string.Empty;
            if (ChronicSeq is null)
                MedicalNumber = TempMedicalNumber;
            else
            {
                if (ChronicSeq > 1)
                {
                    MedicalNumber = "IC0" + ChronicSeq;
                    OriginalMedicalNumber = TempMedicalNumber;
                }
                else
                {
                    MedicalNumber = TempMedicalNumber;
                    OriginalMedicalNumber = null;
                }
            }
            return string.Empty;
        }

        private string CheckMedicalNumberEmpty()
        {
            if (string.IsNullOrEmpty(TempMedicalNumber))
            {
                if (!CheckIsHomeCare())
                {
                    return Resources.MedicalNumberError;
                }

                TempMedicalNumber = "N";
                return string.Empty;
            }
            return string.Empty;
        }

        private void SetMedicalNumber()
        {
            if (ChronicSeq is null)
                MedicalNumber = TempMedicalNumber;
            else
            {
                if (ChronicSeq > 1)
                {
                    MedicalNumber = "IC0" + ChronicSeq;
                    OriginalMedicalNumber = TempMedicalNumber;
                }
                else
                {
                    MedicalNumber = TempMedicalNumber;
                    OriginalMedicalNumber = null;
                }
            }
        }

        private string CheckChronicTimes()
        {
            if (string.IsNullOrEmpty(AdjustCase.ID))
            {
                return string.Empty;
            }

            if (!AdjustCase.ID.Equals("2"))
            {
                return string.Empty;
            }

            if (ChronicSeq is null && ChronicTotal is null)
            {
                return Resources.ChronicTimesError;
            }

            if (ChronicSeq is null)
            {
                return Resources.ChronicSeqError;
            }

            return ChronicTotal is null ? Resources.ChronicTotalError : string.Empty;
        }

        public string CheckPrescribeRule()
        {
            CheckPrescribeInstitution();
            if (AdjustCase is null || !AdjustCase.ID.Equals("0"))
            {
                AdjustCase = VM.GetAdjustCase("0").DeepCloneViaJson();
            }

            return CheckPharmacist();
        }

        private void CheckPrescribeInstitution()
        {
            if (string.IsNullOrEmpty(Institution?.FullName))
            {
                Institution =
                    new Institution
                    {
                        ID = VM.CurrentPharmacy.ID,
                        Name = VM.CurrentPharmacy.Name,
                        FullName = VM.CurrentPharmacy.ID + VM.CurrentPharmacy.Name
                    };
            }
        }

        public void CheckPrescriptionVariable()
        {
            CheckCurrentPharmacyInstitution();
            CheckIllegalAdjustCase();
        }

        private void CheckCurrentPharmacyInstitution()
        {
            if (!IsPrescribe && Institution.CheckIDEqualsCurrentPharmacy())
                Institution = new Institution();
        }

        private void CheckIllegalAdjustCase()
        {
            if (!IsPrescribe && AdjustCase.CheckIsPrescribe())
            {
                if (ChronicSeq is null)
                    AdjustCase = VM.GetAdjustCase("1");

                if (CheckChronicSeqValid())
                    AdjustCase = VM.GetAdjustCase("2");
            }

            if (ChronicSeq is null && AdjustCase.ID.Equals("2"))
                AdjustCase = VM.GetAdjustCase("1");

            if (CheckChronicSeqValid())
                AdjustCase = VM.GetAdjustCase("2");
        }

        public bool CheckChronicSeqValid()
        {
            return ChronicSeq != null && ChronicSeq > 0;
        }

        public bool CheckChronicTotalValid()
        {
            return ChronicTotal != null && ChronicTotal > 0;
        }

        public string GetWareHouseID()
        {
            return WareHouse.ID;
        }

        public bool CheckPatientDataEmpty(string data)
        {
            switch (data)
            {
                case "IDNumber":
                    return Patient.CheckIDNumberEmpty();

                case "Name":
                    return Patient.CheckNameEmpty();

                case "Birthday":
                    return Patient.CheckBirthdayNull();

                case "Tel":
                    return Patient.CheckTelEmpty();

                case "CellPhone":
                    return Patient.CheckCellPhoneEmpty();

                default:
                    return false;
            }
        }

        public void CountDeposit()
        {
            PrescriptionPoint.CountDeposit();
        }

        public void GetMedicinesBySet(MedicineSet currentSet)
        {
            Medicines.GetMedicineBySet(currentSet, WareHouse is null ? "0" : WareHouse.ID, AdjustDate);
        }

        public void SetDetail()
        {
            CountPrescriptionPoint();
            PrescriptionPoint.CountAmountsPay();
            SetPrescriptionDetail();//產生藥品資料
            SetValue();
        }

        public bool CheckCanSendOrder()
        {
            return AdjustDate != null && !string.IsNullOrEmpty(AdjustCase.ID) && AdjustCase.ID.Equals("2") && DateTime.Compare((DateTime)AdjustDate, DateTime.Today) >= 0;
        }

        public string CheckMedicinesNegativeStock()
        {
            MainWindow.ServerConnection.OpenConnection();
            var usableAmountList = CheckUsableMedicinesByType();
            MainWindow.ServerConnection.CloseConnection();
            Medicines.CheckUsableAmount(usableAmountList);

            return WareHouse is null ? string.Empty : Medicines.CheckNegativeStock(WareHouse?.ID, usableAmountList, Patient.Name, $"{Patient.Name} {DateTimeExtensions.ConvertToTaiwanCalendarChineseFormat(AdjustDate, true)} 欠藥採購");
        }

        public void CountSelfPay()
        {
            var selfPay = Medicines.CountSelfPay();
            if (selfPay >= 0)
                PrescriptionPoint.AmountSelfPay = selfPay;
        }

        public bool CheckCanEdit()
        {
            return InsertTime != null && DateTime.Compare((DateTime)InsertTime, DateTime.Today) >= 0 || !PrescriptionStatus.IsAdjust;
        }

        public string CheckMedicinesRule()
        {
            return Medicines.Check();
        }

        public void Delete()
        {
            switch (Type)
            {
                default:
                    var resultTable = PrescriptionDb.DeletePrescription(this);

                    PrescriptionDb.DeleteStoreOrder(this.ID);

                    while (resultTable.Rows.Count == 0 || !resultTable.Rows[0].Field<bool>("Result"))
                    {
                        MessageWindow.ShowMessage("處方刪除異常，按下OK重試", MessageType.WARNING);
                        resultTable = PrescriptionDb.DeletePrescription(this);
                    }
                    break;

                case PrescriptionType.ChronicReserve:
                    PrescriptionDb.DeleteReserve(SourceId);
                    break;
            }
        }

        public string CheckMedicinesIdEmpty()
        {
            var emptyMedicine = string.Empty;
            var sameList = (from m in Medicines.Where(m => !(m is MedicineVirtual)) where string.IsNullOrEmpty(m.ID) select "藥品:" + m.FullName + "代碼不得為空。\n").ToList();
            return sameList.Count <= 0 ? emptyMedicine : sameList.Distinct().Aggregate(emptyMedicine, (current, s) => current + s);
        }

        public void GetMedicines()
        {
            switch (Type)
            {
                case PrescriptionType.ChronicReserve:
                    Medicines.Clear();
                    Medicines.GetDataByReserveId(int.Parse(SourceId));
                    break;

                default:
                    Medicines.Clear();
                    Medicines.GetDataByPrescriptionId(ID);
                    break;
            }
        }

        public MedicineInventoryStructs CheckUsableMedicinesByType()
        {
            var usableInventoryStructs = new MedicineInventoryStructs();
            switch (Type)
            {
                case PrescriptionType.ChronicRegister:
                    usableInventoryStructs.GetUsableAmountByPrescriptionID(ID);
                    break;

                case PrescriptionType.ChronicReserve:
                    usableInventoryStructs.GetUsableAmountByReserveID(int.Parse(SourceId));
                    break;
            }
            return usableInventoryStructs;
        }

        public void Reset()
        {
            PrescriptionStatus.ReserveSend = false;
            PrescriptionStatus.OrderStatus = string.Empty;
            OrderContent = string.Empty;
            foreach (var m in Medicines)
            {
                m.AdjustNoBuckle = false;
                m.IsClosed = false;
                m.BuckleAmount = m.Amount;
                m.SendAmount = -1;
            }
            UpdateMedicines();
        }

        public bool CheckChronicAdjustDateValid()
        {
            var adjust = (DateTime)AdjustDate;
            var treat = (DateTime)TreatDate;
            var seq = (int)ChronicSeq - 1;
            switch (ChronicSeq)
            {
                case 1:
                    return DateTime.Compare(adjust, treat.AddDays(10)) <= 0 && DateTime.Compare(adjust, treat) >= 0;

                default:
                    var standardDate = treat.AddDays(MedicineDays * seq);
                    var start = standardDate.AddDays(-10);
                    var end = standardDate.AddDays(10);
                    return DateTime.Compare(adjust, start) >= 0 && DateTime.Compare(adjust, end) <= 0;
            }
        }

        public void CheckCopaymentRule()
        {
            DateTime date = TreatDate is null ? DateTime.Today : Convert.ToDateTime(TreatDate);
            if (DateTime.Compare(date, new DateTime(2023, 7, 1)) < 0)
            {
                if (CheckIsChronic() && MedicineDays >= 28)
                    Copayment = VM.GetCopayment("I22");
                else
                    Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");

                return;
            }

            var isChronic = CheckIsChronic();

            if (isChronic)
            {
                if (ChronicSeq == 1)
                {
                    //醫學中心or區域醫院
                    if (Institution.LevelType == "1" || Institution.LevelType == "2")
                    {
                        Copayment = VM.GetCopayment("I20");
                    }
                    else if (Institution.LevelType == "4")
                    {
                        Copayment = VM.GetCopayment("I22");
                    }
                    else
                    {
                        Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
                    }
                }
                else
                    Copayment = VM.GetCopayment("I22");
            }
            else
            {
                if (MedicineDays < 28)
                    Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");

                Copayment = VM.GetCopayment(Institution.LevelType == "4" ? "I22" : Copayment.Id);
            }
        }
    }
}