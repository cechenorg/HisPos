using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using His_Pos.InfraStructure;
using Newtonsoft.Json;
using WebServiceDTO;

namespace His_Pos.Service
{
    internal class HISPOSWebApiService
    {

        //private const string webapiPath = @"https://kaokaodepon.singde.com.tw:60005/ServerWebAPI/";

        private const string webapiPath = @"https://localhost:7129/ServerWebAPI/";

        private CommonDataRepository _commonDataRepository = new CommonDataRepository();

        public async Task SyncData()
        {

            List<Task> taskList = new List<Task>()
            {
                SyncSpecialMedicines(),
                SyncSmokeMedicines(),
                SyncMedicines(),
                SyncInstitutions(),
                SyncDivisions(),
                SyncDiseasesCode(),
                SyncAdjustCase(),
            };

            await Task.WhenAll(taskList);
        }

        private async Task SyncSpecialMedicines()
        {
            var data = await GetAPIData<NHISpecialMedicineDTO>("GetNHISpecialMedicines");
            _commonDataRepository.SyncNHISpecialMedicine(data.ToList());
        }

        private async Task SyncSmokeMedicines()
        {
            var data = await GetAPIData<NHISmokeMedicineDTO>("GetNHISmokeMedicines");
            _commonDataRepository.SyncSmokeMedicines(data.ToList());
        }

        private async Task SyncMedicines()
        {
            var data = await GetAPIData<NHIMedicineDTO>("GetNHIMedicines");
            _commonDataRepository.SyncMedicines(data.ToList());
        }

        private async Task SyncInstitutions()
        {
            var data = await GetAPIData<InstitutionDTO>("GetInstitutions");
            _commonDataRepository.SyncInstitutions(data.ToList());
        }

        private async Task SyncDivisions()
        {
            var data = await GetAPIData<DivisionDTO>("GetDivisions");
            _commonDataRepository.SyncDivisions(data.ToList());
        }

        private async Task SyncDiseasesCode()
        {
            var data = await GetAPIData<DiseaseCodeDTO>("GetDiseaseCode");
            _commonDataRepository.SyncDiseasesCode(data.ToList());
        }

        private async Task SyncAdjustCase()
        {
            var data = await GetAPIData<AdjustCaseDTO>("GetAdjustCase");
            _commonDataRepository.SyncAdjustCase(data.ToList());
        }



        private async Task<IEnumerable<T>> GetAPIData<T>(string route)
        {
            string targetUrl = webapiPath + route;

            HttpWebRequest request = WebRequest.Create(targetUrl) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 30000;

            string result = "";

            try
            {
                var response = await request.GetResponseAsync();

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }



            ResponseData<T> responseData = new ResponseData<T>();
            try
            {
                if (string.IsNullOrEmpty(result) == false)
                    responseData = JsonConvert.DeserializeObject<ResponseData<T>>(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return responseData.Data;
        }

    }

    public class ResponseData<T>
    {
        public DateTime DateTime => DateTime.Now;

        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
