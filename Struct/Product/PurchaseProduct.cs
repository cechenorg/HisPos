using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Struct.Product
{
    public struct PurchaseProduct
    {
        public PurchaseProduct(DataRow dataRow)
        {
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            ChiName = dataRow["PRO_CHI"].ToString();
            EngName = dataRow["PRO_ENG"].ToString();
            Inventory = Double.Parse((dataRow["PRO_INVENTORY"].ToString() == "")? "0" : dataRow["PRO_INVENTORY"].ToString());
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            BasicAmount = dataRow["PRO_BASICQTY"].ToString();
            LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
            Mans = dataRow["MANS"].ToString();
            Type = dataRow["TYPE"].ToString();
            WarId = dataRow["PROWAR_ID"].ToString(); 
            Status = Boolean.Parse(dataRow["PRO_STATUS"].ToString());
            OnTheWayAmount = dataRow["PRO_ONTHEWAY"].ToString(); 
        }

        public string Mans;
        public string Id { get; }
        public string Name { get; }
        public string ChiName;
        public string EngName;
        public string WarId;
        public double Inventory { get; }
        public string SafeAmount { get; }
        public string BasicAmount { get; }
        public string OnTheWayAmount { get; }
        public double LastPrice;
        public string Type;
        public bool Status;
    }
}
