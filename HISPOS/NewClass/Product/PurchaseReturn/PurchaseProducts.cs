using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.ObjectModel;
using System.Data;
using DomainModel.Enum;
using His_Pos.NewClass.Product.ProductGroupSetting;
using System.Linq;
using System.Collections.Generic;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProducts : ObservableCollection<PurchaseProduct>, ICloneable
    {
        public PurchaseProducts()
        {
        }

        public PurchaseProducts(DataTable dataTable, OrderStatusEnum orderStatus)
        {
            string lastID = "";

            foreach (DataRow row in dataTable.Rows)
            {
                PurchaseProduct purchaseProduct;

                switch (row.Field<string>("TYPE"))
                {
                    case "O":
                        purchaseProduct = new PurchaseOTC(row, orderStatus);
                        break;

                    case "M":
                        purchaseProduct = new PurchaseMedicine(row, orderStatus);
                        break;

                    default:
                        purchaseProduct = null;
                        break;
                }

                if (purchaseProduct.ID.Equals(lastID))
                    purchaseProduct.IsFirstBatch = false;

                Add(purchaseProduct);

                lastID = row.Field<string>("Pro_ID");
            }
        }

        internal static PurchaseProducts GetProductsByStoreOrderID(string orederID, OrderStatusEnum orderStatus)
        {
            return new PurchaseProducts(PurchaseReturnProductDB.GetProductsByStoreOrderID(orederID), orderStatus);
        }

        internal static bool UpdateSingdeProductsByStoreOrderID(string orederID, string receiveID, string checkCode, string sourceID)
        {
            DataTable dataTable;
            if (sourceID != null)
            {
                dataTable = PurchaseReturnProductDB.GetSingdeProductsByStoreOrderID(sourceID);
            }
            else
            {
                dataTable = PurchaseReturnProductDB.GetSingdeProductsByStoreOrderID(orederID);
            }

            if (dataTable.Rows.Count == 0)
                return false;

            #region 測試資料
            //if (1 == 1)
            //{
            //    DataTable testtable = dataTable.Clone();
            //    DataRow testrow1 = testtable.NewRow();
            //    testrow1["TYPE"] = "O";
            //    testrow1["PRO_ID"] = "N013396209";
            //    testrow1["AMOUNT"] = 5;
            //    testrow1["PRICE"] = 10;
            //    testrow1["BATCHNUM"] = "TEST1-1";
            //    testrow1["VALIDDATE"] = "1151231";
            //    testrow1["rep_no"] = 2;
            //    testtable.Rows.Add(testrow1);
            //    dataTable = testtable;
            //    receiveID = "1110830W01";//更改ReceiveID(測試用)
            //}
            #endregion
            DataTable table = PurchaseReturnProductDB.GetProductsByStoreOrderID(orederID);//找訂單明細
            Dictionary<string, int> pairsProID = new Dictionary<string, int>();//取代商品、取代數量(商品取代
            Dictionary<int, int> pairsDetID = new Dictionary<int, int>();//取代代碼、取代數量(項次取代
            DataView dv = table.DefaultView;
            dv.Sort = "StoOrdDet_ID";
            DataTable SortTable = dv.ToTable();
            foreach (DataRow dr in dataTable.Rows)
            {
                int rep_no = Convert.ToInt32(Convert.ToString(dr["rep_no"]) == string.Empty ? 0 : dr["rep_no"]);//傳回需取代的代碼
                string pro_ID = Convert.ToString(dr["PRO_ID"]);//傳回藥品代號
                int qty = Math.Abs(Convert.ToInt32(dr["AMOUNT"]));//取代數量
                string wareID = Convert.ToString(SortTable.Rows[0]["ProInv_WareHouseID"]);//倉庫代碼
                var dataList = ProductGroupSettingDB.GetProductGroupSettingListByID(pro_ID, wareID);//商品群組
                bool isContains = SortTable.Select(string.Format("Pro_ID = '{0}'", pro_ID)).Length > 0;
                if (!isContains)//如果傳回藥品不存在系統訂單內(不存在同商品)
                {
                    bool isSucces = false;
                    foreach (var data in dataList)//迴圈判斷群組(紀錄取代商品
                    {
                        if(data.ID != pro_ID)//不等於已存在明細中的商品
                        {
                            DataRow[] drs = SortTable.Select(string.Format("Pro_ID = '{0}'", data.ID));
                            bool isContainsGroup = drs.Length > 0;
                            if (isContainsGroup)//如果是同群組商品
                            {
                                if (!pairsProID.ContainsKey(data.ID))
                                {
                                    pairsProID.Add(data.ID, qty);//新增取代商品、取代數量
                                }
                                else
                                {
                                    pairsProID[data.ID] += qty;//同商品累計取代數量
                                }
                                isSucces = true;
                            }
                        }
                    }
                    if (isSucces == false)//群組也不包含商品(找項次)(紀錄取代項次
                    {
                        DataRow[] drs = SortTable.Select(string.Format("StoOrdDet_ID = '{0}'", rep_no));
                        bool isContainsDetID = drs.Length > 0;
                        if (isContainsDetID)
                        {
                            if (!pairsDetID.ContainsKey(rep_no))
                                pairsDetID.Add(rep_no, qty);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, int> value in pairsProID)//依StoOrdDet_ProductID取代商品
            {
                StoreOrderDB.UpdateReplaceProduct(value.Value, orederID, value.Key, 0);
            }
            foreach (KeyValuePair<int, int> value in pairsDetID)//依StoOrdDet_ID取代商品
            {
                StoreOrderDB.UpdateReplaceProduct(value.Value, orederID, string.Empty, value.Key);
            }
            dataTable = StoreOrderDB.UpdateSingdeProductsByStoreOrderID(dataTable, orederID, receiveID, checkCode);
            if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                return false;

            return true;
        }

        internal void SetToSingde()
        {
            foreach (var product in Items)
                product.IsSingde = true;
        }

        internal void SetToProcessing()
        {
            foreach (var product in Items)
                product.IsProcessing = true;
        }

        internal void SetStartEditToPrice()
        {
            //foreach (var product in Items)
                //product.StartInputVariable = ProductStartInputVariableEnum.PRICE;
        }

        public object Clone()
        {
            PurchaseProducts products = new PurchaseProducts();

            foreach (var product in Items)
                products.Add(product.Clone() as PurchaseProduct);

            return products;
        }
    }
}