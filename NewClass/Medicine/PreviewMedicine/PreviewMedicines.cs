using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.PreviewMedicine
{
    public class PreviewMedicines : ObservableCollection<PreviewMedicine>
    {
        public PreviewMedicines()
        {
        }

        public PreviewMedicines(PreviewMedicines medicines)
        {
            foreach (var m in medicines)
            {
                Add(m);
            }
        }

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

        public void GetDataByPrescriptionId(int id)
        {
            var table = MedicineDb.GetDataByPrescriptionId(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new PreviewMedicine(r));
            }
        }

        public void GetDataByReserveId(int id)
        {
            var table = MedicineDb.GetDataByReserveId(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new PreviewMedicine(r));
            }
        }
    }
}