using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    internal class DeclareData
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
        public Prescription Prescription {get;set;}
        public ChronicPrescription ChronicPrescription {get; set;}
    }
}
