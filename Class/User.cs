using System.Collections.Generic;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class User : Person
	{
	    public string Password { get; set; }
	    public Department Department { get; set; }
        public Leave Leave { get; set; }= new Leave();
	    public ContactInfo ContactInfo { get; set; }
        public Authority Authority { get; set; } = new Authority();
	}
}
