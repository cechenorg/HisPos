using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product
{
    public class ProductDB
    {
        internal static DataTable GetProductStructsBySearchString(string searchString, string wareID,int noOTC)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));
            parameters.Add(new SqlParameter("noOTC", noOTC));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStructBySearchString]", parameters);
        }

        internal static DataTable GetPurchaseProductStructCountBySearchString(string searchString,string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PurchaseProductStructCountBySearchString]", parameters);
        }

        internal static DataTable GetProductStructCountBySearchString(string searchString, string wareID, int type, int noOTC)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));
            parameters.Add(new SqlParameter("TYPE", type));
            parameters.Add(new SqlParameter("noOTC", noOTC));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStructCountBySearchString]", parameters);
        }

        internal static DataTable GetProductUsageRecordByIDForExport(string proID, DateTime startDate, DateTime endDate, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductUsageRecordByIDForExport]", parameters);
        }

        internal static DataTable GetProductInventoryRecordByIDForExport(string proID, DateTime startDate, DateTime endDate, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductInventoryRecordByIDForExport]", parameters);
        }

        internal static DataTable GetTradeProductStructCountBySearchString(string searchString)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[TradeProductStructCountBySearchString]", parameters);
        }

        internal static DataTable GetProductConsumeRecordByID(string productID, string wareID, DateTime startDate, DateTime endDate)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));
            parameters.Add(new SqlParameter("SDATE", startDate));
            parameters.Add(new SqlParameter("EDATE", endDate));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductConsumeRecordByID]", parameters);
        }

        internal static DataTable GetReturnProductStructCountBySearchString(string searchString, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductStructCountBySearchString]", parameters);
        }

        internal static string GetProductNewID(int ID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "TypeID", ID);
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ProductNewIDByType]", parameterList);
            return table.Rows[0].Field<string>("NewID");
        }

        public static DataTable InsertProduct(string typeID, string proID, string proChinese, string proEnglish)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataTable result = new DataTable();

            /*if (!string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
            {
                DataTable table = PharmacyDb.GroupPharmacySchemaList();
                foreach (DataRow r in table.Rows)
                {
                    parameterList.Clear();
                    DataBaseFunction.AddSqlParameter(parameterList, "TypeID", typeID);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_ID", proID);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_Chinese", proChinese);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_English", proEnglish);
                    result = MainWindow.ServerConnection.ExecuteProcBySchema(r.Field<string>("SchemaList"), "[Set].[InsertProduct]", parameterList);
                }
                return result;
            }
            else {
                DataBaseFunction.AddSqlParameter(parameterList, "TypeID", typeID);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_ID", proID);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_Chinese", proChinese);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_English", proEnglish);
                return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertProduct]", parameterList);
            }*/

            DataBaseFunction.AddSqlParameter(parameterList, "TypeID", typeID);
            DataBaseFunction.AddSqlParameter(parameterList, "Pro_ID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "Pro_Chinese", proChinese);
            DataBaseFunction.AddSqlParameter(parameterList, "Pro_English", proEnglish);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertProduct]", parameterList);
        }

        internal static DataTable GetAllInventoryByProIDs(List<string> MedicineIds, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Products", SetMedicines(MedicineIds));
            DataBaseFunction.AddSqlParameter(parameterList, "Ware_ID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AllInventoryByProIDs]", parameterList);
        }

        public static DataTable SetMedicines(List<string> MedicineIds)
        {
            DataTable medicineListTable = MedicineListTable();
            foreach (string m in MedicineIds)
            {
                DataRow newRow = medicineListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m);
                medicineListTable.Rows.Add(newRow);
            }
            return medicineListTable;
        }

        public static DataTable MedicineListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("MedicineID", typeof(string));
            return masterTable;
        }

        public static void UpdateAllInventoryMedBagAmount()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateAllInventoryMedBagAmount]");
        }

        public static DataTable GetAllProductsInventory()
        {
            var table = MainWindow.SingdeConnection.ExecuteProc($"call GetAllProductsInventory");
            return table;
        }

        public static DataTable GetDataToUpdateSingdeStock()
        {
            var table = MainWindow.SingdeConnection.ExecuteProc($"call GetDataToUpdateSingdeStock");
            return table;
        }
        public static DataTable GetDataToInsertSingdeOTC()
        {
            var table = MainWindow.SingdeConnection.ExecuteProc($"call GetDataToInsertSingdeOTC");
            return table;
        }
    }
}