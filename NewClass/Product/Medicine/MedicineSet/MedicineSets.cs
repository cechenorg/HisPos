using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Documents;

namespace His_Pos.NewClass.Product.Medicine.MedicineSet
{
    public class MedicineSets:ObservableCollection<MedicineSet>
    {
        public MedicineSets()
        {
            Init();
        }
        public void Init()
        {
            var setTable = MedicineSetDb.GetMedicineSets();
            foreach (DataRow r in setTable.Rows)
            {
                Add(new MedicineSet(r));
            }
        }
    }
}
