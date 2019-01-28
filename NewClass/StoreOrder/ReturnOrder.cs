using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
            StoreOrderDB.SaveReturnOrder(this);
        }

        public override void AddProductByID(string iD)
        {


            //DataTable dataTable = PurchaseReturnProductDB.GetReturnProductByProductID(iD);

            //ReturnProduct returnProduct;

            //switch (dataTable.Rows[0].Field<string>(""))
            //{
            //    case "O":
            //        returnProduct = new ReturnOTC(dataTable.Rows[0]);
            //        break;
            //    case "M":
            //        returnProduct = new ReturnMedicine(dataTable.Rows[0]);
            //        break;
            //    default:
            //        returnProduct = new ReturnProduct();
            //        break;
            //} 

            //OrderProducts.Add(returnProduct);
        }

        protected override bool CheckUnProcessingOrder()
        {
            if (OrderProducts.Count == 0)
            {
                MessageWindow.ShowMessage("退貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            return false;
        }

        protected override bool CheckNormalProcessingOrder()
        {
            throw new NotImplementedException();
        }

        protected override bool CheckSingdeProcessingOrder()
        {
            throw new NotImplementedException();
        }
    }
}
