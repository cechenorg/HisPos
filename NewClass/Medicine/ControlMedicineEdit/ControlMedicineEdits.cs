using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineEdit
{
    public class ControlMedicineEdits : ObservableCollection<ControlMedicineEdit>
    {
        public ControlMedicineEdits(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new ControlMedicineEdit(r));
            }
        }

        public static ControlMedicineEdits GetData(string medID, string warID)
        {
            return new ControlMedicineEdits(ControlMedicineEditDb.GetData(medID, warID));
        }

        public void Update(string medID, string warID)
        {
            ControlMedicineEditDb.Update(medID, warID, this);
        }
    }
}