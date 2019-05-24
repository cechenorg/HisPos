using System.Collections.ObjectModel;
using System.Linq;

namespace His_Pos.NewClass.Product.Medicine.MedBag
{
    public class MedBagMedicines : Collection<MedBagMedicine>
    {
        public MedBagMedicines(Medicines medList,bool singleMode)
        {
            if (!singleMode)
            {
                foreach (var m in medList.Where(med => !(med is MedicineVirtual)))
                {
                    switch (m)
                    {
                        case MedicineNHI medicine:
                            Add(new MedBagMedicine(medicine, false));
                            break;
                        case MedicineOTC otc:
                            Add(new MedBagMedicine(otc, false));
                            break;
                        case MedicineSpecialMaterial specialMaterial:
                            Add(new MedBagMedicine(specialMaterial, false));
                            break;
                    }
                }

                var i = 1;
                var order = 1;
                foreach (var g in Items.GroupBy(m => m.Usage).Select(group => group.ToList()).ToList())
                {
                    foreach (var med in g)
                    {
                        med.MedNo = i.ToString();
                        med.Order = order;
                        i++;
                    }
                    order++;
                }


                //foreach (var m in medList.Where(med => !(med is MedicineVirtual)).GroupBy(m => m.Usage))
                //{
                //    switch (m)
                //    {
                //        case MedicineNHI medicine:
                //            Add(new MedBagMedicine(medicine, false));
                //            break;
                //        case MedicineOTC otc:
                //            Add(new MedBagMedicine(otc, false));
                //            break;
                //        case MedicineSpecialMaterial specialMaterial:
                //            Add(new MedBagMedicine(specialMaterial, false));
                //            break;
                //    }
                //}
            }
            else
            {
                foreach (var m in medList.Where(med => !(med is MedicineVirtual)))
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
        public MedBagMedicines(MedicineRefactoring.Medicines medList, bool singleMode)
        {
            foreach (var m in medList.Where(med => !(med is MedicineRefactoring.MedicineVirtual)))
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
