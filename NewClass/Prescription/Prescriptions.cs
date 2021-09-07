using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.Service;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;

namespace His_Pos.NewClass.Prescription
{
    public class Prescriptions : ObservableCollection<Prescription>
    {
        public Prescriptions()
        {
        }

        private void GetOrthopedics(DateTime sDate, DateTime eDate)
        {
            var table = PrescriptionDb.GetOrthopedicsPrescriptions(sDate, eDate);
            foreach (var xmlDocument in table)
            {
                Add(new Prescription(XmlService.Deserialize<OrthopedicsPrescription>(xmlDocument.InnerXml)));
            }
        }

        public void GetCooperative(DateTime sDate, DateTime eDate)
        {
            NewFunction.GetXmlFiles();
            GetOrthopedics(sDate, eDate);
            DataTable table = PrescriptionDb.GetXmlOfPrescriptionsByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                var xDocument = new XmlDocument();
                xDocument.LoadXml(r["CooCli_XML"].ToString());
                Add(new Prescription(XmlService.Deserialize<CooperativePrescription.Prescription>(xDocument.InnerXml), r.Field<DateTime>("CooCli_InsertTime"), r.Field<int>("CooCli_ID").ToString(), r.Field<bool>("CooCli_IsRead"), r.Field<bool>("CooCli_IsPrint")));
            }
        }

        public void GetAutoRegisterReserve(Prescription p)
        {
            var table = PrescriptionDb.GetReserveByPrescription(p);
            foreach (DataRow r in table.Rows)
            {
                var resTable = PrescriptionDb.GetReservePrescriptionByID(r.Field<int>("ID"));
                var pre = new Prescription(resTable.Rows[0], PrescriptionType.ChronicReserve) { AdjustDate = null };
                pre.TempMedicalNumber = p.TempMedicalNumber;
                Add(pre);
            }
        }
    }
}