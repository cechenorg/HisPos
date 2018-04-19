﻿using His_Pos.AbstractClass;
using His_Pos.Class.Manufactory;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public static class ProductDb
    {

        internal static ObservableCollection<ProductUnit> GetProductUnitById(string ID)
        {
            ObservableCollection<ProductUnit> collection = new ObservableCollection<ProductUnit>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", ID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProductUnit]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new ProductUnit(Int32.Parse(row["PRO_BASETYPE_STATUS"].ToString()), row["PROUNI_TYPE"].ToString(), row["PROUNI_QTY"].ToString(), row["PRO_SELL_PRICE"].ToString(), row["PRO_VIP_PRICE"].ToString(), row["PRO_EMP_PRICE"].ToString()));
            }

            return collection;
        }
        public static void UpdateOtcUnit(ProductUnit productunit, string proid)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proid));
            parameters.Add(new SqlParameter("PROUNI_TYPE", productunit.Unit));
            parameters.Add(new SqlParameter("PROUNI_QTY", productunit.Amount));
            parameters.Add(new SqlParameter("PRO_SELL_PRICE", productunit.Price));
            parameters.Add(new SqlParameter("PRO_VIP_PRICE", productunit.VIPPrice));
            parameters.Add(new SqlParameter("PRO_EMP_PRICE", productunit.EmpPrice));
            parameters.Add(new SqlParameter("PRO_BASETYPE_STATUS", productunit.Id));
            dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateUnit]", parameters);
        }
        internal static ObservableCollection<object> GetItemDialogProduct(string manId)
        {
            ObservableCollection<object> collection = new ObservableCollection<object>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[ITEMDIALOGPRODUCT]", parameters);

            foreach (DataRow row in table.Rows)
            {
                //switch((SearchType)Int16.Parse(row["TYPE"].ToString()))
                //{
                //    case SearchType.MED:
                //        collection.Add(new Medicine(row, DataSource.ITEMDIALOGPRODUCT));
                //        break;
                //    case SearchType.OTC:
                //        collection.Add(new Otc(row, DataSource.ITEMDIALOGPRODUCT));
                //        break;
                //}
            }

            return collection;
        }

        internal static string GetTotalWorth()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[InventoryManagementView].[InventoryTotalWorth]");
            
            return table.Rows[0]["TOTAL"].ToString();
        }
        
        internal static ObservableCollection<OTCStockOverview> GetProductStockOverviewById(string productID)
        {
            ObservableCollection<OTCStockOverview> collection = new ObservableCollection<OTCStockOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProductDetailStock]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new OTCStockOverview(row["VALIDDATE"].ToString(), row["PRICE"].ToString(), row["STOCK"].ToString()));
            }

            return collection;
        }
        internal static void UpdateProductManufactory(string productId,string manId,int orderId) {
            
        var dd = new DbConnection(Settings.Default.SQL_global);

        var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productId));
            parameters.Add(new SqlParameter("MAN_ID", manId));
            parameters.Add(new SqlParameter("ORDER_ID", orderId));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEPROMAN]", parameters);
        }
        public static void UpdateOtcDataDetail(AbstractClass.Product product,string type)
        {

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            if (type == "InventoryMedicine") {
                parameters.Add(new SqlParameter("PRO_ID", ((InventoryMedicine)product).Id));
                parameters.Add(new SqlParameter("SAFEQTY", ((InventoryMedicine)product).Stock.SafeAmount));
                parameters.Add(new SqlParameter("BASICQTY", ((InventoryMedicine)product).Stock.BasicAmount));
                parameters.Add(new SqlParameter("LOCATION", ((InventoryMedicine)product).Location));
                parameters.Add(new SqlParameter("PRO_DESCRIPTION", ((InventoryMedicine)product).Note));
                dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProductDataDetail]", parameters);
            }
            else if (type == "InventoryOtc") {
                parameters.Add(new SqlParameter("PRO_ID", ((InventoryOtc)product).Id));
                parameters.Add(new SqlParameter("SAFEQTY", ((InventoryOtc)product).Stock.SafeAmount));
                parameters.Add(new SqlParameter("BASICQTY", ((InventoryOtc)product).Stock.BasicAmount));
                parameters.Add(new SqlParameter("LOCATION", ((InventoryOtc)product).Location));
                parameters.Add(new SqlParameter("PRO_DESCRIPTION", ((InventoryOtc)product).Note));
                dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProductDataDetail]", parameters);

            }
        }
        
    }
}