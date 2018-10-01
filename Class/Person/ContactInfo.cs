namespace His_Pos.Class.Person
{
    public class ContactInfo
    {
        public ContactInfo()
        {
        }

        public ContactInfo(string address, string tel, string email)
        {
            Address = address;
            Tel = tel;
            Email = email;
        }

        public string Address { get; set; }
        public string Tel { get; set; } = "";
        public string Email { get; set; }
    }
}
