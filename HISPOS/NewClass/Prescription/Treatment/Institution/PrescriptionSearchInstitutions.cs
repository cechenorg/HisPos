using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class PrescriptionSearchInstitutions : ObservableCollection<PrescriptionSearchInstitution>
    {
        public PrescriptionSearchInstitutions()
        {
        }

        public void GetAdjustedInstitutions()
        {
            var table = InstitutionDb.GetAdjustedInstitutions();
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionSearchInstitution(r));
            }
        }

        public bool IsNeedReSearch { get; set; }
    }
}