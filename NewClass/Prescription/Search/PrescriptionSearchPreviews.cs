using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.NewClass.Prescription.Search
{
    public class PrescriptionSearchPreviews:ObservableCollection<PrescriptionSearchPreview>
    {
        public void GetSearchPrescriptions(DateTime? sDate, DateTime? eDate, AdjustCase adj, Institution ins, MedicalPersonnel pharmacist)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsData(sDate, eDate, adj, ins, pharmacist);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r,PrescriptionSource.Normal));
            }
        }

        public void GetReservePrescription(DateTime? sDate, DateTime? eDate, AdjustCase adj, Institution ins, MedicalPersonnel pharmacist)
        {
            var table = PrescriptionDb.GetReservePrescriptionsData(sDate, eDate, adj, ins, pharmacist);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionSource.ChronicReserve));
            }
        }
    }
}
