using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDTO
{
    public class InstitutionDTO
    {
        public string Ins_ID { get; set; }
        public string Ins_Name { get; set; }
        public string Ins_Address { get; set; }
        public string Ins_Telephone { get; set; }
        public DateTime Ins_EndContractDate { get; set; }
        public string Ins_InsertPharmacy { get; set; }
        public string Ins_Type { get; set; }
    }
}
