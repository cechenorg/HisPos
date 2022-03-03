using System.Collections.Generic;
using InfraStructure.SQLService.SQLServer.StoreOrder;
using System.Collections.ObjectModel;
using System.Data;
using DomainModel.Class.StoreOrder;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrders : Collection<SingdeTotalOrder>
    {
        private SingdeTotalOrders(List<dSingdeTotalOrder> datas)
        {
            foreach (dSingdeTotalOrder row in datas)
            {
                Add(new SingdeTotalOrder(row));
            }
         
        }

        internal static SingdeTotalOrders GetSingdeTotalOrders()
        {
            StoreOrderService storeOrderService = new StoreOrderService(Properties.Settings.Default.SQL_localWithDB); 

            return new SingdeTotalOrders(storeOrderService.Get_SingdeTotalOrdersNotDone());
        }
    }
}