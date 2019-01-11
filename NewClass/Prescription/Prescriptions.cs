using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription
{
    public class Prescriptions:Collection<Prescription>
    {
        public Prescriptions() { }

        public void GetCooperativePrescriptions()
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsData();
            foreach (var p in prescriptions)
            {
                Add(p);
            }
        }
    }
}
