using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport
{
    public class PrescriptionDetailReport : ObservableObject
    {
        public PrescriptionDetailReport()
        {
        }

        public PrescriptionDetailReport(DataRow r)
        {
            Id = r.Field<int>("PreMas_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            AdjustCaseID = r.Field<string>("AdjustCaseID");
            MedicalServicePoint = r.Field<int>("MedicalServicePoint");
            MedicalPoint = r.Field<int>("MedicinePoint");
            Meduse = r.Field<int>("Meduse");
            Profit = r.Field<int>("Profit");
            PaySelfPoint = r.Field<int>("PaySelfPoint");
        }

        private string insName;
        private double medicalServicePoint;
        private double medicalPoint;
        private double paySelfPoint;
        private decimal meduse;
        private int profit;
        private int count;
        private int coopCount;
        private int paySelfCount;
        private int slowCount;
        private int normalCount;
        private int coopProfit;
        private int paySelfProfit;
        private int slowProfit;
        private int normalProfit;
        private int coopMeduse;
        private int paySelfMeduse;
        private int slowMeduse;
        private int normalMeduse;
        private double coopIncome;
        private double paySelfIncome;
        private double slowIncome;
        private double normalIncome;

        private decimal coopChange;
        private decimal paySelfChange;
        private decimal slowChange;
        private decimal normalChange;

        private int medTotalCount;
        private decimal medTotalProfit;
        private int medTotalMeduse;
        private double medTotalIncome;
        private decimal medTotalChange;

        public int Id { get; set; }
        public string AdjustCaseID { get; set; }
        public string CusName { get; set; }

        public string InsName
        {
            get => insName;
            set
            {
                Set(() => InsName, ref insName, value);
            }
        }

        public double MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }

        public double MedicalPoint
        {
            get => medicalPoint;
            set
            {
                Set(() => MedicalPoint, ref medicalPoint, value);
            }
        }

        public double PaySelfPoint
        {
            get => paySelfPoint;
            set
            {
                Set(() => PaySelfPoint, ref paySelfPoint, value);
            }
        }

        public decimal Meduse
        {
            get => meduse;
            set
            {
                Set(() => Meduse, ref meduse, value);
            }
        }

        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        public int CoopCount
        {
            get => coopCount;
            set
            {
                Set(() => CoopCount, ref coopCount, value);
            }
        }
        public int PaySelfCount
        {
            get => paySelfCount;
            set
            {
                Set(() => PaySelfCount, ref paySelfCount, value);
            }
        }
        public int SlowCount
        {
            get => slowCount;
            set
            {
                Set(() => SlowCount, ref slowCount, value);
            }
        }

        public int NormalCount
        {
            get => normalCount;
            set
            {
                Set(() => NormalCount, ref normalCount, value);
            }
        }
        public int CoopProfit
        {
            get => coopProfit;
            set
            {
                Set(() => CoopProfit, ref coopProfit, value);
            }
        }
        public int PaySelfProfit
        {
            get => paySelfProfit;
            set
            {
                Set(() => PaySelfProfit, ref paySelfProfit, value);
            }
        }
        public int SlowProfit
        {
            get => slowProfit;
            set
            {
                Set(() => SlowProfit, ref slowProfit, value);
            }
        }
        public int NormalProfit
        {
            get => normalProfit;
            set
            {
                Set(() => NormalProfit, ref normalProfit, value);
            }
        }
        public int CoopMeduse
        {
            get => coopMeduse;
            set
            {
                Set(() => CoopMeduse, ref coopMeduse, value);
            }
        }
        public int PaySelfMeduse
        {
            get => paySelfMeduse;
            set
            {
                Set(() => PaySelfMeduse, ref paySelfMeduse, value);
            }
        }
        public int SlowMeduse
        {
            get => slowMeduse;
            set
            {
                Set(() => SlowMeduse, ref slowMeduse, value);
            }
        }
        public int NormalMeduse
        {
            get => normalMeduse;
            set
            {
                Set(() => NormalMeduse, ref normalMeduse, value);
            }
        }
        public double CoopIncome
        {
            get => coopIncome;
            set
            {
                Set(() => CoopIncome, ref coopIncome, value);
            }
        }
        public double PaySelfIncome
        {
            get => paySelfIncome;
            set
            {
                Set(() => PaySelfIncome, ref paySelfIncome, value);
            }
        }
        public double SlowIncome
        {
            get => slowIncome;
            set
            {
                Set(() => SlowIncome, ref slowIncome, value);
            }
        }
        public double NormalIncome
        {
            get => normalIncome;
            set
            {
                Set(() => NormalIncome, ref normalIncome, value);
            }
        }




        public decimal CoopChange
        {
            get => coopChange;
            set
            {
                Set(() => CoopChange, ref coopChange, value);
            }
        }
        public decimal PaySelfChange
        {
            get => paySelfChange;
            set
            {
                Set(() => PaySelfChange, ref paySelfChange, value);
            }
        }
        public decimal SlowChange
        {
            get => slowChange;
            set
            {
                Set(() => SlowChange, ref slowChange, value);
            }
        }
        public decimal NormalChange
        {
            get => normalChange;
            set
            {
                Set(() => NormalChange, ref normalChange, value);
            }
        }

        public int MedTotalCount
        {
            get => medTotalCount;
            set
            {
                Set(() => MedTotalCount, ref medTotalCount, value);
            }
        }
        public decimal MedTotalProfit
        {
            get => medTotalProfit;
            set
            {
                Set(() => MedTotalProfit, ref medTotalProfit, value);
            }
        }
        public int MedTotalMeduse
        {
            get => medTotalMeduse;
            set
            {
                Set(() => MedTotalMeduse, ref medTotalMeduse, value);
            }
        }
        public double MedTotalIncome
        {
            get => medTotalIncome;
            set
            {
                Set(() => MedTotalIncome, ref medTotalIncome, value);
            }
        }

        public decimal MedTotalChange
        {
            get => medTotalChange;
            set
            {
                Set(() => MedTotalChange, ref medTotalChange, value);
            }
        }



    }
}