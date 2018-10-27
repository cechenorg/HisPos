using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Data;
using His_Pos.Class.StockTakingOrder;
using His_Pos.Interface;
using His_Pos.Class.ProductType;
using His_Pos.Struct.Product;
using LiveCharts;
using LiveCharts.Wpf;
using His_Pos.H7_ACCOUNTANCY_REPORT.EntrySerach;

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

        internal static string AddNewType(string parent, string name, string engName)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            if(parent.Equals(""))
                parameters.Add(new SqlParameter("PARENT", DBNull.Value));
            else
                parameters.Add(new SqlParameter("PARENT", parent));
            parameters.Add(new SqlParameter("CHINAME", name));
            parameters.Add(new SqlParameter("ENGNAME", engName));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[AddNewType]", parameters);

            return table.Rows[0]["ID"].ToString();
        }

        internal static void GetProductTypeManage(ObservableCollection<ProductTypeManageMaster> typeManageMasters, ObservableCollection<ProductTypeManageDetail> typeManageDetails)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[GetProductType]");
            
            foreach (DataRow row in table.Rows)
            {
                switch (row["PROTYP_RANK"].ToString())
                {
                    case "1":
                        typeManageMasters.Add(new ProductTypeManageMaster(row));
                        break;
                    case "2":
                        typeManageDetails.Add(new ProductTypeManageDetail(row));
                        break;
                }
            }
        }

        internal static ObservableCollection<AbstractClass.Product> GetProductTypeManageProducts()
        {
            ObservableCollection<AbstractClass.Product> collection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[GetProducts]");

            foreach (DataRow row in table.Rows)
            {
                switch (row["PROTYP_ID"].ToString())
                {
                    case "MED":
                        collection.Add(new ProductTypeMedicine(row));
                        break;
                    default:
                        collection.Add(new ProductTypeOTC(row));
                        break;
                }
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

        internal static Collection<PurchaseProduct> GetItemDialogProduct()
        {
            Collection<PurchaseProduct> collection = new Collection<PurchaseProduct>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetItemDialogProduct]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new PurchaseProduct(row));
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
                collection.Add(new OTCStockOverview(row));
            }

            return collection;
        }

        internal static void GetProductTypeLineSeries(LineSeries yearSalesLineSeries, LineSeries lastYearSalesLineSeries, LineSeries monthSalesLineSeries, LineSeries lastMonthSalesLineSeries, string typeId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PROTYP_ID", typeId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[GetTypeSalesByYears]", parameters);

            string thisYear = DateTime.Today.Year.ToString();
            ChartValues <double> thisYearSales = new ChartValues<double>();
            ChartValues<double> lastYearSales = new ChartValues<double>();

            foreach (DataRow row in table.Rows)
            {
                if (row["MONTH"].ToString().Contains(thisYear))
                {
                    thisYearSales.Add(Double.Parse(row["TOTAL"].ToString()));
                }
                else
                {
                    lastYearSales.Add(Double.Parse(row["TOTAL"].ToString()));
                }
            }
            yearSalesLineSeries.Values = thisYearSales;
            lastYearSalesLineSeries.Values = lastYearSales;

            parameters.Clear();
            parameters.Add(new SqlParameter("PROTYP_ID", typeId));

            table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[GetTypeSalesByMonths]", parameters);
            
            ChartValues<double> thisMonthSales = new ChartValues<double>();
            ChartValues<double> lastYearMonthSales = new ChartValues<double>();

            foreach (DataRow row in table.Rows)
            {
                if (row["DAY"].ToString().Contains(thisYear))
                {
                    thisMonthSales.Add(Double.Parse(row["TOTAL"].ToString()));
                }
                else
                {
                    lastYearMonthSales.Add(Double.Parse(row["TOTAL"].ToString()));
                }
            }
            monthSalesLineSeries.Values = thisMonthSales;
            lastMonthSalesLineSeries.Values = lastYearMonthSales;
        }

     

        internal static ObservableCollection<StockTakingOverview> GetProductStockTakingDate(string proId)
        {
            ObservableCollection<StockTakingOverview> collection = new ObservableCollection<StockTakingOverview>();
            
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetStockTakingOverview]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new StockTakingOverview(row));
            }

            return collection;
        }

        internal static AbstractClass.Product GetProductById(string id)
        {
            AbstractClass.Product product;

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[GetTypeProductDetail]", parameters);

            if (table.Rows[0]["PROTYP_ID"].ToString().Equals("MED"))
            {
                product = new InventoryMedicine(table.Rows[0]);
            }
            else
            {
                product = new InventoryOtc(table.Rows[0]);
            }

            return product;
        }

        internal static ListCollectionView GetProductType() {
            
            List<ProductType.ProductType> productTypes = new List<ProductType.ProductType>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProductType]");
            productTypes.Add(new ProductType.ProductType());
            foreach (DataRow row in table.Rows)
            {
                productTypes.Add(new ProductType.ProductType(row));
            }
            ListCollectionView collection = new ListCollectionView(productTypes);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("Rank"));
            return collection;
        }

        internal static void UpdateProductType(string typeId, string newTypeName, string newEngName)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE_ID", typeId));
            parameters.Add(new SqlParameter("TYPE_NAME", newTypeName));
            parameters.Add(new SqlParameter("ENG_NAME", newEngName));

            dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[UpdateTypeName]", parameters);
        }

        internal static void UpdateProductManufactory(string productId,string manId,int orderId) {
            
        var dd = new DbConnection(Settings.Default.SQL_global);

        var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productId));
            parameters.Add(new SqlParameter("MAN_ID", manId));
            parameters.Add(new SqlParameter("ORDER_ID", orderId));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEPROMAN]", parameters);
        }
        public static void SaveStockTaking(ObservableCollection<AbstractClass.Product> takingCollection, bool isComplete) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            DataTable details = new DataTable();
            details.Columns.Add("PRO_ID", typeof(string));
            details.Columns.Add("EMP_ID", typeof(string));
            details.Columns.Add("PROCHE_OLDVAL", typeof(string));
            details.Columns.Add("PROCHE_NEWVAL", typeof(string));
            details.Columns.Add("PROCHE_REASON", typeof(string));
            details.Columns.Add("PROCHE_VALUEDIFF", typeof(double));
            foreach (var product in takingCollection)
            {
                var newRow = details.NewRow();
                newRow["PRO_ID"] = product.Id;
                newRow["EMP_ID"] = ((IStockTaking)product).EmpId;
                newRow["PROCHE_OLDVAL"] = ((IStockTaking)product).Inventory;
                newRow["PROCHE_NEWVAL"] = ((IStockTaking)product).TakingResult;
                newRow["PROCHE_REASON"] = ((IStockTaking)product).TakingReason;
                newRow["PROCHE_VALUEDIFF"] = ((IStockTaking)product).ValueDiff;
                details.Rows.Add(newRow);
            }
            parameters.Add(new SqlParameter("DETAILS", details));
            parameters.Add(new SqlParameter("ISCOMPLETE", isComplete));
            dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[SaveStockTakingProducts]", parameters);
    }

        internal static void DeleteProductType(string id)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE_ID", id));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[DeleteType]", parameters);
        }
        internal static void ChangeProductType(string id,string newvalue)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id)); 
              parameters.Add(new SqlParameter("NEW_VALUE", newvalue));
            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductTypeManage].[ChangeProductType]", parameters);
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
                parameters.Add(new SqlParameter("STATUS", ((InventoryMedicine)product).Status));
                parameters.Add(new SqlParameter("CONTROL", ((InventoryMedicine)product).Control));
                parameters.Add(new SqlParameter("FROZEN", ((InventoryMedicine)product).Frozen));
                parameters.Add(new SqlParameter("COMMON", ((InventoryMedicine)product).Common));
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
                parameters.Add(new SqlParameter("STATUS", ((InventoryOtc)product).Status));
                parameters.Add(new SqlParameter("CONTROL", DBNull.Value));
                parameters.Add(new SqlParameter("FROZEN",DBNull.Value));
                parameters.Add(new SqlParameter("COMMON", DBNull.Value));
                dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProductDataDetail]", parameters);

            }
        }
        internal static ObservableCollection<ProductGroup> GetProductGroup(string proId,string warId)
        {
            ObservableCollection<ProductGroup> productGroups = new ObservableCollection<ProductGroup>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PROWAR_ID", warId)); 
            var table =  dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProductGroup]", parameters);
            foreach (DataRow row in table.Rows) {
                productGroups.Add(new ProductGroup(row));
            }
            return productGroups;
        }
        internal static void DemolitionProduct(string newProInvId,string proId,string proWarId,string demoAmount)
        { 
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NEWPROINV_ID", newProInvId));
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PROWAR_ID", proWarId));
            parameters.Add(new SqlParameter("DemolitionAmount", demoAmount));
            DataTable a = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[DemolitionProduct]", parameters);
        }
        internal static void MergeProduct(string soureProId,string targetProId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SourcePRO_ID", soureProId));
            parameters.Add(new SqlParameter("TargetPRO_ID", targetProId));
            dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[MergeProduct]", parameters);
        }
        internal static string GetMaxProInvId()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetMaxProInvId]");
            return table.Rows[0][0].ToString();
        }

        internal static ObservableCollection<IndexView.IndexView.ProductPurchaseList> DailyPurchaseReturn() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[Index].[DailyPurchaseReturn]");
            ObservableCollection<IndexView.IndexView.ProductPurchaseList> collection = new ObservableCollection<IndexView.IndexView.ProductPurchaseList>();
            foreach (DataRow row in table.Rows) {
                collection.Add(new IndexView.IndexView.ProductPurchaseList(row));
            }
            return collection;
        }
        internal static void InsertEntry(string entryName,string entryValue,string entrySource,string entrySourceId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ENTRY_NAME", entryName));
            parameters.Add(new SqlParameter("ENTRY_VALUE", entryValue));
            parameters.Add(new SqlParameter("ENTRY_SOURCE", entrySource));
            parameters.Add(new SqlParameter("ENTRY_SOURCE_ID", entrySourceId)); 
            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[InsertEntry]",parameters);
             
        }
        internal static string GetBucklePrice(string proId,string buckleAmount) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PROWAR_ID", "1"));
            parameters.Add(new SqlParameter("BuckleAmount", buckleAmount)); 
            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[GetBucklePrice]",parameters);
            return table.Rows[0][0].ToString();
        }
        internal static void UpdateDailyStockValue() {
            var dd = new DbConnection(Settings.Default.SQL_global); 
              dd.ExecuteProc("[HIS_POS_DB].[Index].[UpdateDailyStockValue]"); 
        }
        internal static ObservableCollection<EntrySearchView.DailyStockValue> GetDailyStockValue() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[Index].[GetDailyStockValue]");
            ObservableCollection<EntrySearchView.DailyStockValue> collection = new ObservableCollection<EntrySearchView.DailyStockValue>();
            foreach (DataRow row in table.Rows)
            {
                collection.Add(new EntrySearchView.DailyStockValue(row));
            }
            return collection;
        }
        
    }
} 