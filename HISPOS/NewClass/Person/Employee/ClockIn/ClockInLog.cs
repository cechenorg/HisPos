using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.Employee.ClockIn
{
    public class ClockInLog : Collection<ClockIn>
    {
        public ClockInLog(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ClockIn(row));
            }
        }


    }
}