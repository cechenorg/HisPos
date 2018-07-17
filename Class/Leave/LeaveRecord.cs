using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Leave
{
    class LeaveRecord
    {
        public LeaveRecord(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Day = dataRow["EMPLEVREC_DATE"].ToString();
        }

        public string Id { get; set; }
        public string Day { get; set; }
    }
}
