using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class Institutions:Collection<Institution>
    {
        public Institutions(bool isInit)
        {
            if(isInit)
                Init();
        }

        private void Init()
        {
            var table = InstitutionDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Institution(row));
            }
        }

        public void GetCommon()//取得常用院所
        {
            var table = InstitutionDb.GetCommonInstitution();
            foreach (DataRow row in table.Rows)
            {
                Add(new Institution(row));
            }
        }
    }
}
