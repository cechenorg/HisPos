using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public class RegisterAndReservePrescription: CustomerPrescription
    {
        public RegisterAndReservePrescription(DataRow r,PrescriptionSource source):base(r, source)
        {
            ChronicSeq = r.Field<byte>("ChronicSequence");
            ChronicTotal = r.Field<byte>("ChronicTotal");
        }
        public int ChronicSeq { get; set; }
        public int ChronicTotal { get; set; }
    }
}
