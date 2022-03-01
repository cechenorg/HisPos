using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription
{
    public class CustomerDetailPrescriptions : ObservableCollection<CustomerDetailPrescription>
    {
        public CustomerDetailPrescriptions()
        {
        }

        public void GetDataByID(int cusID)
        {
            var table = CustomerDetailPrescriptionDb.GetDataByCusID(cusID);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new CustomerDetailPrescription(r));
            }
        }
    }
}