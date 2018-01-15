using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Medicate
    {
        public Medicate(double dosage, string usage, int days, string position)
        {
            Dosage = dosage;
            Usage = usage;
            Days = days;
            Position = position;
        }

        public double Dosage { get; set; }
        public string Usage { get; set; }
        public int Days { get; set; }
        public string Position { get; set; }
    }
}
