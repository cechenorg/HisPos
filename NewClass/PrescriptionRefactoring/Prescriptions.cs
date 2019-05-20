using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class Prescriptions : ObservableCollection<Prescription>
    {
        public Prescriptions() { }


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
                Add(new Prescription(XmlService.Deserialize<CooperativePrescription.Prescription>(xDocument.InnerXml),r.Field<DateTime>("CooCli_InsertTime"), r.Field<int>("CooCli_ID").ToString(), r.Field<bool>("CooCli_IsRead")));
            }
        }
    }
}
