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
            Medicines = new ObservableCollection<Medicine>();
        }

        public Prescription(Customer customer,Pharmacy pharmacy, Treatment treatment, ObservableCollection<Medicine> medicines)
        {
            Customer = customer;
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }

        public Customer Customer { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Treatment Treatment { get; set; }
        public ObservableCollection<Medicine> Medicines { get; set; }
        public ObservableCollection<CustomerHistory.CustomerHistory> CustomerHistories { get; set; }
        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
    }
}
