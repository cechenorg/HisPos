using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.NewClass.Prescription.Search
{
    public class PrescriptionSearchPreviews : ObservableCollection<PrescriptionSearchPreview>
    {

        public void GetSearchPrescriptionsRe(Dictionary<string, string> conditionTypes, Dictionary<string, string> conditions, Dictionary<string, DateTime?> dates, AdjustCase adj, List<string> insIDList, Division div)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsDataRe(conditionTypes, conditions, dates, adj, insIDList, div);
            switch (conditionTypes["TimeInterval"])
            {
                case "預約日":
                    foreach (DataRow r in table.Rows)
                    {
                        Add(new PrescriptionSearchPreview(r, PrescriptionType.ChronicReserve));
                    }
                    break;
                default:
                    foreach (DataRow r in table.Rows)
                    {
                        Add(new PrescriptionSearchPreview(r, PrescriptionType.Normal));
                    }
                    break;
            }
        }

        public void GetNoBucklePrescriptions(DateTime? sDate, DateTime? eDate)
        {
            var table = PrescriptionDb.GetNoBucklePrescriptions(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionType.Normal));
            }
        }


        public void GetSearchPrescriptions(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsData(sDate, eDate, patientName, patientIDNumber, patientBirth, adj, medID, medName, ins, div);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionType.Normal));
            }
        }

        public void GetReservePrescription(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            var table = PrescriptionDb.GetReservePrescriptionsData(sDate, eDate, patientName, patientIDNumber, patientBirth, adj, medID, medName, ins, div);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchPreview(r, PrescriptionType.ChronicReserve));
            }
        }

        public List<int> GetSummary()
        {
            var presID = new List<int>();
            var summary = new List<int>();
            foreach (var p in this)
            {
                presID.Add(p.ID);
            }

            var table = PrescriptionDb.GetSearchPrescriptionsSummary(presID);
            foreach (DataColumn c in table.Rows[0].Table.Columns)
            {
                summary.Add(table.Rows[0].Field<int>(c.ColumnName));
            }
            return summary;
        }

        public List<int> GetReserveSummary()
        {
            var presID = new List<int>();
            var summary = new List<int>();
            foreach (var p in this)
            {
                presID.Add(p.ID);
            }
            var table = PrescriptionDb.GetSearchReservesSummary(presID);
            foreach (DataColumn c in table.Rows[0].Table.Columns)
            {
                summary.Add(table.Rows[0].Field<int>(c.ColumnName));
            }
            return summary;
        }
    }
}
