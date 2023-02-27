using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DTO.WebService;
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

        public void SyncData()
        {

            var updateList = GetNeededUpdateTimeList();
            updateList.Wait();

            List<Task> taskList = GetNeededSyncTask(updateList.Result);
            Task.WhenAll(taskList).Wait();

            _commonDataRepository.SyncUpdateTime(updateList.Result);
        }

        private List<Task> GetNeededSyncTask(List<UpdateTimeDTO> updateTimeList)
        {
            List<Task> taskList = new List<Task>();

            var updateTableNameList = updateTimeList.Select(_ => _.UpdTime_TableName);

            if (updateTableNameList.Contains("AdjustCase"))
                taskList.Add(SyncAdjustCase());
            if (updateTableNameList.Contains("DiseaseCode"))
                taskList.Add(SyncDiseasesCode());
            if (updateTableNameList.Contains("Division"))
                taskList.Add(SyncDivisions());
            if (updateTableNameList.Contains("Institution"))
                taskList.Add(SyncInstitutions());
            if (updateTableNameList.Contains("Medicine"))
                taskList.Add(SyncMedicines());
            if (updateTableNameList.Contains("SmokeMedicine"))
                taskList.Add(SyncSmokeMedicines());
            if (updateTableNameList.Contains("SpecialMedicine"))
                taskList.Add(SyncSpecialMedicines());
            if (updateTableNameList.Contains("DiseaseCodeMapping"))
                taskList.Add(SyncDiseaseCodeMapping());
            if (updateTableNameList.Contains("MedicineForm"))
                taskList.Add(SyncMedicineForm());
            if (updateTableNameList.Contains("MedicineNHISingde"))
                taskList.Add(SyncMedicineNHISingde());
            if (updateTableNameList.Contains("Accounts"))
                taskList.Add(SyncAccounts());
            


            return taskList;
        }

        public async Task<List<UpdateTimeDTO>> GetNeededUpdateTimeList()
        {
            List<UpdateTimeDTO> needUpdateList = new List<UpdateTimeDTO>();
            var apiUpdateTimeList = await GetAPIData<UpdateTimeDTO>("GetDataSourceUpdateTime");

            var currentUpdateTimeList = _commonDataRepository.GetCurrentUpdateTime();
            var currentTableList = currentUpdateTimeList.Select(_ => _.UpdTime_TableName).ToList();

            foreach (var apiUpdateTime in apiUpdateTimeList)
            {
                if (currentTableList.Contains(apiUpdateTime.UpdTime_TableName))
                {
                    var currentUpdateTimeData = currentUpdateTimeList.Single(_ => _.UpdTime_TableName == apiUpdateTime.UpdTime_TableName);

                    if (currentUpdateTimeData.UpdTime_LastUpdateTime < apiUpdateTime.UpdTime_LastUpdateTime)
                    {
                        needUpdateList.Add(apiUpdateTime);
                    }
                }
                else
                {
                    needUpdateList.Add(apiUpdateTime);
                }
            }
            return needUpdateList;
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

        private async Task SyncDiseaseCodeMapping()
        {
            var data = await GetAPIData<DiseaseCodeMappingDTO>("GetDiseaseCodeMapping");
            _commonDataRepository.SyncDiseaseCodeMapping(data.ToList());
        }

        private async Task SyncMedicineForm()
        {
            var data = await GetAPIData<MedicineFormDTO>("GetNHIMedicineForm");
            _commonDataRepository.SyncMedicineForm(data.ToList());
        }


        private async Task SyncMedicineNHISingde()
        {
            var data = await GetAPIData<MedicineNHISingdeDTO>("GetMedicineNHISingde");
            _commonDataRepository.SyncMedicineNHISingde(data.ToList());
        }

        private async Task SyncAccounts()
        {
            var data = await GetAPIData<AccountDTO>("GetAccounts");
            _commonDataRepository.SyncAccounts(data.ToList());
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


}
