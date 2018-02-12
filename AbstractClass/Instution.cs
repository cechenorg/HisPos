using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.Interface;

namespace His_Pos.AbstractClass
{
    public class Institution : Selection
    {
        public Institution(string id,string name)
        {
            Id = id;
            Name = name;
        }

        protected Institution()
        {
            
        }
    }
}
