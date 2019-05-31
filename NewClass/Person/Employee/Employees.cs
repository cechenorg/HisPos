using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.Employee
{ 
    public class Employees : ObservableCollection<Employee>
    {
        public Employees()
        {

        }

        public void Init()
        {
            Clear();
            var table = EmployeeDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Employee(row));
            }
        }
       
    }
}
