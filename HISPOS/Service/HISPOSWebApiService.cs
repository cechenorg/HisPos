using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using WebServiceDTO;

namespace His_Pos.Service
{
    internal class HISPOSWebApiService
    {

        private const string webapiPath = @"https://kaokaodepon.singde.com.tw:60005/ServerAPI/";
        public async Task SyncData()
        {

            await GetAPIData<AdjustCaseDTO>("GetAdjustCase");
        }

        private async Task<T> GetAPIData<T>(string route)
        {
            string targetUrl = webapiPath + "GetAdjustCase";

            HttpWebRequest request = WebRequest.Create(targetUrl) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;

            string result = "";
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            
            var DTO = JsonConvert.DeserializeObject<T>(result);

            return DTO;
        }

    }
}
