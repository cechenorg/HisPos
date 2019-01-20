using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Prescription.Treatment;

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
