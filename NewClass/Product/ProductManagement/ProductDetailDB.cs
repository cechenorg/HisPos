using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductDetailDB
    {
        internal static DataTable GetProductManageStructsByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", searchID));
            parameters.Add(new SqlParameter("PRO_NAME", searchName));
            parameters.Add(new SqlParameter("SHOW_DISABLE", searchIsEnable));
            parameters.Add(new SqlParameter("SHOW_INV_ZERO", searchIsInventoryZero));
            parameters.Add(new SqlParameter("WAREID", int.Parse(wareID)));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageStructBySearchCondition]", parameters);
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

        internal static DataTable GetProductManageMedicineDataByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageMedicineByID]", parameters);
        }

        internal static void UpdateMedicineDetailData(ProductManageMedicine productManageMedicine)
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

            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateMedicineDetailData]", parameters);
        }

        internal static DataTable GetTotalStockValue(string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WAREID", int.Parse(wareID)));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTotalStockValue]", parameters);
        }

        internal static void StockTakingProductManageMedicineByID(string productID, string newInventory)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ProID", productID));
            parameters.Add(new SqlParameter("Inventory", newInventory));

            MainWindow.ServerConnection.ExecuteProc("[Set].[ProductStockCheck]", parameters);
        }

        internal static void UpdateProductLastPrice(string productID, double price)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));
            parameters.Add(new SqlParameter("LAST_PRICE", price));

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

        internal static DataTable GetMedicineStockDetailByID(string id, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineStockDetailByID]", parameters);
        }
    }
}
