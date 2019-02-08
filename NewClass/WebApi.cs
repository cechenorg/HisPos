using His_Pos.ChromeTabViewModel;
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


namespace His_Pos.NewClass
{
   public static class WebApi
    {

        internal static void SendToCooperClinic() {
            CooperativeClinicJson.CooperativeClinicJson cooperativeClinicJson = new CooperativeClinicJson.CooperativeClinicJson();
            string json = JsonConvert.SerializeObject(cooperativeClinicJson);
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmacyMedicalNum",ViewModelMainWindow.CurrentPharmacy.Id },
                     {"json",json }
                };
            if (json.Equals(@"{""sHospId"":null,""sRxId"":null,""sMedList"":[]}"))
                return;
            CooperativeClinicJson.CooperativeClinicJsonDb.InsertCooperJson(json);
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/SendToCooperClinic", keyValues);
        } 
        internal static string GetCooperativeClinicId(string medicalNum) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                     {"medicalNum", medicalNum }
                };
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetCooperativeClinicId", keyValues);
            if (table.Count == 0)
                return string.Empty;
            return table[0].SelectSingleNode("ArrayOfString/string").InnerText;
        }
        internal static XmlDocument GetPharmacyInfoByVerify(string verifyKey) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                     {"verifyKey", verifyKey }
                };
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetPharmacyInfoByRemark", keyValues);
            return table[0];
        }
        internal static void UpdatePharmacyMedicalNum(string medicalNum) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                     {"VerifyKey", Properties.Settings.Default.SystemSerialNumber },
                     {"MedicalNum", medicalNum }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/UpdatePharmacyMedicalNum", keyValues);
        }
    }
}
