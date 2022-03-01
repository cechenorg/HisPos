using System.Data;

namespace His_Pos.NewClass.Person.Employee.ClockIn
{
    public class ClockIn
    {
        public ClockIn()
        {
        }

        public ClockIn(DataRow r)
        {
            EmpNo = r.Field<int>("Emp_ID");
            EmpAccount = r.Field<string>("Emp_Account");
            EmpName = r.Field<string>("Emp_Name");
            Date = r.Field<string>("WDate");
            Time = r.Field<string>("WTime");
            Type = r.Field<string>("Type");
            Time2 = r.Field<string>("WTime2");
            WMin = r?.Field<int>("WMin");
        }

        public int EmpNo { get; set; }
        public string EmpAccount { get; set; }
        public string EmpName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Time2 { get; set; }
        public string Type { get; set; }
        public int? WMin { get; set; }
    }
}