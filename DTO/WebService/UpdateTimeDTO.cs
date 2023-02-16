using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class UpdateTimeDTO
    {
        public UpdateTimeDTO()
        {
        }

        public string UpdTime_TableName { get; set; }
        public DateTime UpdTime_LastUpdateTime { get; set; }
    }
}
