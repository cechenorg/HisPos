using System.Data;

namespace His_Pos.Struct.StoreOrder
{
    public struct StoreOrderOverview
    {
        public StoreOrderOverview(DataRow dataRow)
        {
            Id = dataRow["STOORD_ID"].ToString();
            OrderEmp = dataRow["ORD_EMP"].ToString();
            ReceiveEmp = dataRow["REC_EMP"].ToString();
            ReceiveDate = dataRow["STOORD_RECDATE"].ToString();
        }
        
        public string Id { get; }
        public string OrderEmp { get; }
        public string ReceiveEmp { get; }
        public string ReceiveDate { get; }
    }
}