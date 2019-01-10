using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person
{
    
    public class Customers : Collection<Customer>
    {
        public Customers()
        {

        }

        public void Init()
        {
            var table = EmployeeDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Customer(row));
            }
        }
    }
}
