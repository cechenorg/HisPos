using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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

        #region ----- Override Function -----
        public override void CalculateTotalPrice()
        {
            TotalPrice = OrderProducts.Sum(p => p.SubTotal);
        }
        public override void GetOrderProducts()
        {
            OrderProducts = ReturnProducts.GetProductsByStoreOrderID(ID);
        }

        public override void SaveOrder()
        {
            ReturnOrder returnOrder = this.Clone() as ReturnOrder;
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                StoreOrderDB.SaveReturnOrder(returnOrder);
            };

            backgroundWorker.RunWorkerAsync();
        }

        public override void AddProductByID(string iD, bool isFromAddButton)
        {
            if (OrderProducts.Count(p => p.ID == iD) > 0)
            {
                MessageWindow.ShowMessage("訂單中已有 " + iD + " 商品", MessageType.ERROR);
                return;
            }

            DataTable dataTable = PurchaseReturnProductDB.GetReturnProductByProductID(iD);
            
            ReturnProduct returnProduct;

            switch (dataTable.Rows[0].Field<string>("TYPE"))
            {
                case "O":
                    returnProduct = new ReturnOTC(dataTable.Rows[0]);
                    break;
                case "M":
                    returnProduct = new ReturnMedicine(dataTable.Rows[0]);
                    break;
                default:
                    returnProduct = new ReturnProduct();
                    break;
            }

            if (SelectedItem is PurchaseProduct && !isFromAddButton)
            {
                int selectedProductIndex = OrderProducts.IndexOf((ReturnProduct)SelectedItem);

                returnProduct.CopyOldProductData((ReturnProduct)SelectedItem);

                OrderProducts.RemoveAt(selectedProductIndex);
                OrderProducts.Insert(selectedProductIndex, returnProduct);
            }
            else
                OrderProducts.Add(returnProduct);

            RaisePropertyChanged(nameof(ProductCount));
        }

        protected override bool CheckUnProcessingOrder()
        {
            if (OrderProducts.Count == 0)
            {
                MessageWindow.ShowMessage("退貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            foreach (var product in OrderProducts)
            {
                if (product.ReturnAmount == 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量為0!", MessageType.ERROR);
                    return false;
                }
                else if (product.ReturnAmount < 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量不可小於0!", MessageType.ERROR);
                    return false;
                }
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認轉成退貨單?\n(資料內容將不能修改)", "", true);

            return (bool)confirmWindow.DialogResult;
        }

        protected override bool CheckNormalProcessingOrder()
        {
            throw new NotImplementedException();
        }

        protected override bool CheckSingdeProcessingOrder()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成退貨單?\n(資料內容將不能修改)", "", false);

            return (bool)confirmWindow.DialogResult;
        }

        public override void DeleteSelectedProduct()
        {
            OrderProducts.Remove((ReturnProduct)SelectedItem);

            RaisePropertyChanged(nameof(ProductCount));
        }
        #endregion

        #region ----- Define Function -----
        #endregion
    }
}
