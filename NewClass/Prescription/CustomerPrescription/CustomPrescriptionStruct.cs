using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.CooperativeClinicJson;
using His_Pos.NewClass.Product.Medicine;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public struct CustomPrescriptionStruct
    {
        public int? ID { get; set; }
        public PrescriptionSource Source { get; set; }
        public string Remark { get; set; }
        public Medicines Medicines { get; set; }

        public CustomPrescriptionStruct(int? id, PrescriptionSource source,string remark, Medicines m = null)
        {
            ID = id;
            Source = source;
            Remark = remark;
            Medicines = new Medicines();
            switch (Source)
            {
                case PrescriptionSource.ChronicReserve:
                    Medicines.GetDataByReserveId(ID.ToString());
                    break;
                case PrescriptionSource.Normal:
                case PrescriptionSource.Register:
                    Medicines.GetDataByPrescriptionId((int)ID);
                    break;
                case PrescriptionSource.Cooperative:
                case PrescriptionSource.XmlOfPrescription:
                    Medicines = m;
                    break;
            }
        }
    }
}
