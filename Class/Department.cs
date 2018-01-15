using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Department
    {
        public Department(string id, string name)
        {
            Id = id;
            Name = name;
        }

        private string Id { get; set; }
        private string Name { get; set; }
        private string Tel { get; set; }
        private string Fax { get; set; }

        public void SetTel(string tel)
        {
            Tel = tel;
        }

        public void SetFax(string fax)
        {
            Fax = fax;
        }
    }
}
