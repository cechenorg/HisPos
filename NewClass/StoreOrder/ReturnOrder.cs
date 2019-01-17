using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.NewClass.StoreOrder
{
    public class ReturnOrder: StoreOrder
    {
        #region ----- Define Variables -----
        private ReturnProducts orderProducts;

        public ReturnProducts OrderProducts
        {
            get { return orderProducts; }
            set { Set(() => OrderProducts, ref orderProducts, value); }
        }
        public int ProductCount
        {
            get
            {
                if (OrderProducts is null) return initProductCount;
                else return OrderProducts.Count;
            }
        }
        #endregion

        public ReturnOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.RETURN;
        }

        public override void GetOrderProducts()
        {
            OrderProducts = ReturnProducts.GetProductsByStoreOrderID(ID);
        }

        public override void SaveOrder()
        {
        }

        public override void AddProductByID(string iD)
        {
            
        }
    }
}
