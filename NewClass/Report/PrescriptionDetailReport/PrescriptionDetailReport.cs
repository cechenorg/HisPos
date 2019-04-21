using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport
{
    public class PrescriptionDetailReport: ObservableObject
    {
        public PrescriptionDetailReport(DataRow r) {
            Id = r.Field<int>("PreMas_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            MedicalServicePoint = r.Field<double>("MedicalServicePoint");
            MedicalPoint = r.Field<double>("MedicinePoint");
            Meduse = r.Field<double>("Meduse");
            Profit = r.Field<double>("Profit"); 
        }
        public int Id { get; set; }
        public string CusName { get; set; }
        public string InsName { get; set; }
        public double MedicalServicePoint { get; set; }
        public double MedicalPoint { get; set; }
        public double Meduse { get; set; }
        public double Profit { get; set; }
    }
}
