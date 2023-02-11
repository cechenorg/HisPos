using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDTO
{
    public class NHISmokeMedicineDTO
    {
        public string SmoMed_ID { get; set; }
        public string SmoMed_Name { get; set; }
        public string SmoMed_EngName { get; set; }
        public string SmoMed_Ingredient { get; set; }
        public double SmoMed_Price { get; set; }
        public string SmoMed_Manufactory { get; set; }
        public string SmoMed_ForeignMedicine { get; set; }
    }
}
