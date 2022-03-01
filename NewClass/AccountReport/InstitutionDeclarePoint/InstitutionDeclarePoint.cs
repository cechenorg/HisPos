using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.AccountReport.InstitutionDeclarePoint
{
    public class InstitutionDeclarePoint : ObservableObject
    {
        public InstitutionDeclarePoint()
        {
        }

        public InstitutionDeclarePoint(DataRow r)
        {
            InsName = r.Field<string>("Ins_Name");
            MedicinePoint = r.Field<int>("MedicinePoint");
            SpecialMedPoint = r.Field<int>("SpecialMaterialPoint");
            MedicalServicePoint = r.Field<int>("MedicalServicePoint");
            SubTotal = r.Field<int>("SubTotal");
            CopayMentPoint = r.Field<int>("CopaymentPoint");
            DeclarePoint = r.Field<int>("DeclarePoint");
            PrescriptionCount = r.Field<int>("PrescriptionCount");
        }

        public string InsName { get; set; }
        public int MedicinePoint { get; set; }
        public int SpecialMedPoint { get; set; }
        public int MedicalServicePoint { get; set; }
        public int SubTotal { get; set; }
        public int CopayMentPoint { get; set; }
        public int DeclarePoint { get; set; }
        public int PrescriptionCount { get; set; }
    }
}