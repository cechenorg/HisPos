using System.Data;

namespace His_Pos.Class.Person
{
    public class User : Person
	{
	    public User() : base()
	    {

	    }

        public User(DataRow dataRow) : base(dataRow)
	    {
            Authority.AuthorityValue = dataRow["EMP_GROUP"].ToString();
	    }
        
	    public Pharmacy.Pharmacy Pharmacy { get; set; }
        public string Position { get; set; }
	    public ContactInfo ContactInfo { get; set; }
        public Authority.Authority Authority { get; set; } = new Authority.Authority();
        public string password { get; set; }
	}
}
