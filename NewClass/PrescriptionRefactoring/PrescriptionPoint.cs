using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions;

// ReSharper disable InconsistentNaming

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class PrescriptionPoint:ObservableObject
    {
        public PrescriptionPoint()
        {
        }

        public PrescriptionPoint(DataRow r,ChronicType type)
        {
            ApplyPoint = r.Field<int>("ApplyPoint");
            TotalPoint = r.Field<int>("TotalPoint");
            CopaymentPoint = r.Field<short>("CopaymentPoint");
            SpecialMaterialPoint = r.Field<int>("SpecialMaterialPoint");
            TreatmentPoint = r.Field<int>("TreatmentPoint");
            MedicinePoint = r.Field<int>("MedicinePoint");
            MedicalServicePoint = r.Field<int>("MedicalServicePoint");
            if (type != ChronicType.Reserve)
            {
                AmountSelfPay = r.Field<int>("PaySelfPoint");
                Deposit = r.Field<int>("DepositPoint");
            }
        }

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
        private int administrativeAssistanceCopaymentPoint;//行政協助部分負擔點數
        public int AdministrativeAssistanceCopaymentPoint
        {
            get => administrativeAssistanceCopaymentPoint;
            set
            {
                Set(() => AdministrativeAssistanceCopaymentPoint, ref administrativeAssistanceCopaymentPoint, value);
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
                ActualReceive = value;
                CountChange();
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
                CountChange();
            }
        }
        private int amountSelfPay;//自費金額
        public int AmountSelfPay
        {
            get => amountSelfPay;
            set
            {
                Set(() => AmountSelfPay, ref amountSelfPay, value);
                CountAmountsPay();
            }
        }
        private int change;//自費金額
        public int Change
        {
            get => change;
            set
            {
                Set(() => Change, ref change, value);
            }
        }
        private void CountChange()
        {
            Change = ActualReceive - AmountsPay;

        }
        public void CountDeposit()
        {
            Deposit = MedicalServicePoint + MedicinePoint - CopaymentPoint;
        }

        public void CountAmountsPay()
        {
            AmountsPay = AmountSelfPay + CopaymentPoint;
        }

        public void GetDeposit(int id)
        {
            Deposit = (int)PrescriptionDb.GetDeposit(id).Rows[0].Field<decimal>("Deposit");
        }

        public void GetAmountPaySelf(int id)
        {
            AmountSelfPay = (int)PrescriptionDb.GetAmountPaySelf(id).Rows[0].Field<decimal>("AmountPaySelf");
        }
    }
}
