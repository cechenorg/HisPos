using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Hospital : Institution
    {
        public Hospital(string id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }
        public MedicalPersonnel Doctor { get; set; }
        public Division.Division Division { get; set; }
        
    }
}
