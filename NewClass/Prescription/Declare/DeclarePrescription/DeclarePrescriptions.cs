using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescriptions:ObservableCollection<DeclarePrescription>
    {
        public DeclarePrescriptions()
        {

        }

        public void GetSearchPrescriptions(DateTime decStart, DateTime decEnd)
        {
            var table = DeclarePrescriptionDb.GetDeclarePrescriptionsByMonthRange(decStart, decEnd);
            foreach (DataRow r in table.Rows)
            {
                Add(new DeclarePrescription(r));
            }
        }
        public void AddPrescriptions(List<DeclarePrescription> pres)
        {
            foreach (var p in pres)
            {
                Add(p);
            }
        }
    }
}
