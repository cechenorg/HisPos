using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public class MedicineInventoryStructs : Collection<MedicineInventoryStruct>
    {
        public MedicineInventoryStructs()
        {
        }

        public void GetUsableAmountByPrescriptionID(int id)
        {
            var table = MedicineDb.GetUsableAmountByPrescriptionID(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicineInventoryStruct(r.Field<int>("Inv_ID"), r.Field<double>("CanUseAmount")));
            }
        }

        public void GetUsableAmountByReserveID(int id)
        {
            var table = MedicineDb.GetUsableAmountByReserveID(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicineInventoryStruct(r.Field<int>("Inv_ID"), r.Field<double>("CanUseAmount")));
            }
        }
    }
}