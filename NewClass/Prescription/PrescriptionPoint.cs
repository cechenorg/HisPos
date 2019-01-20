﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription {
    public class PrescriptionPoint:ObservableObject
    {
        public int ApplyPoint { get; set; }//申請點數 
        private int totalPoint;//總點數
        public int TotalPoint
        {
            get => totalPoint;
            set
            {
                Set(() => TotalPoint, ref totalPoint, value);
            }
        } 
        private int copaymentPoint;//部分負擔點數
        public int CopaymentPoint
        {
            get => copaymentPoint;
            set
            {
                Set(() => CopaymentPoint, ref copaymentPoint, value);
            }
        } 
        public int SpecialMaterialPoint { get; set; } //特殊材料費用
        public int TreatmentPoint { get; set; } //診療點數
        private int medicinePoint;//藥品點數
        public int MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }
        private int medicalServicePoint;//藥事服務點數
        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }
        private int amountPay;//應付金額
        public int AmountsPay
        {
            get => amountPay;
            set
            {
                Set(() => AmountsPay, ref amountPay, value);
            }
        }
        private int deposit;//押金
        public int Deposit
        {
            get => deposit;
            set
            {
                Set(() => Deposit, ref deposit, value);
            }
        }
        private int actualReceive;//實付金額
        public int ActualReceive
        {
            get => actualReceive;
            set
            {
                Set(() => ActualReceive, ref actualReceive, value);
            }
        }
        private int amountSelfPay;//自費金額
        public int AmountSelfPay
        {
            get => amountSelfPay;
            set
            {
                Set(() => AmountSelfPay, ref amountSelfPay, value);
            }
        }
    }
    
}
