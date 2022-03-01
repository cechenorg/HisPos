using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
    public class PrescriptionDetailMedicineRepot : ObservableObject
    {
        public PrescriptionDetailMedicineRepot(DataRow r)
        {
            Id = r.Field<string>("Id");
            Name = r.Field<string>("Name");
            MedicinePrice = r.Field<double>("MedicinePrice");
            MedUse = r.Field<double>("MedUse");
            Profit = r.Field<double>("Profit");
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public double MedicinePrice { get; set; }
        public double MedUse { get; set; }
        public double Profit { get; set; }
    }
}