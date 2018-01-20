using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
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
        public string Tel { get; set; }
        public string Email { get; set; }
    }
}
