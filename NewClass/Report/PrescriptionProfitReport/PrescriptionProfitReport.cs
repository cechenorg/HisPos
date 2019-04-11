using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionProfitReport: ObservableObject
    {
        public PrescriptionProfitReport()
        {
        }
            public PrescriptionProfitReport(DataRow r) {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            Count = r.Field<int>("Count");
            MedicalServicePoint = r.Field<double>("MedicalServicePoint");
            MedicinePoint = r.Field<double>("MedicinePoint");
            PaySelfPoint = r.Field<int>("PaySelfPoint");
            MedUse = r.Field<double>("MedUse"); 
            Profit = r.Field<double>("Profit"); 
        }
        private string typeId;
        public string TypeId {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }
        public string typeName;
        public string TypeName
        {
            get => typeName;
            set
            {
                Set(() => TypeName, ref typeName, value);
            }
        }
        public int count;
        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        public double medicalServicePoint;
        public double MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }
        public double medicinePoint;
        public double MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }
        public double paySelfPoint;
        public double PaySelfPoint
        {
            get => paySelfPoint;
            set
            {
                Set(() => PaySelfPoint, ref paySelfPoint, value);
            }
        }
        public double medUse;
        public double MedUse
        {
            get => medUse;
            set
            {
                Set(() => MedUse, ref medUse, value);
            }
        }
        public double profit;
        public double Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }
    }
}
