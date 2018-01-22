using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Hospital : Institution
    {
        public Hospital(string id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }
        public MedicalPersonnel Doctor { get; set; }
        public Division Division { get; set; }
        
    }
}
