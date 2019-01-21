using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription.DeclareFile;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using Dbody = His_Pos.NewClass.Prescription.DeclareFile.Dbody;
using Ddata = His_Pos.NewClass.Prescription.DeclareFile.Ddata;
using Dhead = His_Pos.NewClass.Prescription.DeclareFile.Dhead;
using Pdata = His_Pos.NewClass.Prescription.DeclareFile.Pdata;

namespace His_Pos.NewClass.Prescription
{
    public class Prescription : ObservableObject
    {
        public Prescription()
        {
            Patient = new Customer();
            Card = new IcCard();
            Treatment = new Treatment.Treatment();
            Medicines = new Medicines();
        }

        public Prescription(DataRow r)
        {
            Id = r.Field<int>("");

        }
        public Prescription(CooperativePrescription c) {
            #region CooPreVariable
            var prescription = c.DeclareXmlDocument.Prescription;
            var customer = prescription.CustomerProfile.Customer;
            var birthYear = int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = int.Parse(customer.Birth.Substring(5, 2));
            #endregion 
            Source = PrescriptionSource.Cooperative;
            SourceId = c.CooperativePrescriptionId;
            Remark = customer.Remark;
            MedicineDays = string.IsNullOrEmpty(prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(prescription.MedicineOrder.Days);
            Treatment = new Treatment.Treatment(c);
            Patient = new Customer
            {
                IDNumber = customer.IdNumber,
                Name = customer.Name,
                Birthday = new DateTime(birthYear, birthMonth, birthDay),
                Tel = customer.Phone
            };
            Card = new IcCard(); 
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead.Equals("D");
            foreach (var m in prescription.MedicineOrder.Item) {
                Medicines.Add(new Medicine(m));
            }
        }
        public int Id { get; set; }
        private Customer patient;
        public Customer Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
            }
        } //病患

        public IcCard Card { get; set; }
        public Treatment.Treatment Treatment { get; set; }//處方資料
        public PrescriptionSource Source { get; set; }
        public string SourceId { get; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public string Remark { get; }//回傳合作診所單號 
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceID { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public Medicines Medicines { get; set; } = new Medicines();//調劑用藥
        public void InitialCurrentPrescription()
        {
            Treatment.Initial();
            Medicines.Add(new Medicine());
        }
        private int CountCopaymentPoint()
        {
            if (CheckFreeCopayment())
                return 0;
            var point = PrescriptionPoint.MedicinePoint;
            if (point > 100 && point <= 200)
                return 20;
            if (point >= 201 && point <= 300)
                return 40;
            if (point >= 301 && point <= 400)
                return 60;
            if (point >= 401 && point <= 500)
                return 80;
            if (point >= 501 && point <= 600)
                return 100;
            if (point >= 601 && point <= 700)
                return 120;
            if (point >= 701 && point <= 800)
                return 140;
            if (point >= 801 && point <= 900)
                return 160;
            if (point >= 901 && point <= 1000)
                return 180;
            return 200;
        }

        private bool CheckFreeCopayment()
        {
            if (Treatment.AdjustCase.Id.Equals("2") || Treatment.AdjustCase.Id.Equals("4"))
                return true;
            switch (Treatment.Copayment.Id)
            {
                case "009"://其他免負擔
                case "I21"://藥費100元以下
                case "I22"://免收
                    return true;
            }
            switch (Treatment.PrescriptionCase.Id)
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
                    return true;
            }
            return false;
        }

        #region Function
        public int InsertPresription()
        {
            if(Medicines.Count(m => m is MedicineNHI && !m.PaySelf) > 0)
                MedicineDays = (int)Medicines.Where(m => m is MedicineNHI && !m.PaySelf).Max(m => m.Days);//計算最大給藥日份

            CheckMedicalServiceData();//確認藥事服務資料
            var details = SetPrescriptionDetail();//產生藥品資料
            PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
            PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                           PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
            PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
            CreateDeclareFileContent(details);//產生申報資料
            return PrescriptionDb.InsertPrescription(this, details);
        }

        private List<Pdata> SetPrescriptionDetail()
        {
            var details = new List<Pdata>();
            var serialNumber = 1;
            foreach (var med in Medicines.Where(m => m is MedicineNHI && !m.PaySelf))
            {
                details.Add(new Pdata(med, serialNumber.ToString()));
                serialNumber++;
            }
            details.AddRange(Medicines.Where(m => m.PaySelf).Select(med => new Pdata(med, string.Empty)));
            var medicalService = new Pdata(PDataType.Service, MedicalServiceID, Patient.CheckAgePercentage(), 1);
            details.Add(medicalService);
            int dailyPrice = CheckIfSimpleFormDeclare();
            if (dailyPrice > 0)
            {
                foreach (var d in details)
                {
                    if (!d.P1.Equals("1")) continue;
                    d.P1 = "4";
                    d.P8 = $"{0.00:0000000.00}";
                    d.P9 = "00000000";
                }
                var simpleForm = new Pdata(PDataType.SimpleForm, dailyPrice.ToString(), 100, 1);
                details.Add(simpleForm);
            }
            return details;
        }

        private void CheckMedicalServiceData()
        {
            if (Treatment.ChronicSeq is null || string.IsNullOrEmpty(Treatment.ChronicSeq.ToString()))
            {
                MedicalServiceID = "05202B";//一般處方給付(7天以內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
            else
            {
                SetChronicMedicalServiceCode();
            }
        }

        private void SetChronicMedicalServiceCode()
        {
            if (MedicineDays >= 28)
            {
                MedicalServiceID = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 69;
            }
            else if (MedicineDays < 14)
            {
                MedicalServiceID = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
            else
            {
                MedicalServiceID = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 59;
            }
        }

        private int CheckIfSimpleFormDeclare()
        {
            if (MedicineDays > 3 || !Treatment.AdjustCase.Id.Equals("1")) return 0;
            double medicinePoint = Medicines.Where(m => !m.PaySelf).Sum(med => med.NHIPrice * med.Amount);
            var medFormCount = CountOralLiquidAgent();//口服液劑(原瓶包裝)數量
            var dailyPrice = CountDayPayAmount(Patient.CountAge(), medFormCount);//計算日劑藥費金額
            if (dailyPrice*MedicineDays < medicinePoint) return 0;
            Treatment.AdjustCase = ViewModelMainWindow.GetAdjustCase("3");
            PrescriptionPoint.MedicinePoint = dailyPrice * MedicineDays;
            return dailyPrice;
        }

        private int CountOralLiquidAgent()
        {
            return Medicines.Count(m=>m is MedicineNHI med && !string.IsNullOrEmpty(med.Note) && med.Note.Contains(Properties.Resources.口服液劑));
        }

        private int CountDayPayAmount(int cusAge, int medFormCount)
        {
            const int ma1 = 22, ma2 = 31, ma3 = 37, ma4 = 41;
            if (cusAge <= 12 && medFormCount == 1) return ma2;
            if (cusAge <= 12 && medFormCount == 2) return ma3;
            if (cusAge <= 12 && medFormCount >= 3) return ma4;
            return ma1;
        }

        public void AddMedicineBySearch(string proId, int selectedMedicinesIndex) {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = MedicineDb.GetMedicinesBySearchId(proId);
            MainWindow.ServerConnection.CloseConnection();
            foreach (DataRow r in table.Rows) 
            {
                switch (r.Field<int>("DataType"))
                {
                    case 0:
                        Medicines[selectedMedicinesIndex] = new MedicineOTC(r);
                        break;
                    case 1:
                        Medicines[selectedMedicinesIndex] = new MedicineNHI(r);
                        break;
                }
            }
        }
        public void DeleteReserve() {
            PrescriptionDb.DeleteReserve(SourceId);
        }
        public void PredictResere() {
            PrescriptionDb.PredictResere(SourceId);
        }
        
        public void PrintMedBag() { 
        }

        #endregion
        public void AddCooperativePrescriptionMedicines() {
            for(int medCount = 0; medCount < Medicines.Count; medCount++){
                var table = MedicineDb.GetMedicinesBySearchId(Medicines[medCount].ID);
                var temp = new Medicine();
                if (table.Rows.Count > 0)
                {
                    switch (table.Rows[0].Field<int>("DataType"))
                    {
                        case 0:
                            temp = new MedicineOTC(table.Rows[0]); 
                            break;
                        case 1:
                            temp = new MedicineNHI(table.Rows[0]); 
                            break;
                    }
                }
                else
                {
                    temp = new MedicineOTC
                    {
                        ID = Medicines[medCount].ID,
                        ChineseName = Medicines[medCount].ChineseName,
                        EnglishName = Medicines[medCount].EnglishName
                    };
                    MedicineDb.InsertCooperativeMedicineOTC(temp.ID , temp.ChineseName);//新增合作診所MedicineOtc
                }
                temp.UsageName = Medicines[medCount].UsageName;
                temp.PositionName = Medicines[medCount].Position.Name;
                ViewModelMainWindow.CheckContainsUsage(temp.UsageName);
                ViewModelMainWindow.CheckContainsPosition(temp.PositionName);

                temp.Amount = Medicines[medCount].Amount;
                temp.Dosage = Medicines[medCount].Dosage;
                temp.Days = Medicines[medCount].Days;
                temp.PaySelf = Medicines[medCount].PaySelf;
                temp.TotalPrice = Medicines[medCount].TotalPrice;
                Medicines[medCount] = temp; 
            }
           
        }
        public int UpdatePrescriptionCount()//計算處方張數
        {
            return PrescriptionDb.GetPrescriptionCountByID(Treatment.Pharmacist.IdNumber).Rows[0].Field<int>("PrescriptionCount");
        }

        public string CheckPrescriptionRule()//檢查健保邏輯
        {
            return Treatment.Check() + Patient.CheckBasicData();
        }

        public void ProcessInventory()//扣庫
        {
            foreach (var m in Medicines)
            {
                PrescriptionDb.ProcessInventory(m.ID, m.Amount);
            }
        }

        public void ProcessEntry(string entryName, string source, int sourceId)//計算庫存現值
        { 
            double total = 0;//總金額
            foreach (var m in Medicines)
            { 
                total += m.TotalPrice;
            }
            PrescriptionDb.ProcessEntry(entryName, source, sourceId , total);
        }

        public void ProcessCopaymentCashFlow(string name)//計算部分金流
        {
            PrescriptionDb.ProcessCashFlow(name, "PreMasId", Id, PrescriptionPoint.CopaymentPoint);
        }
        public void ProcessSelfPayCashFlow(string name)//計算自費金流
       {
            PrescriptionDb.ProcessCashFlow(name, "PreMasId", Id, PrescriptionPoint.AmountSelfPay);
        }
        public void ProcessDepositCashFlow(string name)//計算押金金流
       {
            PrescriptionDb.ProcessCashFlow(name, "PreMasId", Id, PrescriptionPoint.Deposit);
        }

        public void CreateDeclareFileContent(List<Pdata> details)//產生申報檔內容
        {
            Ddata d = new Ddata();
            d.Dhead = new Dhead();
            d.Dbody = new Dbody();
            d.Dbody.Pdata = new List<Pdata>();
            d.Dhead.D1 = Treatment.AdjustCase.Id;
            d.Dhead.D2 = string.Empty;
            d.Dhead.D3 = Patient.IDNumber;
            d.Dhead.D4 = string.Empty;
            d.Dhead.D5 = Treatment.PaymentCategory?.Id;
            d.Dhead.D6 = DateTimeExtensions.ConvertToTaiwanCalender((DateTime)Patient.Birthday,false);
            d.Dhead.D7 = Treatment.MedicalNumber;
            d.Dhead.D8 = Treatment.MainDisease.Id;
            d.Dhead.D9 = Treatment.SubDisease?.Id;
            d.Dhead.D13 = Treatment.Division?.Id;
            d.Dhead.D14 = Treatment.TreatDate is null? string.Empty: DateTimeExtensions.ConvertToTaiwanCalender((DateTime)Treatment.TreatDate, false);
            d.Dhead.D15 = Treatment.Copayment.Id;
            d.Dhead.D16 = $"{PrescriptionPoint.ApplyPoint:00000000}";
            d.Dhead.D17 = $"{PrescriptionPoint.CopaymentPoint:0000}";
            d.Dhead.D18 = $"{PrescriptionPoint.TotalPoint:00000000}";
            d.Dhead.D20 = Patient.Name;
            d.Dhead.D21 = Treatment.Institution.Id;
            d.Dhead.D22 = Treatment.PrescriptionCase?.Id;
            d.Dhead.D23 = DateTimeExtensions.ConvertToTaiwanCalender((DateTime)Treatment.AdjustDate, false);
            if (!Treatment.CheckIsQuitSmoking() && !Treatment.CheckIsHomeCare())
            {
                d.Dhead.D24 = d.Dhead.D21;
            }
            else
            {
                d.Dhead.D24 = string.Empty;
            }
            d.Dhead.D25 = Treatment.Pharmacist.IdNumber;
            d.Dbody.D26 = Treatment.SpecialTreat?.Id;
            d.Dbody.D30 = MedicineDays.ToString();
            d.Dbody.D31 = $"{PrescriptionPoint.SpecialMaterialPoint:0000000}";
            d.Dbody.D32 = "00000000";
            d.Dbody.D33 = details.Where(p => p.P1.Equals("1")).Sum(p => int.Parse(p.P9)).ToString();
            d.Dbody.D35 = Treatment.ChronicSeq is null ? string.Empty : Treatment.ChronicSeq.ToString();
            d.Dbody.D36 = Treatment.ChronicTotal is null ? string.Empty : Treatment.ChronicTotal.ToString();
            d.Dbody.D37 = MedicalServiceID;
            d.Dbody.D38 = details.Single(p => p.P1.Equals("9")).P9;
            d.Dbody.D43 = Treatment.OriginalMedicalNumber;
            d.Dbody.D44 = Card.NewBornBirthday is null ? string.Empty : DateTimeExtensions.ConvertToTaiwanCalender((DateTime) Card.NewBornBirthday, false);
            d.Dbody.Pdata = details;
            DeclareContent = d.SerializeObjectToXDocument<Ddata>();
        }

        public void CountPrescriptionPoint()
        {
            if (Medicines.Count(m => (m is MedicineNHI || m is MedicineOTC) && m.Amount > 0) <= 0) return;
            PrescriptionPoint.MedicinePoint = Medicines.CountMedicinePoint();
            if (PrescriptionPoint.MedicinePoint <= 100)
                Treatment.Copayment = ViewModelMainWindow.GetCopayment("I21");
            PrescriptionPoint.CopaymentPoint = CountCopaymentPoint();
            PrescriptionPoint.AmountSelfPay = Medicines.CountSelfPay();
            PrescriptionPoint.AmountsPay = PrescriptionPoint.CopaymentPoint + PrescriptionPoint.AmountSelfPay;
            PrescriptionPoint.ActualReceive = PrescriptionPoint.AmountsPay;
        }
        public void UpdateCooperativePrescriptionIsRead() {
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(SourceId);
        }
    }
}
