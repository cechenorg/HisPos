using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Product;
using His_Pos.NewClass.Product.Medicine.MedBag;

namespace His_Pos.Class.ReportClass
{
    public class MedBagItem
    {
        public MedBagItem(string u)
        {
            Usage = u;
            Medicines = new List<MedBagMedicine>();
        }
        public string Usage { get; set; }
        public List<MedBagMedicine> Medicines { get; set; }
    }
}
