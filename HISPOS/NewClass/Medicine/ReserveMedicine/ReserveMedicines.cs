using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.Product.PrescriptionSendData;
using System.Collections.ObjectModel;

namespace His_Pos.NewClass.Medicine.ReserveMedicine
{
    public class ReserveMedicines : Collection<ReserveMedicine>
    {
        public ReserveMedicines(IndexReserveDetails indexReserveDetails)
        {
            Clear();
            foreach (var i in indexReserveDetails)
            {
                Add(new ReserveMedicine(i));
            }
        }

        public ReserveMedicines(PrescriptionSendDatas prescriptionSendDatas)
        {
            Clear();
            foreach (var p in prescriptionSendDatas)
            {
                Add(new ReserveMedicine(p));
            }
        }
    }
}