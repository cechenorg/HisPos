using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public struct MedicineInventoryStruct
    {
        public int ID { get; set; }
        public double Inventory { get; set; }

        public MedicineInventoryStruct(int id, double inventory)
        {
            ID = id;
            Inventory = inventory;
        }
    }
}