using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using His_Pos.Class.Person;
using His_Pos.Class.Product;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
            Customer = new Customer();
            Pharmacy = new Pharmacy();
            Treatment = new Treatment();
            Medicines = new ObservableCollection<DeclareMedicine>();
        }

        public Prescription(Customer customer,Pharmacy pharmacy, Treatment treatment, ObservableCollection<DeclareMedicine> medicines)
        {
            Customer = customer;
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }

        public Customer Customer { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Treatment Treatment { get; set; }
        public string ChronicSequence { get; set; }//D35連續處方箋調劑序號
        public string ChronicTotal { get; set; }//D36連續處方可調劑次數
        public ObservableCollection<DeclareMedicine> Medicines { get; set; }
        public ObservableCollection<CustomerHistory.CustomerHistory> CustomerHistories { get; set; }
        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
    }
}
