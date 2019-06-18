﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
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
using Employee = His_Pos.NewClass.Person.Employee.Employee;
// ReSharper disable ClassTooBig

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
                    Medicines.GetDataByPrescriptionId(ID,WareHouse?.ID);
                    break;
                case ChronicType.Reserve:
                    Medicines = new Medicines();
                    Medicines.GetDataByReserveId(ID, WareHouse?.ID);
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
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null)
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
                TempMedicalNumber = c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode;
            #endregion
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead?.Equals("D") ?? false;
            Medicines = new Medicines();
            Medicines.GetDataByOrthopedicsPrescription(prescription.MedicineOrder.Item, WareHouse?.ID, IsBuckle);
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
            if (ChronicSeq != null)
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
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = isRead;
            Medicines = new Medicines();
            Medicines.GetDataByCooperativePrescription(prescription.MedicineOrder.Item, WareHouse?.ID, IsBuckle);
        }

        #endregion
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
                CheckDivisions();
            }
        }

        private Division division;//就醫科別 D13
        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
                CheckDivisionValid();
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
                CheckVariableByChronicSequence();
            }
        }//連續處方箋調劑序號 D35

        private AdjustCase adjustCase;//調劑案件 D1
        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                if (value.CheckIsPrescribe() && MedicineDays > 3)
                    value = VM.GetAdjustCase("1");
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
                if (value.Id.Equals("I21") && PrescriptionPoint?.MedicinePoint > 100)
                    value = VM.GetCopayment("I20");
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

        public WareHouse.WareHouse WareHouse => VM.CooperativeClinicSettings.GetWareHouseByPrescription(Institution,AdjustCase?.ID);
        public bool IsPrescribe => AdjustCase.ID.Equals("0") || Medicines.Count(m => !m.PaySelf) == 0;
        public bool IsBuckle => WareHouse != null;
        #endregion
        public bool CheckDiseaseEquals(List<string> parameters)
        {
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            return diseaseID.Equals(elementName.Equals("MainDiagnosis") ? MainDisease.FullName : SubDisease.FullName);
        }

        private void CheckTypeByInstitution()
        {
            if (Institution != null && Institution.CheckCooperative())
                CheckIsOrthopedics();
            else
                Type = PrescriptionType.Normal;

            if (Medicines is null) return;

            foreach (var m in Medicines)
            {
                m.IsBuckle = IsBuckle;
            }
        }
        private void CheckDivisions()
        {
            if (string.IsNullOrEmpty(Institution.ID)) return;
            var table = InstitutionDb.GetEnableDivisions(Institution.ID);
            if (table.Rows.Count <= 0) return;
            var divListString = table.Rows[0].Field<string>("Divisions");
            if(string.IsNullOrEmpty(divListString)) return;
            var divisions = new List<string>();
            if(divListString.Contains(','))
                divisions = divListString.Split(',').ToList();
            else
                divisions.Add(divListString);
            if (divisions.Count == 1)
                Division = VM.GetDivision(divisions[0]);
        }

        private void CheckIsOrthopedics()
        {
            Type = Institution.CheckIsOrthopedics() ? PrescriptionType.Orthopedics : PrescriptionType.Cooperative;
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

        public void CountPrescriptionPoint()
        {
            PrescriptionPoint.MedicinePoint = Medicines.CountMedicinePoint();
            PrescriptionPoint.SpecialMaterialPoint = Medicines.CountSpecialMedicinePoint();
            PrescriptionPoint.AmountSelfPay = Medicines.CountSelfPay();
            PrescriptionPoint.CountAmountsPay();
            if (!AdjustCase.ID.Equals("0"))
            {
                CheckCopayment();
                if (Patient.Birthday != null)
                {
                    SetMedicalService();//確認藥事服務資料
                }
                PrescriptionPoint.CountTotal();
                PrescriptionPoint.CountApply();
            }
        }

        private void CheckCopayment()
        {
            if (CheckIsChronic())
                Copayment = VM.GetCopayment("I22");
            if (!CheckFreeCopayment())
                Copayment = VM.GetCopayment(PrescriptionPoint.MedicinePoint <= 100 ? "I21" : "I20");
            PrescriptionPoint.CopaymentPoint = CheckNotFreeCopayment() ? CountCopaymentPoint() : 0;
        }
        private bool CheckIsChronic()
        {
            return AdjustCase.ID.Equals("2") || ChronicSeq != null && ChronicSeq > 0 ||
                   ChronicTotal != null && ChronicTotal > 0;
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
            if (!CheckFreeCopayment())
                return PrescriptionPoint.CopaymentValue;
            if (CheckIsAdministrativeAssistanceCopayment())
            {
                if(PrescriptionPoint.CopaymentValue > 0)
                    PrescriptionPoint.AdministrativeAssistanceCopaymentPoint = PrescriptionPoint.CopaymentValue;
            }
            return 0;
        }

        private bool CheckIsAdministrativeAssistanceCopayment()
        {
            if(AdjustCase.ID.Equals("5"))
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
                    PrescriptionPoint.MedicalServicePoint = 69;
                    break;
                case int n when n >= 14 && n < 28:
                    MedicalServiceCode = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                    PrescriptionPoint.MedicalServicePoint = 59;
                    break;
                case int n when n >= 7 && n < 14:
                    MedicalServiceCode = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                    PrescriptionPoint.MedicalServicePoint = 48;
                    break;
                default:
                    MedicalServiceCode = "05202B";//一般處方給付(7天以內)
                    PrescriptionPoint.MedicalServicePoint = 48;
                    break;
            }
        }

        private List<Pdata> SetPrescriptionDetail()
        {
            var details = new List<Pdata>();
            CreateMedicinesDetail(details);
            if (IsPrescribe || CheckOnlyBloodGlucoseTestStrip()) return details;
            MedicineDays = Medicines.CountMedicineDays();//計算最大給藥日份
            var medicalService = new Pdata(PDataType.Service, MedicalServiceCode, Patient.CheckAgePercentage(), 1);
            details.Add(medicalService);
            if (CheckNotNormalPrescription()) return details;
            var dailyPrice = CheckIfSimpleFormDeclare();
            if (dailyPrice <= 0) return details;
            CreateSimpleFormDetail(details,dailyPrice);
            return details;
        }

        private void CreateSimpleFormDetail(List<Pdata> details,int dailyPrice)
        {
            foreach (var d in details)
            {
                if (!d.P1.Equals("1")) continue;
                d.P1 = "4";
                d.P8 = $"{0.00:0000000.00}";
                d.P9 = "00000000";
            }
            var simpleForm = new Pdata(PDataType.SimpleForm, dailyPrice.ToString(), 100, MedicineDays);
            details.Add(simpleForm);
        }

        private void CreateMedicinesDetail(List<Pdata> details)
        {
            var serialNumber = 1;
            foreach (var med in Medicines.GetDeclare())
            {
                details.Add(new Pdata(med, serialNumber.ToString()));
                serialNumber++;
            }
            details.AddRange(Medicines.Where(m => m.PaySelf).Select(med => new Pdata(med, string.Empty)));
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
        public void PrintMedBagAndReceipt()
        {
            PrintMedBagConfirm();
            var receiptPrint = false;
            if (PrescriptionPoint.AmountsPay > 0)
            {
                var receiptResult = new ConfirmWindow(Resources.收據列印確認, Resources.列印確認, true);
                if (receiptResult.DialogResult != null)
                    receiptPrint = (bool)receiptResult.DialogResult;
            }
            if (receiptPrint)
                PrintReceipt();
        }

        private void PrintMedBagConfirm()
        {
            var medBagPrint = new ConfirmWindow(Resources.藥袋列印確認, Resources.列印確認, true);
            Debug.Assert(medBagPrint.DialogResult != null, "medBagPrint.DialogResult != null");
            if (!(bool)medBagPrint.DialogResult) return;
            var printBySingleMode = new MedBagSelectionWindow();
            // ReSharper disable once PossibleInvalidOperationException
            var singleMode = (bool)printBySingleMode.ShowDialog();
            if (singleMode)
                PrintMedBagSingleMode();
            else
            {
                PrintMedBagMultiMode();
            }
        }

        private void PrintMedBagSingleMode()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new MedBagMedicines(Medicines, true);
            foreach (var m in medBagMedicines)
            {
                SetSingleModeReportViewer(rptViewer,m);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                });
            }
        }

        private void PrintMedBagMultiMode()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            SetMultiModeReportViewer(rptViewer);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
            });
        }

        private void PrintReceipt()
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
        private void SetSingleModeReportViewer(ReportViewer rptViewer, MedBagMedicine m)
        {
            rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReportSingle.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateSingleMedBagParameter(m, this);
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
        private void SetReceiptReportViewer(ReportViewer rptViewer)
        {
            rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateReceiptParameters(this);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }
        private void SetDepositReportViewer(ReportViewer rptViewer)
        {
            rptViewer.LocalReport.ReportPath = @"RDLC\DepositSheet.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = PrescriptionService.CreateDepositSheetParameters(this);
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
        }
        #endregion
        #endregion

        public void AddMedicine(string medicineID)
        {
            var paySelf = AdjustCase.CheckIsPrescribe();
            int? selectedMedicinesIndex = null;
            if (SelectedMedicine != null)
                selectedMedicinesIndex = Medicines.IndexOf(SelectedMedicine);
            Medicines.AddMedicine(medicineID, paySelf, selectedMedicinesIndex,WareHouse?.ID);
        }

        public void Init()
        {
            TreatDate = DateTime.Today;
            AdjustDate = DateTime.Today;
            AdjustCase = VM.GetAdjustCase("1");
            PrescriptionCase = VM.GetPrescriptionCases("09");
            Copayment = VM.GetCopayment("I21");
            PaymentCategory = VM.GetPaymentCategory("4");
        }

        public void DeleteMedicine()
        {
            Medicines.Remove(SelectedMedicine);
            CountPrescriptionPoint();
        }

        public void SetBuckleAmount()
        {
            if (IsBuckle)
            {
                foreach (var m in Medicines)
                {
                    m.IsBuckle = true;
                    m.BuckleAmount = m.Amount;
                }
            }
            else
            {
                foreach (var m in Medicines)
                {
                    m.IsBuckle = false;
                    m.BuckleAmount = 0;
                }
            }
        }

        public bool CheckPatientWithCard(Customer patientFromCard)
        {
            if (!string.IsNullOrEmpty(Patient.IDNumber))
            {
                if (Patient.IDNumber.Equals(patientFromCard.IDNumber))
                {
                    Patient = patientFromCard;
                    return true;
                }
                MessageWindow.ShowMessage("卡片讀取結果與目前處方病患不符，請確認卡片或病患資料",MessageType.ERROR);
                return false;
            }
            Patient = patientFromCard;
            return true;
        }

        private void CheckDivisionValid()
        {
            if (string.IsNullOrEmpty(Institution.ID) || string.IsNullOrEmpty(Division.ID)) return;
            var table = InstitutionDb.CheckDivisionValid(Institution.ID, Division.ID);
            if (table.Rows.Count <= 0) return;
            var result = table.Rows[0].Field<bool>("Result");
            if (!result)
                MessageWindow.ShowMessage("該院所登記之診療科別不包含目前選取科別。", MessageType.WARNING);
        }

        private void CheckVariableByDivision()
        {
            if (AdjustCase is null) return;
            if (!(AdjustCase.ID.Equals("1") || AdjustCase.ID.Equals("3"))) return;
            switch (division.ID)
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

        public void SetPrescribeAdjustCase()
        {
            AdjustCase = VM.GetAdjustCase("0");
        }

        public string CheckPrescriptionRule(bool noCard)
        {
            var errorMsg = string.Empty;
            if (IsPrescribe) return errorMsg;
            //檢查健保規則
            return errorMsg;
        }

        public void UpdateMedicines()
        {
            Medicines.Update(IsBuckle,WareHouse?.ID);
        }
    }
}
