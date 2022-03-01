using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionProfitReport : ObservableObject
    {
        public PrescriptionProfitReport()
        {
        }

        public PrescriptionProfitReport(DataRow r)
        {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            Count = r.Field<int>("Count");
            MedicalServicePoint = r.Field<int>("MedicalServicePoint");
            MedicinePoint = r.Field<int>("MedicinePoint");
            PaySelfPoint = r.Field<int>("PaySelfPoint");
            MedUse = r.Field<int>("MedUse");
            Profit = r.Field<int>("Profit");
        }

        private string typeId;

        public string TypeId
        {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }

        private string typeName;

        public string TypeName
        {
            get => typeName;
            set
            {
                Set(() => TypeName, ref typeName, value);
            }
        }

        private int count;

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }

        private int medicalServicePoint;

        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }

        private int medicinePoint;

        public int MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }

        private int paySelfPoint;

        public int PaySelfPoint
        {
            get => paySelfPoint;
            set
            {
                Set(() => PaySelfPoint, ref paySelfPoint, value);
            }
        }

        private int medUse;

        public int MedUse
        {
            get => medUse;
            set
            {
                Set(() => MedUse, ref medUse, value);
            }
        }

        private int profit;

        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }

        private int totalMed;

        public int TotalMed
        {
            get => totalMed;
            set
            {
                Set(() => TotalMed, ref totalMed, value);
            }
        }

        public void CountEditPoint(DataRow editDataRow)
        {
            Count += editDataRow.Field<int>("Count");
            MedicalServicePoint += editDataRow.Field<int>("MedicalServicePoint");
            MedicinePoint += editDataRow.Field<int>("MedicinePoint");
            PaySelfPoint += editDataRow.Field<int>("PaySelfPoint");
            Profit += editDataRow.Field<int>("Profit");
        }
    }
}