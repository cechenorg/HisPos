using System.Collections.ObjectModel;
using System.Data;
using PharmacyDb = His_Pos.NewClass.Prescription.Treatment.Institution.PharmacyDb;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePharmacy
{
    public class DeclarePharmacies : ObservableCollection<DeclarePharmacy>
    {
        public DeclarePharmacies()
        {
            Init();
        }

        private void Init()
        {
            DataTable table = PharmacyDb.GetCurrentPharmacyRecord();
            foreach (DataRow r in table.Rows)
            {
                Add(new DeclarePharmacy(r));
            }
        }
    }
}