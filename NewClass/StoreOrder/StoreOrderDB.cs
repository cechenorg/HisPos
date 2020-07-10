﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;

using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Product.PrescriptionSendData;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrderDB
    {
        public static string ReadSettingFilePharmacyName()
        {
            var filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (var fileReader = new StreamReader(filePath))
            {
                var medReg = new Regex(@"M (.*)");
                var recReg = new Regex(@"Rc (.*)");
                var recRegWithForm = new Regex(@"Rc (.*)[$](.*)");
                var repReg = new Regex(@"Rp (.*)");
                var verifyKey = fileReader.ReadLine();
                verifyKey = verifyKey.Substring(2, verifyKey.Length - 2);
                var xml = WebApi.GetPharmacyInfoByVerify(verifyKey);
                var PharmacyName = xml.SelectSingleNode("CurrentPharmacyInfo/Name").InnerText;
                return PharmacyName;
            }

        }

        #region ----- Define DataTable -----
        public static DataTable StoreOrderMasterTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("StoOrd_ID", typeof(string));
            masterTable.Columns.Add("StoOrd_OrderEmployeeID", typeof(int));
            masterTable.Columns.Add("StoOrd_ReceiveEmployeeID", typeof(int));
            masterTable.Columns.Add("StoOrd_CreateTime", typeof(DateTime));
            masterTable.Columns.Add("StoOrd_ReceiveTime", typeof(DateTime));
            masterTable.Columns.Add("StoOrd_ManufactoryID", typeof(int));
            masterTable.Columns.Add("StoOrd_Status", typeof(string));
            masterTable.Columns.Add("StoOrd_Type", typeof(string));
            masterTable.Columns.Add("StoOrd_WarehouseID", typeof(int));
            masterTable.Columns.Add("StoOrd_Note", typeof(string));
            masterTable.Columns.Add("StoOrd_PrescriptionID", typeof(int));
            masterTable.Columns.Add("StoOrd_IsEnable", typeof(bool));
            return masterTable;
        }
        public static DataTable StoreOrderDetailTable()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("StoOrdDet_MasterID", typeof(string));
            detailTable.Columns.Add("StoOrdDet_ProductID", typeof(string));
            detailTable.Columns.Add("StoOrdDet_ID", typeof(short));
            detailTable.Columns.Add("StoOrdDet_OrderAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_UnitName", typeof(string));
            detailTable.Columns.Add("StoOrdDet_UnitAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_RealAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_Price", typeof(double));
            detailTable.Columns.Add("StoOrdDet_SubTotal", typeof(double));
            detailTable.Columns.Add("StoOrdDet_ValidDate", typeof(DateTime));
            detailTable.Columns.Add("StoOrdDet_BatchNumber", typeof(string));
            detailTable.Columns.Add("StoOrdDet_Note", typeof(string));
            detailTable.Columns.Add("StoOrdDet_FreeAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_Invoice", typeof(string));
            return detailTable;
        }
        public static DataTable InventoryDetailTable()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("InvDet_ID", typeof(int));
            detailTable.Columns.Add("InvDet_ReturnAmount", typeof(double));
            return detailTable;
        }
        private static DataTable IDTable()
        {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("ID", typeof(int));
            return idTable;
        }
        public static DataTable SetIDTable(List<int> IDList)
        {
            DataTable table = IDTable();
            foreach (int id in IDList)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", id);
                table.Rows.Add(newRow);
            }
            return table;
        }
        private static string GetOrderStatus(OrderStatusEnum OrderStatus)
        {
            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    return "U";
                case OrderStatusEnum.WAITING:
                    return "W";
                case OrderStatusEnum.SINGDE_PROCESSING:
                case OrderStatusEnum.NORMAL_PROCESSING:
                    return "P";
                case OrderStatusEnum.SCRAP:
                    return "S";
                case OrderStatusEnum.DONE:
                    return "D";
                default:
                    return string.Empty;
            }
        }
        private static string GetOrderType(OrderTypeEnum orderTypeEnum)
        {
            switch (orderTypeEnum)
            {
                case OrderTypeEnum.PURCHASE:
                    return "P";
                case OrderTypeEnum.RETURN:
                    return "R";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region ----- Set DataTable ----- 
        #region ///// StoreOrderMasterTable /////
        public static DataTable SetPrescriptionOrderMaster(IndexReserve indexReserve)
        {
            DataTable storeOrderMasterTable = StoreOrderMasterTable();

            DataRow newRow = storeOrderMasterTable.NewRow();
            newRow["StoOrd_ID"] = indexReserve.StoOrdID;
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_OrderEmployeeID", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveEmployeeID", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_CreateTime", DateTime.Now);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveTime", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ManufactoryID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Status", "U");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Type", "P");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_WarehouseID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Note", "");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_PrescriptionID", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_IsEnable", true);
            storeOrderMasterTable.Rows.Add(newRow);

            return storeOrderMasterTable;
        }
        public static DataTable SetPrescriptionOrderMaster(Prescription.Prescription p)
        {
            DataTable storeOrderMasterTable = StoreOrderMasterTable();
            DataRow newRow = storeOrderMasterTable.NewRow();
            newRow["StoOrd_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_OrderEmployeeID", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveEmployeeID", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_CreateTime", DateTime.Now);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveTime", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ManufactoryID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Status", "U");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Type", "P");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_WarehouseID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Note", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_PrescriptionID", p.ID);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_IsEnable", true);




            storeOrderMasterTable.Rows.Add(newRow);
            return storeOrderMasterTable;
        }
        #endregion

        #region ///// StoreOrderDetailTable /////
        public static DataTable SetPrescriptionOrderDetail(IndexReserve indexReserves)
        {
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            int detailId = 1;
            foreach (var pro in indexReserves.IndexReserveDetailCollection)
            {
                if (pro.SendAmount == 0) continue;
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", pro.StoOrdID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.SendAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "顆");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        public static DataTable SetPrescriptionOrderDetail(PrescriptionSendDatas datas)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in datas)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.MedId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.SendAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "顆");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        private static DataTable SetPurchaseOrderDetail(PurchaseOrder p)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in p.OrderProducts)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", p.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.OrderAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", pro.UnitName);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", pro.UnitAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", pro.RealAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", pro.Price);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", pro.SubTotal);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", pro.ValidDate);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", pro.BatchNumber);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", pro.Note);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", pro.FreeAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", pro.Invoice);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        private static DataTable SetReturnOrderDetail(ReturnOrder r)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in r.ReturnProducts)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", r.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.ReturnAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", pro.UnitName);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", pro.UnitAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", pro.RealAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", pro.Price);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", pro.SubTotal);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", "");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", pro.Note);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }

        public static DataTable SetPrescriptionNotEnoughOrderDetail(NotEnoughMedicines purchaseList)
        {
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            int detailId = 1;
            foreach (var pro in purchaseList)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", purchaseList.StoreOrderID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.Amount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "顆");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }

        private static DataTable SetReturnInventoryDetail(ReturnProducts returnProducts)
        {
            DataTable inventoryDetailTable = InventoryDetailTable();
            foreach (var pro in returnProducts)
            {
                foreach (var inventoryDetail in pro.InventoryDetailCollection)
                {
                    DataRow newRow = inventoryDetailTable.NewRow();
                    DataBaseFunction.AddColumnValue(newRow, "InvDet_ID", inventoryDetail.ID);
                    DataBaseFunction.AddColumnValue(newRow, "InvDet_ReturnAmount", inventoryDetail.ReturnAmount);
                    inventoryDetailTable.Rows.Add(newRow);
                }
            }
            return inventoryDetailTable;
        }
        private static DataTable SetPurchaseOrderDetail(DataTable table, string storeOrderID)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (DataRow row in table.Rows)
            {
                string dateString = row.Field<string>("VALIDDATE");
                DateTime? validDate = null;
                if (dateString != null && dateString != string.Empty)
                    validDate = new DateTime(int.Parse(dateString.Substring(0, dateString.Length - 4)) + 1911, int.Parse(dateString.Substring(dateString.Length - 4, 2)), int.Parse(dateString.Substring(dateString.Length - 2, 2)));

                string realProductID = row.Field<string>("PRO_ID");

                switch (realProductID)
                {
                    case "NAN02A1000Z4":
                        realProductID = "NAN02A1000ZZ-4";
                        break;
                    case "NAN02A1000Z5":
                        realProductID = "NAN02A1000ZZ-5";
                        break;
                    case "NAN020632GNR":
                        realProductID = "NAN02A1000ZZ-N6";
                        break;
                    case "NAN02A1000Z8":
                        realProductID = "NAN02A1000ZZ-8";
                        break;
                    case "NAN020050NNR":
                        realProductID = "NAN02A1000ZZ-N8";
                        break;
                    case "NCS03A1000.3":
                        realProductID = "NCS03A1000ZZ-0.3";
                        break;
                    case "NCS03A1000.5":
                        realProductID = "NCS03A1000ZZ-0.5";
                        break;
                    case "NCS03A1001.0":
                        realProductID = "NCS03A1000ZZ-1.0";
                        break;
                }

                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", storeOrderID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", realProductID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", Math.Abs(double.Parse(row["AMOUNT"].ToString())));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", Math.Abs(double.Parse(row["AMOUNT"].ToString())));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", row.Field<float>("PRICE") / Math.Abs(double.Parse(row["AMOUNT"].ToString())));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", row.Field<float>("PRICE"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", validDate);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", row.Field<string>("BATCHNUM"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        private static DataTable SetPurchaseOrderDetail(string productsFromSingde, string storeOrderID, bool IsPrescription)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();

            if (IsPrescription)
            {
                string[] drugs = productsFromSingde.Replace("\n", "").Split(new[] { "\r" }, StringSplitOptions.None);

                for (int x = 3; x < drugs.Length; x++)
                {
                    if (drugs[x].Equals(string.Empty)) continue;
                    DataRow newRow = storeOrderDetailTable.NewRow();
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", storeOrderID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", drugs[x].Substring(0, 12).Trim());
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", (drugs[x].Length > 60) ? Math.Abs(Double.Parse(drugs[x].Substring(55, 10).Trim())) : Math.Abs(Double.Parse(drugs[x].Substring(39, 8).Trim())));
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", "");
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                    storeOrderDetailTable.Rows.Add(newRow);
                    detailId++;
                }
            }
            else
            {
                string[] drugs = productsFromSingde.Replace("\n", "").Split(new[] { "\r" }, StringSplitOptions.None);

                foreach (string drug in drugs)
                {
                    if (drug.Equals(string.Empty)) continue;
                    DataRow newRow = storeOrderDetailTable.NewRow();
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", storeOrderID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", drug.Substring(0, 12).Trim());
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", Math.Abs(Double.Parse(drug.Substring(12, 10).Trim())));
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", (drug.Length >= 22) ? drug.Substring(22).Trim() : "");
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                    storeOrderDetailTable.Rows.Add(newRow);
                    detailId++;
                }
            }

            return storeOrderDetailTable;
        }
        private static DataTable SetPurchaseOrderDetail(PurchaseProducts orderProducts)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();

            var productsAmount = orderProducts.GroupBy(p => p.ID).Select(g => new { ProductID = g.Key, Price = g.First().Price, OrderAmount = g.First().OrderAmount, RealAmount = g.Sum(p => p.RealAmount) }).ToList();

            foreach (var pro in productsAmount)
            {
                double newOrderAmount = pro.OrderAmount - pro.RealAmount;

                if (newOrderAmount <= 0) continue;

                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", "");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ProductID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", newOrderAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", pro.Price);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", pro.Price * newOrderAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", $"上次訂購量 {pro.OrderAmount} 到貨量 {pro.RealAmount}");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        #endregion

        #endregion


        internal static DataTable GetSingdeTotalOrders()
        {
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[SingdeTotalOrdersNotDone]");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetSingdeTotalOrders"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);


                return MainWindow.ServerConnection.ExecuteProc("[Get].[SingdeTotalOrdersNotDone]");

            }
        }
        internal static DataTable GetProcessingStoreOrdersByDate(string date)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE_STRING", date));




            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[SingdeTotalOrderProcessingOrders]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetProcessingStoreOrdersByDate"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);


                return MainWindow.ServerConnection.ExecuteProc("[Get].[SingdeTotalOrderProcessingOrders]", parameters);

            }
            //return MainWindow.ServerConnection.ExecuteProc("[Get].[SingdeTotalOrderProcessingOrders]", parameters);
        }
        internal static DataTable ReturnOrderRePurchase(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));


            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertReturnOrderRePurchase]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "ReturnOrderRePurchase"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);


                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertReturnOrderRePurchase]", parameters);

            }
            //return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertReturnOrderRePurchase]", parameters);
        }
        internal static DataTable RemoveStoreOrderByID(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));



            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrder]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "RemoveStoreOrderByID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);


                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrder]", parameters);

            }
            //return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrder]", parameters);
        }
        internal static DataTable RemoveStoreOrderToSingdeByID(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));

            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrderToSingde]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "RemoveStoreOrderToSingdeByID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrderToSingde]", parameters);
            }
        }

        internal static DataTable CheckReturnProductValid(ReturnOrder order)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameters, "DETAILS", SetReturnInventoryDetail(order.ReturnProducts));

            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckReturnStoreOrderValid]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "CheckReturnProductValid"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckReturnStoreOrderValid]", parameters);
            }
        }
        internal static DataTable ReturnOrderToProccessing(ReturnOrder order)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", order.ID));
            DataBaseFunction.AddSqlParameter(parameters, "DETAILS", SetReturnInventoryDetail(order.ReturnProducts));

            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToProcessing]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "ReturnOrderToProccessing"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToProcessing]", parameters);
            }
            //return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToProcessing]", parameters);
        }
        internal static DataTable AddStoreOrderLowerThenOrderAmount(string storeOrderID, string manufactoryID, string warehouseID, PurchaseProducts orderProducts)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMPLOYEE", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));
            parameters.Add(new SqlParameter("WARE_ID", warehouseID));
            parameters.Add(new SqlParameter("PRODUCTS", SetPurchaseOrderDetail(orderProducts)));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[StoreOrderAddLowerThenOrderAmount]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "AddStoreOrderLowerThenOrderAmount"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[StoreOrderAddLowerThenOrderAmount]", parameters);
            }
        }

        internal static DataTable AddNewStoreOrder(OrderTypeEnum orderType, Manufactory.Manufactory orderManufactory, int employeeID, int wareHouseID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", orderType.ToString()));
            parameters.Add(new SqlParameter("MANUFACTORY", orderManufactory.ID));
            parameters.Add(new SqlParameter("EMPLOYEE", employeeID));
            parameters.Add(new SqlParameter("WARE_ID", wareHouseID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[StoreOrderAddNewOrder]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "AddNewStoreOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[StoreOrderAddNewOrder]", parameters);
            }
        }

        internal static DataTable GetDoneStoreOrders(DateTime? searchStartDate, DateTime? searchEndDate, string searchOrderID, string searchManufactoryID, string searchProductID, string searchWareName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameters, "START_DATE", searchStartDate);
            DataBaseFunction.AddSqlParameter(parameters, "END_DATE", searchEndDate);
            parameters.Add(new SqlParameter("ORDER_ID", searchOrderID));
            parameters.Add(new SqlParameter("MANUFACTORY", searchManufactoryID));
            parameters.Add(new SqlParameter("PRODUCT", searchProductID));
            parameters.Add(new SqlParameter("WAREHOUSE", searchWareName));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderDone]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetDoneStoreOrders"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderDone]", parameters);
            }
        }

        internal static DataTable UpdateSingdeProductsByStoreOrderID(DataTable dataTable, string orederID, string receiveID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", orederID));
            parameters.Add(new SqlParameter("RECSTOORD_ID", receiveID));
            parameters.Add(new SqlParameter("DETAILS", SetPurchaseOrderDetail(dataTable, orederID)));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateSingdeProductsByStoreOrderID]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "UpdateSingdeProductsByStoreOrderID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateSingdeProductsByStoreOrderID]", parameters);
            }
        }

        internal static DataTable AddNewStoreOrderFromSingde(DataRow row)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", row.Field<string>("sht_no")));
            parameters.Add(new SqlParameter("NOTE", row.Field<string>("sht_memo")));
            parameters.Add(new SqlParameter("CREATE_DATE", row.Field<DateTime>("upload_date")));
            double num = Double.Parse(row.Field<string>("drug_list").Substring(12, 10).Trim());
            parameters.Add(new SqlParameter("STOORD_TYPE", num < 0 ? "R" : "P"));
            parameters.Add(new SqlParameter("DETAILS", SetPurchaseOrderDetail(row.Field<string>("drug_list"), row.Field<string>("sht_no"), false)));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderFromSingde]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "AddNewStoreOrderFromSingde"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderFromSingde]", parameters);
            }
        }

        internal static DataTable AddNewPrescriptionOrderFromSingde(DataRow row)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", row.Field<string>("rx_order")));
            parameters.Add(new SqlParameter("NOTE", row.Field<string>("inv_msg")));
            parameters.Add(new SqlParameter("CREATE_DATE", row.Field<DateTime>("upload_date")));
            parameters.Add(new SqlParameter("STOORD_TYPE", "P"));
            parameters.Add(new SqlParameter("DETAILS", SetPurchaseOrderDetail(row.Field<string>("dtl_data"), row.Field<string>("rx_order"), true)));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderFromSingde]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "AddNewPrescriptionOrderFromSingde"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderFromSingde]", parameters);
            }
        }

        internal static DataTable GetNotDoneStoreOrders()
        {
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderNotDone]");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetNotDoneStoreOrders"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderNotDone]");
            }
        }





        internal static void SaveReturnOrder(ReturnOrder returnOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", returnOrder.ID));
            DataBaseFunction.AddSqlParameter(parameters, "CUS_NAME", null);
            DataBaseFunction.AddSqlParameter(parameters, "TARGET_CUS_NAME", null);
            DataBaseFunction.AddSqlParameter(parameters, "PLAN_DATE", null);
            DataBaseFunction.AddSqlParameter(parameters, "STOORD_NOTE", returnOrder.Note);
            parameters.Add(new SqlParameter("STOORD_DETAIL", SetReturnOrderDetail(returnOrder)));
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[SaveStoreOrder]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "SaveReturnOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[SaveStoreOrder]", parameters);
            }
        }
        internal static void SavePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", purchaseOrder.ID));
            DataBaseFunction.AddSqlParameter(parameters, "CUS_NAME", purchaseOrder.PreOrderCustomer);
            DataBaseFunction.AddSqlParameter(parameters, "TARGET_CUS_NAME", purchaseOrder.TargetPreOrderCustomer);
            DataBaseFunction.AddSqlParameter(parameters, "PLAN_DATE", purchaseOrder.PlanArriveDate);
            DataBaseFunction.AddSqlParameter(parameters, "STOORD_NOTE", purchaseOrder.Note);
            parameters.Add(new SqlParameter("STOORD_DETAIL", SetPurchaseOrderDetail(purchaseOrder)));
            try
            {
                new SQLServerConnection().ExecuteProc("[Set].[SaveStoreOrder]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "SavePurchaseOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                new SQLServerConnection().ExecuteProc("[Set].[SaveStoreOrder]", parameters);
            }
        }
        public static DataTable InsertPrescriptionOrder(PrescriptionSendDatas prescriptionSendDatas, Prescription.Prescription p)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderMaster", SetPrescriptionOrderMaster(p));
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderDetail", SetPrescriptionOrderDetail(prescriptionSendDatas));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionStoreOrder]", parameterList);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "InsertPrescriptionOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionStoreOrder]", parameterList);
            }
        }
        public static DataTable InsertIndexReserveOrder(IndexReserve indexReserve)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderMaster", SetPrescriptionOrderMaster(indexReserve));
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderDetail", SetPrescriptionOrderDetail(indexReserve));
            DataBaseFunction.AddSqlParameter(parameterList, "CusName", indexReserve.CusName);
            DataBaseFunction.AddSqlParameter(parameterList, "RESERVE_ID", indexReserve.Id);
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertIndexReservesStoreOrder]", parameterList);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "InsertIndexReserveOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertIndexReservesStoreOrder]", parameterList);
            }
        }
        internal static DataTable GetStoOrdMasterCountByDate()
        {
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoOrdMasterCountByDate]");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetStoOrdMasterCountByDate"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoOrdMasterCountByDate]");
            }
        }
        internal static DataTable GetSingdeOrderNewStatus(string dateTime)
        {
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call GetOrderStatus('{ViewModelMainWindow.CurrentPharmacy.ID}', '{dateTime}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetSingdeOrderNewStatus"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call GetOrderStatus('{ViewModelMainWindow.CurrentPharmacy.ID}', '{dateTime}')");
            }
        }
        public static DataTable RemoveSingdeStoreOrderByID(string storeOrderID)
        {
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call RemoveOrder('{ViewModelMainWindow.CurrentPharmacy.ID}', '{storeOrderID}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "RemoveSingdeStoreOrderByID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call RemoveOrder('{ViewModelMainWindow.CurrentPharmacy.ID}', '{storeOrderID}')");
            }
        }
        public static DataTable UpdateSingdeStoreOrderSyncFlagByID(string storeOrderID)
        {
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call UpdateStoreOrderSyncFlag('{storeOrderID}', '{ViewModelMainWindow.CurrentPharmacy.ID}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "UpdateSingdeStoreOrderSyncFlagByID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call UpdateStoreOrderSyncFlag('{storeOrderID}', '{ViewModelMainWindow.CurrentPharmacy.ID}')");
            }
        }
        internal static DataTable PurchaseStoreOrderToDone(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePurchaseStoreOrderToDone]", parameters);

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "PurchaseStoreOrderToDone"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePurchaseStoreOrderToDone]", parameters);
            }
        }
        internal static void StoreOrderToNormalProcessing(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToNormalProcessing]", parameters);

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderToNormalProcessing"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToNormalProcessing]", parameters);
            }
        }
        internal static DataTable ReturnStoreOrderToDone(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToDone]", parameters);

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "ReturnStoreOrderToDone"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReturnStoreOrderToDone]", parameters);
            }
        }

        internal static void StoreOrderToScrap(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToScrap]", parameters);

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderToScrap"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToScrap]", parameters);
            }
        }
        internal static DataTable GetNewSingdePrescriptionOrders()
        {
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call GetNewPrescriptionOrderBySingde('{ViewModelMainWindow.CurrentPharmacy.ID}')");

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetNewSingdePrescriptionOrders"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call GetNewPrescriptionOrderBySingde('{ViewModelMainWindow.CurrentPharmacy.ID}')");
            }
        }

        internal static void StoreOrderToWaiting(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToWaiting]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderToWaiting"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToWaiting]", parameters);
            }
        }
        internal static DataTable SendStoreOrderToSingde(IndexReserve indexReserve, string note)
        {
            string orderMedicines = "";
            string cusName = "";
            string planDate = "";
            foreach (var product in indexReserve.IndexReserveDetailCollection)
            {
                if (product.SendAmount == 0) continue;
                if (product.ID.Length > 12)
                    orderMedicines += product.ID.Substring(0, 12);
                else
                    orderMedicines += product.ID.PadRight(12, ' ');

                orderMedicines += product.SendAmount.ToString().PadLeft(10, ' ');

                if (product.ID.Length > 12)
                    orderMedicines += product.ID.Substring(13);

                orderMedicines += "\r\n";
            }
            cusName = indexReserve.CusName;
            //planDate = (indexReserve.AdjustDate.Year - 1911) + indexReserve.AdjustDate.ToString("MMdd"); 

            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{indexReserve.StoOrdID}','{cusName}','','{note}', '{orderMedicines}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "SendStoreOrderToSingde"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{indexReserve.StoOrdID}','{cusName}','','{note}', '{orderMedicines}')");
            }
        }
        internal static DataTable SendStoreOrderToSingde(StoreOrder storeOrder)
        {
            string orderMedicines = "";
            string cusName = "";
            string planDate = "";
            if (storeOrder is PurchaseOrder)
            {
                foreach (var product in ((PurchaseOrder)storeOrder).OrderProducts)
                {
                    if (product.ID.Length > 12)
                        orderMedicines += product.ID.Substring(0, 12);
                    else
                        orderMedicines += product.ID.PadRight(12, ' ');

                    orderMedicines += product.OrderAmount.ToString().PadLeft(10, ' ');

                    if (product.ID.Length > 12)
                        orderMedicines += product.ID.Substring(13);

                    orderMedicines += product.Note;
                    orderMedicines += "\r\n";
                }

                cusName = ((PurchaseOrder)storeOrder).PreOrderCustomer;

                if (((PurchaseOrder)storeOrder).PlanArriveDate != null)
                    planDate = (((PurchaseOrder)storeOrder).PlanArriveDate?.Year - 1911) + ((PurchaseOrder)storeOrder).PlanArriveDate?.ToString("MMdd");
            }
            else
            {
                foreach (var product in ((ReturnOrder)storeOrder).ReturnProducts)
                {
                    if (product.ID.Length > 12)
                        orderMedicines += product.ID.Substring(0, 12);
                    else
                        orderMedicines += product.ID.PadRight(12, ' ');

                    orderMedicines += (-product.ReturnAmount).ToString().PadLeft(10, ' ');

                    if (product.ID.Length > 12)
                        orderMedicines += product.ID.Substring(13);

                    orderMedicines += product.Note;
                    orderMedicines += "\r\n";
                }
            }
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{storeOrder.ID}','{cusName}','{planDate}','{storeOrder.Note}', '{orderMedicines}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "SendStoreOrderToSingde"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{storeOrder.ID}','{cusName}','{planDate}','{storeOrder.Note}', '{orderMedicines}')");
            }
        }

        internal static DataTable SendStoreOrderToSingde(NotEnoughMedicines purchaseList, string note)
        {
            string orderMedicines = "";
            string cusName = "";
            string planDate = "";

            foreach (var product in purchaseList)
            {
                if (product.ID.Length > 12)
                    orderMedicines += product.ID.Substring(0, 12);
                else
                    orderMedicines += product.ID.PadRight(12, ' ');

                orderMedicines += product.Amount.ToString().PadLeft(10, ' ');

                if (product.ID.Length > 12)
                    orderMedicines += product.ID.Substring(13);

                orderMedicines += "\r\n";
            }
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{purchaseList.StoreOrderID}','','','{note}', '{orderMedicines}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "SendStoreOrderToSingde"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrderOrPreOrder('{ViewModelMainWindow.CurrentPharmacy.ID}','{purchaseList.StoreOrderID}','','','{note}', '{orderMedicines}')");
            }
        }

        internal static DataTable GetNewSingdeOrders()
        {
            try
            {
                return MainWindow.SingdeConnection.ExecuteProc($"call GetNewStoreOrderBySingde('{ViewModelMainWindow.CurrentPharmacy.ID}')");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetNewSingdeOrders"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.SingdeConnection.ExecuteProc($"call GetNewStoreOrderBySingde('{ViewModelMainWindow.CurrentPharmacy.ID}')");
            }
        }

        internal static DataTable GetManufactoryOrdersBySearchCondition(DateTime? startDate, DateTime? endDate, string manufactoryName, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));
            parameters.Add(new SqlParameter("MAN_NAME", manufactoryName));
            parameters.Add(new SqlParameter("WAREID", wareID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderManufactoryOrder]", parameters);

            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetManufactoryOrdersBySearchCondition"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderManufactoryOrder]", parameters);
            }
        }
        internal static DataTable GetManufactoryOrderDetails(int manufactoryID, DateTime searchStartDate, DateTime searchEndDate, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SDATE", searchStartDate));
            parameters.Add(new SqlParameter("EDATE", searchEndDate));
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));
            parameters.Add(new SqlParameter("WAREID", wareID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderManufactoryOrderDetail]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "GetManufactoryOrderDetails"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderManufactoryOrderDetail]", parameters);
            }
        }
        internal static DataTable StoreOrderReserveByResIDList(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMPLOYEE", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("sDate", sDate));
            parameters.Add(new SqlParameter("eDate", eDate));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderReserveByResIDList]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderReserveByResIDList"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderReserveByResIDList]", parameters);
            }
        }
        internal static DataTable StoreOrderReturnReserve()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMPLOYEE", ViewModelMainWindow.CurrentUser.ID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderReturnReserve]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderReturnReserve"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderReturnReserve]", parameters);
            }
        }
        internal static DataTable StoreOrderCommonMedicine()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMPLOYEE", ViewModelMainWindow.CurrentUser.ID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderCommonMedicine]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "StoreOrderCommonMedicine"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderCommonMedicine]", parameters);
            }
        }
        internal static DataTable DeleteDoneOrder(string orderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", orderID));
            parameters.Add(new SqlParameter("EMPLOYEE", ViewModelMainWindow.CurrentUser.ID));
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrderDoneOrderByID]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "DeleteDoneOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrderDoneOrderByID]", parameters);
            }
        }
        internal static void UpdateDetailByStoOrdID(PrescriptionSendDatas prescriptionSendDatas, string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameters, "STOORD_ID", storeOrderID);
            DataBaseFunction.AddSqlParameter(parameters, "DETAILS", SetPrescriptionOrderDetail(prescriptionSendDatas));
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescriptionStoreOrder]", parameters);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "UpdateDetailByStoOrdID"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescriptionStoreOrder]", parameters);
            }
        }

        public static DataTable InsertNotEnoughPurchaseOrder(NotEnoughMedicines purchaseList, string note, string cusName)
        {

            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderDetail", SetPrescriptionNotEnoughOrderDetail(purchaseList));
            DataBaseFunction.AddSqlParameter(parameterList, "EMP_ID", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "NOTE", note);
            DataBaseFunction.AddSqlParameter(parameterList, "CUS_NAME", cusName);
            try
            {
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionNotEnoughStoreOrder]", parameterList);
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "InsertNotEnoughPurchaseOrder"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionNotEnoughStoreOrder]", parameterList);
            }
        }

        public static void UpdateProductOnTheWay()
        {
            try
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateProductOnTheWay]");
            }
            catch
            {
                List<SqlParameter> errorparameters = new List<SqlParameter>();
                errorparameters.Add(new SqlParameter("PharmacyName", ReadSettingFilePharmacyName().ToString()));
                errorparameters.Add(new SqlParameter("ErrorLog", "UpdateProductOnTheWay"));
                MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Set].[RecordErrorLog]", errorparameters);
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateProductOnTheWay]");
            }
        }
    }
}
