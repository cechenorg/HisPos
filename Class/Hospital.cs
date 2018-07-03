using System.Data;
using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Hospital : Selection
    {
        public Hospital()
        {
            Doctor = new MedicalPersonnel();
            Division = new Division.Division();
        }

        public Hospital(DataRow dataRow)
        {
            Id = dataRow["INS_ID"].ToString();
            Name = dataRow["INS_NAME"].ToString();
            FullName = dataRow["FULLNAME"].ToString();
            Doctor = new MedicalPersonnel();
            Division = new Division.Division();
        }

        public MedicalPersonnel Doctor { get; set; }
        public Division.Division Division { get; set; }
    }
}
