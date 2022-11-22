using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Product.PurchaseReturn;
using System.Data;
using System.Linq;
using DomainModel.Enum;
using System;
using System.Windows;

namespace His_Pos.NewClass.StoreOrder
{
    public class ReturnOrder : StoreOrder
    {
        #region ----- Define Variables -----

        private ReturnProducts returnProducts;
        private ReturnProducts oldReturnProducts;
        private double returnStockValue;

        public ReturnProducts ReturnProducts
        {
            get { return returnProducts; }
            set { Set(() => ReturnProducts, ref returnProducts, value); }
        }

        public ReturnProducts OldReturnProducts
        {
            get { return oldReturnProducts; }
            set { Set(() => OldReturnProducts, ref oldReturnProducts, value); }
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
            get { return Math.Round(returnStockValue, MidpointRounding.AwayFromZero); }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
        }

        public double ReturnDiff { get { return TotalPrice - ReturnStockValue; } }

        #endregion ----- Define Variables -----

        public ReturnOrder()
        {
        }

        public ReturnOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.RETURN;
            ReturnStockValue = (double)row.Field<decimal>("RETURN_PRICE");
        }

        #region ----- Override Function -----

        #region ///// Check Function /////

        protected override bool CheckUnProcessingOrder()
        {
            var flagNotOTC = 0;
            var flagOTC = 0;
            if (ReturnProducts.Count == 0)
            {
                MessageWindow.ShowMessage("退貨單中不可以沒有商品!", MessageType.ERROR);
                return false;
            }

            foreach (var product in ReturnProducts)
            {
                if (!product.IsChecked)
                    continue;
                if (product.ReturnAmount == 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 退貨量為0!", MessageType.ERROR);
                    return false;
                }
                else if (product.ReturnAmount < 0)
                {
                    MessageWindow.ShowMessage(product.ID + " 退貨量不可小於0!", MessageType.ERROR);
                    return false;
                }
                else if (product.ReturnAmount > product.Inventory)
                {
                    MessageWindow.ShowMessage(product.ID + " 退貨量不可大於架上量", MessageType.ERROR);
                    return false;
                }
                else if (product.TypeOTC == 2)
                {
                    flagOTC = 1;
                }
                else if (product.TypeOTC != 2)
                {
                    flagNotOTC = 1;
                }
            }
            if (flagOTC == 1 && flagNotOTC == 1)
            {
                MessageWindow.ShowMessage($"此訂單包含藥品與OTC商品\n請分開建立退貨單！", MessageType.ERROR);
                return false;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認退貨?\n(確認後直接扣除庫存)", "", true);

            if (!(bool)confirmWindow.DialogResult)
                return false;

            DataTable dataTable;
            if (this is ReturnOrder)
            {
                ReturnOrder returnOrder = new ReturnOrder
                {
                    returnProducts = new ReturnProducts()
                };
                foreach (var item in this.returnProducts)
                {
                    if (item.IsChecked == true)
                    {
                        returnOrder.returnProducts.Add(item);
                    }
                }
                if (returnOrder.returnProducts.Count == 0)
                {
                    MessageWindow.ShowMessage("未選擇退貨項目 請重新設定後再傳送", MessageType.ERROR);
                    return false;
                }
                dataTable = StoreOrderDB.CheckReturnProductValid(returnOrder);
            }
            else
            {
                dataTable = StoreOrderDB.CheckReturnProductValid(this);
            }

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
            bool hasZeroPrice = false;
            bool hasZeroRealAmount = TotalPrice == 0 ? true : false;

            foreach (var product in ReturnProducts)
            {
                if (product is ReturnMedicine && (product as ReturnMedicine).IsControl != null)
                {
                    hasControlMed = true;
                }
                else if (product.Price == 0) 
                {
                    hasZeroPrice = true;
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

            if (hasZeroPrice)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow($"部分品項退貨價為0，\n是否確認完成退貨單?", "", false);
                return (bool)confirmWindow.DialogResult;
            }
            else if(hasZeroRealAmount)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow("退貨金額為零\n是否確認完成退貨單?", "", false);
                return (bool)confirmWindow.DialogResult;
            }
            else
            {
                ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認完成退貨單?", "", false);
                return (bool)confirmWindow.DialogResult;
            }
        }

        protected override bool CheckSingdeProcessingOrder()
        {
            return true;
        }

        #endregion ///// Check Function /////

        #region ///// Product Function /////

        public override void CalculateTotalPrice(int isDone)
        {
            if (OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
            {
                ReturnStockValue = ReturnProducts.Where(w => w.IsChecked && w.TypeOTC != 4).Sum(p => p.ReturnStockValue);
            }
            else
            {
                ReturnStockValue = ReturnProducts.Where(w => w.TypeOTC != 4).Sum(p => p.ReturnStockValue);
            }
                

            TotalPrice = ReturnProducts.Sum(p => Math.Round(p.SubTotal, 2, MidpointRounding.AwayFromZero));
            TotalPrice = Math.Round(TotalPrice, 0, MidpointRounding.AwayFromZero);
            RaisePropertyChanged(nameof(ReturnDiff));
        }

        public override void GetOrderProducts()
        {
            SelectedItem = null;

            ReturnProducts = ReturnProducts.GetProductsByStoreOrderID(ID);
            if(OrderStatus != OrderStatusEnum.DONE && OrderStatus != OrderStatusEnum.SCRAP)
            {
                foreach (ReturnProduct returnProduct in ReturnProducts)
                {
                    if (!returnProduct.IsDone)
                    {
                        double value = 0;
                        double avgPrice = 0;
                        foreach (ReturnProductInventoryDetail detail in returnProduct.InventoryDetailCollection)
                        {
                            value = detail.ReturnStockValue;
                            avgPrice = detail.ReceiveAmount;
                        }
                        returnProduct.Price = (OrderStatus== OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.SINGDE_PROCESSING) ? returnProduct.Price : avgPrice;//(平均單價)
                        returnProduct.SubTotal = (OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.SINGDE_PROCESSING) ? returnProduct.SubTotal : Math.Round(Convert.ToDouble(avgPrice * returnProduct.RealAmount), MidpointRounding.AwayFromZero);
                        returnProduct.ReceiveAmount = Math.Round(avgPrice * returnProduct.ReturnAmount, MidpointRounding.AwayFromZero);
                    }
                }
            }
            
            OldReturnProducts = ReturnProducts.GetOldReturnProductsByStoreOrderID(ID);
            TotalPrice = ReturnProducts.Sum(p => p.SubTotal);

            if (OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
                ReturnProducts.SetReturnInventoryDetail();

            if (OrderStatus == OrderStatusEnum.NORMAL_PROCESSING || OrderStatus == OrderStatusEnum.DONE)
                ReturnProducts.SetToProcessing();

            ReturnProducts.SetStartEditToPrice();

            CalculateTotalPrice(0);
        }

        internal void CalculateReturnAmount()
        {
            foreach (var returnProduct in ReturnProducts)
            {
                returnProduct.CalculateReturnAmount();
            }

            CalculateTotalPrice(0);
        }

        internal void ReturnOrderRePurchase()
        {
            foreach (ReturnProduct returnProduct in this.returnProducts)
            {
                if(!returnProduct.IsChecked)
                {
                    this.returnProducts.Remove(returnProduct);
                }
            }
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
                        case "D":
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
                    ConfirmWindow confirmWindow = new ConfirmWindow("管藥正常不可退貨，確認繼續退貨作業?", "確認");
                    if (!(bool)confirmWindow.DialogResult)
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
            CalculateTotalPrice(0);

            RaisePropertyChanged(nameof(ProductCount));
        }

        #endregion ///// Product Function /////

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

        public override int GetOrderProductsIsOTC()
        {
            ReturnProducts returnProductsOTC = new ReturnProducts();
            returnProductsOTC = ReturnProducts.GetProductsByStoreOrderID(ID);
            int type = returnProductsOTC[0].TypeOTC;
            return type;
        }

        public override void SaveOrderCus()
        {
        }

        public override void SetRealAmount(string id)
        {
        }

        public override bool ChkPrice()
        {
            return true;
        }

        public override bool ChkPurchase()
        {
            return true;
        }
        protected override bool CheckStoreOrderLower()
        {
            return true;
        }

        #endregion ----- Override Function -----
    }
}