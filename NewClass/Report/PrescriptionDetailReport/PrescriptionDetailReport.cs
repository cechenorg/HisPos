using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport
{
    public class PrescriptionDetailReport : ObservableObject
    {
        public PrescriptionDetailReport() { }
        public PrescriptionDetailReport(DataRow r) {
            Id = r.Field<int>("PreMas_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            AdjustCaseID = r.Field<string>("AdjustCaseID");
            MedicalServicePoint = r.Field<double>("MedicalServicePoint");
            MedicalPoint = r.Field<double>("MedicinePoint");
            Meduse = r.Field<double>("Meduse");
            Profit = r.Field<double>("Profit");
            PaySelfPoint = r.Field<double>("PaySelfPoint");
        }
        private string insName;
        private double medicalServicePoint;
        private double medicalPoint;
        private double paySelfPoint;
        private double meduse;
        private double profit;
        private int count;
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
        public double Meduse
        {
            get => meduse;
            set
            {
                Set(() => Meduse, ref meduse, value);
            }
        }
        public double Profit
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
    }
}
