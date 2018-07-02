using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using His_Pos.Class.Product;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
            IcCard = new IcCard();
            Pharmacy = new Pharmacy();
            Treatment = new Treatment();
            Medicines = new ObservableCollection<Medicine>();
        }

        public Prescription(Pharmacy pharmacy, Treatment treatment, ObservableCollection<Medicine> medicines)
        {
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }
        
        public IcCard IcCard { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Treatment Treatment { get; set; }
        public ObservableCollection<Medicine> Medicines { get; set; }
        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
    }
}
