namespace His_Pos.Class.Person
{
    public class MedicalPersonnel : Person
    {
        public MedicalPersonnel()
        {
        }

        public MedicalPersonnel(string id,string name,string icNumber)
        {
            Id = id;
            Name = name;
            IcNumber = icNumber;
        }
    }
}
