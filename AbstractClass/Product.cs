using System.Security.Permissions;
using System.Windows.Media.Imaging;

namespace His_Pos.AbstractClass
{
    public class Product
    {
        public BitmapImage TypeIcon { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public double LastPrice { get; set; }
        public double TotalPrice { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public double Cost { get; set; }
        public double Inventory { get; set; }
        public string SafeAmount { get; set; } 
        public string ManufactoryName { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
        public string BasicAmount { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string Location { get; set; }
    }
}
