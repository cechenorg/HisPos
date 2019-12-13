using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

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
        #endregion

        public TradeProduct() { }

        public TradeProduct(DataRow row) : base(row)
        {

        }

        #region ----- Define Functions -----
        #endregion
    }
}
