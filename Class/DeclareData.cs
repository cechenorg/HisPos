using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class DeclareData
    {
        public DeclareData(Prescription prescription)
        {
            Prescription = new Prescription();
            Prescription = prescription;
        }
        public DeclareData(ChronicPrescription chronicPrescription)
        {
            ChronicPrescription = new ChronicPrescription();
            ChronicPrescription = chronicPrescription;
        }

        Prescription Prescription {get;set;}
        private ChronicPrescription ChronicPrescription { get; set; }
    }
}
