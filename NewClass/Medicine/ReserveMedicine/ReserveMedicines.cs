using System.Collections.ObjectModel;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;

namespace His_Pos.NewClass.Medicine.ReserveMedicine
{
    public class ReserveMedicines:Collection<ReserveMedicine>
    {
        public ReserveMedicines(IndexReserveDetails indexReserveDetails)
        {
            Clear();
            foreach (var i in indexReserveDetails) {
                Add(new ReserveMedicine(i));
            }
        }
    }
}
