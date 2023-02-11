using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDTO
{
    public class NHISpecialMedicineDTO
    {
        public string SpeMed_ProductID { get; set; }
        public string SpeMed_Name { get; set; }
        public string SpeMed_EngName { get; set; }
        public string SpeMed_BigCategory { get; set; }
        public string SpeMed_SmallCategory { get; set; }
        public string SpeMed_ChineseCategory { get; set; }
        public string SpeMed_Unit { get; set; }
        public decimal SpeMed_NHIPrice { get; set; }
        public string SpeMed_Manufactory { get; set; }
        public string SpeMed_DocumentID { get; set; }
        public DateTime SpeMed_StartDate { get; set; }
        public DateTime SpeMed_EndDate { get; set; }
    }
}
