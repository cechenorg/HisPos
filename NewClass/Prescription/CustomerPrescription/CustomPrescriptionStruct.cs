using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public struct CustomPrescriptionStruct
    {
        public int? ID { get; set; }
        public PrescriptionSource Source { get; set; }
        public string Remark { get; set; }

        public CustomPrescriptionStruct(int? id, PrescriptionSource source,string remark)
        {
            ID = id;
            Source = source;
            Remark = remark;
        }
    }
}
