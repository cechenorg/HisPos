using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class MedicalPersonnel : Person
    {
        protected MedicalPersonnel(string id,string name,string icNumber)
        {
            Id = id;
            Name = name;
            IcNumber = icNumber;
        }
    }
}
