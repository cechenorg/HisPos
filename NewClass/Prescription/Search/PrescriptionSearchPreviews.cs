using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.PrescriptionRefactoring;

namespace His_Pos.NewClass.Prescription.Search
{
    public class PrescriptionSearchPreviews:ObservableCollection<PrescriptionSearchPreview>
    {

        public void GetSearchPrescriptions(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsData(sDate, eDate, patientName, patientIDNumber, patientBirth, adj, medID, medName, ins, div);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionSource.Normal));
            }
        }

        public void GetReservePrescription(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            var table = PrescriptionDb.GetReservePrescriptionsData(sDate, eDate, patientName, patientIDNumber, patientBirth, adj, medID, medName, ins, div);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionSource.ChronicReserve));
            }
        }

        public List<int> GetSummary(bool reserve, string MedicineID)
        {
            var presID = new List<int>();
            var summary = new List<int>();
            foreach (var p in this)
            {
                presID.Add(p.ID);
            }
            DataTable table = new DataTable();
            if (reserve)
                table = PrescriptionDb.GetSearchReservesSummary(presID, MedicineID);
            else
            {
                table = PrescriptionDb.GetSearchPrescriptionsSummary(presID, MedicineID);
            }
            foreach (DataColumn c in table.Rows[0].Table.Columns)
            {
                summary.Add(table.Rows[0].Field<int>(c.ColumnName));
            }
            return summary;
        }
    }
}
