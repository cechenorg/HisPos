using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Employee;

namespace His_Pos.NewClass.Person
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
