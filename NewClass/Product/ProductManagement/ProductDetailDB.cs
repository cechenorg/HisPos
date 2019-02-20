using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductDetailDB
    {
        internal static DataTable GetProductManageStructsByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", searchID));
            parameters.Add(new SqlParameter("PRO_NAME", searchName));
            parameters.Add(new SqlParameter("SHOW_DISABLE", searchIsEnable));
            parameters.Add(new SqlParameter("SHOW_INV_ZERO", searchIsInventoryZero));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageStructBySearchCondition]", parameters);
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
            parameters.Add(new SqlParameter("PRO_CHINAME", productManageMedicine.ChineseName));
            parameters.Add(new SqlParameter("PRO_ENGNAME", productManageMedicine.EnglishName));
            parameters.Add(new SqlParameter("PRO_ISENABLE", productManageMedicine.Status));
            parameters.Add(new SqlParameter("PRO_BARCODE", productManageMedicine.BarCode));
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

        internal static DataTable GetTotalStockValue()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTotalStockValue]");
        }

        internal static void StockTakingProductManageMedicineByID(string productID, string newInventory)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ProID", productID));
            parameters.Add(new SqlParameter("Inventory", newInventory));

            MainWindow.ServerConnection.ExecuteProc("[Set].[ProductStockCheck]", parameters);
        }
    }
}
