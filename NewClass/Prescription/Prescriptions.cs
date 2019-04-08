using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription
{
    public class Prescriptions:ObservableCollection<Prescription>
    {
        public Prescriptions()
        {
        }

        public void GetCooperativePrescriptions(string pharmacyID, DateTime sDate, DateTime eDate)
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsDataByDate(pharmacyID, sDate, eDate);
            foreach (var p in prescriptions)
            {
                Add(p);
            }
           
        }
        public void GetXmlOfPrescriptions  (DateTime sDate, DateTime eDate) {
            DataTable table = PrescriptionDb.GetXmlOfPrescriptionsByDate( sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                XmlDocument xDocument = new XmlDocument();
                xDocument.LoadXml(r["CooCli_XML"].ToString());
               
                Add(new Prescription(XmlService.Deserialize<XmlOfPrescription.Prescription>(xDocument.InnerXml)
                    ,r.Field<DateTime>("CooCli_InsertTime"),r.Field<int>("CooCli_ID").ToString(),r.Field<bool>("CooCli_IsRead")));
            }

        }
        
        public  void GetPrescriptionsByCusIdNumber(string cusIDNumber) //取得處方
        {
            var table = PrescriptionDb.GetPrescriptionsByCusIdNumber(cusIDNumber);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        }
        
        public void GetCooperativePrescriptionsByCusIDNumber(string cusIDNum) //取得合作診所
        {
            Prescriptions table = PrescriptionDb.GetCooperaPrescriptionsDataByCusIdNumber(ViewModelMainWindow.CurrentPharmacy.ID, cusIDNum);
            foreach (var r in table)
            {
                Add(r);
            }
        }

        public static void PredictThreeMonthPrescription() {
            PrescriptionDb.PredictThreeMonthPrescription();
        }
         
    }
}
