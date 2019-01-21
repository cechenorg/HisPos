using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.MedBag
{
    public class MedBagMedicines : Collection<MedBagMedicine>
    {
        public MedBagMedicines(Medicines medList,bool singleMode)
        {
            new ObservableCollection<MedBagMedicine>();
            foreach (var m in medList)
            {
                switch (m)
                {
                    case MedicineNHI medicine:
                        Add(new MedBagMedicine(medicine, singleMode));
                        break;
                    case MedicineOTC otc:
                        Add(new MedBagMedicine(otc, singleMode));
                        break;
                }
            }
        }
    }
}
