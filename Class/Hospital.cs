using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Hospital : Institution
    {
        public Hospital()
        {
            Doctor = new MedicalPersonnel();
            Division = new Division.Division();
        }

        public MedicalPersonnel Doctor { get; set; }
        public Division.Division Division { get; set; }

        public string GetFullHospitalData()
        {
            return Id + " " + Name;
        }

        public string GetFullDivisonData()
        {
            return Division.Id + ". " + Division.Name;
        }
    }
}
