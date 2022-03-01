using His_Pos.Interface;
using System.Data;

namespace His_Pos.NewClass.Product.ProductTransaction
{
    public class TradeProduct : Product, IDeletableProduct
    {
        #region ----- Define Variables -----

        private bool isSelected = false;

        public int OrderNumber { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public int DiscountPrice { get; set; }
        public double DiscountPercent { get; set; }
        public double SubTotal { get; set; }
        public int Point { get; set; }
        public string Doctor { get; set; }
        public double Inventory { get; set; }

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }

        #endregion ----- Define Variables -----

        public TradeProduct()
        {
        }

        public TradeProduct(DataRow row) : base(row)
        {
        }
    }
}