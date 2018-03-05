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

        public Otc()
        {
        }

        public int Total { get; set; }//商品數量
        public double TotalPrice { get; set; }//商品總價
    }
}
