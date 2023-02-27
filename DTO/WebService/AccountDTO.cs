using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class AccountDTO
    {

        public AccountDTO(){}

        public string acct_Level { get; set; }
        public string acct_PreLevel { get; set; }
        public string acct_ID { get; set; }
        public string acct_Name { get; set; }
        public bool acct_Enable { get; set; }
        public DateTime? acct_InsertTime { get; set; }
        public DateTime? acct_LastModifyTime { get; set; }
        public int? acct_LastModifyEmpID { get; set; }

    }
}
