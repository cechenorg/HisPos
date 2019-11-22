﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Policy;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
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
        public double ReturnDiff { get { return TotalPrice - ReturnStockValue; } }
        #endregion

        private ReturnOrder() { }
        public ReturnOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.RETURN;
            ReturnStockValue = (double)row.Field<decimal>("RETURN_PRICE");
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
                else if (product.ReturnAmount > product.Inventory)
                {
                    MessageWindow.ShowMessage(product.ID + " 退貨量不可大於架上量", MessageType.ERROR);
                    return false;
                }
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認退貨?\n(確認後直接扣除庫存)", "", true);

            if (!(bool) confirmWindow.DialogResult)
                return false;

            DataTable dataTable = StoreOrderDB.CheckReturnProductValid(this);

            if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
            {
                MessageWindow.ShowMessage("庫存有異動 請重新設定後再傳送", MessageType.ERROR);
                return false;
            }

            return true;
        }
        protected override bool CheckNormalProcessingOrder()
        {
            bool hasControlMed = false;

            foreach (var product in ReturnProducts)
            {
                if (product is ReturnMedicine && (product as ReturnMedicine).IsControl != null)
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

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成退貨單?\n(資料內容將不能修改)", "", false);

            return (bool)confirmWindow.DialogResult;
        }
        protected override bool CheckSingdeProcessingOrder()
        {
            return true;
        }
        #endregion

        #region ///// Product Function /////
        public override void CalculateTotalPrice()
        {
            if(OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
                ReturnStockValue = ReturnProducts.Sum(p => p.ReturnStockValue);

            TotalPrice = ReturnProducts.Sum(p => p.SubTotal);

            RaisePropertyChanged(nameof(ReturnDiff));
        }
        public override void GetOrderProducts()
        {
            SelectedItem = null;

            ReturnProducts = ReturnProducts.GetProductsByStoreOrderID(ID);
            TotalPrice = ReturnProducts.Sum(p => p.SubTotal);

            if(OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
                ReturnProducts.SetReturnInventoryDetail();

            if (OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.DONE)
                ReturnProducts.SetToProcessing();

            ReturnProducts.SetStartEditToPrice();

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
        internal void ReturnOrderRePurchase()
        {
            SaveOrder();
            
            OrderStatus = OrderStatusEnum.DONE;

            DataTable result = StoreOrderDB.ReturnOrderRePurchase(ID);

            if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
            {
                MessageWindow.ShowMessage("退貨單未完成\r\n請重新整理後重試", MessageType.ERROR);
                return;
            }
            else
                MessageWindow.ShowMessage("已完成退貨單\r\n(詳細資料可至進退貨紀錄查詢)", MessageType.SUCCESS);
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

            if (ReturnProducts.Count(p => p.InvID == tempProduct.InvID) > 0)
            {
                MessageWindow.ShowMessage("訂單中已有同群組商品", MessageType.ERROR);
                return;
            }

            if (tempProduct is ReturnMedicine && OrderManufactory.ID == "0")
            {
                if ((tempProduct as ReturnMedicine).IsFrozen)
                {
                    MessageWindow.ShowMessage("冰品無法退貨", MessageType.ERROR);
                    return;
                }

                if ((tempProduct as ReturnMedicine).IsControl != null)
                {
                    MessageWindow.ShowMessage("管藥無法退貨", MessageType.ERROR);
                    return;
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
            CalculateTotalPrice();

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
