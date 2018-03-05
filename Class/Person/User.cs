using MahApps.Metro.Controls;

namespace His_Pos.Class.Person
{
    public class User : AbstractClass.Person
	{
	    public string Password { get; set; }
	    public Pharmacy Pharmacy { get; set; }
        public string Position { get; set; }
        public Leave Leave { get; set; }= new Leave();
	    public ContactInfo ContactInfo { get; set; }
        public Authority Authority { get; set; } = new Authority();
	}
}
