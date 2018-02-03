namespace His_Pos.Class.Person
{
    public class MedicalPersonnel : AbstractClass.Person
    {
        protected MedicalPersonnel(string id,string name,string icNumber)
        {
            Id = id;
            Name = name;
            IcNumber = icNumber;
        }
    }
}
