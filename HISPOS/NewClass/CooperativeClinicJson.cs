using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using System.Collections.Generic;

namespace His_Pos.Class
{
    public class CooperativeClinicJson
    {
        public CooperativeClinicJson(CooperativePrescriptions cooperativeClinics)
        {
            // sHospId = cooperativeClinics[0].Prescription.Treatment.MedicalInfo.Hospital.Id;
            // sRxId = ViewModelMainWindow.CurrentPharmacy.Id;
            //
            // foreach (OrthopedicsPrescription declareData in cooperativeClinics) {
            //     msMedList msMedList = new msMedList();
            //     msMedList.sMedDate = declareData.Prescription.Treatment.TreatmentDate.AddYears(-1911).ToString("yyyMMdd");
            //     msMedList.sShtId = declareData.Remark.Substring(0,16);
            //     foreach (var declareDetail in declareData.Prescription.Medicines)
            //     {
            //         if (declareDetail is DeclareMedicine)
            //         {
            //             msList msList = new msList();
            //             msList.sOrder = ((DeclareMedicine)declareDetail).Id;
            //             msList.sTqty = Convert.ToInt32(((DeclareMedicine)declareDetail).Amount).ToString();
            //             msMedList.sList.Add(msList);
            //         }
            //     }
            //     sMedList.Add(msMedList);
            // }
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