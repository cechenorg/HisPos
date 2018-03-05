using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Pharmacy : Institution
    {
        public Pharmacy()
        {
            MedicalPersonnel = new MedicalPersonnel();
        }

        public Pharmacy(string id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
            MedicalPersonnel = new MedicalPersonnel();
        }
        public MedicalPersonnel MedicalPersonnel { get; set; }
    }
}
