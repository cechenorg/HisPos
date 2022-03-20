using System;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.NewClass.Cooperative.CooperativeClinicJson;
using His_Pos.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using CooperativeClinicJson = His_Pos.NewClass.Cooperative.CooperativeClinicJson.CooperativeClinicJson;
using static His_Pos.NewClass.Cooperative.CooperativeClinicJson.CooperativeClinicJsonClass;

namespace His_Pos.NewClass
{
    public static class WebApi
    {
        internal static void SendToCooperClinic()
        {
            CooperativeClinicJson cooperativeClinicJson = InitCooperativeClinicJson("");
            
             
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
            {
                CooperativeClinicJsonDb.InsertCooperJson(json);
                CooperativeClinicJsonDb.UpdateCooperAdjustMedcinesStatus();
            }
            else
            {
                NewFunction.ShowMessageFromDispatcher("骨科回傳扣庫失敗, 請通知資訊人員", MessageType.ERROR);
            }
        }

        internal static string GetCooperativeClinicId(string medicalNum)
        {
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

        internal static XmlDocument GetPharmacyInfoByVerify(string verifyKey)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                     {"verifyKey", verifyKey }
                };
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetPharmacyInfoByRemark", keyValues);
            return table[0];
        }

        internal static void UpdatePharmacyMedicalNum(string medicalNum)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                     {"VerifyKey", Properties.Settings.Default.SystemSerialNumber },
                     {"MedicalNum", medicalNum }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.Post(@"http://kaokaodepon.singde.com.tw:59091/api/UpdatePharmacyMedicalNum", keyValues);
        }

        private static CooperativeClinicJson InitCooperativeClinicJson(string json)
        {
            CooperativeClinicJson cooperativeClinicJson = new CooperativeClinicJson();
            List<CooperativeClinicJsonClass> newCollection = new List<CooperativeClinicJsonClass>();
            DataTable masterTable = CooperativeClinicJsonDb.GetCooperAdjustTop100();
            foreach (DataRow r in masterTable.Rows)
            {
                CooperativeClinicJsonClass temp = new CooperativeClinicJsonClass(r);

                List<Medicines> tempdata = new List<Medicines>();
                DataTable table = CooperativeClinicJsonDb.GetCooperAdjustMedicines(temp.Id);
                foreach (DataRow row in table.Rows)
                {
                    tempdata.Add(new Medicines(row));
                }

                temp.MedicineCollection = tempdata;
                 
                newCollection.Add(temp);
            }
            if (newCollection.Count > 0)
            {
                cooperativeClinicJson.sHospId = ViewModelMainWindow.CooperativeInstitutionID;
                cooperativeClinicJson.sRxId = ViewModelMainWindow.CurrentPharmacy.ID;

                foreach (CooperativeClinicJsonClass c in newCollection)
                {
                    CooperativeClinicJson.msMedList msMedList = new CooperativeClinicJson.msMedList();
                    msMedList.sMedDate = Convert.ToDateTime(c.AdjustTime).AddYears(-1911).ToString("yyyMMdd");
                    msMedList.sShtId = c.Remark;
                    foreach (var med in c.MedicineCollection)
                    {
                        CooperativeClinicJson.msList msList = new CooperativeClinicJson.msList();
                        msList.sOrder = med.Id;
                        msList.sTqty = Convert.ToInt32(med.Amount).ToString();
                        msMedList.sList.Add(msList);
                    }
                    cooperativeClinicJson.sMedList.Add(msMedList);
                }
            }
              
            return cooperativeClinicJson;
        }

        internal static bool SendToCooperClinicLoop100()
        {
            CooperativeClinicJson cooperativeClinicJson = InitCooperativeClinicJson("Loop");
            string json = JsonConvert.SerializeObject(cooperativeClinicJson);
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmacyMedicalNum",ViewModelMainWindow.CurrentPharmacy.ID },
                     {"json",json }
                };
            if (json.Equals(@"{""sHospId"":null,""sRxId"":null,""sMedList"":[]}"))
                return false;
            HttpMethod httpMethod = new HttpMethod();
            if (httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/SendToCooperClinic", keyValues))
            {
                CooperativeClinicJsonDb.InsertCooperJson(json);
                CooperativeClinicJsonDb.UpdateCooperAdjustMedcinesStatusTop100();
                return true;
            }
            NewFunction.ShowMessageFromDispatcher("骨科回傳扣庫失敗, 請通知資訊人員", MessageType.ERROR);
            return false;
        }
    }
}