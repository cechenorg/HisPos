using System.Data;

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
