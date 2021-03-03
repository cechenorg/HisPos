using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.DuplicatePrescription
{
    public class DuplicatePrescriptions : ObservableCollection<DuplicatePrescription>
    {
        public DuplicatePrescriptions()
        {
        }

        public void GetData(DateTime startDate, DateTime endDate)
        {
            var table = PrescriptionDb.DuplicatePrescriptions(startDate, endDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new DuplicatePrescription(r));
            }
        }
    }
}