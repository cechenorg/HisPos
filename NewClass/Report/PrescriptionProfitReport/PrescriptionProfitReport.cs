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
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public int Count { get; set; }
        public double MedicalServicePoint { get; set; }
        public double MedicinePoint { get; set; }
        public double PaySelfPoint { get; set; }
        public double MedUse  { get; set; }
        public double Profit { get; set; }
    }
}
