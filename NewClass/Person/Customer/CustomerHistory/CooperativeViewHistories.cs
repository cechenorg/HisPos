using System.Collections.ObjectModel;
using His_Pos.NewClass.Prescription;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CooperativeViewHistories:ObservableCollection<CooperativeViewHistory>
    {
        public CooperativeViewHistories()
        { 
        }
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
