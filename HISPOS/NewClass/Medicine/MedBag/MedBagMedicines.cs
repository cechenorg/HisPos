using His_Pos.NewClass.Medicine.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace His_Pos.NewClass.Medicine.MedBag
{
    public class MedBagMedicines : Collection<MedBagMedicine>
    {
        public MedBagMedicines(Medicines medList, bool singleMode)
        {
            foreach (var m in medList.Where(med => !(med is MedicineVirtual)))
            {
                var med = m.CreateMedBagMedicine(singleMode);
                if (med is null) continue;
                Add(med);
            }
            if (singleMode) return;
            var i = 1;
            var order = 1;
            foreach (var g in Items.GroupBy(m => m.Usage).Select(group => @group.ToList()).ToList())
            {
                foreach (var med in g)
                {
                    med.MedNo = i.ToString();
                    med.Order = order;
                    i++;
                }
                order++;
            }
        }
    }
}