using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    internal class ChronicPrescription : Prescription
    {
        public int ChronicSequence { get; set; }
        public int ChronicTotal { get; set; }
        public string OriginalMedicalNumber { get; set; } //d43 原處方就醫序號
    }
}
