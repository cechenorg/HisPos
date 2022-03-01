using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.MedicineSet
{
    public class MedicineSetItems : ObservableCollection<MedicineSetItem>
    {
        public MedicineSetItems()
        {
        }

        public void GetItems(int id)
        {
            Clear();
            var itemsTable = MedicineSetDb.GetMedicineSetDetail(id);
            foreach (DataRow r in itemsTable.Rows)
            {
                var item = new MedicineSetItem(r);
                item.UsageName = r.Field<string>("MedSetDet_Usage");
                item.PositionID = r.Field<string>("MedSetDet_Position");
                Add(item);
            }
        }
    }
}