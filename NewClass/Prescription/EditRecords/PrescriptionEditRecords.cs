using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.EditRecords
{
    public class PrescriptionEditRecords : ObservableCollection<PrescriptionEditRecord>
    {
        public PrescriptionEditRecords()
        {
        }

        public void GetData(string prescriptionID)
        {
            var table = PrescriptionDb.GetEditedRecords(prescriptionID);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionEditRecord(r));
            }
            //this.OrderBy(r => r.ProductID).ThenBy(r => r.Time);
        }
    }
}