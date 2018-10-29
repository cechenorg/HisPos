using System.Data;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryPos : CustomerHistoryDetail
    {
        public CustomerHistoryPos(DataRow dataRow)
        {
            ProductName = dataRow["COL0"].ToString(); ;
            Price = dataRow["COL1"].ToString(); ;
            Amount = dataRow["COL2"].ToString();;
            Subtotal = dataRow["COL3"].ToString(); ;
        }

        public string ProductName { get; }
        public string Price { get; }
        public string Amount { get; }
        public string Subtotal { get; }
    }
}