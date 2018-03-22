using System.Windows.Media.Imaging;

namespace His_Pos.AbstractClass
{
    public class Product
    {
        public BitmapImage TypeIcon { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }
        public double Inventory { get; set; }
        public string SafeAmount { get; set; } 
        public string ManufactoryName { get; set; }
        public string LastCheckDate { get; set; }
        public string Note { get; set; }
    }
}
