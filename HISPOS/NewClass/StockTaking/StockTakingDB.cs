using His_Pos.Database;
using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.StockTaking
{
    public class StockTakingDB
    {
        internal static DataTable GetStockTakingPlans()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlans]");
        }

        internal static void NewStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "name", stockTakingPlan.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", stockTakingPlan.WareHouse.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "note", stockTakingPlan.Note);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStockTakingPlan]", parameterList);
        }

        internal static void DeleteStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", stockTakingPlan.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStockTakingPlan]", parameterList);
        }

        internal static DataTable GetStockTakingPlanProductByType(string type, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "type", type);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductByType]", parameterList);
        }

        internal static DataTable GetStockTakingPlanProductByProName(string name, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", name);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductByProName]", parameterList);
        }

        internal static DataTable GetStockTakingPlanProductsByID(int planID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PLAN_ID", planID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductsByID]", parameterList);
        }

        internal static DataTable GetStockTakingProductsByID(string ID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoTakID", ID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingProductsByID]", parameterList);
        }

        internal static DataTable GetStockTakingProductsInventory(StockTakingPlanProducts s, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProductIDs", SetStockTakingPlanProducts(s));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingProductsInventory]", parameterList);
        }

        internal static void UpdateStockTakingPlan(StockTakingPlan.StockTakingPlan stockTakingPlan)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", stockTakingPlan.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Name", stockTakingPlan.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Note", stockTakingPlan.Note);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductIDs", SetStockTakingPlanProducts(stockTakingPlan.StockTakingProductCollection));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStockTakingPlan]", parameterList);
        }

        internal static DataTable GetStockTakingRecordByCondition(DateTime? sDate, DateTime? eDate, string proID, string proName)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate is null ? null : sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate is null ? null : eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductID", string.IsNullOrEmpty(proID) ? null : proID);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductName", string.IsNullOrEmpty(proName) ? null : proName);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingRecordByCondition]", parameterList);
        }

        internal static void InsertStockTaking(StockTaking.StockTaking stockTaking, string typeName)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "EmpID", ChromeTabViewModel.ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", stockTaking.WareHouse.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "TypeName", typeName);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductIDs", SetStockTakingPlanProducts(stockTaking.StockTakingProductCollection));
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStockTaking]", parameterList);
        }

        internal static void InsertStockChange(StockTaking.StockTaking stockTaking, string typeName, string Number)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "EmpID", ChromeTabViewModel.ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", stockTaking.WareHouse.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "TypeName", typeName);
            DataBaseFunction.AddSqlParameter(parameterList, "ProductIDs", SetStockTakingPlanProducts(stockTaking.StockTakingProductCollection));
            DataBaseFunction.AddSqlParameter(parameterList, "Number", Number);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStockChange]", parameterList);
        }

        internal static DataTable GetStockTakingTotalPrice(StockTakingProduct.StockTakingProduct stockTakingProduct, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "IDAmount", SetProductAmount(stockTakingProduct));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductAmountTotalPrice]", parameterList);
        }

        internal static DataTable StockTakingPlanProductByWarID(StockTakingPlanProducts stockTakingPlanProducts, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "products", SetStockTakingPlanProducts(stockTakingPlanProducts));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingPlanProductByWarID]", parameterList);
        }

        #region TableSet

        public static DataTable ProductListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("MedicineID", typeof(string));
            return masterTable;
        }

        public static DataTable ProductAmountTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("ID", typeof(int));
            masterTable.Columns.Add("Amount", typeof(double));
            return masterTable;
        }

        public static DataTable StockTakingProductListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("StoTakDet_ProductID", typeof(string));
            masterTable.Columns.Add("StoTakDet_EmployeeID", typeof(int));
            masterTable.Columns.Add("StoTakDet_OldValue", typeof(double));
            masterTable.Columns.Add("StoTakDet_NewValue", typeof(double));
            masterTable.Columns.Add("StoTakDet_Note", typeof(string));
            masterTable.Columns.Add("StoTakDet_ValueDiff", typeof(double));
            masterTable.Columns.Add("TakingPrice", typeof(double));
            return masterTable;
        }

        public static DataTable SetProductAmount(StockTakingProduct.StockTakingProduct productId)
        {
            DataTable productListTable = ProductAmountTable();

            DataRow newRow = productListTable.NewRow();
            DataBaseFunction.AddColumnValue(newRow, "ID", productId.InvID);
            DataBaseFunction.AddColumnValue(newRow, "Amount", productId.NewInventory + productId.MedBagAmount);
            productListTable.Rows.Add(newRow);
            return productListTable;
        }

        public static DataTable SetStockTakingPlanProducts(StockTakingProducts productIds)
        {
            DataTable productListTable = StockTakingProductListTable();
            foreach (var m in productIds)
            {
                DataRow newRow = productListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_ProductID", m.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_EmployeeID", ChromeTabViewModel.ViewModelMainWindow.CurrentUser.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_OldValue", m.Inventory);
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_NewValue", m.NewInventory);
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_Note", m.Note);
                DataBaseFunction.AddColumnValue(newRow, "StoTakDet_ValueDiff", m.NewInventoryTotalPrice - m.TotalPrice);
                DataBaseFunction.AddColumnValue(newRow, "TakingPrice", m.TakingPrice);
                productListTable.Rows.Add(newRow);
            }
            return productListTable;
        }

        public static DataTable SetStockTakingPlanProducts(StockTakingPlanProducts productIds)
        {
            DataTable productListTable = ProductListTable();
            foreach (var m in productIds)
            {
                DataRow newRow = productListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m.ID);
                productListTable.Rows.Add(newRow);
            }
            return productListTable;
        }

        #endregion TableSet
    }
}