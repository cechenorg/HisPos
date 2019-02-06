using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFilePreview
{
    public class DeclareFilePreview
    {
        public DeclareFilePreview()
        {

        }
        public int DeclareMonth { get; set; }
        public int NormalCount { get; set; }
        public int ChronicCount { get; set; }
        public int SimpleFormCount { get; set; }
        public int TotalPoint { get; set; }
        public Prescriptions DeclarePrescriptions { get; set; }
    }
}
