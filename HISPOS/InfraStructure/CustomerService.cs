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

            if (!string.IsNullOrEmpty(customer.CellPhone))
            {
                if(customer.CellPhone.Length == 10)
                    customer.CellPhone = customer.CellPhone.Insert(4, "-").Insert(8, "-");

                if(customer.SecondPhone.Length == 10)
                    customer.SecondPhone = customer.SecondPhone.Insert(4, "-").Insert(8, "-");
            }
            /* 格式化電話 */
            if (!string.IsNullOrEmpty(customer.Tel))
            {
                var telLen = customer.Tel.Length;
                string FormatTel = default;
                switch (telLen)
                {
                    case 7:
                        FormatTel = customer.Tel.Insert(3, "-");
                        break;
                    case 8:
                        FormatTel = customer.Tel.Insert(4, "-");
                        break;
                    case 9:
                        FormatTel = customer.Tel.Insert(2, "-").Insert(6, "-");
                        break;
                    case 10:
                        FormatTel = customer.Tel.Insert(2, "-").Insert(7, "-");
                        break;

                }
                customer.Tel = FormatTel;
            }

            return customer;
        }
    }
}
