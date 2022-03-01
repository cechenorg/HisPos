using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
    public class PrescriptionDetailMedicineRepots : ObservableCollection<PrescriptionDetailMedicineRepot>
    {
        public PrescriptionDetailMedicineRepots()
        {
        }

        public void GerDataById(int Id)
        {
            Clear();
            DataTable table = PrescriptionDetailMedicineRepotDb.GetDataById(Id);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionDetailMedicineRepot(r));
            }
        }
    }
}