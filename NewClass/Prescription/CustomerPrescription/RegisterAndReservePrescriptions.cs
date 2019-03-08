using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public class RegisterAndReservePrescriptions:ObservableCollection<RegisterAndReservePrescription>
    {
        public RegisterAndReservePrescriptions() { }
        public void GetReservePrescriptionByCusId(int cusID) //取得預約慢箋
        {
            var table = PrescriptionDb.GetReservePrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new RegisterAndReservePrescription(r, PrescriptionSource.ChronicReserve));
            }
        }
        public void GetRegisterPrescriptionByCusId(int cusID) //取得登錄慢箋
        {
            var table = PrescriptionDb.GetRegisterPrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new RegisterAndReservePrescription(r, PrescriptionSource.Normal));
            }
        }
    }

}
