using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.CustomerHistoryProduct
{
    public class CustomerHistoryProducts:Collection<CustomerHistoryProduct>
    {
        public CustomerHistoryProducts()
        {
            
        }
        public CustomerHistoryProducts(DataTable dataTable)
        {
            foreach (DataRow r in dataTable.Rows)
            {
                Add(new CustomerHistoryProduct(r));
            }
        }
        internal CustomerHistoryProducts GetCustomerHistoryProducts(int id)
        {
            var dataTable = ProductDB.GetCustomerHistoryProducts(id);
            return new CustomerHistoryProducts(dataTable);
        }
    }
}
