using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Employee
{
    public class EmployeeService
    {

        public static Employee GetDataByID(int id)
        { 
            return EmployeeDb.GetData().SingleOrDefault(_ => _.ID == id);
        }

        public static void Insert(Employee emp)
        {
            EmployeeDb.Insert(emp);
        }

        public static void Update(Employee emp)
        {

            EmployeeDb.Update(emp);
        }

        public static void Delete(Employee emp)
        {
            EmployeeDb.Delete(emp.ID);
        }
    }
}
