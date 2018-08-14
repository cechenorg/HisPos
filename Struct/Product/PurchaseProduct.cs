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
            IsThisMan = Boolean.Parse(dataRow["IS_MAN"].ToString());
            Type = dataRow["TYPE"].ToString();
            Status = Boolean.Parse(dataRow["PRO_STATUS"].ToString());
        }

        public string Id { get; }
        public string Name { get; }
        public string ChiName;
        public string EngName;
        public double Inventory { get; }
        public string SafeAmount { get; }
        public string BasicAmount { get; }
        public double LastPrice;
        public bool IsThisMan;
        public string Type;
        public bool Status;
    }
}
