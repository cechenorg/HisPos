using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public class CustomerPrescriptions:ObservableCollection<CustomerPrescription>
    {
        public CustomerPrescriptions()
        {

        }
        public void GetPrescriptionsNoGetCardByCusId(int cusID) //取得未過卡處方
        {
            var table = PrescriptionDb.GetPrescriptionsNoGetCardByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new CustomerPrescription(r, PrescriptionSource.Normal));
            }
        }
    }
}
