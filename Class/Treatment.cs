using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Treatment
    {
        public Hospital Hospital { get; set; }
        public Division Division { get; set; }
        public MedicalPersonnel Doctor { get; set; }
        public SpecialCode SpecialCode { get; set; }
        public PaymentCategory PaymentCategory { get; set; }
        public Copayment Copayment { get; set; }
        public TreatmentCase TreatmentCase { get; set; }
        public AdjustCase AdjustCase { get; set; }
        public List<DiseaseCode> DiseaseCodes { get; set; } = new List<DiseaseCode>();
        public string TreatmentDate { get; set; }
        public string AdjustDate { get; set; }
        public string MedicineDays { get; set; }
    }
}
