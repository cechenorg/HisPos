using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;

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
