using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Product;

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
