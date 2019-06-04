﻿using His_Pos.Database;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product
{
    public class ProductDB
    {
        internal static DataTable GetProductStructsBySearchString(string searchString, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStructBySearchString]", parameters);
        }

        internal static DataTable GetPurchaseProductStructCountBySearchString(string searchString)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[PurchaseProductStructCountBySearchString]", parameters);
        }

        internal static DataTable GetReturnProductStructCountBySearchString(string searchString, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductStructCountBySearchString]", parameters);
        }
        internal static string GetProductNewID (int ID) {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            DataBaseFunction.AddSqlParameter(parameterList, "TypeID", ID);
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ProductNewIDByType]", parameterList);
            return  table.Rows[0].Field<string>("NewID"); 
        }
        public static void InsertProduct(string typeID,string proID,string proChinese,string proEnglish,int proPrice) {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 

            if (!string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
            {
                DataTable table = PharmacyDb.GroupPharmacySchemaList();
                foreach (DataRow r in table.Rows)
                {
                    parameterList.Clear();
                    DataBaseFunction.AddSqlParameter(parameterList, "TypeID", typeID);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_ID", proID);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_Chinese", proChinese);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_English", proEnglish);
                    DataBaseFunction.AddSqlParameter(parameterList, "Pro_Price", proPrice);
                    MainWindow.ServerConnection.ExecuteProcBySchema(r.Field<string>("SchemaList"), "[Set].[InsertProduct]", parameterList);
                }
            }
            else {
                DataBaseFunction.AddSqlParameter(parameterList, "TypeID", typeID);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_ID", proID);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_Chinese", proChinese);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_English", proEnglish);
                DataBaseFunction.AddSqlParameter(parameterList, "Pro_Price", proPrice);
                MainWindow.ServerConnection.ExecuteProc("[Set].[InsertProduct]", parameterList);
            }
             
        }
         
    }
}
