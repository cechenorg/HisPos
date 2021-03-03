namespace His_Pos.Class.Product
{
    public class OTCStoreOrderOverview
    {
        public string StoreOrderId { get; }
        public string StoreOrderDate { get; }
        public string StoreValidDate { get; }
        public string OrderEmployee { get; }
        public string StoreReceiveDate { get; }
        public string Price { get; }
        public string Amount { get; }
        public string FreeAmount { get; }
        public string ManufactoryName { get; }

        public OTCStoreOrderOverview(string storeOrderId, string manufactoryName, string storeOrderDate, string storeValidDate, string orderEmployee, string storeReceiveDate, string price, string amount, string freeAmount)
        {
            StoreOrderId = storeOrderId;
            ManufactoryName = manufactoryName;
            StoreOrderDate = storeOrderDate;
            StoreValidDate = storeValidDate;
            OrderEmployee = orderEmployee;
            StoreReceiveDate = storeReceiveDate;
            Price = price;
            Amount = amount;
            FreeAmount = freeAmount;
        }
    }
}