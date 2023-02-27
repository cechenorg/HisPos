using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class PharmacyDTO
    {

        public PharmacyDTO(){}
        public string PHAMAS_ID { get; set; }
        public string PHAMAS_NAME { get; set; }
        public string PHAMAS_MEDICALNUM { get; set; }
        public string PHAMAS_TEL { get; set; }
        public string PHAMAS_ADDR { get; set; }
        public DateTime PHAMAS_VALIDUSEDATE { get; set; }
        public string PHAMAS_VerifyKey { get; set; }
        public string PHAMAS_DbTarget { get; set; }
        public string PHAMAS_GroupServer { get; set; }
        public string PHAMAS_TAXNUM { get; set; }
        public string PHAMAS_GUID { get; set; }

    }
}
