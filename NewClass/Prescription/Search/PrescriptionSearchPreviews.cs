using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public List<int> GetSummary(bool reserve)
        {
            var presID = new List<int>();
            var summary = new List<int>();
            foreach (var p in this)
            {
                presID.Add(p.ID);
            }
            DataTable table = new DataTable();
            if(reserve)
                table = PrescriptionDb.GetSearchReservesSummary(presID);
            else
            {
                table = PrescriptionDb.GetSearchPrescriptionsSummary(presID);
            }
            foreach (DataColumn c in table.Rows[0].Table.Columns)
            {
                summary.Add(table.Rows[0].Field<int>(c.ColumnName));
            }
            return summary;
        }
    }
}
