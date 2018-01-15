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

        private string Address { get; set; }
        private string Tel { get; set; }
        private string Email { get; set; }

        public void SetAddress(string address)
        {
            Address = address;
        }
        public void SetTel(string tel)
        {
            Tel = tel;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }
    }
}
