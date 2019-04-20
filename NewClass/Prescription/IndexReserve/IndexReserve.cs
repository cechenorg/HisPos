using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
    public class IndexReserve : ObservableObject
    {
        public IndexReserve(DataRow r) {
            Id = r.Field<int>("Id");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            TreatDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            PhoneNote = r.Field<string>("Cus_UrgentNote");
            PrepareStatus = r.Field<string>("MedPrepareStatus");
            PhoneCallStatus = r.Field<string>("CallStatus");
            IsSend = PrepareStatus == "F" ? false : true;
        }

        public int Id { get; set; }
        public bool IsSend { get; set; } 
        public string CusName { get; set; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public string PhoneNote { get; set; }
        public string PrepareStatus { get; set; }
        public string PhoneCallStatus { get; set; }
    }
}
