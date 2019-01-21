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
         

        internal static void UpdateXmlStatus(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty }, 
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateXmlStatus", keyValues);
        }
        internal static void UpdateIsRead(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateIsReadByDeclareId", keyValues);
        }
        //internal static void SendToCooperClinic(CooperativeClinicJson cooperativeClinicJson)
        //{
        //    string json = JsonConvert.SerializeObject(cooperativeClinicJson);
        //    Dictionary<string, string> keyValues;
        //    keyValues = new Dictionary<string, string> {
        //            {"pharmacyMedicalNum",ViewModelMainWindow.CurrentPharmacy.Id },
        //             {"json",json }
        //        };
        //    HttpMethod httpMethod = new HttpMethod();
        //    httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/SendToCooperClinic", keyValues);
        //}

        internal static void SyncServerData(string localIP)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {{ "ip", localIP },{"file", ""}};
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/SyncServerData", keyValues);
        }
    }
}
