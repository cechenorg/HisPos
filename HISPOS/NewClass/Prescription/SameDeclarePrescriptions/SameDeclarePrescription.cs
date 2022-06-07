using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.SameDeclarePrescriptions
{
    public class SameDeclarePrescription : ObservableObject
    {
        public SameDeclarePrescription(DataRow r)
        {
            ID = r.Field<int>("PreMas_ID");
            PatientName = r.Field<string>("PatientName");
            PatientIDNumber = r.Field<string>("PatientIDNumber");
            TreatDate = r.Field<DateTime>("TreatDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            Institution = ViewModelMainWindow.GetInstitution(r.Field<string>("Institution"));
            Division = ViewModelMainWindow.GetDivision(r.Field<string>("Division"));
            AdjustCase = ViewModelMainWindow.GetAdjustCase(r.Field<string>("AdjustCase"));
            MedicalNumber = r.Field<string>("MedicalNumber");
            if (r.Field<string>("AdjustCase").Equals("2"))
            {
                ChronicTotal = r.Field<byte>("ChronicTotal");
                ChronicSeq = r.Field<byte>("ChronicSeq");
                ChronicTimes = ChronicTotal + " - " + ChronicSeq;
            }
            Medicines = new Medicines();
        }

        public int ID { get; }
        public string PatientName { get; }
        public string PatientIDNumber { get; }
        public DateTime TreatDate { get; }
        public DateTime AdjustDate { get; }
        public Institution Institution { get; }
        public Division Division { get; }
        public AdjustCase AdjustCase { get; }
        public string MedicalNumber { get; }
        public int ChronicTotal { get; }
        public int ChronicSeq { get; }
        public string ChronicTimes { get; }
        private Medicines medicines;
        public Medicines Medicines { get => medicines; set { Set(() => Medicines, ref medicines, value); } }

        public void GetMedicines()
        {
            Medicines.GetDataByPrescriptionId(ID);
        }
    }
}