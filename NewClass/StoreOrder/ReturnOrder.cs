﻿using System;
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
        private ReturnProducts returnProducts;
        private double returnStockValue;

        public ReturnProducts ReturnProducts
        {
            get { return returnProducts; }
            set { Set(() => ReturnProducts, ref returnProducts, value); }
        }
        public int ProductCount
        {
            get
            {
                if (ReturnProducts is null) return initProductCount;
                else return ReturnProducts.Count;
            }
        }
        public double ReturnStockValue
        {
            get { return returnStockValue; }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
        }
        #endregion

        private ReturnOrder() { }
        public ReturnOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.RETURN;
        }

        #region ----- Override Function -----

        #region ///// Check Function /////
        protected override bool CheckUnProcessingOrder()
        {
            if (ReturnProducts.Count == 0)
            {
                MessageWindow.ShowMessage("退貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            var products = ReturnProducts.GroupBy(p => p.ID).Select(g => new { ProductID = g.Key, ReturnAmount = g.Sum(p => p.ReturnAmount), Inventory = g.First().Inventory }).ToList();

            foreach (var product in products)
            {
                if (product.Inventory < product.ReturnAmount)
                {
                    MessageWindow.ShowMessage(product.ProductID + " 商品退貨量不可大於庫存量!", MessageType.ERROR);
                    return false;
                }
            }

            foreach (var product in ReturnProducts)
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
            if (ReturnProducts.Sum(p => p.RealAmount) == 0.0)
            {
                MessageWindow.ShowMessage("退貨單總退貨量不可為0!", MessageType.ERROR);
                return false;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成退貨單?\n(資料內容將不能修改)", "", false);

            return (bool)confirmWindow.DialogResult;
        }
        protected override bool CheckSingdeProcessingOrder()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成退貨單?\n(資料內容將不能修改)", "", false);

            return (bool)confirmWindow.DialogResult;
        }
        #endregion

        #region ///// Product Function /////
        public override void CalculateTotalPrice()
        {
            ReturnStockValue = ReturnProducts.Sum(p => p.ReturnStockValue);
        }
        public override void GetOrderProducts()
        {
            ReturnProducts = ReturnProducts.GetProductsByStoreOrderID(ID);
            TotalPrice = ReturnProducts.Sum(p => p.SubTotal);

            if (OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.DONE)
                ReturnProducts.SetToProcessing();

            ReturnProducts.SetStartEditToPrice();

            CalculateTotalPrice();
        }
        internal void SetInventoryDetail(string proID)
        {
            foreach (var returnProduct in ReturnProducts)
            {
                if (returnProduct.ID.Equals(proID))
                {
                    returnProduct.SetReturnInventoryDetail();
                    break;
                }
            }

            CalculateTotalPrice();
        }
        internal void CalculateReturnAmount()
        {
            foreach (var returnProduct in ReturnProducts)
            {
                returnProduct.CalculateReturnAmount();
            }

            CalculateTotalPrice();
        }
        public override void SetProductToProcessingStatus()
        {
            ReturnProducts.SetToProcessing();
        }
        public override void AddProductByID(string iD, bool isFromAddButton)
        {
            if (ReturnProducts.Count(p => p.ID == iD) > 0)
            {
                MessageWindow.ShowMessage("訂單中已有 " + iD + " 商品", MessageType.ERROR);
                return;
            }

            DataTable dataTable = PurchaseReturnProductDB.GetReturnProductByProductID(iD, OrderWarehouse.ID);

            ReturnProduct tempProduct = null;

            foreach (DataRow row in dataTable.Rows)
            {
                if (tempProduct is null)
                {
                    switch (row.Field<string>("TYPE"))
                    {
                        case "O":
                            tempProduct = new ReturnOTC(row);
                            break;
                        case "M":
                            tempProduct = new ReturnMedicine(row);
                            break;
                    }
                }
                else
                {
                    tempProduct.AddInventoryDetail(row);
                }
            }

            if (SelectedItem is PurchaseProduct && !isFromAddButton)
            {
                int selectedProductIndex = ReturnProducts.IndexOf((ReturnProduct)SelectedItem);

                tempProduct.CopyOldProductData((ReturnProduct)SelectedItem);

                ReturnProducts.RemoveAt(selectedProductIndex);
                ReturnProducts.Insert(selectedProductIndex, tempProduct);
            }
            else
                ReturnProducts.Add(tempProduct);

            RaisePropertyChanged(nameof(ProductCount));
        }
        public override void DeleteSelectedProduct()
        {
            ReturnProducts.Remove((ReturnProduct)SelectedItem);

            RaisePropertyChanged(nameof(ProductCount));
        }
        #endregion

        public override void SaveOrder()
        {
            //ReturnOrder returnOrder = this.Clone() as ReturnOrder;
            //BackgroundWorker backgroundWorker = new BackgroundWorker();

            //backgroundWorker.DoWork += (sender, args) =>
            //{
                StoreOrderDB.SaveReturnOrder(this);
            //};

            //backgroundWorker.RunWorkerAsync();
        }
        public override object Clone()
        {
            ReturnOrder returnOrder = new ReturnOrder();

            returnOrder.CloneBaseData(this);

            returnOrder.ReturnProducts = ReturnProducts.Clone() as ReturnProducts;

            return returnOrder;
        }
        #endregion

        #region ----- Define Function -----
        #endregion
    }
}
