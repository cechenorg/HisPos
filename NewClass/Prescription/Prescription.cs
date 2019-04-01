using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.MedBag;
using His_Pos.Service;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using Ddata = His_Pos.NewClass.Prescription.Declare.DeclareFile.Ddata;
using MedicineDb = His_Pos.NewClass.Product.Medicine.MedicineDb;
using Pdata = His_Pos.NewClass.Prescription.Declare.DeclareFile.Pdata;
using StringRes = His_Pos.Properties.Resources;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription
{
    public class Prescription : ObservableObject,ICloneable
    {
        public Prescription()
        {
            Patient = new Customer();
            Patient.ID = -1;
            Card = new IcCard();
            Treatment = new Treatment.Treatment();
            Medicines = new Medicines();
            PrescriptionSign = new List<string>();
        }

        public Prescription(DataRow r,PrescriptionSource prescriptionSource)
        { 
            Patient = new Customer();
            Patient = Patient.GetCustomerByCusId(r.Field<int>("CustomerID"));
            Card = new IcCard();
            Treatment = new Treatment.Treatment(r);
            Medicines = new Medicines();
            PrescriptionPoint = new PrescriptionPoint(r);
            if(r.Field<int?>("DeclareFileID") != null)
                DeclareFileID = r.Field<int>("DeclareFileID");
            MedicineDays = r.Field<byte>("MedicineDays");
            switch (prescriptionSource) {
                case PrescriptionSource.Normal:
                    Id = r.Field<int>("ID");
                    MainWindow.ServerConnection.OpenConnection();
                    Medicines.GetDataByPrescriptionId(Id);
                    PrescriptionStatus = new PrescriptionStatus(r, PrescriptionSource.Normal);
                    MainWindow.ServerConnection.CloseConnection();
                    break;
                case PrescriptionSource.ChronicReserve:
                    Source = PrescriptionSource.ChronicReserve;
                    SourceId = r.Field<int>("ID").ToString();
                    MainWindow.ServerConnection.OpenConnection();
                    Medicines.GetDataByReserveId(SourceId);
                    MainWindow.ServerConnection.CloseConnection();
                    PrescriptionStatus = new PrescriptionStatus(r, PrescriptionSource.ChronicReserve);
                    break;
            }
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
          
            PrescriptionStatus.IsCooperativeVIP = Remark.EndsWith("Y");
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
            PrescriptionStatus.IsRead = c.IsRead is null ? false : c.IsRead.Equals("D");
            foreach (var m in prescription.MedicineOrder.Item) {
                Medicines.Add(new Medicine(m));
            }
        }
        public Prescription(XmlOfPrescription.Prescription c,string sourceId,bool IsRead) { 
            #region CooPreVariable
            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var birthYear = int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = int.Parse(customer.Birth.Substring(5, 2));
            #endregion 
            Source = PrescriptionSource.XmlOfPrescription;
            SourceId = sourceId; 
             
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
            PrescriptionStatus.IsRead = IsRead;
            foreach (var m in prescription.MedicineOrder.Item)
            {
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
        public string SourceId { get; set; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public string Remark { get; set; }//回傳合作診所單號 
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceID { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int? DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public int WriteCardSuccess { get; set; }
        public Medicines Medicines { get; set; } = new Medicines();//調劑用藥 
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
        public void InitialCurrentPrescription()
        {
            Treatment.Initial();
            PrescriptionStatus.Init();
        }
        public int CountCopaymentPoint()
        {
            if (CheckFreeCopayment())
                return 0;
            var point = PrescriptionPoint.MedicinePoint;
            if (point <= 100)
                return 0;
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

        public bool CheckFreeCopayment()
        {
            if (Treatment.Copayment is null) return false;
            if (Treatment.AdjustCase.ID.Equals("2"))
                Treatment.Copayment = VM.GetCopayment("I22");
            if(Treatment.AdjustCase.ID.Equals("4") || Treatment.AdjustCase.ID.Equals("0"))
                Treatment.Copayment = VM.GetCopayment("009");
            //、006，001~009(除006)，801，802，901，902，903，904
            switch (Treatment.Copayment.Id)
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
            switch (Treatment.PrescriptionCase.ID)
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
                    Treatment.Copayment = VM.GetCopayment("I22");
                    return true;
            }
            return false;
        }

        #region Function
        public int InsertPrescription()
        {
            if (PrescriptionStatus.IsPrescribe)
                Treatment.AdjustCase = VM.GetAdjustCase("0").DeepCloneViaJson();
            if(Medicines.Count(m => m is MedicineNHI && !m.PaySelf) > 0)
                CountMedicineDays();
            if (!Treatment.AdjustCase.ID.Equals("0"))
                CheckMedicalServiceData();//確認藥事服務資料
            var details = SetPrescriptionDetail();//產生藥品資料
            PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
            PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                           PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
            PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
            if(!Treatment.AdjustCase.ID.Equals("0"))
                CreateDeclareFileContent(details);//產生申報資料
            Treatment.Institution.UpdateUsedTime();
            return PrescriptionDb.InsertPrescription(this, details);
        }

        public List<Pdata> SetPrescriptionDetail()
        {
            var details = new List<Pdata>();
            var serialNumber = 1;
            foreach (var med in Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf))
            {
                details.Add(new Pdata(med, serialNumber.ToString()));
                serialNumber++;
            }
            details.AddRange(Medicines.Where(m => m.PaySelf).Select(med => new Pdata(med, string.Empty)));
            if (!Treatment.AdjustCase.ID.Equals("0"))
            {
                var medicalService = new Pdata(PDataType.Service, MedicalServiceID, Patient.CheckAgePercentage(), 1);
                details.Add(medicalService);
                CountMedicineDays();//計算最大給藥日份
                if (Treatment.AdjustCase.ID.Equals("1") || Treatment.AdjustCase.ID.Equals("3"))
                {
                    var dailyPrice = CheckIfSimpleFormDeclare();
                    if (dailyPrice > 0)
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
                }
            }
            return details;
        }
       
        private void CheckMedicalServiceData()
        {
            if (MedicineDays >= 28)
            {
                MedicalServiceID = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 69;
            }
            else if (MedicineDays > 7 && MedicineDays < 14)
            {
                MedicalServiceID = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
            else if (MedicineDays >= 14 && MedicineDays < 28)
            {
                MedicalServiceID = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                PrescriptionPoint.MedicalServicePoint = 59;
            }
            else
            {
                MedicalServiceID = "05202B";//一般處方給付(7天以內)
                PrescriptionPoint.MedicalServicePoint = 48;
            }
        }

        public int CheckIfSimpleFormDeclare()
        {
            if (Patient.Birthday is null) return 0;
            if (MedicineDays > 3)
            {
                if (Treatment.AdjustCase.ID.Equals("3"))
                    Treatment.AdjustCase = VM.GetAdjustCase("1");
                return 0;
            }
            var medicinePoint = Medicines.Where(m => !m.PaySelf).Sum(med => med.NHIPrice * med.Amount);
            var medFormCount = CountOralLiquidAgent();//口服液劑(原瓶包裝)數量
            var dailyPrice = CountDayPayAmount(Patient.CountAge(), medFormCount);//計算日劑藥費金額
            if (dailyPrice * MedicineDays < medicinePoint)
            {
                if (Treatment.AdjustCase.ID.Equals("3"))
                    Treatment.AdjustCase = VM.GetAdjustCase("1");
                return 0;
            }
            Treatment.AdjustCase = VM.GetAdjustCase("3");
            PrescriptionPoint.MedicinePoint = dailyPrice * MedicineDays;
            return dailyPrice;
        }

        private int CountOralLiquidAgent()
        {
            return Medicines.Count(m=>m is MedicineNHI med && !string.IsNullOrEmpty(med.Note) && med.Note.Contains(StringRes.口服液劑));
        }

        private int CountDayPayAmount(int cusAge, int medFormCount)
        {
            const int ma1 = 22, ma2 = 31, ma3 = 37, ma4 = 41;
            if (cusAge <= 12 && medFormCount == 1) return ma2;
            if (cusAge <= 12 && medFormCount == 2) return ma3;
            if (cusAge <= 12 && medFormCount >= 3) return ma4;
            return ma1;
        }

        public void AddMedicineBySearch(string proId) {
            DataTable table = MedicineDb.GetMedicinesBySearchId(proId);
            var medicine = new Medicine();
            foreach (DataRow r in table.Rows) 
            {
                switch (r.Field<int>("DataType"))
                {
                    case 1:
                        medicine = new MedicineNHI(r);
                        medicine.CheckPaySelf(Treatment.AdjustCase.ID);
                        break;
                    case 2:
                        medicine = new MedicineOTC(r);
                        medicine.CheckPaySelf(Treatment.AdjustCase.ID);
                        break;
                    case 3:
                        medicine = new MedicineSpecialMaterial(r);
                        medicine.CheckPaySelf(Treatment.AdjustCase.ID);
                        break;
                }
            }
            if (medicine.ID.EndsWith("00") ||
                medicine.ID.EndsWith("G0"))
                medicine.PositionID = "PO";
            //if(Medicines[selectedMedicinesIndex].Amount > 0 && !Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
            //    Medicines[selectedMedicinesIndex].BuckleAmount = Medicines[selectedMedicinesIndex - 1].BuckleAmount;
            var selectedMedicinesIndex = Medicines.IndexOf(SelectedMedicine);
            if (SelectedMedicine != null)
            {
                if(selectedMedicinesIndex > 0)
                    medicine.CopyPrevious(Medicines[selectedMedicinesIndex-1]);
                Medicines[selectedMedicinesIndex] = medicine;
            }
            else
            {
                if(Medicines.Count > 0)
                    medicine.CopyPrevious(Medicines[Medicines.Count-1]);
                Medicines.Add(medicine);
            } 
        }
        public void PredictReserve() {
            PrescriptionDb.PredictReserve(Id);
        }
        public void AdjustPredictReserve() {
            PrescriptionDb.AdjustPredictReserve(Id.ToString());
        }
        
        #endregion
        public void AdjustMedicinesType() {
            for(var medCount = 0; medCount < Medicines.Count; medCount++){
                var table = MedicineDb.GetMedicinesBySearchId(Medicines[medCount].ID);
                var temp = new Medicine();
                if (Medicines[medCount].ID.Equals("R001") || Medicines[medCount].ID.Equals("R002") ||
                    Medicines[medCount].ID.Equals("R003") || Medicines[medCount].ID.Equals("R004"))
                {
                    temp = new MedicineVirtual();
                    temp.ID = Medicines[medCount].ID;
                    switch (temp.ID)
                    {
                        case "R001":
                            temp.ChineseName = "處方箋遺失或毀損，提前回診";
                            break;
                        case "R002":
                            temp.ChineseName = "醫師請假，提前回診";
                            break;
                        case "R003":
                            temp.ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                            break;
                        case "R004":
                            temp.ChineseName = "其他提前回診或慢箋提前領藥";
                            break;
                    }
                }
                else
                {
                    if (table.Rows.Count > 0)
                    {
                        switch (table.Rows[0].Field<int>("DataType"))
                        {
                            case 1:
                                temp = new MedicineNHI(table.Rows[0]);
                                break;
                            case 2:
                                temp = new MedicineOTC(table.Rows[0]);
                                break;
                            case 3:
                                temp = new MedicineSpecialMaterial(table.Rows[0]);
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
                        if (!string.IsNullOrEmpty(temp.ID))
                            MedicineDb.InsertCooperativeMedicineOTC(temp.ID, temp.ChineseName);//新增合作診所MedicineOtc
                    }
                }
                temp.UsageName = Medicines[medCount].UsageName;
                temp.PositionID = Medicines[medCount].PositionID;
                temp.Dosage = Medicines[medCount].Dosage;
                temp.Days = Medicines[medCount].Days;
                temp.Amount = Medicines[medCount].Amount;
                temp.PaySelf = Medicines[medCount].PaySelf;
                temp.Price = Medicines[medCount].Price;
                temp.TotalPrice = Medicines[medCount].TotalPrice;
                temp.BuckleAmount = Medicines[medCount].BuckleAmount;
                //if (Medicines[medCount].PaySelf && Medicines[medCount].TotalPrice > 0)
                //{
                //    temp.Price = Medicines[medCount].Price == 0 ? 
                //        Math.Round(Medicines[medCount].TotalPrice / Medicines[medCount].Amount, 2, MidpointRounding.AwayFromZero) : Medicines[medCount].Price;
                //}
                if (!string.IsNullOrEmpty(temp.ID))
                    Medicines[medCount] = temp;
            }
        }

        public decimal ProcessInventory(string type,string source,string sourceId)//扣庫
        {
            decimal buckleValue = 0;
            foreach (var m in Medicines)
            { 
                if(!string.IsNullOrEmpty(m.ID) && (bool)m.IsBuckle)
                    buckleValue += PrescriptionDb.ProcessInventory(m.ID, m.BuckleAmount, type, source, sourceId).Rows[0].Field<decimal>("BuckleTotalValue");
            }
            return buckleValue;
        } 
        public void ProcessMedicineUseEntry(decimal bucklevalue)//計算調劑耗用
        {
            PrescriptionDb.ProcessEntry("調劑耗用", "PreMasId", Id, (double)bucklevalue);
        }
        public void ProcessVipCopaymentCashFlow(string name)//計算VIP部分金流
       {
            PrescriptionDb.ProcessCashFlow(name, "PreMasId", Id, 0);
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
        public void UpdateCooperativePrescriptionIsRead() {
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(SourceId);
        }
        public void UpdateCooperativePrescriptionStatus() {
            if(!string.IsNullOrEmpty(SourceId))
            PrescriptionDb.UpdateCooperativePrescriptionStatus(SourceId);
        }
        public void InsertCooperAdjust() {
            Treatment.Institution.UpdateUsedTime();
            PrescriptionDb.InsertCooperAdjust(this, SetPrescriptionDetail(), Remark.Substring(0, 16));
        }
        public void Delete()
        {
            switch (Source)
            {
                default:
                    PrescriptionDb.DeletePrescription(Id);
                    break;
                case PrescriptionSource.ChronicReserve:
                    PrescriptionDb.DeleteReserve(SourceId);
                    break;
            }
            if (!PrescriptionStatus.IsAdjust) return;

            if (!Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID)) {
                decimal entryvalue = 0;
                foreach (Medicine m in Medicines)
                {
                    if (m.IsBuckle && m.BuckleAmount > 0)
                        entryvalue += PrescriptionDb.ReturnInventory(m.ID, (double)m.BuckleAmount, "刪單補耗用", "PreMasId", Id.ToString()).Rows[0].Field<decimal>("returnTotalValue");
                }
                if (entryvalue != 0)
                    PrescriptionDb.ProcessEntry("刪單補耗用", "PreMasId", Id, (double)entryvalue); 
            }
           

            PrescriptionPoint.GetAmountPaySelf(Id);
            PrescriptionPoint.GetDeposit(Id);
            string copayname = "部分負擔刪除";
            string payself = "自費刪除";
            string deposit = "押金刪除";

            if (Treatment.AdjustCase.ID == "0")
                payself = "自費調劑刪除";

            if (Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
            {
                Medicines.Clear();
                PrescriptionDb.InsertCooperAdjust(this, SetPrescriptionDetail(), string.Empty);
                copayname = "合作" + copayname;
                payself = "合作" + payself;
                deposit = "合作" + deposit;
            } 

            if(PrescriptionPoint.CopaymentPoint != 0)
                PrescriptionDb.ProcessCashFlow(copayname, "PreMasId", Id, PrescriptionPoint.CopaymentPoint * -1); 
            if(PrescriptionPoint.AmountSelfPay != 0)
                PrescriptionDb.ProcessCashFlow(payself, "PreMasId", Id, PrescriptionPoint.AmountSelfPay * -1);  
            if (PrescriptionPoint.Deposit != 0)
                PrescriptionDb.ProcessCashFlow(deposit, "PreMasId", Id, PrescriptionPoint.Deposit * -1);
        }
        #region DeclareFunctions
        public string CheckPrescriptionRule(bool noCard)//檢查健保邏輯
        {
            return CheckMedicines() + Treatment.Check(noCard) + Patient.CheckBasicData();
        }
        public string CheckPrescribeRule()
        {
            return CheckMedicines() + Treatment.CheckPrescribe() + Patient.CheckBasicData();
        }
        public string CheckMedicines()
        {
            foreach (var med in Medicines)
            {
                if (!string.IsNullOrEmpty(med.UsageName) && med.Usage is null)
                    med.Usage = VM.GetUsage(med.UsageName);
                if (!string.IsNullOrEmpty(med.PositionID) && med.Position is null)
                    med.Position = VM.GetPosition(med.PositionID);
            }
            if (!Medicines.Any())
                return StringRes.MedicineEmpty;
            if (Medicines.Count(m => m.Amount == 0) == 0)
                return string.Empty;
            return Medicines.Where(m => (m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial) && m.Amount == 0).Aggregate(string.Empty, (current, m) => current + ("藥品:" + m.FullName + "總量不可為0\r\n"));
        }
        public void CountPrescriptionPoint(bool countSelfPay)
        {
            if (!Treatment.AdjustCase.ID.Equals("0"))
            {
                PrescriptionPoint.MedicinePoint = Medicines.Count(m => m.Amount > 0) <= 0 ? 0 : Medicines.CountMedicinePoint();
                if (Treatment.AdjustCase.ID.Equals("2") || (Treatment.ChronicSeq != null && Treatment.ChronicSeq > 0) ||
                    (Treatment.ChronicTotal != null && Treatment.ChronicTotal > 0))
                {
                    Treatment.Copayment = VM.GetCopayment("I22");
                }
                if (!CheckFreeCopayment())
                {
                    if (PrescriptionPoint.MedicinePoint <= 100)
                        Treatment.Copayment = VM.GetCopayment("I21");
                    else
                    {
                        Treatment.Copayment = VM.GetCopayment("I20");
                    }
                }
                else
                {
                    if (Treatment.Copayment != null)
                    {
                        if (Treatment.Copayment.Id.Equals("I21") && PrescriptionPoint.MedicinePoint > 100)
                            Treatment.Copayment = VM.GetCopayment("I20");
                    }
                }
                if (Treatment.Copayment != null && !CheckFreeCopayment())
                    PrescriptionPoint.CopaymentPoint = CountCopaymentPoint();
                else
                {
                    PrescriptionPoint.CopaymentPoint = 0;
                }
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
            if(countSelfPay)
                PrescriptionPoint.AmountSelfPay = Medicines.CountSelfPay();
            PrescriptionPoint.AmountsPay = PrescriptionPoint.CopaymentPoint + PrescriptionPoint.AmountSelfPay;
            PrescriptionPoint.ActualReceive = PrescriptionPoint.AmountsPay;
        }

        public void CountMedicineDays()
        {
            if (Medicines.Count(m => m is MedicineNHI && !m.PaySelf && m.Days != null) > 0)
                MedicineDays = (int)Medicines.Where(m => m is MedicineNHI && !m.PaySelf).Max(m => m.Days);//計算最大給藥日份
        }

        private void CreateDeclareFileContent(List<Pdata> details)//產生申報檔內容
        {
            var medDeclare = details.Where(p => !p.P1.Equals("0")).ToList();
            var d = new Ddata(this, medDeclare);
            DeclareContent = d.SerializeObjectToXDocument();
            d.Dbody.Pdata = details;
        }
        #endregion
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
                    var parameters = CreateSingleMedBagParameter(m);
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
                var parameters = CreateMultiMedBagParameter();
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
                var adjustDate =
                    DateTimeExtensions.NullableDateToTWCalender(Treatment.AdjustDate, true);
                var cusGender = Patient.CheckGender();
                int copaymentPoint = PrescriptionPoint.CopaymentPoint;
                int actualReceive = PrescriptionPoint.ActualReceive;
                if (PrescriptionStatus.IsCooperativeVIP)
                {
                    copaymentPoint = 0;
                    actualReceive = PrescriptionPoint.ActualReceive - PrescriptionPoint.CopaymentPoint;
                }

                if (Treatment.AdjustCase.ID.Equals("0"))
                {
                    var birth = DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true);
                    string patientName;
                    if (string.IsNullOrEmpty(Patient.Name) || Patient.Name.Equals("匿名"))
                    {
                        patientName = " ";
                        birth = "  /  /  ";
                    }
                    else
                        patientName = Patient.Name;
                    var parameters = new List<ReportParameter>
                    {
                        new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                        new ReportParameter("PatientName", patientName),
                        new ReportParameter("Gender", cusGender),
                        new ReportParameter("Birthday",string.IsNullOrEmpty(birth)?"  /  /  ":birth),
                        new ReportParameter("AdjustDate", adjustDate),
                        new ReportParameter("Hospital", Treatment.Institution.Name),
                        new ReportParameter("Doctor", " "), //病歷號
                        new ReportParameter("MedicalNumber"," "),
                        new ReportParameter("MedicineCost", PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("MedicalServiceCost", (PrescriptionPoint.AmountsPay - PrescriptionPoint.AmountSelfPay).ToString()),
                        new ReportParameter("TotalMedicalCost","0"),
                        new ReportParameter("CopaymentCost", "0"),
                        new ReportParameter("HcPay", "0"),
                        new ReportParameter("SelfCost", PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ActualReceive", PrescriptionPoint.ActualReceive.ToString()),
                        new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(PrescriptionPoint.ActualReceive))
                    };
                    rptViewer.LocalReport.SetParameters(parameters);
                }
                else
                {
                    var birth = DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true);
                    string patientName;
                    if (string.IsNullOrEmpty(Patient.Name) || Patient.Name.Equals("匿名"))
                        patientName = " ";
                    else
                        patientName = Patient.Name;
                    var parameters = new List<ReportParameter>
                    {
                        new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                        new ReportParameter("PatientName", patientName),
                        new ReportParameter("Gender", cusGender),
                        new ReportParameter("Birthday",string.IsNullOrEmpty(birth)?"  /  /  ":birth),
                        new ReportParameter("AdjustDate", adjustDate),
                        new ReportParameter("Hospital", string.IsNullOrEmpty(Treatment.Institution.Name)?" ":Treatment.Institution.Name),
                        new ReportParameter("Doctor", " "), //病歷號
                        new ReportParameter("MedicalNumber", string.IsNullOrEmpty(Treatment.TempMedicalNumber)?" ":Treatment.TempMedicalNumber),
                        new ReportParameter("MedicineCost", PrescriptionPoint.MedicinePoint.ToString()),
                        new ReportParameter("MedicalServiceCost", PrescriptionPoint.MedicalServicePoint.ToString()),
                        new ReportParameter("TotalMedicalCost",PrescriptionPoint.TotalPoint.ToString()),
                        new ReportParameter("CopaymentCost", copaymentPoint.ToString()),
                        new ReportParameter("HcPay", PrescriptionPoint.ApplyPoint.ToString()),
                        new ReportParameter("SelfCost", PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ActualReceive", actualReceive.ToString()),
                        new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(actualReceive))
                    };
                    rptViewer.LocalReport.SetParameters(parameters);
                }
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.Refresh();
                MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintReceipt(rptViewer);
                }));
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage("列印報表發生問題，請重試",MessageType.WARNING);
            }
        }
        public void PrintDepositSheet()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.ReportPath = @"RDLC\DepositSheet.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var adjustDate =
                DateTimeExtensions.NullableDateToTWCalender(Treatment.AdjustDate, true);
            var dateString = DateTimeExtensions.ConvertDateStringSplitToChinese(adjustDate);
            var printTime = adjustDate + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";

            var parameters = new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", Patient.Name),
                new ReportParameter("AdjustDate", dateString),
                new ReportParameter("Deposit", PrescriptionPoint.Deposit.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(PrescriptionPoint.Deposit)),
                new ReportParameter("PrintTime", printTime)
            };
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
            MainWindow.Instance.Dispatcher.Invoke((Action)(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintDeposit(rptViewer);
            }));
        }
        private IEnumerable<ReportParameter> CreateSingleMedBagParameter(MedBagMedicine m)
        {
            var treatmentDate = DateTimeExtensions.NullableDateToTWCalender(Treatment.AdjustDate, true);
            var treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                   treatmentDate.Split('/')[2] + "日";
            var cusGender = Patient.CheckGender();
            var patientTel = string.IsNullOrEmpty(Patient.Tel) ? Patient.ContactNote : Patient.Tel;
            return  new List<ReportParameter>
                    {
                        new ReportParameter("PharmacyName_Id",
                            VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                        new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson",VM.CurrentPharmacy.GetPharmacist().Name),
                        new ReportParameter("PatientName", Patient.Name),
                        new ReportParameter("PatientGender_Birthday",(cusGender) + "/" + DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true)),
                        new ReportParameter("TreatmentDate", treatmentDateChi),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",Treatment.Division is null ?string.Empty:Treatment.Division.Name),
                        new ReportParameter("Hospital", Treatment.Institution.Name),
                        new ReportParameter("PaySelf", PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ServicePoint", PrescriptionPoint.MedicalServicePoint.ToString()),
                        new ReportParameter("TotalPoint", PrescriptionPoint.TotalPoint.ToString()),
                        new ReportParameter("CopaymentPoint", PrescriptionPoint.CopaymentPoint.ToString()),
                        new ReportParameter("HcPoint", PrescriptionPoint.ApplyPoint.ToString()),
                        new ReportParameter("MedicinePoint", PrescriptionPoint.MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                        new ReportParameter("MedicineId", m.Id),
                        new ReportParameter("MedicineName", m.Name),
                        new ReportParameter("MedicineChineseName", m.ChiName),
                        new ReportParameter("Ingredient", m.Ingredient),
                        new ReportParameter("Indication", m.Indication),
                        new ReportParameter("SideEffect", m.SideEffect),
                        new ReportParameter("Note", m.Note),
                        new ReportParameter("Usage", m.Usage),
                        new ReportParameter("MedicineDay", m.MedicineDays),
                        new ReportParameter("Amount", m.Total),
                        new ReportParameter("Form", m.Form),
                        new ReportParameter("PatientTel", patientTel)
                    };
        }
        private IEnumerable<ReportParameter> CreateMultiMedBagParameter()
        {
            var treatmentDate =
                DateTimeExtensions.NullableDateToTWCalender(Treatment.AdjustDate, true);
            var treatmentDateChi = string.Empty;
            if(!string.IsNullOrEmpty(treatmentDate))
                treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                      treatmentDate.Split('/')[2] + "日";
            var cusGender = Patient.CheckGender();
            var patientTel = string.IsNullOrEmpty(Patient.Tel) ? Patient.ContactNote : Patient.Tel;
            return new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id",
                    VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson", VM.CurrentPharmacy.GetPharmacist().Name),
                new ReportParameter("PatientName", Patient.Name),
                new ReportParameter("PatientGender_Birthday",cusGender + "/" +DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true)),
                new ReportParameter("TreatmentDate", treatmentDateChi),
                new ReportParameter("Hospital", Treatment.Institution.Name),
                new ReportParameter("PaySelf", PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ServicePoint", PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint", PrescriptionPoint.CopaymentPoint.ToString()),
                new ReportParameter("HcPoint", PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("MedicinePoint", PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("Division", Treatment.Division is null ?string.Empty:Treatment.Division.Name),
                new ReportParameter("PatientTel", patientTel)
            };
        }
        #endregion
        public bool GetCard()
        {
            var success = Card.GetBasicData();
            if (success)
            {
                var cus = new Customer(Card);
                Patient = cus;
                var customers = Patient.Check();
                if(customers.Count == 0)
                    Patient.InsertData();
                else
                    Patient = customers[0];
                PrescriptionStatus.IsGetCard = true;
            }
            return success;
        }

        public void Update()
        {
            CountMedicineDays();
            CheckMedicalServiceData();//確認藥事服務資料
            var details = SetPrescriptionDetail();//產生藥品資料
            if (Treatment.AdjustCase.ID.Equals("0"))
            {
                PrescriptionPoint.SpecialMaterialPoint = 0;
                PrescriptionPoint.TotalPoint = 0;
                PrescriptionPoint.ApplyPoint = 0;
            }
            else
            {
                PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
                PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                               PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
                PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
                CreateDeclareFileContent(details);//產生申報資料
            }
            Treatment.Institution.UpdateUsedTime();
            switch (Source)
            {
                default:
                    PrescriptionDb.UpdatePrescription(this, details);
                    break;
                case PrescriptionSource.ChronicReserve:
                    PrescriptionDb.UpdateReserve(this, details);
                    break;
            }
        }
        public void AdjustMedicines(Prescription originPrescription)
        {
            if (!PrescriptionStatus.IsAdjust) return;
            foreach (Medicine m in Medicines) {
                if (m.UsageName == null)
                    m.UsageName = string.Empty; 
            }
            foreach (Medicine m in originPrescription.Medicines)
            {
                if (m.UsageName == null)
                    m.UsageName = string.Empty;
            }


            Medicines compareMeds = new Medicines();
            foreach (var orm in originPrescription.Medicines) {
                if ((bool)orm.IsBuckle && !string.IsNullOrEmpty(orm.ID) && !(orm is MedicineVirtual))
                {
                    Medicine medicine = new Medicine();
                    medicine.ID = orm.ID;
                    medicine.Amount = Medicines.Count(m => m.ID == orm.ID && m.UsageName.Equals(orm.UsageName) && m.Days.Equals(orm.Days) && m.PaySelf == orm.PaySelf) > 0 
                        ? Medicines.Single(m => m.ID == orm.ID && m.UsageName.Equals(orm.UsageName) && m.Days == orm.Days && m.PaySelf == orm.PaySelf).Amount : orm.Amount;
                    medicine.BuckleAmount = Medicines.Count(m => m.ID == orm.ID && m.UsageName.Equals(orm.UsageName) && m.Days.Equals(orm.Days) && m.PaySelf == orm.PaySelf) > 0 
                        ? Medicines.Single(m => m.ID == orm.ID && m.UsageName.Equals(orm.UsageName) && m.Days == orm.Days && m.PaySelf == orm.PaySelf).BuckleAmount - orm.BuckleAmount : orm.BuckleAmount * -1;
                    compareMeds.Add(medicine);
                }
               
            }
            foreach (var nem in Medicines) {
                if ((bool)nem.IsBuckle && !string.IsNullOrEmpty(nem.ID) && !(nem is MedicineVirtual) )
                {
                    if (originPrescription.Medicines.Count(m => m.ID == nem.ID && m.UsageName.Equals(nem.UsageName) && m.Days == nem.Days && m.PaySelf == nem.PaySelf) == 0)
                    {
                        Medicine medicine = new Medicine();
                        medicine.ID = nem.ID;
                        medicine.Amount = nem.Amount;
                        medicine.BuckleAmount = nem.BuckleAmount;
                        compareMeds.Add(medicine);
                    }
                   
                }
            }
            decimal entryvalue = 0;
            foreach (var com in compareMeds) {

                if (com.BuckleAmount > 0)
                    entryvalue += PrescriptionDb.ProcessInventory(com.ID, (double)com.BuckleAmount, "處方調劑調整", "PreMasId", Id.ToString()).Rows[0].Field<decimal>("BuckleTotalValue"); 
                else if (com.BuckleAmount < 0)
                    entryvalue += PrescriptionDb.ReturnInventory(com.ID, (double)com.BuckleAmount*-1, "處方調劑調整", "PreMasId", Id.ToString()).Rows[0].Field<decimal>("returnTotalValue");
            }

            PrescriptionDb.ProcessEntry("調劑耗用修改", "PreMasId", Id, (double)entryvalue);
            PrescriptionDb.ProcessCashFlow("部分負擔修改", "PreMasId", Id, PrescriptionPoint.CopaymentPoint - originPrescription.PrescriptionPoint.CopaymentPoint);
            PrescriptionDb.ProcessCashFlow( Treatment.AdjustCase.ID == "0" ? "自費調劑修改" : "自費修改", "PreMasId", Id, PrescriptionPoint.AmountSelfPay - originPrescription.PrescriptionPoint.AmountSelfPay);
           
        }

        public object Clone()
        {
            Prescription p = new Prescription();
            p.Treatment = (Treatment.Treatment)Treatment.Clone();
            foreach (var m in Medicines)
            {
                Medicine med = new Medicine();
                med.Amount = m.Amount;
                med.ControlLevel = m.ControlLevel;
                med.CostPrice = m.CostPrice;
                med.Days = m.Days;
                med.Dosage = m.Dosage;
                med.Enable = m.Enable;
                med.Frozen = m.Frozen;
                med.Inventory = m.Inventory;
                med.IsBuckle = m.IsBuckle;
                med.IsPriceReadOnly = m.IsPriceReadOnly;
                med.IsSelected = m.IsSelected;
                med.NHIPrice = m.NHIPrice;
                med.PaySelf = m.PaySelf;
                med.PositionID = m.PositionID;
                med.UsageName = m.UsageName;
                med.Price = m.Price;
                med.TotalPrice = m.TotalPrice;
                med.Vendor = m.Vendor;
                med.ID = m.ID;
                med.EnglishName = m.EnglishName;
                med.ChineseName = m.ChineseName;
                med.IsCommon = m.IsCommon;
                med.BuckleAmount = m.BuckleAmount;
                p.Medicines.Add(med);
            }
            p.PrescriptionPoint = PrescriptionPoint.DeepCloneViaJson();
            p.PrescriptionStatus = PrescriptionStatus.DeepCloneViaJson();
            p.DeclareContent = DeclareContent;
            p.Patient = (Customer)Patient.Clone();
            p.Card = (IcCard)Card.Clone();
            p.MedicalServiceID = MedicalServiceID;
            p.DeclareFileID = DeclareFileID;
            p.MedicineDays = MedicineDays;
            p.SourceId = SourceId;
            p.Source = Source;
            p.OrderNumber = OrderNumber;
            p.Id = Id;
            if(!string.IsNullOrEmpty(Remark))
                p.Remark = Remark;
            if (!string.IsNullOrEmpty(OrderNumber))
                p.OrderNumber = OrderNumber;
            return p;
        }

        public void CheckPrescriptionVariable()
        {
            if(Treatment.AdjustCase.ID != "0" && (!string.IsNullOrEmpty(Treatment.Institution.ID) && Treatment.Institution.ID.Equals(VM.CurrentPharmacy.ID)))
                Treatment.Institution = new Institution();

            if (Treatment.ChronicSeq is null && Treatment.AdjustCase.ID.Equals("2"))
            {
                Treatment.AdjustCase = VM.GetAdjustCase("1");
            }

            if (Treatment.ChronicSeq != null && Treatment.ChronicSeq > 0)
            {
                Treatment.AdjustCase = VM.GetAdjustCase("2");
            }

            switch (Treatment.AdjustCase.ID)
            {
                case "1":
                case "3":
                    if (Treatment.Division != null && Treatment.Division.ID.Equals("40"))
                        Treatment.PrescriptionCase = VM.GetPrescriptionCases("19");
                    else
                        Treatment.PrescriptionCase = VM.GetPrescriptionCases("09");
                    if(!CheckFreeCopayment())
                        Treatment.Copayment = VM.GetCopayment("I20");
                    Treatment.OriginalMedicalNumber = null;
                    break;
                case "2":
                    Treatment.PrescriptionCase = VM.GetPrescriptionCases("04");
                    Treatment.Copayment = VM.GetCopayment("I22");
                    break;
                case "D":
                    Treatment.PrescriptionCase = new PrescriptionCase();
                    Treatment.Institution = VM.GetInstitution("N");
                    Treatment.Copayment = VM.GetCopayment("009");
                    Treatment.SpecialTreat = new SpecialTreat();
                    Treatment.TreatDate = null;
                    Treatment.OriginalMedicalNumber = null;
                    break;
            }
        }

        public void GetCompletePrescriptionData(bool updateIsRead,bool getDeposit)
        {
            MainWindow.ServerConnection.OpenConnection();
            Treatment.MainDisease.GetDataByCodeId(Treatment.MainDisease.ID);
            Treatment.SubDisease.GetDataByCodeId(Treatment.SubDisease.ID);
            AdjustMedicinesType();
            if(updateIsRead)
                UpdateCooperativePrescriptionIsRead();
            if(getDeposit)
                PrescriptionPoint.GetDeposit(Id);
            MainWindow.ServerConnection.CloseConnection();
        }

        public void AdjustCooperativeMedicines(Prescription originprescription)
        { 
            PrescriptionDb.InsertCooperAdjust(this, SetPrescriptionDetail(), string.Empty); 
            PrescriptionDb.ProcessCashFlow("合作自費修改", "PreMasId", Id, PrescriptionPoint.AmountSelfPay - originprescription.PrescriptionPoint.AmountSelfPay);
            PrescriptionDb.ProcessCashFlow("合作部分負擔修改", "PreMasId", Id, PrescriptionPoint.CopaymentPoint - originprescription.PrescriptionPoint.CopaymentPoint);
        }

        public void SetAdjustStatus(bool errorAdjust)
        {
            if (PrescriptionStatus.IsPrescribe) //處方全自費
            {
                PrescriptionStatus.SetCooperativePrescribeStatus();
            }
            else
            {
                if(errorAdjust)
                    PrescriptionStatus.SetErrorAdjustStatus();
                else
                    PrescriptionStatus.SetNormalAdjustStatus();
            }
        }
        //檢查是否為全自費處方
        public void CheckIsPrescribe()
        {
            PrescriptionStatus.IsPrescribe =
                Medicines.Count(m => (m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial) && !m.PaySelf) == 0;
        }

        public void NormalAdjust(bool noCard)
        {
            if (Id == 0)
                Id = InsertPrescription();
            else
                Update();
            if(Treatment.ChronicSeq != null && Treatment.ChronicTotal != null) //如果慢箋直接調劑 做預約慢箋
                AdjustPredictReserve();
            var bucklevalue = ProcessInventory("處方調劑", "PreMasId", Id.ToString());
            ProcessMedicineUseEntry(bucklevalue);
            ProcessCopaymentCashFlow("部分負擔");
            ProcessSelfPayCashFlow("自費");
            if (noCard)
                ProcessDepositCashFlow("押金");
        }

        public void CooperativeAdjust(bool noCard)
        {
            Id = InsertPrescription();
            InsertCooperAdjust();
            if (PrescriptionStatus.IsCooperativeVIP)
                ProcessVipCopaymentCashFlow("合作VIP部分負擔");
            else
                ProcessCopaymentCashFlow("合作部分負擔");
            ProcessSelfPayCashFlow("合作自費");
            if (noCard)
                ProcessDepositCashFlow("合作押金");
            UpdateCooperativePrescriptionStatus();
        }

        public void ChronicAdjust(bool noCard)
        {
            Id = InsertPrescription();
            AdjustPredictReserve();
            var bucklevalue = ProcessInventory("處方調劑", "PreMasId", Id.ToString());
            ProcessMedicineUseEntry(bucklevalue);
            ProcessCopaymentCashFlow("部分負擔");
            ProcessSelfPayCashFlow("自費");
            if (noCard)
                ProcessDepositCashFlow("押金");
        }

        public void NormalRegister()
        {
            if (Id == 0)
            {
                Id = InsertPrescription();
                PredictReserve();
            }
            else
                Update();
        }

        public void ChronicRegister()
        {
            Id = InsertPrescription();
            AdjustPredictReserve();
        }

        public void Prescribe()
        {
            Id = InsertPrescription();
            var bucklevalue = ProcessInventory("自費調劑", "PreMasId", Id.ToString());
            ProcessMedicineUseEntry(bucklevalue);
            ProcessSelfPayCashFlow("自費調劑");
        }

        public void CheckIsCooperative()
        {
            if(Treatment.Institution != null && !string.IsNullOrEmpty(Treatment.Institution.ID))
                PrescriptionStatus.IsCooperative = Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID);
        }

        public string CheckSameMedicine()
        {
            var sameList = new List<string>();
            var sameMed = string.Empty;
            foreach (var m in Medicines.Where(m => !(m is MedicineVirtual)))
            {
                var compareList = new List<Medicine>(Medicines.Where(med => !(med is MedicineVirtual)));
                compareList.Remove(m);
                if (compareList.Count(med => med.ID.Equals(m.ID) && (!string.IsNullOrEmpty(m.UsageName) && !string.IsNullOrEmpty(med.UsageName) && med.UsageName.Equals(m.UsageName)) && (m.Days!=null && med.Days != null && med.Days.Equals(m.Days)) ) > 0)
                {
                    sameList.Add("藥品:" + m.ID + "重複。\n");
                }
            }
            return sameList.Count <= 0 ? sameMed : sameList.Distinct().Aggregate(sameMed, (current, s) => current + s);
        }

    }
}
