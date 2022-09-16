using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
    public class PrescriptionDetailMedicineRepot : ObservableObject
    {
     
        public string Id { get; set; }
        public string Name { get; set; }

        public double PaySelfValue { get; set; }
        public double MedicinePrice { get; set; }
        public double MedUse { get; set; }
        public double Profit { get; set; }

        public bool IsPaySelf { get; set; }
    }
}