﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PrescriptionSendData;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.Service;


namespace His_Pos.NewClass.StoreOrder
{
    public class PurchaseOrder: StoreOrder
    {
        #region ----- Define Variables -----
        private PurchaseProducts orderProducts;

        public string PreOrderCustomer { get; set; }
        public string TargetPreOrderCustomer { get; set; }
        
        public DateTime? PlanArriveDate { get; set; }
        public string PatientData { get; set; }
        public bool HasPatient => !string.IsNullOrEmpty(PatientData);
        public bool HasCustomer => !string.IsNullOrEmpty(PreOrderCustomer);
        public DateTime Day { get; set; }
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

        private PurchaseOrder() { }
        public PurchaseOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.PURCHASE;
            PatientData = row.Field<string>("CUS_DATA");
            PreOrderCustomer = row.Field<string>("StoOrd_CustomerName");
            TargetPreOrderCustomer = row.Field<string>("StoOrd_TargetCustomerName");
            PlanArriveDate = row.Field<DateTime?>("StoOrd_PlanArrivalDate");
            
        }

        #region ----- Override Function -----

        #region ///// Check Function /////
        protected override bool CheckUnProcessingOrder()
        {
            var flagNotOTC = 0;
            var flagOTC = 0;

            if (PlanArriveDate != null && PlanArriveDate <= DateTime.Today)
            {
                MessageWindow.ShowMessage("預定到貨日需大於今日!", MessageType.ERROR);
                return false;
            }

            if (OrderProducts.Count == 0)
            {
                MessageWindow.ShowMessage("進貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            if (String.IsNullOrEmpty(TargetPreOrderCustomer) && OrderManufactory.ID.Equals("0"))
            {
                MessageWindow.ShowMessage("進貨單必須指定顧客!", MessageType.ERROR);
                return false;
            }

            foreach (var product in OrderProducts)
            {
                if (product.OrderAmount + product.FreeAmount == 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量為0!", MessageType.ERROR);
                    return false;
                }
                else if (product.OrderAmount < 0 || product.FreeAmount < 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量不可小於0!", MessageType.ERROR);
                    return false;
                }
                else if (product.Type == 2) {
                    flagOTC = 1;
                }
                else if (product.Type != 2)
                {
                    flagNotOTC = 1;
                }
            }
            if (flagOTC == 1 && flagNotOTC == 1)
            {
                MessageWindow.ShowMessage($"此訂單包含藥品與OTC商品\n請分開建立採購單！", MessageType.ERROR);
                return false;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認轉成進貨單?\n(資料內容將不能修改)", "");

            return (bool)confirmWindow.DialogResult;
        }
        protected override bool CheckNormalProcessingOrder()
        {
            bool isLowerThenOrderAmount = false;
            bool hasControlMed = false;

            var products = OrderProducts.GroupBy(p => p.ID).Select(g => new { ProductID = g.Key, OrderAmount = g.First().OrderAmount, RealAmount = g.Sum(p => p.RealAmount) }).ToList();

            foreach (var product in OrderProducts)
            {
                if (product.OrderAmount < 0 || product.FreeAmount < 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 商品數量不可小於0!", MessageType.ERROR);
                    return false;
                }

                if (product is PurchaseMedicine && (product as PurchaseMedicine).IsControl != null)
                {
                    hasControlMed = true;
                }
            }

            if (hasControlMed)
            {
                DataTable dataTable = ManufactoryDB.ManufactoryHasControlMedicineID(OrderManufactory.ID);

                if (dataTable.Rows.Count > 0 && !dataTable.Rows[0].Field<bool>("RESULT"))
                {
                    MessageWindow.ShowMessage("有管制藥品時，供應商必須有管藥證號!", MessageType.ERROR);
                    return false;
                }
            }

            if (OrderProducts.Sum(p => p.RealAmount) == 0.0)
            {
                MessageWindow.ShowMessage("訂單總進貨量不可為0!", MessageType.ERROR);
                return false;
            }

            foreach (var product in products)
            {
                if (product.RealAmount < product.OrderAmount)
                {
                    isLowerThenOrderAmount = true;
                    break;
                }
            }

            if (isLowerThenOrderAmount)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow($"收貨單中有商品的收貨量低於訂購量\r\n是否將不足的部分轉成新的收貨單?", "", false);

                if ((bool)confirmWindow.DialogResult)
                {
                    bool isSuccess = AddNewStoreOrderLowerThenOrderAmount();

                    if (!isSuccess) return false;
                }
            }

            ConfirmWindow confirmWindow1 = new ConfirmWindow($"是否確認完成進貨單?\n(資料內容將不能修改)", "", false);

            return (bool)confirmWindow1.DialogResult;
        }
        protected override bool CheckSingdeProcessingOrder()
        {
            return true;
        }
        #endregion

        #region ///// Product Function /////
        public override void CalculateTotalPrice()
        {
            TotalPrice = OrderProducts.Sum(p => p.SubTotal);
        }

        public override void SetProductToProcessingStatus()
        {
            OrderProducts.SetToProcessing();
        }

        public override int GetOrderProductsIsOTC() {
            PurchaseProducts purchaseProductsOTC = new PurchaseProducts();
            purchaseProductsOTC = PurchaseProducts.GetProductsByStoreOrderID(ID);
            int type = purchaseProductsOTC[0].Type;
            return type;
        }


        public override void GetOrderProducts()
        {
            OrderProducts = PurchaseProducts.GetProductsByStoreOrderID(ID);
            TotalPrice = OrderProducts.Sum(p => p.SubTotal);

            if (OrderManufactory.ID.Equals("0"))
                OrderProducts.SetToSingde();

            if (OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.DONE)
                OrderProducts.SetToProcessing();

            OrderProducts.SetStartEditToPrice();

            CalculateTotalPrice();
        }
        public override void AddProductByID(string iD, bool isFromAddButton)
        {
            if (OrderProducts.Count(p => p.ID == iD) > 0)
            {
                MessageWindow.ShowMessage("訂單中已有 " + iD + " 商品", MessageType.ERROR);
                return;
            }

            DataTable dataTable = PurchaseReturnProductDB.GetPurchaseProductByProductID(iD, OrderWarehouse.ID);

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
                    purchaseProduct = null;
                    break;
            }

            if (OrderManufactory.ID.Equals("0")) purchaseProduct.IsSingde = true;

            if (SelectedItem is PurchaseProduct && !isFromAddButton)
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
            OrderProducts.Remove((PurchaseProduct)SelectedItem);
            CalculateTotalPrice();

            RaisePropertyChanged(nameof(ProductCount));
        }
        #endregion

        public override void SaveOrder()
        {
            //PurchaseOrder saveStoreOrder = this.Clone() as PurchaseOrder;
            //BackgroundWorker backgroundWorker = new BackgroundWorker();

            //backgroundWorker.DoWork += (sender, args) =>
            //{

            StoreOrderDB.SavePurchaseOrder(this);


        }
        public override object Clone()
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();

            purchaseOrder.CloneBaseData(this);

            purchaseOrder.OrderProducts = OrderProducts.Clone() as PurchaseProducts;
            purchaseOrder.PatientData = PatientData;
            
            return purchaseOrder;
        }
        #endregion

        #region ----- Define Function -----

        #region ///// Batch Function /////

        public void SplitBatch(string productID)
        {
            for (int x = 0; x < OrderProducts.Count; x++)
            {
                if (OrderProducts[x].ID.Equals(productID))
                {
                    OrderProducts.Insert(x + 1, AddNewProductBySplit(OrderProducts[x]));
                    return;
                }
            }
        }
        private PurchaseProduct AddNewProductBySplit(PurchaseProduct purchaseProduct)
        {
            PurchaseProduct newProduct;

            if (purchaseProduct is PurchaseMedicine)
            {
                newProduct = new PurchaseMedicine();

                (newProduct as PurchaseMedicine).IsControl = (purchaseProduct as PurchaseMedicine).IsControl;
                (newProduct as PurchaseMedicine).IsFrozen = (purchaseProduct as PurchaseMedicine).IsFrozen;
            }
            else
            {
                newProduct = new PurchaseOTC();
            }

            newProduct.ID = purchaseProduct.ID;
            newProduct.ChineseName = purchaseProduct.ChineseName;
            newProduct.EnglishName = purchaseProduct.EnglishName;
            newProduct.IsCommon = purchaseProduct.IsCommon;

            newProduct.UnitName = purchaseProduct.UnitName;
            newProduct.UnitAmount = purchaseProduct.UnitAmount;

            newProduct.CopyOldProductData(purchaseProduct);

            newProduct.IsFirstBatch = false;
            newProduct.IsProcessing = true;
            newProduct.BatchNumber = "";

            int leftAmount = ((int)purchaseProduct.RealAmount) % 2;

            newProduct.RealAmount = ((int)purchaseProduct.RealAmount) / 2;
            purchaseProduct.RealAmount = ((int)purchaseProduct.RealAmount) / 2 + leftAmount;

            RaisePropertyChanged(nameof(ProductCount));

            return newProduct;
        }

        public void MergeBatch(PurchaseProduct product)
        {
            if (!string.IsNullOrEmpty(product.BatchNumber))
            {
                MessageWindow.ShowMessage("請先將批號清除再進行合批!", MessageType.ERROR);
                return;
            }

            PurchaseProduct originProduct = OrderProducts.Single(p => p.ID.Equals(product.ID) && p.IsFirstBatch);

            originProduct.RealAmount += product.RealAmount;

            OrderProducts.Remove(product);

            RaisePropertyChanged(nameof(ProductCount));
        }
        #endregion
        
        private bool AddNewStoreOrderLowerThenOrderAmount()
        {
            DataTable dataTable = StoreOrderDB.AddStoreOrderLowerThenOrderAmount(ReceiveID, OrderManufactory.ID, OrderWarehouse.ID, OrderProducts);

            if (dataTable.Rows.Count > 0)
            {
                MessageWindow.ShowMessage($"已新增收貨單 {dataTable.Rows[0].Field<string>("NEW_ID")} !", MessageType.SUCCESS);
                return true;
            }
            else
            {
                MessageWindow.ShowMessage($"新增失敗 請稍後再試!", MessageType.ERROR);
                return false;
            }
        }
        public static bool InsertPrescriptionOrder(Prescription.Prescription p, PrescriptionSendDatas pSendData)
        {
            string newstoordId = StoreOrderDB.InsertPrescriptionOrder(pSendData, p).Rows[0].Field<string>("newStoordId");
            try
            {
                if (PrescriptionDb.SendDeclareOrderToSingde(newstoordId, p, pSendData))
                {
                    StoreOrderDB.StoreOrderToWaiting(newstoordId);
                    return true;
                }
                StoreOrderDB.RemoveStoreOrderByID(newstoordId);
                MessageWindow.ShowMessage("傳送藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR);
                return false;
            }
            catch (Exception)
            {
                StoreOrderDB.RemoveStoreOrderByID(newstoordId);
                MessageWindow.ShowMessage("傳送藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR);
                return false;
            }
        }
        
        public static void UpdatePrescriptionOrder(Prescription.Prescription p, PrescriptionSendDatas pSendData) {
            string stoordId = PrescriptionDb.GetStoreOrderIDByPrescriptionID(p.ID).Rows[0][0].ToString();
            try
            {
                int result = PrescriptionDb.UpdateDeclareOrderToSingde(stoordId, p, pSendData);
                if (result == 0)
                    MessageWindow.ShowMessage("傳送藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR);
                else if (result == 2)
                    MessageWindow.ShowMessage("藥健康已出貨 不可修改傳送藥袋 (若已修改處方 需注意處方與藥袋藥品差異)", MessageType.WARNING);
                else
                    StoreOrderDB.UpdateDetailByStoOrdID(pSendData, stoordId);
            }
            catch (Exception)
            {
                MessageWindow.ShowMessage("更新藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR); 
            }
        }

        public override void SaveOrderCus()
        {
            PurchaseOrder saveStoreOrder = this.Clone() as PurchaseOrder;
            DateTime dt = Day;
            if (dt == default(DateTime))
            {
                saveStoreOrder.Note = TargetPreOrderCustomer + " " + saveStoreOrder.Note;
                StoreOrderDB.SavePurchaseOrder(saveStoreOrder);
            }
            else
            {
                saveStoreOrder.Note = TargetPreOrderCustomer + " " + dt.ToString("yyyy年MM月dd日") + " " + saveStoreOrder.Note;
                StoreOrderDB.SavePurchaseOrder(saveStoreOrder);
            }
        }

        public override void SetRealAmount(string id)
        {
            for (int x = 0; x < OrderProducts.Count; x++)
            {
                if (OrderProducts[x].ID.Equals(id))
                {
                    OrderProducts[x].RealAmount = OrderProducts[x].OrderAmount;
                    return;
                }
            }
            RaisePropertyChanged(nameof(ProductCount));
        }

        public override bool ChkPrice()
        {
            int flag = 0;
            for (int x = 0; x < OrderProducts.Count; x++)
            {
                if (OrderProducts[x].LastPrice!= OrderProducts[x].Price)
                {
                    flag = 1;
                }
            }
            if (flag == 1)
            {
                return false;
            }
            else {
                return true;
            }
           
        }
    }
    #endregion
}

