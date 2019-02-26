using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class MedicalPersonnels:Collection<MedicalPersonnel>
    {
        public MedicalPersonnels()
        {
            Init();
        }

        private void Init()
        {
            var tabel = MedicalPersonnelDb.GetData();
            foreach (DataRow r in tabel.Rows)
            {
                Add(new MedicalPersonnel(r));
            }
        }
    }
}
