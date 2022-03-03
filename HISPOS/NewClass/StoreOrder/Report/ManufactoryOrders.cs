using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrders : Collection<ManufactoryOrder>
    {
        private ManufactoryOrders(DataTable dataTable)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Add(new ManufactoryOrder(dataRow));
            }
        }

        internal static ManufactoryOrders GetManufactoryOrdersBySearchCondition(DateTime? startDate, DateTime? endDate, string manufactoryName, string wareID)
        {
            return new ManufactoryOrders(StoreOrderDB.GetManufactoryOrdersBySearchCondition(startDate, endDate, manufactoryName, wareID));
        }
    }
}