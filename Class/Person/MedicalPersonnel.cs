using System.Data;
using System.Xml;

namespace His_Pos.Class.Person
{
    public class MedicalPersonnel : Person
    {
        public MedicalPersonnel()
        {
        }

        public MedicalPersonnel(DataRow dataRow) : base(dataRow)
        {
        }

        public MedicalPersonnel(DataRow row,bool isDoctor)
        {
            if (isDoctor)
            {
                Id = string.Empty;
                Name = string.Empty;
                IcNumber = row["HISDECMAS_DOCTOR"].ToString();
            }
            else
            {
                Id = row["EMP_ID"].ToString();
                Name = row["EMP_NAME"].ToString();
                IcNumber = row["EMP_IDNUM"].ToString();
            }
        }
        public MedicalPersonnel(string id, string name, string icNumber)
        {
            Id = id;
            Name = name;
            IcNumber = icNumber;
        }
    }
}