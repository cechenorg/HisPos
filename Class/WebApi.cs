using His_Pos.Class.Declare;
using His_Pos.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
   public static class WebApi
    {
        internal static ObservableCollection<CooperativeClinic> GetXmlByMedicalNum(string pharmcyMedicalNum, string cusIdnum = null)
        {
            Dictionary<string, string> keyValues;
                keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"cusIdnum",cusIdnum }
                };
            ObservableCollection<CooperativeClinic> cooperativeClinics = new ObservableCollection<CooperativeClinic>();
             HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByMedicalNum", keyValues);
            foreach (XmlDocument xmlDocument in table) {
                cooperativeClinics.Add(new CooperativeClinic(xmlDocument,"ByNedicalNum"));
            }
            return cooperativeClinics;
        }
        internal static ObservableCollection<CooperativeClinic> GetXmlByDate(string pharmcyMedicalNum, DateTime sDate,DateTime eDate)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"sDate",sDate.ToString("yyyy-MM-dd") },
                     {"eDate",eDate.ToString("yyyy-MM-dd") }
                };
            ObservableCollection<CooperativeClinic> cooperativeClinics = new ObservableCollection<CooperativeClinic>();
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByDate", keyValues);
            foreach (XmlDocument xmlDocument in table)
            {
                cooperativeClinics.Add(new CooperativeClinic(xmlDocument, "ByDate"));
            }
            return cooperativeClinics;
        }

        internal static void UpdateXmlStatus(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty }, 
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateXmlStatus", keyValues);
        }
        internal static void UpdateIsReadByDeclareId(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateIsReadByDeclareId", keyValues);
        }
        internal static void SendToCooperClinic(CooperativeClinicJson cooperativeClinicJson)
        {
            string json = JsonConvert.SerializeObject(cooperativeClinicJson);
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmacyMedicalNum",MainWindow.CurrentPharmacy.Id },
                     {"json",json }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/SendToCooperClinic", keyValues);
        }

        internal static void SyncServerData(string localIP)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {{ "ip", localIP },{"file", ""}};
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/SyncServerData", keyValues);
        }
    }
}
