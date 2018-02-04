using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
        }
        private Pharmacy Pharmacy { get; set; }
        private Treatment Treatment { get; set; }
        private List<Medicine> Medicines { get; set; }
    }
}
