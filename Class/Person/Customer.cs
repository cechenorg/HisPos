namespace His_Pos.Class.Person
{
    public class Customer : Person
    {
        public Customer()
        {
            IcCard = new IcCard();
        }
        public Customer(IcCard icCard)
        {
            IcCard = icCard;
        }

        public string Qname { get; set; }
        public bool Gender { get; set; }
        public IcCard IcCard { get; set; }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
    }
}
