using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class Inventorys : ObservableCollection<Inventory>
    {
        public Inventorys(DataTable table) { 
            foreach (DataRow r in table.Rows) {
                Add(new Inventory(r));
            }
        }
        public static Inventorys GetAllInventoryByProIDs(List<string> MedicineIds) {
            return new Inventorys(ProductDB.GetAllInventoryByProIDs(MedicineIds));
        }
    }
}
