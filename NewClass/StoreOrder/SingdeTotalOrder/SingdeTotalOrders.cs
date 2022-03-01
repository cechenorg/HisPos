using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrders : Collection<SingdeTotalOrder>
    {
        private SingdeTotalOrders(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new SingdeTotalOrder(row));
            }
        }

        internal static SingdeTotalOrders GetSingdeTotalOrders()
        {
            return new SingdeTotalOrders(StoreOrderDB.GetSingdeTotalOrders());
        }
    }
}