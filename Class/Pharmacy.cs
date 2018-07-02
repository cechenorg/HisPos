using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Pharmacy : Selection
    {
        public Pharmacy()
        {
            MedicalPersonnel = new MedicalPersonnel();
        }

        public Pharmacy(string id, string name)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            MedicalPersonnel = new MedicalPersonnel();
        }
        public MedicalPersonnel MedicalPersonnel { get; set; }
    }
}
