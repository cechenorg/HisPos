using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class DeclareMedicalPersonnel:MedicalPersonnel
    {
        public DeclareMedicalPersonnel(MedicalPersonnel selectedMedicalPersonnel) : base()
        {
            ID = selectedMedicalPersonnel.ID;
            Name = selectedMedicalPersonnel.Name;
            IdNumber = selectedMedicalPersonnel.IdNumber;
            PrescriptionCount = 0;
            //StartDate = r.Field<DateTime>("Emp_StartDate");
            //LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
            //PrescriptionCount = r.Field<int>("");
        }

        public DeclareMedicalPersonnel(DataRow r) : base(r)
        {
            //PrescriptionCount = r.Field<int>("");
        }
        private int prescriptionCount;
        public int PrescriptionCount
        {
            get => prescriptionCount;
            set
            {
                Set(() => PrescriptionCount, ref prescriptionCount, value);
            }
        }
        public string Content
        {
            get => Name + " 處方張數:" + PrescriptionCount;
        }
    }
}
