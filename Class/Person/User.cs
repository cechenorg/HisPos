﻿using System.Data;
using MahApps.Metro.Controls;

namespace His_Pos.Class.Person
{
    public class User : Person
	{
	    public User() : base()
	    {

	    }

        public User(DataRow dataRow) : base(dataRow)
	    {
	        
	    }
        
	    public Pharmacy Pharmacy { get; set; }
        public string Position { get; set; }
        public Leave Leave { get; set; }= new Leave();
	    public ContactInfo ContactInfo { get; set; }
        public Authority Authority { get; set; } = new Authority();
	}
}
