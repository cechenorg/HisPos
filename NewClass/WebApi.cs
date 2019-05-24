using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                    {"pharmacyMedicalNum",ViewModelMainWindow.CurrentPharmacy.ID },
                     {"json",json }
                };
            if (json.Equals(@"{""sHospId"":null,""sRxId"":null,""sMedList"":[]}"))
                return;
            HttpMethod httpMethod = new HttpMethod();
            if (httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/SendToCooperClinic", keyValues))
                CooperativeClinicJson.CooperativeClinicJsonDb.InsertCooperJson(json);
            else
                MessageWindow.ShowMessage("骨科回傳扣庫失敗, 請通知資訊人員",Class.MessageType.ERROR);
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
