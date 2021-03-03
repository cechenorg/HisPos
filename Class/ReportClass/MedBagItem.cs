using His_Pos.NewClass.Medicine.MedBag;
using System.Collections.Generic;

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