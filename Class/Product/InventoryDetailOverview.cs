using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Class.Product
{
    public class InventoryDetailOverview
    {
        public InventoryDetailOverview(DataRow dataRow)
        {
            Id = dataRow["PROINVREC_FOREIN"].ToString();

            switch (dataRow["CHANGE_TYPE"].ToString())
            {
                case "調劑耗用":
                    TypeIcon = "/Images/OrangeDot.png";
                    Type = InventoryDetailOverviewType.Treatment;
                    break;
                case "進貨":
                case "退貨":
                    TypeIcon = "/Images/BlueDot.png";
                    Type = InventoryDetailOverviewType.PurchaseReturn;
                    break;
                case "盤點":
                    TypeIcon = "/Images/GreenDot.png";
                    Type = InventoryDetailOverviewType.StockTaking;
                    break;
                default:
                    TypeIcon = "";
                    Type = InventoryDetailOverviewType.Error;
                    break;
            }

            Time = dataRow["CHANGE_TIME"].ToString();
            Amount = Double.Parse(dataRow["AMOUNT"].ToString());
            Stock = 0;
        }

        public string Id { get; }
        public string TypeIcon { get; }
        public InventoryDetailOverviewType Type { get; }
        public string Time { get; }
        public double Amount { get; }
        public double Stock { get; set; }
    }
}
