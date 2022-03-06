using System.Collections.Generic;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicJson
{
    public class CooperativeClinicJson
    {
        public CooperativeClinicJson()
        {
           
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