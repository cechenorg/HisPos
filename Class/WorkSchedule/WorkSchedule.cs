using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.WorkSchedule
{
    public class WorkSchedule
    {
        public WorkSchedule(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Day = dataRow["EMPSCH_DATE"].ToString();
            Period = dataRow["EMPSCH_PERIOD"].ToString();
        }

        public string Id { get; set; }
        public string Day { get; set; }
        public string Period { get; set; }
    }
}
