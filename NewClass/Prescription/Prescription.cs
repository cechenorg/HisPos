﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;
using JetBrains.Annotations;
using Customer = His_Pos.NewClass.Person.Customer.Customer;

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
            Source = PrescriptionSource.Cooperative;
            SourceId = c.CooperativePrescriptionId;
            Remark = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Remark;
            MedicineDays = string.IsNullOrEmpty(c.DeclareXmlDocument.Prescription.MedicineOrder.Days) ? 0 : Convert.ToInt32(c.DeclareXmlDocument.Prescription.MedicineOrder.Days); 
            Treatment = new Treatment.Treatment(c); 
            Patient = new Customer();
            Patient.IDNumber = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.IdNumber;
            Patient.Name = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Name;
            string birthyear = (Convert.ToInt32(c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Birth.Substring(0, 3)) + 1911).ToString();
            string birthmonth = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Birth.Substring(3, 2);
            string birthday = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Birth.Substring(5, 2);
            Patient.Birthday = Convert.ToDateTime(birthyear + "/" + birthmonth + "/" + birthday);
            Patient.Tel = c.DeclareXmlDocument.Prescription.CustomerProfile.Customer.Phone;
            Card = new IcCard(); 
            PrescriptionStatus.IsSendToSingde = false;
            PrescriptionStatus.IsAdjust = false;
            PrescriptionStatus.IsRead = c.IsRead == "Y" ? true : false;
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
        private Treatment.Treatment treatment;
        public Treatment.Treatment Treatment { get; set; }//處方資料
        public PrescriptionSource Source { get; set; }
        public string SourceId { get; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public string Remark { get; }//回傳合作診所單號 
        public int MedicineDays { get; set; } //給藥日份
        public int MedicalServiceID { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區 = 
        public Medicines Medicines { get; set; } = new Medicines(); //調劑用藥
        #region Function
        public int InsertPresription()
        {
            //if (MedicineDays <= 3)
            //{
            //    CheckIfSimpleFormDeclare();
            //}
            return PrescriptionDb.InsertPrescription(this);
        }

        private void CheckIfSimpleFormDeclare()
        {
            double medicinePoint = Medicines.Where(m => !m.PaySelf).Sum(med => med.NHIPrice * med.Amount);
            var medFormCount = CountOralLiquidAgent();//口服液劑(原瓶包裝)數量
            var dailyPrice = CountDayPayAmount(Patient.CountAge(), medFormCount);//計算日劑藥費金額
            if (dailyPrice > medicinePoint)
            {
                Treatment.AdjustCase = ViewModelMainWindow.AdjustCases.SingleOrDefault(a=>a.Id.Equals("3"));
                PrescriptionPoint.MedicinePoint = dailyPrice * MedicineDays;
                
            }
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
            DataTable table = MedicineDb.GetMedicinesBySearchId(proId);
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

        public int UpdatePrescriptionCount()//計算處方張數
        {
            return PrescriptionDb.GetPrescriptionCountByID(Treatment.Pharmacist.IdNumber).Rows[0].Field<int>("PrescriptionCount");
        }

        public string CheckPrescriptionRule()//檢查健保邏輯
        {
            return string.Empty;
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
    }
}
