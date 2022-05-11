using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using System.Collections.Generic;
using System.Data;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace His_Pos.NewClass.Prescription
{
    public class PrescriptionPoint : ObservableObject
    {
        public PrescriptionPoint()
        {
        }

        public PrescriptionPoint(DataRow r, PrescriptionType type)
        {
            ApplyPoint = r.Field<int>("ApplyPoint");
            TotalPoint = r.Field<int>("TotalPoint");
            CopaymentPoint = r.Field<short>("CopaymentPoint");
            SpecialMaterialPoint = r.Field<int>("SpecialMaterialPoint");
            TreatmentPoint = r.Field<int>("TreatmentPoint");
            MedicinePoint = r.Field<int>("MedicinePoint");
            MedicalServicePoint = r.Field<int?>("MedicalServicePoint") is null ? 0 : r.Field<int>("MedicalServicePoint");
            if (type != PrescriptionType.ChronicReserve)
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

        private int copaymentPointPayable;//應付部分負擔

        public int CopaymentPointPayable
        {
            get => copaymentPointPayable;
            set
            {
                Set(() => CopaymentPointPayable, ref copaymentPointPayable, value);
                CountAmountsPay();
            }
        }

        private int specialMaterialPoint;//特殊材料費用

        public int SpecialMaterialPoint
        {
            get => specialMaterialPoint;
            set
            {
                Set(() => SpecialMaterialPoint, ref specialMaterialPoint, value);
            }
        }

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

        private int? actualReceive;//實付金額

        public int? ActualReceive
        {
            get => actualReceive;
            set
            {
                Set(() => ActualReceive, ref actualReceive, value);
                CountChange();
            }
        }

        private int? amountSelfPay;//自費金額

        public int? AmountSelfPay
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
            Change = (ActualReceive ?? 0) - AmountsPay;
        }

        public void CountDeposit()
        {
            Deposit = MedicalServicePoint + MedicinePoint - CopaymentPoint;
        }

        public void CountAmountsPay()
        {
            var selfPay = AmountSelfPay ?? 0;
            AmountsPay = selfPay + CopaymentPointPayable;
            ActualReceive = AmountsPay;
        }

        public void CountTotal()
        {
            TotalPoint = MedicinePoint + MedicalServicePoint + SpecialMaterialPoint + CopaymentPoint;
        }

        public void CountApply()
        {
            ApplyPoint = TotalPoint - CopaymentPoint;//計算申請點數
        }
         
        public int GetCopaymentValue(string institutionLevelType)
        {
            //醫學中心or區域醫院
            if (institutionLevelType == "1" || institutionLevelType == "2")
            {
                switch (MedicinePoint)
                {
                    case int n when n <= 100:
                        return 0;

                    case int n when n >= 101 && n <= 200:
                        return 20;

                    case int n when n >= 201 && n <= 300:
                        return 40;

                    case int n when n >= 301 && n <= 400:
                        return 60;

                    case int n when n >= 401 && n <= 500:
                        return 80;

                    case int n when n >= 501 && n <= 600:
                        return 100;

                    case int n when n >= 601 && n <= 700:
                        return 120;

                    case int n when n >= 701 && n <= 800:
                        return 140;

                    case int n when n >= 801 && n <= 900:
                        return 160;

                    case int n when n >= 901 && n <= 1000:
                        return 180;

                    case int n when n >= 1001 && n <= 1100:
                        return 200;

                    case int n when n >= 1101 && n <= 1200:
                        return 220;

                    case int n when n >= 1201 && n <= 1300:
                        return 240;

                    case int n when n >= 1301 && n <= 1400:
                        return 260;

                    case int n when n >= 1401 && n <= 1500:
                        return 280;
                        
                    default:
                        return 300;
                }
            }
            else
            {
                switch (MedicinePoint)
                {
                    case int n when n <= 100:
                        return 0;

                    case int n when n >= 101 && n <= 200:
                        return 20;

                    case int n when n >= 201 && n <= 300:
                        return 40;

                    case int n when n >= 301 && n <= 400:
                        return 60;

                    case int n when n >= 401 && n <= 500:
                        return 80;

                    case int n when n >= 501 && n <= 600:
                        return 100;

                    case int n when n >= 601 && n <= 700:
                        return 120;

                    case int n when n >= 701 && n <= 800:
                        return 140;

                    case int n when n >= 801 && n <= 900:
                        return 160;

                    case int n when n >= 901 && n <= 1000:
                        return 180;

                    default:
                        return 200;
                }
            } 
        }

        public void Count(List<Pdata> details)
        {
            MedicinePoint = details.Count(p => p.P1.Equals("1")) > 0 ? details.Where(p => p.P1.Equals("1")).Sum(p => int.Parse(p.P9)) : 0;
            SpecialMaterialPoint = details.Count(p => p.P1.Equals("3")) > 0 ? details.Where(p => p.P1.Equals("3")).Sum(p => int.Parse(p.P9)) : 0;//計算特殊材料點數
            ApplyPoint = MedicinePoint + MedicalServicePoint + SpecialMaterialPoint;//計算申請點數
            TotalPoint = ApplyPoint + CopaymentPoint;
        }
    }
}