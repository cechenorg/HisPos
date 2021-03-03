using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePharmacy
{
    public class DeclarePharmacy
    {
        public DeclarePharmacy(DataRow r)
        {
            ID = r.Field<string>("CurPhaRec_MedicalNumber");
            Start = r.Field<DateTime>("CurPhaRec_StartDate");
            End = r.Field<DateTime?>("CurPhaRec_EndDate");
        }

        public string ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}