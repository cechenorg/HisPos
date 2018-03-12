using System.Data;

namespace His_Pos.Class.Product
{
    public class Otc : AbstractClass.Product
    {
        public Otc(string id, string name, string price, double inventory)
        {
            Id = id;
            Name = name;
            Price = double.Parse(price);
            Inventory = inventory;
        }

        public Otc(DataRow dataRow)
        {
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            Price = double.Parse(dataRow["PRO_PRICE"].ToString());
            Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString());
            Cost = double.Parse(dataRow["PRO_PROFIT"].ToString());
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            ManufactoryName = dataRow["MAN_NAME"].ToString();
        }

        public int Total { get; set; }//商品數量
        public double TotalPrice { get; set; }//商品總價
    }
}
