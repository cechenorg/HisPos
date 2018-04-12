using His_Pos.AbstractClass;
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

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[PRODUCTUNIT]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new ProductUnit(Int32.Parse(row["PRO_BASETYPE_STATUS"].ToString()), row["PROUNI_TYPE"].ToString(), row["PROUNI_QTY"].ToString(), row["PRO_SELL_PRICE"].ToString(), row["PRO_VIP_PRICE"].ToString(), row["PRO_EMP_PRICE"].ToString()));
            }

            return collection;
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
                switch((SearchType)Int16.Parse(row["TYPE"].ToString()))
                {
                    case SearchType.MED:
                        collection.Add(new Medicine(row, DataSource.ITEMDIALOGPRODUCT));
                        break;
                    case SearchType.OTC:
                        collection.Add(new Otc(row, DataSource.ITEMDIALOGPRODUCT));
                        break;
                }
            }

            return collection;
        }

        internal static string GetTotalWorth()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[INVENTORYTOTALWORTH]");
            
            return table.Rows[0]["TOTAL"].ToString();
        }
        
        internal static ObservableCollection<OTCStockOverview> GetProductStockOverviewById(string productID)
        {
            ObservableCollection<OTCStockOverview> collection = new ObservableCollection<OTCStockOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[PRODUCTDETAILSTOCK]", parameters);

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
        public static void UpdateOtcDataDetail(AbstractClass.Product product)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", product.Id));
            parameters.Add(new SqlParameter("SAFEQTY", product.SafeAmount));
            parameters.Add(new SqlParameter("BASICQTY", product.BasicAmount));
            parameters.Add(new SqlParameter("LOCATION", product.Location));
            parameters.Add(new SqlParameter("PRO_DESCRIPTION", product.Note));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEOTCDATADETAIL]", parameters);
        }
        
    }
}