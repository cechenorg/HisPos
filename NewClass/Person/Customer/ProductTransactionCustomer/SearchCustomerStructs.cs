using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Customer.ProductTransactionCustomer
{
    public class SearchCustomerStructs : Collection<SearchCustomerStruct>
    {
        private SearchCustomerStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new SearchCustomerStruct(row));
            }
        }

        #region ----- Define Functions -----

        #endregion
    }
}
