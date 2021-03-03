using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product
{
    public class Inventorys : ObservableCollection<Inventory>
    {
        public Inventorys(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new Inventory(r));
            }
        }

        public static Inventorys GetAllInventoryByProIDs(List<string> MedicineIds, string warID = "0")
        {
            return new Inventorys(ProductDB.GetAllInventoryByProIDs(MedicineIds, warID));
        }
    }
}