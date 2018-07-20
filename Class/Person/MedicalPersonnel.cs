using System.Data;

namespace His_Pos.Class.Person
{
    public class MedicalPersonnel : Person
    {
        public MedicalPersonnel()
        {
        }

        public MedicalPersonnel(DataRow row)
        {
            Id = row["EMP_ID"].ToString();
            Name = row["EMP_NAME"].ToString();
            IcNumber = row["EMP_IDNUM"].ToString();
        }

        public MedicalPersonnel(string id, string name, string icNumber)
        {
            Id = id;
            Name = name;
            IcNumber = icNumber;
        }
    }
}