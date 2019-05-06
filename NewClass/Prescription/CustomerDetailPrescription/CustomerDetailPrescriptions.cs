using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription {
    public class CustomerDetailPrescriptions : ObservableCollection<CustomerDetailPrescription> {
        public CustomerDetailPrescriptions() {

        }

        public void GetDataByID(int cusID) {
            var table = CustomerDetailPrescriptionDb.GetDataByCusID(cusID);
            foreach (DataRow r in table.Rows) {
                Add(new CustomerDetailPrescription(r));
            }
        }
    }
}
