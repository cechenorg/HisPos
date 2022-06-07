using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductDetailDB
    {
        internal static DataTable GetProductManageStructsByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero, string wareID, bool searchIsSingdeInventory)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", searchID));
            parameters.Add(new SqlParameter("PRO_NAME", searchName));
            parameters.Add(new SqlParameter("SHOW_DISABLE", searchIsEnable));
            parameters.Add(new SqlParameter("SHOW_INV_ZERO", searchIsInventoryZero));
            parameters.Add(new SqlParameter("WAREID", int.Parse(wareID)));
            parameters.Add(new SqlParameter("SHOW_INV_SINGDE", searchIsSingdeInventory));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageStructBySearchCondition]", parameters);
        }

        internal static DataTable GetOTCMedBagDetailByID(string proID, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductOTCMedBagDetailByID]", parameters);
        }

        internal static DataTable GetOnTheWayDetailByID(string proID, string wareID,int isMedBag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("IsMedBag", isMedBag));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageOnTheWayDetailByID]", parameters);
        }

        internal static DataTable GetRegisterPrescriptionsByID(string proID, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductRegisterPrescriptionsByID]", parameters);
        }

        internal static DataTable GetStockDetailByID(string proID, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStockDetailByID]", parameters);
        }

        internal static DataTable GetMedicineHistoryPrices(string medicineID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MED_ID", medicineID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineHistoryPrices]", parameters);
        }

        internal static DataTable GetInventoryRecordsByID(string proID, string wareID, DateTime startDate, DateTime endDate)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductInventoryRecordByID]", parameters);
        }

        internal static DataTable SetSelfPayMultiplier(double selfPayMultiplier)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NAME", "SelfPayMultiplier"));
            parameters.Add(new SqlParameter("VALUE", selfPayMultiplier.ToString("N1")));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateSystemParameters]", parameters);
        }

        internal static DataTable SetPrices(string proid, double retailprice, double memberprice, double employeeprice, double specialprice)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Pro_ID", proid));
            parameters.Add(new SqlParameter("Pro_RetailPrice", retailprice));
            parameters.Add(new SqlParameter("Pro_MemberPrice", memberprice));
            parameters.Add(new SqlParameter("Pro_EmployeePrice", employeeprice));
            parameters.Add(new SqlParameter("Pro_SpecialPrice", specialprice));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateOTCPrice]", parameters);
        }

        internal static DataTable GetMedBagDetailByID(string proID, string wareID,int isInsuff)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("IsInsuff", isInsuff));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductMedBagDetailByID]", parameters);
        }

        internal static DataTable GetProductManageMedicineDataByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageMedicineByID]", parameters);
        }

        internal static DataTable UpdateMedicineDetailData(ProductManageMedicine productManageMedicine)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productManageMedicine.ID));
            DataBaseFunction.AddSqlParameter(parameters, "PRO_CHINAME", productManageMedicine.ChineseName);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_ENGNAME", productManageMedicine.EnglishName);
            parameters.Add(new SqlParameter("PRO_ISENABLE", productManageMedicine.Status));
            DataBaseFunction.AddSqlParameter(parameters, "PRO_BARCODE", productManageMedicine.BarCode);
            parameters.Add(new SqlParameter("PRO_ISCOMMON", productManageMedicine.IsCommon));
            DataBaseFunction.AddSqlParameter(parameters, "PRO_SAFEAMOUNT", productManageMedicine.SafeAmount);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_BASICAMOUNT", productManageMedicine.BasicAmount);
            parameters.Add(new SqlParameter("PRO_MINORDER", productManageMedicine.MinOrderAmount));
            DataBaseFunction.AddSqlParameter(parameters, "PRO_INDICATION", productManageMedicine.Indication);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_SIDEEFFECT", productManageMedicine.SideEffect);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_WARNING", productManageMedicine.Warnings);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_NOTE", productManageMedicine.Note);
            DataBaseFunction.AddSqlParameter(parameters, "PRO_SELFPAYTYPE", productManageMedicine.SelfPayType == SelfPayTypeEnum.Default ? "D" : "C");
            DataBaseFunction.AddSqlParameter(parameters, "PRO_SELFPAYPRICE", productManageMedicine.SelfPayPrice);
            parameters.Add(new SqlParameter("Pro_IsReward", productManageMedicine.IsReward));
            DataBaseFunction.AddSqlParameter(parameters, "Pro_RewardPercent", productManageMedicine.RewardPercent);

            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateMedicineDetailData]", parameters);
        }

        internal static DataTable GetTotalStockValue(string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WAREID", int.Parse(wareID)));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTotalStockValue]", parameters);
        }

        internal static void StockTakingProductManageMedicineByID(string productID, string newInventory, string warID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ProID", productID));
            parameters.Add(new SqlParameter("Inventory", newInventory));
            parameters.Add(new SqlParameter("warID", warID));
            parameters.Add(new SqlParameter("typeName", "單品盤點"));
            parameters.Add(new SqlParameter("Source", "無"));
            parameters.Add(new SqlParameter("SourceID", "無"));
            MainWindow.ServerConnection.ExecuteProc("[Set].[ProductStockCheck]", parameters);
        }

        internal static void UpdateProductLastPrice(string productID, double price, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));
            parameters.Add(new SqlParameter("LAST_PRICE", price));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateProductLastPrice]", parameters);
        }

        internal static DataTable GetProductTypeByID(string newProductID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", newProductID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTypeByID]", parameters);
        }

        internal static DataTable GetProductManageOTCMedicineDetailByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageOTCMedicineDetailByID]", parameters);
        }

        internal static DataTable GetProductManageNHIMedicineDetailByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageNHIMedicineDetailByID]", parameters);
        }

        internal static DataTable GetProductManageSpecialMedicineDetailByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageSpecialMedicineDetailByID]", parameters);
        }

        internal static DataTable ScrapProductByID(string id, string scrapAmount, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("SCRAP_AMOUNT", scrapAmount));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductScrap]", parameters);
        }

        internal static DataTable GetMedicineStockDetailByID(string id, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineStockDetailByID]", parameters);
        }

        internal static DataTable GetOTCStockDetailByID(string id, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[OTCStockDetailByID]", parameters);
        }

        internal static DataTable RecycleProductByID(string id, string recycleAmount, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("RECYCLE_AMOUNT", recycleAmount));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductRecycle]", parameters);
        }
    }
}