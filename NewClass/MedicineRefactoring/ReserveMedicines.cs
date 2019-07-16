using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.MedicineRefactoring
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
