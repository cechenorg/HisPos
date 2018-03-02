using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public class Customer : AbstractClass.Person
    {
        public string Qname { get; set; }
        public bool Gender { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }
}
