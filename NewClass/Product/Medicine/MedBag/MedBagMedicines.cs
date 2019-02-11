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
            if (!singleMode)
            {
                int i = 1;
                foreach (var m in medList)
                {
                    switch (m)
                    {
                        case MedicineNHI medicine:
                            Add(new MedBagMedicine(medicine, false, i));
                            break;
                        case MedicineOTC otc:
                            Add(new MedBagMedicine(otc, false, i));
                            break;
                    }
                    i++;
                }
            }
            else
            {
                foreach (var m in medList)
                {
                    switch (m)
                    {
                        case MedicineNHI medicine:
                            Add(new MedBagMedicine(medicine, true));
                            break;
                        case MedicineOTC otc:
                            Add(new MedBagMedicine(otc, true));
                            break;
                    }
                }
            }
        }
    }
}
