using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class InstitutionFromnhiDTO
    {

        public InstitutionFromnhiDTO(){}
        public string Ins_ID { get; set; }
        public string Ins_Name { get; set; }
        public string Ins_Address { get; set; }
        public string Ins_Telephone { get; set; }
        public DateTime? Ins_EndContractDate { get; set; }

    }
}
