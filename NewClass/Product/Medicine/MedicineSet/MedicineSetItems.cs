using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.MedicineSet
{
    public class MedicineSetItems:ObservableCollection<MedicineSetItem>
    {
        public MedicineSetItems()
        {

        }

        public void GetItems(int id)
        {
            var itemsTable = MedicineSetDb.GetMedicineSetDetail(id);
            foreach (DataRow r in itemsTable.Rows)
            {
                Add(new MedicineSetItem(r));
            }
        }
    }
}
