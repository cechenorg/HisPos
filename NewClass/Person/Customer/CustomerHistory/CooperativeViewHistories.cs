using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CooperativeViewHistories:ObservableCollection<CooperativeViewHistory>
    {
        public CooperativeViewHistories(string customerIDNumber)
        {
            Init(customerIDNumber);
        }
        
        public void Init(string customerIDNumber)
        {
            Prescriptions ps = new Prescriptions();
            ps.GetPrescriptionsByCusIdNumber(customerIDNumber);
            foreach (Prescription.Prescription p in ps) {
                Add(new CooperativeViewHistory(p));
            }
        }
    }
}
