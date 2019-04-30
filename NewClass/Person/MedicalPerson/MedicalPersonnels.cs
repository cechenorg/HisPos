using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class MedicalPersonnels:Collection<MedicalPersonnel>
    {
        public MedicalPersonnels(bool init)
        {
            if(init)
                Init();
        }

        private void Init()
        {
            var table = MedicalPersonnelDb.GetData();
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicalPersonnel(r));
            }
        }

        public void GetEnablePharmacist(DateTime start,DateTime end)
        {
            var table = MedicalPersonnelDb.GetEnableMedicalPersonnels(start, end);
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicalPersonnel(r));
            }
        }
    }
}
