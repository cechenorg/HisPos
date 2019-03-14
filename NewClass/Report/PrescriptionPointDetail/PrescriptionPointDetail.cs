using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionPointDetail
{
   public class PrescriptionPointDetail : ObservableObject
    {
        public PrescriptionPointDetail() { }
        public PrescriptionPointDetail(DataRow r ) {
            PrescriptionId = r.Field<int>("PreMas_ID");
            CusName = r.Field<string>("Cus_Name");
            DivisionName = r.Field<string>("Div_Name");
            AdjustCaseName = r.Field<string>("Adj_Name");
            Point = (double)r.Field<decimal>("Point");
            MedUse = r.Field<double>("Meduse");
            TreatmentDate = r.Field<DateTime>("PreMas_TreatmentDate");
            AdjustDate = r.Field<DateTime>("PreMas_AdjustDate"); 
        }

        public int PrescriptionId { get; set; }
        public string CusName { get; set; }
        public string DivisionName { get; set; }
        public string AdjustCaseName { get; set; }
        public double Point { get; set; }
        public double MedUse { get; set; }
        public DateTime TreatmentDate { get; set; }
        public DateTime AdjustDate { get; set; }
    }
}
