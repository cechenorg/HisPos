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

        internal static bool UpdateSingdeProductsByStoreOrderID(string orederID, string receiveID, string checkCode, string sourceID, OrderTypeEnum type)
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
            #region 測試資料
            //if (1 == 1 && orederID == "R20220930-00")
            //{
            //    DataTable testtable = dataTable.Clone();
            //    DataRow testrow1 = testtable.NewRow();
            //    testrow1["TYPE"] = "M";
            //    testrow1["PRO_ID"] = "A001046100";
            //    testrow1["AMOUNT"] = 10;
            //    testrow1["PRICE"] = 2;
            //    testrow1["BATCHNUM"] = "TEST1";
            //    testrow1["VALIDDATE"] = "1151231";
            //    testrow1["rep_no"] = 1;
            //    testtable.Rows.Add(testrow1);
            //    DataRow testrow2 = testtable.NewRow();
            //    testrow2["TYPE"] = "M";
            //    testrow2["PRO_ID"] = "BC22376100";
            //    testrow2["AMOUNT"] = 14;
            //    testrow2["PRICE"] = 20;
            //    testrow2["BATCHNUM"] = "TEST2_1";
            //    testrow2["VALIDDATE"] = "1151231";
            //    testrow2["rep_no"] = 2;
            //    testtable.Rows.Add(testrow2);
            //    DataRow testrow3 = testtable.NewRow();
            //    testrow3["TYPE"] = "M";
            //    testrow3["PRO_ID"] = "BC22376100";
            //    testrow3["AMOUNT"] = 14;
            //    testrow3["PRICE"] = 15;
            //    testrow3["BATCHNUM"] = "TEST2_2";
            //    testrow3["VALIDDATE"] = "1151231";
            //    testrow3["rep_no"] = 2;
            //    testtable.Rows.Add(testrow3);
            //    DataRow testrow4 = testtable.NewRow();
            //    testrow4["TYPE"] = "M";
            //    testrow4["PRO_ID"] = "N013396209";
            //    testrow4["AMOUNT"] = 5;
            //    testrow4["PRICE"] = 20;
            //    testrow4["BATCHNUM"] = "TEST3";
            //    testrow4["VALIDDATE"] = "1151231";
            //    testrow4["rep_no"] = 3;
            //    testtable.Rows.Add(testrow4);
            //    dataTable = testtable;
            //    receiveID = "1111001ABC";//更改ReceiveID(測試用)
            //}
            #endregion
            if (dataTable.Rows.Count == 0)
                return false;
            if (type != OrderTypeEnum.RETURN)
            {
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
                    string repProID = StoreOrderDB.ReplaceProduct(pro_ID);
                    if (pro_ID != repProID)//已經被取代，跳過取代由預存填入資料
                    {
                        continue;
                    }
                    int qty = Math.Abs(Convert.ToInt32(dr["AMOUNT"]));//取代數量
                    string wareID = Convert.ToString(SortTable != null && SortTable.Rows.Count > 0 ? SortTable.Rows[0]["ProInv_WareHouseID"] : "0");//倉庫代碼
                    var dataList = ProductGroupSettingDB.GetProductGroupSettingListByID(pro_ID, wareID);//商品群組
                    bool isContains = SortTable.Select(string.Format("Pro_ID = '{0}'", pro_ID)).Length > 0;
                    if (!isContains)//如果傳回藥品不存在系統訂單內(不存在同商品)
                    {
                        bool isSucces = false;
                        foreach (var data in dataList)//迴圈判斷群組(紀錄取代商品
                        {
                            if (data.ID != pro_ID)//不等於已存在明細中的商品
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
            }
            else
            {
                DataTable table = dataTable.Clone();
                foreach (DataRow dr in dataTable.Rows)
                {
                    DataRow[] drs = table.Select(string.Format("PRO_ID = '{0}'", Convert.ToString(dr["PRO_ID"])));
                    if (drs == null || drs.Length == 0)
                    {
                        DataRow newRow = table.NewRow();
                        newRow["TYPE"] = dr["TYPE"];
                        newRow["PRO_ID"] = dr["PRO_ID"];
                        newRow["AMOUNT"] = dataTable.Compute("Sum(AMOUNT)", string.Format("PRO_ID = '{0}'", Convert.ToString(dr["PRO_ID"])));
                        newRow["PRICE"] = dataTable.Compute("Sum(PRICE)", string.Format("PRO_ID = '{0}'", Convert.ToString(dr["PRO_ID"])));
                        table.Rows.Add(newRow);
                    }
                }
                dataTable = StoreOrderDB.UpdateSingdeProductsByStoreOrderID(table, orederID, receiveID, checkCode);
            }

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