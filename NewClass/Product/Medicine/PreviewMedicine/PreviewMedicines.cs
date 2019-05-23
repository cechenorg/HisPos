using System.Collections.Generic;
using System.Collections.ObjectModel;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;

namespace His_Pos.NewClass.Product.Medicine.PreviewMedicine
{
    public class PreviewMedicines : ObservableCollection<PreviewMedicine>
    {
        public PreviewMedicines() { }

        public void AddItemsFromCooperative(IEnumerable<CooperativePrescription.Item> medicineOrderItem)
        {
            foreach (var m in medicineOrderItem)
            {
                Add(new PreviewMedicine(m));
            }
        }

        public void AddItemsFromOrthopedics(IEnumerable<Item> medicineOrderItem)
        {
            foreach (var m in medicineOrderItem)
            {
                Add(new PreviewMedicine(m));
            }
        }
    }
}
