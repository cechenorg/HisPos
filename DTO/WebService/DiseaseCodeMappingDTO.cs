using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class DiseaseCodeMappingDTO
    {
        public DiseaseCodeMappingDTO(){}

        public string DisCodeMap_ICD9_ID { get; set; }
        public string DisCodeMap_ICD9_EngName { get; set; }
        public string DisCodeMap_ICD9_ChiName { get; set; }
        public string DisCodeMap_ICD10_ID { get; set; }
        public string DisCodeMap_Mapping { get; set; }
    }
}
