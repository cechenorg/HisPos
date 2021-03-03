using System.Collections.ObjectModel;
using System.Data;

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
    }
}