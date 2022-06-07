using System;
using System.Data;
using DomainModel.Enum;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public struct ManufactoryTradeRecord
    {
        #region ----- Define Variables -----

        public OrderTypeEnum OrderType { get; set; }
        public string OrderID { get; set; }
        public string RecOrderID { get; set; }
        public DateTime ReceiveTime { get; set; }
        public double TotalPrice { get; set; }

        #endregion ----- Define Variables -----

        public ManufactoryTradeRecord(DataRow row)
        {
            OrderType = row.Field<string>("StoOrd_Type").Equals("P") ? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN;
            OrderID = row.Field<string>("StoOrd_ID");
            RecOrderID = row.Field<string>("StoOrd_ReceiveID");
            ReceiveTime = row.Field<DateTime>("StoOrd_ReceiveTime");
            TotalPrice = (double)row.Field<decimal>("TOTAL");
        }
    }
}