using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public struct NotEnoughMedicineStruct
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public bool IsCommon { get; set; }
        public bool Frozen { get; set; }
        public int? ControlLevel { get; set; }
        public NotEnoughMedicineStruct(string id,string name, double amount,bool isCommon,bool isFrozen,int? controlLevel)
        {
            ID = id;
            Name = name;
            Amount = amount;
            IsCommon = isCommon;
            Frozen = isFrozen;
            ControlLevel = controlLevel;
        }
    }
}
