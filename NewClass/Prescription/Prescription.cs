﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Customer;
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
        }

        public Prescription(DataRow r)
        {
            Id = (int)r[""];
        }
        public Prescription(CooperativePrescription c) { 
            Source = PrescriptionSource.Cooperative;
            SourceId = c.CooperativePrescriptionId;
            Remark = c.DeclareXmlDocument.Prescription.Customer.Remark;
            Treatment = new Treatment.Treatment(c);

            Patient = new Customer();
            Patient.IDNumber = c.DeclareXmlDocument.Prescription.Customer.IdNumber;
            Patient.Name = c.DeclareXmlDocument.Prescription.Customer.Name;
            Patient.Birthday = Convert.ToDateTime(c.DeclareXmlDocument.Prescription.Customer.Birth);
            Patient.Tel = c.DeclareXmlDocument.Prescription.Customer.Phone;
            Patient.CheckCustomer(Patient);
            Card = new IcCard(); 
            IsSendToSingde = false;
            IsAdjust = false;
            IsRead = c.IsRead == "Y" ? true : false;
        }
        public int Id { get; }
        public Customer Patient { get; set; }//病患
        public IcCard Card { get; set; }
        private Treatment.Treatment treatment;
        public Treatment.Treatment Treatment { get; set; }//處方資料
        public PrescriptionSource Source { get; set; }
        public string SourceId { get; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public string Remark { get; }//回傳合作診所單號
        public bool IsSendToSingde { get; set; }//是否傳送藥健康
        public bool IsAdjust { get; set; }//是否調劑.扣庫
        public bool IsRead { get; set; }//是否已讀

        public void PrintMedBag()
        {

        }
    }
}
