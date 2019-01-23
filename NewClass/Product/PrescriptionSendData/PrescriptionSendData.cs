using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class PrescriptionSendData : ObservableObject
    {
        public PrescriptionSendData()
        {
        }

        public string MedId { get; set; }
        public string MedName { get; set; }
        public string Stock { get; set; }
        public string TreatAmount { get; set; }
        public string SendAmount { get; set; }
        
    }
}
