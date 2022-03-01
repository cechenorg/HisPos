using System.Collections.Generic;

namespace His_Pos.Class.ReportClass
{
    public class MedBagReport
    {
        public MedBagReport()
        {
            ReportItems = new List<MedBagItem>();
        }

        public List<MedBagItem> ReportItems { get; set; }
    }
}