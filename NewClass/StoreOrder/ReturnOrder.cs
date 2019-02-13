using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.ChooseBatchWindow;

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
            Messenger.Default.Register<NotificationMessage<ChooseBatchProducts>>(this, AddBatchProducts);
            ChooseBatchWindow chooseBatchWindow = new ChooseBatchWindow(iD);
            Messenger.Default.Unregister(this);
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

        public override void DeleteSelectedProduct()
        {
            OrderProducts.Remove((ReturnProduct)SelectedItem);
        }

        protected override void UpdateOrderProductsFromSingde()
        {
            //DataTable dataTable = StoreOrderDB.GetOrderProductsFromSingde(ID);

        }

        #endregion

        #region ----- Define Function -----
        private void AddBatchProducts(NotificationMessage<ChooseBatchProducts> notificationMessage)
        {
            if (notificationMessage.Sender is ChooseBatchWindowViewModel)
            {
                DataTable dataTable = PurchaseReturnProductDB.GetReturnProductByProductID(notificationMessage.Notification);

                int insertIndex = OrderProducts.Count;

                if (SelectedItem is ReturnProduct)
                {
                    insertIndex = OrderProducts.IndexOf((ReturnProduct)SelectedItem);

                    OrderProducts.RemoveAt(insertIndex);
                }

                foreach (var product in notificationMessage.Content)
                {
                    if (product.ReturnAmount > 0 || notificationMessage.Content.Count == 1)
                    {
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

                        returnProduct.BatchNumber = product.BatchNumber;
                        returnProduct.BatchLimit = product.BatchNumberLimit;
                        returnProduct.ReturnAmount = product.ReturnAmount;
                        
                        OrderProducts.Insert(insertIndex, returnProduct);
                        insertIndex++;
                    }
                }
                
                RaisePropertyChanged(nameof(ProductCount));
            }
        }
        #endregion
    }
}
