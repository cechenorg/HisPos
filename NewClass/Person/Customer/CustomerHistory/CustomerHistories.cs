using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistories : Collection<CustomerHistory>
    {
        public CustomerHistories()
        {
        }

        public CustomerHistories(int id)
        {
            var table = CustomerHistoryDb.GetData(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new CustomerHistory(r));
            }
        }
    }
}