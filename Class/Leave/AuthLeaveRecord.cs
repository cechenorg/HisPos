using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Leave
{
    public class AuthLeaveRecord
    {
        public AuthLeaveRecord(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            LeaveType = dataRow["EMPLEVTYP_NAME"].ToString();
            Dates = dataRow["LEAVEDATE"].ToString();
            Notes = dataRow["EMPLEVREC_NOTE"].ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string LeaveType { get; set; }
        public string Dates { get; set; }
        public string Notes { get; set; }
    }
}
