﻿using System;
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

        }

        public Prescription(DataRow r)
        {
            Treatment = new Treatment(r);
            PrescriptionPoint = new PrescriptionPoint(r);
            PrescriptionStatus = new PrescriptionStatus(r);
        }
        #region Properties
        public IcCard Card { get; set; }
        public Treatment Treatment { get; set; }//處方資料
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

        public bool CheckDiseaseEmptyOrEquals(List<string> parameters)
        {
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (elementName.Equals("MainDiagnosis"))
            {
                return Treatment.MainDisease is null || !string.IsNullOrEmpty(Treatment.MainDisease.FullName) &&
                       diseaseID.Equals(Treatment.MainDisease.FullName);
            }
            return Treatment.SubDisease is null || !string.IsNullOrEmpty(Treatment.SubDisease.FullName) &&
                   diseaseID.Equals(Treatment.SubDisease.FullName);
        }

        private void CheckTypeByInstitution()
        {
            if (Institution != null && !string.IsNullOrEmpty(Treatment.Institution.ID) && Institution.CheckCooperative())
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
    }
}
