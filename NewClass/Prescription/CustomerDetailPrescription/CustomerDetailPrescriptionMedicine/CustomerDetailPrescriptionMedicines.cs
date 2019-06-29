using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine
{
    public class CustomerDetailPrescriptionMedicines : ObservableCollection<CustomerDetailPrescriptionMedicine>
    {
        public CustomerDetailPrescriptionMedicines() { }

        public void GetDataByID(int ID,string typeName) {
            Clear();
            var table = typeName == "預約" ? CustomerDetailPrescriptionMedicineDb.GetReserveDataByCusID(ID) : CustomerDetailPrescriptionMedicineDb.GetPrescriptionDataByCusID(ID);
            foreach (DataRow r in table.Rows) {
                Add(new CustomerDetailPrescriptionMedicine(r));
            }
        }
    }
}
