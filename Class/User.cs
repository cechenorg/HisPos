using System.Collections.Generic;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class User : IPerson
	{
	    #region --IPerson--
	    public string Id { get; set; }
	    public string Name { get; set; }
	    public string IcNumber { get; set; }
	    #endregion
	    public string Password { get; set; }
        public string Birthday { get; set; }
        public string Position { get; set; }
	    public Department Department { get; set; }
        public Leave Leave { get; set; }
	    public ContactInfo ContactInfo { get; set; }
        public Authority Authority { get; set; } = new Authority();
	}
}
