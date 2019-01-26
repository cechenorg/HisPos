using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription.DeclareFile;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.MedBag;
using His_Pos.Service;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using Ddata = His_Pos.NewClass.Prescription.DeclareFile.Ddata;
using MedicineDb = His_Pos.NewClass.Product.Medicine.MedicineDb;
using Pdata = His_Pos.NewClass.Prescription.DeclareFile.Pdata;
using StringRes = His_Pos.Properties.Resources;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription
{
    public class Prescription : ObservableObject,ICloneable
    {
        public Prescription()
        {
            Patient = new Customer();
            Card = new IcCard();
            Treatment = new Treatment.Treatment();
            Medicines = new Medicines();
            PrescriptionSign = new List<string>();
        }

        public Prescription(DataRow r,PrescriptionSource prescriptionSource)
        { 
            Patient = new Customer();
            Patient.ID = r.Field<int>("CustomerID");
            Patient.IDNumber = r.Field<string>("CustomerIDNumber");
            Patient.Name = r.Field<string>("CustomerName");  
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
        public string SourceId { get; set; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public string Remark { get; }//回傳合作診所單號 
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceID { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public Medicines Medicines { get; set; } = new Medicines();//調劑用藥 
        public void InitialCurrentPrescription()
        {
            Treatment.Initial();
            PrescriptionStatus.Init();
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

        public bool CheckFreeCopayment()
        {
            if (Treatment.AdjustCase.Id.Equals("2") || Treatment.AdjustCase.Id.Equals("4"))
                return true;
            switch (Treatment.Copayment.Id)
            {
                case "009"://其他免負擔
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
        public int InsertReserve() {
            if (Medicines.Count(m => m is MedicineNHI && !m.PaySelf) > 0)
                MedicineDays = (int)Medicines.Where(m => m is MedicineNHI && !m.PaySelf).Max(m => m.Days);//計算最大給藥日份

            CheckMedicalServiceData();//確認藥事服務資料
            var details = SetPrescriptionDetail();//產生藥品資料
            PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
            PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                           PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
            PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
            CreateDeclareFileContent(details);//產生申報資料
            return PrescriptionDb.InsertReserve(this, details);
        }
        public void UpdateReserve() {
            if (Medicines.Count(m => m is MedicineNHI && !m.PaySelf) > 0)
                MedicineDays = (int)Medicines.Where(m => m is MedicineNHI && !m.PaySelf).Max(m => m.Days);//計算最大給藥日份

            CheckMedicalServiceData();//確認藥事服務資料
            var details = SetPrescriptionDetail();//產生藥品資料
            PrescriptionPoint.SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
            PrescriptionPoint.TotalPoint = PrescriptionPoint.MedicinePoint + PrescriptionPoint.MedicalServicePoint +
                                           PrescriptionPoint.SpecialMaterialPoint + PrescriptionPoint.CopaymentPoint;
            PrescriptionPoint.ApplyPoint = PrescriptionPoint.TotalPoint - PrescriptionPoint.CopaymentPoint;//計算申請點數
            CreateDeclareFileContent(details);//產生申報資料
            PrescriptionDb.UpdateReserve(this, details);
        }
        public List<Pdata> SetPrescriptionDetail()
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
        public List<Pdata> SetImportDeclareXmlDetail() {
            var details = new List<Pdata>();
            //var serialNumber = 1;
            //foreach (var med in Medicines)
            //{
            //    Pdata pdata = new Pdata(med, serialNumber.ToString());
            //    if(med.)
            //    details.Add();
            //    serialNumber++;
            //} 
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
            Treatment.AdjustCase = VM.GetAdjustCase("3");
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
                        Medicines[selectedMedicinesIndex].CheckPaySelf(Treatment.AdjustCase.Id);
                        break;
                    case 1:
                        Medicines[selectedMedicinesIndex] = new MedicineNHI(r);
                        Medicines[selectedMedicinesIndex].CheckPaySelf(Treatment.AdjustCase.Id);
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
        public void AdjustPredictResere() {
            PrescriptionDb.AdjustPredictResere(Id.ToString());
        }
        public static int GetPrescriptionId() {
            return PrescriptionDb.GetPrescriptionId().Rows[0].Field<int>("MaxPreId"); 
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
                VM.CheckContainsPosition(temp.PositionName);
                temp.Amount = Medicines[medCount].Amount;
                temp.Dosage = Medicines[medCount].Dosage;
                temp.Days = Medicines[medCount].Days;
                temp.PaySelf = Medicines[medCount].PaySelf;
                temp.TotalPrice = Medicines[medCount].TotalPrice;
                Medicines[medCount] = temp; 
            }
            Medicines.Add(new Medicine());
        }
        public void ConvertNHIandOTCPrescriptionMedicines()
        {
            Medicine temp = new Medicine();
            for (int medCount = 0; medCount < Medicines.Count; medCount++)
            {
                var table = MedicineDb.GetMedicinesBySearchId(Medicines[medCount].ID);
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
                temp.UsageName = Medicines[medCount].UsageName;
                temp.PositionName = Medicines[medCount].PositionName;
                temp.Days = Medicines[medCount].Days;
                temp.PaySelf = Medicines[medCount].PaySelf;
                temp.Amount = Medicines[medCount].Amount; 
                Medicines[medCount] = temp; 
            }
        }
        public int UpdatePrescriptionCount()//計算處方張數
        {
            return PrescriptionDb.GetPrescriptionCountByID(Treatment.Pharmacist.IdNumber).Rows[0].Field<int>("PrescriptionCount");
        }

        public void ProcessInventory(string type,string source,string sourceId)//扣庫
        { 
            foreach (var m in Medicines)
            { 
                if(!string.IsNullOrEmpty(m.ID))
                PrescriptionDb.ProcessInventory(m.ID, m.Amount, type, source, sourceId);
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
        public void UpdateCooperativePrescriptionIsRead() {
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(SourceId);
        }
        public void UpdateCooperativePrescriptionStatus() {
            PrescriptionDb.UpdateCooperativePrescriptionStatus(SourceId);
        }
        public void InsertCooperAdjust() {
            PrescriptionDb.InsertCooperAdjust(this, SetPrescriptionDetail(), Remark.Substring(0, 16));
        }
        #region DeclareFunctions
        public string CheckPrescriptionRule()//檢查健保邏輯
        {
            return CheckMedicines() + Treatment.Check() + Patient.CheckBasicData();
        }
        public string CheckMedicines()
        {
            var medList = Medicines.Where(m => m is MedicineNHI || m is MedicineOTC).ToList();
            foreach (var med in medList)
            {
                if (!string.IsNullOrEmpty(med.UsageName) && med.Usage is null)
                {
                    med.Usage = VM.GetUsage(med.UsageName);
                }
                if (!string.IsNullOrEmpty(med.PositionName) && med.Position is null)
                {
                    VM.CheckContainsPosition(med.PositionName);
                    med.Position = VM.GetPosition(med.PositionName);
                }
            }
            if (!medList.Any())
            {
                return StringRes.MedicineEmpty;
            }
            if (medList.Count(m => m.Amount == 0) == 0)
            {
                return string.Empty;
            }
            return medList.Where(m => m.Amount == 0).Aggregate(string.Empty, (current, m) => current + ("藥品:" + m.FullName + "總量不可為0\r\n"));
        }
        public void CountPrescriptionPoint()
        {
            PrescriptionPoint.MedicinePoint = Medicines.Count(m => (m is MedicineNHI || m is MedicineOTC) && m.Amount > 0) <= 0 ? 0 : Medicines.CountMedicinePoint();
            if (Treatment.AdjustCase.Id.Equals("2") || (Treatment.ChronicSeq != null && Treatment.ChronicSeq > 0) ||
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
                if (Treatment.Copayment.Id.Equals("I21") && PrescriptionPoint.MedicinePoint > 100)
                {
                    Treatment.Copayment = VM.GetCopayment("I20");
                }
            }
            if(!Treatment.Copayment.Id.Equals("I21"))
                PrescriptionPoint.CopaymentPoint = CountCopaymentPoint();
            else
            {
                PrescriptionPoint.CopaymentPoint = 0;
            }
            PrescriptionPoint.AmountSelfPay = Medicines.CountSelfPay();
            PrescriptionPoint.AmountsPay = PrescriptionPoint.CopaymentPoint + PrescriptionPoint.AmountSelfPay;
            PrescriptionPoint.ActualReceive = PrescriptionPoint.AmountsPay;
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
        public void PrintMedBag(bool singleMode,bool receiptPrint)
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
                    if(receiptPrint)
                        ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer,this);
                    else
                    {
                        ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                    }
                }
            }
            else
            {
                foreach (var m in medBagMedicines.GroupBy(info => info.Usage)
                    .Select(group => new { UsageName = group.Key, count = group.Count() })
                    .OrderBy(x => x.UsageName))
                {
                    var i = 1;
                    foreach (var med in medBagMedicines)
                    {
                        if (!med.Usage.Equals(m.UsageName)) continue;
                        med.MedNo = i.ToString();
                        i++;
                    }
                }
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
                if (receiptPrint)
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer, this);
                else
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintMedBag(rptViewer);
                }
            }
        }
        public void PrintReceipt()
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var adjustDate =
                DateTimeExtensions.NullableDateToTWCalender(Treatment.AdjustDate, true);
            var doctor = string.Empty;
            var cusGender = string.IsNullOrEmpty(Patient.Gender)?Patient.CheckGender(): Patient.Gender;
            var parameters = new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", Patient.Name),
                new ReportParameter("Gender", cusGender),
                new ReportParameter("Birthday",
                    DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true)),
                new ReportParameter("AdjustDate", adjustDate),
                new ReportParameter("Hospital", Treatment.Institution.Name),
                new ReportParameter("Doctor", doctor), //病歷號
                new ReportParameter("MedicalNumber", Treatment.TempMedicalNumber),
                new ReportParameter("MedicineCost", PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("MedicalServiceCost", PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalMedicalCost",PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentCost", PrescriptionPoint.CopaymentPoint.ToString()),
                new ReportParameter("HcPay", PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("SelfCost", PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ActualReceive", PrescriptionPoint.ActualReceive.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(PrescriptionPoint.ActualReceive))
            };
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
            ((VM)MainWindow.Instance.DataContext).StartPrintReceipt(rptViewer);
        }
        private IEnumerable<ReportParameter> CreateSingleMedBagParameter(MedBagMedicine m)
        {
            var treatmentDate = DateTimeExtensions.NullableDateToTWCalender(Treatment.TreatDate, true);
            var treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                   treatmentDate.Split('/')[2] + "日";
            return  new List<ReportParameter>
                    {
                        new ReportParameter("PharmacyName_Id",
                            VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.Id + ")"),
                        new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson", Treatment.Pharmacist.Name),
                        new ReportParameter("PatientName", Patient.Name),
                        new ReportParameter("PatientGender_Birthday",(Patient.Gender) + "/" + DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true)),
                        new ReportParameter("TreatmentDate", treatmentDateChi),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",Treatment.Division.Name),
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
                        new ReportParameter("Form", m.Form)
                    };
        }
        private IEnumerable<ReportParameter> CreateMultiMedBagParameter()
        {
            var treatmentDate =
                DateTimeExtensions.NullableDateToTWCalender(Treatment.TreatDate, true);
            var treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                      treatmentDate.Split('/')[2] + "日";
            return new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id",
                    VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.Id + ")"),
                new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson", Treatment.Pharmacist.Name),
                new ReportParameter("PatientName", Patient.Name),
                new ReportParameter("PatientGender_Birthday",Patient.Gender + "/" +DateTimeExtensions.NullableDateToTWCalender(Patient.Birthday, true)),
                new ReportParameter("TreatmentDate", treatmentDateChi),
                new ReportParameter("Hospital", Treatment.Institution.Name),
                new ReportParameter("PaySelf", PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ServicePoint", PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint", PrescriptionPoint.CopaymentPoint.ToString()),
                new ReportParameter("HcPoint", PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("MedicinePoint", PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("Division", Treatment.Division.Name)
            };
        }
        #endregion
        public bool GetCard()
        {
            var success = Card.GetBasicData();
            if (success)
            {
                var cus = new Customer(Card);
                MainWindow.ServerConnection.OpenConnection();
                cus = cus.Check();
                MainWindow.ServerConnection.CloseConnection();
                if (Patient is null || string.IsNullOrEmpty(Patient.IDNumber) && string.IsNullOrEmpty(Patient.Name))
                {
                    Patient = cus;
                    PrescriptionStatus.IsReadCard = true;
                }
                else
                {
                    var replace = true;
                    if (!string.IsNullOrEmpty(Patient.IDNumber))
                    {
                        if (!Card.PatientBasicData.IDNumber.Equals(Patient.IDNumber))
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                ConfirmWindow confirm = new ConfirmWindow(StringRes.卡片資料不符, StringRes.資料不符);
                                if (!(bool)confirm.DialogResult)
                                {
                                    PrescriptionStatus.IsReadCard = false;
                                    replace = false;
                                }
                            });
                            if (!replace) return false;
                        }
                    }
                    if (!string.IsNullOrEmpty(Patient.Name))
                    {
                        if (!Card.PatientBasicData.Name.Equals(Patient.Name) && !replace)
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                ConfirmWindow confirm = new ConfirmWindow(StringRes.卡片資料不符, StringRes.資料不符);
                                if (!(bool)confirm.DialogResult)
                                {
                                    PrescriptionStatus.IsReadCard = false;
                                    replace = false;
                                }
                            });
                            if (!replace) return false;
                        }
                    }
                    if (Patient.Birthday != null)
                    {
                        if (DateTime.Compare(Card.PatientBasicData.Birthday, (DateTime)Patient.Birthday) != 0 && !replace)
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                ConfirmWindow confirm = new ConfirmWindow(StringRes.卡片資料不符, StringRes.資料不符);
                                if (!(bool)confirm.DialogResult)
                                {
                                    PrescriptionStatus.IsReadCard = false;
                                    replace = false;
                                }
                            });
                            if (!replace) return false;
                        }
                    }
                    PrescriptionStatus.IsReadCard = true;
                }
                Patient = cus;
            }
            else
            {
                PrescriptionStatus.IsReadCard = true;
            }
            return success;
        }

        public void Update()
        {

        }
        public void AdjustMedicines(Medicines originMedicines)
        {

        }

        public object Clone()
        {
            Prescription p = new Prescription();
            p.Treatment = (Treatment.Treatment)Treatment.Clone();
            foreach (var m in Medicines)
            {
                p.Medicines.Add(m);
            }
            p.PrescriptionPoint = PrescriptionPoint.DeepCloneViaJson();
            p.PrescriptionStatus = PrescriptionStatus.DeepCloneViaJson();
            p.DeclareContent = DeclareContent;
            p.Patient = Patient.DeepCloneViaJson();
            Card = new IcCard();
            p.MedicalServiceID = MedicalServiceID;
            p.DeclareFileID = DeclareFileID;
            p.MedicineDays = MedicineDays;
            p.SourceId = SourceId;
            p.OrderNumber = OrderNumber;
            p.Id = Id;
            return p;
        }
    }
}
