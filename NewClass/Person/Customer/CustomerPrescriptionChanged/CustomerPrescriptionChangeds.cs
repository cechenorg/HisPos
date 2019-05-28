using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Customer.CustomerPrescriptionChanged
{
    public class CustomerPrescriptionChangeds : ObservableCollection<CustomerPrescriptionChanged>
    {
        public CustomerPrescriptionChangeds() {
        }
        public void GetDataByCudID(int CusID) {
            Clear();
            DataTable table = CustomerPrescriptionChangedDb.GetDataByCusId(CusID);
            foreach (DataRow r in table.Rows) {
                Add(new CustomerPrescriptionChanged(r));
            }
        }
    }
}
