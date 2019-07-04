using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public struct BuckleMedicineStruct
    {
        public int ID { get; set; }
        public double BuckleAmount { get; set; }

        public BuckleMedicineStruct(int id, double buckleAmount)
        {
            ID = id;
            BuckleAmount = buckleAmount;
        }
    }
}
