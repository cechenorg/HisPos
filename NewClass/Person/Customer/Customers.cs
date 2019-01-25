using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ImportDeclareXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customers:ObservableCollection<Customer>
    {
        public Customers()
        {

        }
        public void GetDataByCondition(Customer customer) { 
            DataTable table = CustomerDb.GetDataByCondition(customer);
            foreach (DataRow r in table.Rows) {
                Add(new Customer(r));
            }
        }

        public void Init()
        {
            var table = CustomerDb.GetData();
            foreach (DataRow r in table.Rows)
            {
                Add(new Customer(r));
            }
        }
        public Customers SetCustomersByPrescriptions(List<ImportDeclareXml.Ddata> ddatas) {
            DataTable table = CustomerDb.SetCustomersByPrescriptions(ddatas);
            Customers customers = new Customers();
            foreach (DataRow r in table.Rows) {
                customers.Add(new Customer(r));
            }
            return customers;
        }
    }
}
