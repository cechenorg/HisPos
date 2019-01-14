using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription
{
    public static class PrescriptionDb
    {
        public static Prescriptions GetCooperaPrescriptionsDataByCusIdNumber(string pharmcyMedicalNum, string cusIdnum)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"cusIdnum",cusIdnum }
            };
            Prescriptions prescriptions = new Prescriptions();
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByMedicalNum", keyValues);
            XmlSerializer ser = new XmlSerializer(typeof(CooperativePrescription));
            foreach (XmlDocument xmlDocument in table)
            { 
                using (TextReader sr = new StringReader(xmlDocument.InnerXml))
                {
                    CooperativePrescription response = (CooperativePrescription)ser.Deserialize(sr);
                    prescriptions.Add(new Prescription(response));
                }
            }
            return prescriptions;
        }
        public static Prescriptions GetCooperaPrescriptionsDataByDate(string pharmcyMedicalNum, DateTime sDate, DateTime eDate)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"sDate",sDate.ToString("yyyy-MM-dd") },
                     {"eDate",eDate.ToString("yyyy-MM-dd") }
                };
            Prescriptions prescriptions = new Prescriptions();
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByDate", keyValues);
            XmlSerializer ser = new XmlSerializer(typeof(CooperativePrescription));
            foreach (XmlDocument xmlDocument in table)
            {
                using (TextReader sr = new StringReader(xmlDocument.InnerXml))
                {
                    CooperativePrescription response = XmlService.Deserialize<CooperativePrescription>(sr.ReadToEnd());
                    prescriptions.Add(new Prescription(response));
                } 
            }
            return prescriptions;
        }
        
    }
}
