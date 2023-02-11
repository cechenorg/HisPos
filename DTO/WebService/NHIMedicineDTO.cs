using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDTO
{
    public class NHIMedicineDTO
    {
        public string Med_ID { get; set; }
        public string Med_Name { get; set; }
        public string Med_EngName { get; set; }
        public float Med_Amount { get; set; }
        public string Med_Unit { get; set; }
        public double Med_Price { get; set; }
        public string Med_Manufactor { get; set; }
        public string Med_Form { get; set; }
        public string Med_SingleCompound { get; set; }
        public string Med_Ingredient { get; set; }
        public string Med_ATC { get; set; }
        public int Med_Control { get; set; }
        public bool Med_IsFrozen { get; set; }
        public string Med_NhiNote { get; set; }
        public DateTime Med_StartDate { get; set; }
        public DateTime Med_EndDate { get; set; }
    }
}
