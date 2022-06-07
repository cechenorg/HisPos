using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine
{
    public class CustomerDetailPrescriptionMedicine : ObservableObject
    {
        public CustomerDetailPrescriptionMedicine(DataRow r)
        {
            ID = r.Field<string>("ID");
            Name = r.Field<string>("Name");
            Usage = r.Field<string>("Usage");
            Dossage = r.Field<string>("Dosage");
            Amount = r.Field<string>("TotalAmount");
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Usage { get; set; }

        public string Dossage { get; set; }
        public string Amount { get; set; }
    }
}