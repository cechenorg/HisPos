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
using System.Windows.Data;
using His_Pos.ProductPurchase;
using His_Pos.Interface;

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
        internal static DataTable GetStockTakingProduct()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[GetStockTakingProduct]");
        }

        internal static ObservableCollection<object> GetItemDialogProduct()
        {
            ObservableCollection<object> collection = new ObservableCollection<object>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetItemDialogProduct]");

            foreach (DataRow row in table.Rows)
            {
                switch (row["TYPE"].ToString())
                {
                    case "M":
                        collection.Add(new ProductPurchase.ProductPurchaseView.NewItemProduct(Boolean.Parse(row["IS_MAN"].ToString()), new ProductPurchaseMedicine(row, DataSource.GetItemDialogProduct)));
                        break;
                    case "O":
                        collection.Add(new ProductPurchase.ProductPurchaseView.NewItemProduct(Boolean.Parse(row["IS_MAN"].ToString()), new ProductPurchaseOtc(row, DataSource.GetItemDialogProduct)));
                        break;
                }
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
        internal static ListCollectionView GetProductType() {
            
            List<ProductType> productTypes = new List<ProductType>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProductType]");
            productTypes.Add(new ProductType("","","無"));
            foreach (DataRow row in table.Rows)
            {
                productTypes.Add(new ProductType(row["PROTYP_ID"].ToString(), row["PROTYP_PARENT"].ToString(), row["PROTYP_CHINAME"].ToString()));
            }
            ListCollectionView collection = new ListCollectionView(productTypes);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("Rank"));
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
        public static void SaveStockTaking(ObservableCollection<AbstractClass.Product> takingCollection) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            DataTable details = new DataTable();
            details.Columns.Add("PRO_ID", typeof(string));
            details.Columns.Add("EMP_ID", typeof(string));
            details.Columns.Add("PROCHE_OLDVAL", typeof(string));
            details.Columns.Add("PROCHE_NEWVAL", typeof(string));
            foreach (var product in takingCollection)
            {
                var newRow = details.NewRow();
                newRow["PRO_ID"] = product.Id;
                newRow["EMP_ID"] = MainWindow.CurrentUser.Id;
                newRow["PROCHE_OLDVAL"] = ((IStockTaking)product).Inventory;
                newRow["PROCHE_NEWVAL"] = ((IStockTaking)product).TakingResult;
                details.Rows.Add(newRow);
            }
            parameters.Add(new SqlParameter("DETAILS", details));
            dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[SaveStockTakingProducts]", parameters);
             
         
    }

        public static void UpdateOtcDataDetail(AbstractClass.Product product,string type)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            if (type == "InventoryMedicine") {
                parameters.Add(new SqlParameter("PRO_ID", ((InventoryMedicine)product).Id));
                parameters.Add(new SqlParameter("PRO_CHI", ((InventoryMedicine)product).ChiName));
                parameters.Add(new SqlParameter("PRO_ENG", ((InventoryMedicine)product).EngName));
                parameters.Add(new SqlParameter("SAFEQTY", ((InventoryMedicine)product).Stock.SafeAmount));
                parameters.Add(new SqlParameter("BASICQTY", ((InventoryMedicine)product).Stock.BasicAmount));
                parameters.Add(new SqlParameter("LOCATION", ((InventoryMedicine)product).Location));
                parameters.Add(new SqlParameter("PRO_DESCRIPTION", ((InventoryMedicine)product).Note));
                parameters.Add(new SqlParameter("@STATUS", ((InventoryMedicine)product).Status));
                parameters.Add(new SqlParameter("@CONTROL", ((InventoryMedicine)product).Control));
                parameters.Add(new SqlParameter("@FROZEN", ((InventoryMedicine)product).Frozen));
                dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProductDataDetail]", parameters);
            }
            else if (type == "InventoryOtc") {
                parameters.Add(new SqlParameter("PRO_ID", ((InventoryOtc)product).Id));
                parameters.Add(new SqlParameter("PRO_CHI", ((InventoryOtc)product).ChiName));
                parameters.Add(new SqlParameter("PRO_ENG", ((InventoryOtc)product).EngName));
                parameters.Add(new SqlParameter("SAFEQTY", ((InventoryOtc)product).Stock.SafeAmount));
                parameters.Add(new SqlParameter("BASICQTY", ((InventoryOtc)product).Stock.BasicAmount));
                parameters.Add(new SqlParameter("LOCATION", ((InventoryOtc)product).Location));
                parameters.Add(new SqlParameter("PRO_DESCRIPTION", ((InventoryOtc)product).Note));
                parameters.Add(new SqlParameter("@STATUS", ((InventoryOtc)product).Status));
                parameters.Add(new SqlParameter("@CONTROL", DBNull.Value));
                parameters.Add(new SqlParameter("@FROZEN", DBNull.Value));
                dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProductDataDetail]", parameters);

            }
        }
        
    }
}