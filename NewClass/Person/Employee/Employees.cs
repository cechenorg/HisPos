using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.Employee
{ 
    public class Employees : Collection<Employee>
    {
        public Employees()
        {

        }

        public void Init()
        {
            var table = EmployeeDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Employee(row));
            }
        }
    }
}
