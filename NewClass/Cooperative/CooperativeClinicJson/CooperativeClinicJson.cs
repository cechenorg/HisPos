using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicJson
{
    public class CooperativeClinicJson
    {
        public CooperativeClinicJson()
        {
            List<CooperativeClinicJsonClass> newCollection = new List<CooperativeClinicJsonClass>();
            DataTable masterTable = CooperativeClinicJsonDb.GetCooperAdjust();
            foreach (DataRow r in masterTable.Rows)
            {
                CooperativeClinicJsonClass temp = new CooperativeClinicJsonClass(r);
                temp.MedicineCollection = temp.GetCooperAdjustMedicines();
                newCollection.Add(temp);
            }
            if (newCollection.Count > 0)
            {
                sHospId = ViewModelMainWindow.CooperativeInstitutionID;
                sRxId = ViewModelMainWindow.CurrentPharmacy.ID;

                foreach (CooperativeClinicJsonClass c in newCollection)
                {
                    msMedList msMedList = new msMedList();
                    msMedList.sMedDate = Convert.ToDateTime(c.AdjustTime).AddYears(-1911).ToString("yyyMMdd");
                    msMedList.sShtId = c.Remark;
                    foreach (var med in c.MedicineCollection)
                    {
                        msList msList = new msList();
                        msList.sOrder = med.Id;
                        msList.sTqty = Convert.ToInt32(med.Amount).ToString();
                        msMedList.sList.Add(msList);
                    }
                    sMedList.Add(msMedList);
                }
            }
        }

        public CooperativeClinicJson(string loop)
        {
            List<CooperativeClinicJsonClass> newCollection = new List<CooperativeClinicJsonClass>();
            DataTable masterTable = CooperativeClinicJsonDb.GetCooperAdjustTop100();
            foreach (DataRow r in masterTable.Rows)
            {
                CooperativeClinicJsonClass temp = new CooperativeClinicJsonClass(r);
                temp.MedicineCollection = temp.GetCooperAdjustMedicines();
                newCollection.Add(temp);
            }
            if (newCollection.Count > 0)
            {
                sHospId = ViewModelMainWindow.CooperativeInstitutionID;
                sRxId = ViewModelMainWindow.CurrentPharmacy.ID;

                foreach (CooperativeClinicJsonClass c in newCollection)
                {
                    msMedList msMedList = new msMedList();
                    msMedList.sMedDate = Convert.ToDateTime(c.AdjustTime).AddYears(-1911).ToString("yyyMMdd");
                    msMedList.sShtId = c.Remark;
                    foreach (var med in c.MedicineCollection)
                    {
                        msList msList = new msList();
                        msList.sOrder = med.Id;
                        msList.sTqty = Convert.ToInt32(med.Amount).ToString();
                        msMedList.sList.Add(msList);
                    }
                    sMedList.Add(msMedList);
                }
            }
        }

        public class msList
        {
            public string sOrder { get; set; }
            public string sTqty { get; set; }
        }

        public class msMedList
        {
            public string sMedDate { get; set; }
            public string sShtId { get; set; }
            public List<msList> sList { get; set; } = new List<msList>();
        }

        public string sHospId { get; set; }
        public string sRxId { get; set; }
        public List<msMedList> sMedList { get; set; } = new List<msMedList>();
    }
}