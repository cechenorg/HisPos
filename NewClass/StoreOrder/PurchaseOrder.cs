using System;
using System.Data;
using System.Linq;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.Service;

namespace His_Pos.NewClass.StoreOrder
{
    public class PurchaseOrder: StoreOrder
    {
        #region ----- Define Variables -----
        private PurchaseProducts orderProducts;

        public string PatientData { get; set; }
        public bool HasPatient => !string.IsNullOrEmpty(PatientData);
        public PurchaseProducts OrderProducts
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

        public PurchaseOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.PURCHASE;
            PatientData = row.Field<string>("CUS_DATA");
        }

        #region ----- Override Function -----
        public override void CalculateTotalPrice()
        {
            TotalPrice = OrderProducts.Sum(p => p.SubTotal);
        }
        public override void GetOrderProducts()
        {
            OrderProducts = PurchaseProducts.GetProductsByStoreOrderID(ID);
            TotalPrice = OrderProducts.Sum(p => p.SubTotal);
        }

        public override void SaveOrder()
        {
            PurchaseOrder saveStoreOrder = this.Clone() as PurchaseOrder;
            StoreOrderDB.SavePurchaseOrder(saveStoreOrder);
            //BackgroundWorker backgroundWorker = new BackgroundWorker();

            //backgroundWorker.DoWork += (sender, args) =>
            //{
            //    StoreOrderDB.SavePurchaseOrder(saveStoreOrder);
            //};

            //backgroundWorker.RunWorkerAsync();
        }

        public override void AddProductByID(string iD)
        {
            if (OrderProducts.Count(p => p.ID == iD) > 0)
            {
                MessageWindow.ShowMessage("訂單中已有 " + iD + " 商品", MessageType.ERROR);
                return;
            }

            DataTable dataTable = PurchaseReturnProductDB.GetPurchaseProductByProductID(iD);

            PurchaseProduct purchaseProduct;

            switch (dataTable.Rows[0].Field<string>("TYPE"))
            {
                case "O":
                    purchaseProduct = new PurchaseOTC(dataTable.Rows[0]);
                    break;
                case "M":
                    purchaseProduct = new PurchaseMedicine(dataTable.Rows[0]);
                    break;
                default:
                    purchaseProduct = new PurchaseProduct();
                    break;
            }

            if (SelectedItem is PurchaseProduct)
            {
                int selectedProductIndex = OrderProducts.IndexOf((PurchaseProduct)SelectedItem);

                purchaseProduct.CopyOldProductData((PurchaseProduct)SelectedItem);

                OrderProducts[selectedProductIndex] = purchaseProduct;
            }
            else
                OrderProducts.Add(purchaseProduct);

            RaisePropertyChanged(nameof(ProductCount));
        }

        public override void DeleteSelectedProduct()
        {
            OrderProducts.Remove((PurchaseProduct) SelectedItem);

            RaisePropertyChanged(nameof(ProductCount));
        }

        protected override bool CheckUnProcessingOrder()
        {
            if (OrderProducts.Count == 0)
            {
                MessageWindow.ShowMessage("進貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            foreach (var product in OrderProducts)
            {
                if (product.OrderAmount + product.FreeAmount == 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量為0!", MessageType.ERROR);
                    return false;
                }
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認轉成" + (OrderType == OrderTypeEnum.PURCHASE? "進" : "退") + "貨單?\n(資料內容將不能修改)", "",true);

            return (bool)confirmWindow.DialogResult;
        }

        protected override bool CheckNormalProcessingOrder()
        {
            throw new NotImplementedException();
        }

        protected override bool CheckSingdeProcessingOrder()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成" + (OrderType == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單?\n(資料內容將不能修改)", "", true);

            return (bool)confirmWindow.DialogResult;
        }
        #endregion

        public static  void InsertPrescriptionOrder(Prescription.Prescription p,PrescriptionSendDatas pSendData) {
           string newstoordId = StoreOrderDB.InsertPrescriptionOrder(pSendData, p).Rows[0].Field<string>("newStoordId");
            try {
                PrescriptionDb.SendDeclareOrderToSingde(newstoordId, p, pSendData);
                StoreOrderDB.StoreOrderToWaiting(newstoordId);
            }
            catch (Exception ex) {
                MessageWindow.ShowMessage("傳送藥健康失敗 請稍後至進退貨管理傳送",MessageType.ERROR);
            } 
        }
        public static void UpdatePrescriptionOrder(Prescription.Prescription p, PrescriptionSendDatas pSendData) {
            string stoordId = PrescriptionDb.GetStoreOrderIDByPrescriptionID(p.Id).Rows[0][0].ToString();
            try
            {
                PrescriptionDb.UpdateDeclareOrderToSingde(stoordId, p, pSendData); 
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("更新藥健康失敗 請稍後至進退貨管理傳送", MessageType.ERROR);
            }
        }
    }
}
