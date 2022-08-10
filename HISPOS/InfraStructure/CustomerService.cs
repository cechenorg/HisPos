using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.InfraStructure
{
    public class CustomerService
    {

        public static Customer GetCustomerByCusId(int cusId)
        {
            DataTable table = CustomerDb.GetCustomerByCusId(cusId);
            var customer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            /* 格式化手機 */
            if (!string.IsNullOrEmpty(customer.CellPhone) && customer.CellPhone.Length == 10)
            {
                string FormatCell = customer.CellPhone.Insert(4, "-").Insert(8, "-");
                customer.CellPhone = FormatCell;
            }
            if (!string.IsNullOrEmpty(customer.SecondPhone) && customer.SecondPhone.Length == 10)
            {
                string FormatCell = customer.SecondPhone.Insert(4, "-").Insert(8, "-");
                customer.SecondPhone = FormatCell;
            }
            /* 格式化電話 */
            if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 7)
            {
                string FormatTel = customer.Tel.Insert(3, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 8)
            {
                string FormatTel = customer.Tel.Insert(4, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 9)
            {
                string FormatTel = customer.Tel.Insert(2, "-").Insert(6, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 10)
            {
                string FormatTel = customer.Tel.Insert(2, "-").Insert(7, "-");
                customer.Tel = FormatTel;
            }
            return customer;
        }
    }
}
