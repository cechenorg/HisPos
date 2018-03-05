namespace His_Pos.Class.Person
{
    public class Customer : AbstractClass.Person
    {
        public string Qname { get; set; }
        public bool Gender { get; set; }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
    }
}
