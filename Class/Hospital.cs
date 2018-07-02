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
        public Hospital(string id, string name)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            Doctor = new MedicalPersonnel();
            Division = new Division.Division();
        }

        public Hospital(DataRow dataRow)
        {
            Id = dataRow["INS_ID"].ToString();
            Name = dataRow["INS_NAME"].ToString();
            FullName = Id + " " + Name;
        }

        public MedicalPersonnel Doctor { get; set; }
        public Division.Division Division { get; set; }
    }
}
