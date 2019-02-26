using System.Collections.ObjectModel;

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
                        case MedicineSpecialMaterial specialMaterial:
                            Add(new MedBagMedicine(specialMaterial, false, i));
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
                        case MedicineSpecialMaterial specialMaterial:
                            Add(new MedBagMedicine(specialMaterial, true));
                            break;
                    }
                }
            }
        }
    }
}
