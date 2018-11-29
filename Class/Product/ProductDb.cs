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
using His_Pos.InventoryManagement;
using His_Pos.H7_ACCOUNTANCY_REPORT.CooperativeEntry;
using System.Linq;
using His_Pos.H7_ACCOUNTANCY_REPORT.CooperativeAdjustReport;

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
                collection.Add(new ProductUnit(row));
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
            //var dd = new DbConnection(Settings.Default.SQL_global);
            //var parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("PRO_ID", proid));
            //parameters.Add(new SqlParameter("PROUNI_TYPE", productunit.Unit));
            //parameters.Add(new SqlParameter("PROUNI_QTY", productunit.Amount));
            //parameters.Add(new SqlParameter("PRO_SELL_PRICE", productunit.Price));
            //parameters.Add(new SqlParameter("PRO_VIP_PRICE", productunit.VIPPrice));
            //parameters.Add(new SqlParameter("PRO_EMP_PRICE", productunit.EmpPrice));
            //parameters.Add(new SqlParameter("PRO_BASETYPE_STATUS", productunit.Id));
            //dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateUnit]", parameters);
        }

        internal static ObservableCollection<InventoryDetailOverview> GetInventoryDetailOverviews(string proId)
        {
            ObservableCollection<InventoryDetailOverview> collection = new ObservableCollection<InventoryDetailOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetInventoryDetailOverview]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new InventoryDetailOverview(row));
            }

            return collection;
        }

        internal static MedicineDetail.InventoryMedicineDetail GetInventoryMedicineDetail(string proid)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proid));

            var table = dd.ExecuteProc("[HIS_POS_DB].[MedicineDetail].[GetInventoryMedicineDetail]", parameters);

            return new MedicineDetail.InventoryMedicineDetail(table.Rows[0]);
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
        public static void SaveStockTaking(ObservableCollection<AbstractClass.Product> takingCollection) {
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
            dd.ExecuteProc("[HIS_POS_DB].[StockTaking].[SaveStockTakingProducts]", parameters);
    }

        internal static void UpdateInventoryProductUnit(string proId, ObservableCollection<ProductUnit> units)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("PRO_ID", proId));

            DataTable unitTable = new DataTable();
            unitTable.Columns.Add("UNI_ID", typeof(int));
            unitTable.Columns.Add("UNI_TYPE", typeof(string));
            unitTable.Columns.Add("UNI_QTY", typeof(int));
            unitTable.Columns.Add("UNI_SELL", typeof(double));
            unitTable.Columns.Add("UNI_VIP", typeof(double));
            unitTable.Columns.Add("UNI_EMP", typeof(double));

            int index = 1;

            foreach (var unit in units)
            {
                var newRow = unitTable.NewRow();
                newRow["UNI_ID"] = index;
                newRow["UNI_TYPE"] = unit.Unit;
                newRow["UNI_QTY"] = unit.Amount;
                newRow["UNI_SELL"] = unit.Price;
                newRow["UNI_VIP"] = unit.VIPPrice;
                newRow["UNI_EMP"] = unit.EmpPrice;
                unitTable.Rows.Add(newRow);

                index++;
            }
            parameters.Add(new SqlParameter("UNITS", unitTable));
            dd.ExecuteProc("[HIS_POS_DB].[InventoryManagementView].[UpdateProductUnit]", parameters);
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
        internal static void InsertCashFow(string cashflowName, string cashflowValue, string cashflowSource, string cashflowSourceId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CASHFLOW_NAME", cashflowName));
            parameters.Add(new SqlParameter("CASHFLOW_VALUE", cashflowValue));
            parameters.Add(new SqlParameter("CASHFLOW_SOURCE", cashflowSource));
            parameters.Add(new SqlParameter("CASHFLOW_SOURCE_ID", cashflowSourceId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[InsertCashFow]", parameters);

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
        internal static ObservableCollection<EntrySearchView.DailyStockValue> GetDailyStockValue(EntrySearchView.DailyStockValue totalStock,string startdate = null, string enddate = null) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = new DataTable();
            if (startdate == null)
                table = dd.ExecuteProc("[HIS_POS_DB].[Index].[GetDailyStockValue]");
            else {
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("STARTDATE", startdate));
                parameters.Add(new SqlParameter("ENDDATE", enddate));
                table = dd.ExecuteProc("[HIS_POS_DB].[Index].[GetDailyStockValue]",parameters);
            }
            ObservableCollection<EntrySearchView.DailyStockValue> collection = new ObservableCollection<EntrySearchView.DailyStockValue>();

            var InitStockValue = table.Rows[0]["DSV_INITIAL_VALUE"].ToString(); 
            var FinalStockValue = table.Rows[table.Rows.Count-1]["DSV_FINAL_VALUE"].ToString();

            var PurchaseValue = 0;
            var ReturnValue = 0;
            var StockCheckValue = 0;
            var MedUseValue = 0;
            var MedIncomeValue = 0;
            var CopayMentValue = 0;
            var PaySelfValue = 0;
            var DepositValue = 0;
            foreach (DataRow row in table.Rows)
            {
                EntrySearchView.DailyStockValue daily = new EntrySearchView.DailyStockValue(row);
                collection.Add(daily);
                PurchaseValue += Convert.ToInt32(daily.PurchaseValue);
                ReturnValue += Convert.ToInt32(daily.ReturnValue);
                StockCheckValue += Convert.ToInt32(daily.StockCheckValue);
                MedUseValue += Convert.ToInt32(daily.MedUseValue);
            } 

                totalStock.Date = "總和";
                totalStock.InitStockValue = InitStockValue;
                totalStock.PurchaseValue = PurchaseValue.ToString();
                totalStock.ReturnValue = ReturnValue.ToString();
                totalStock.StockCheckValue = StockCheckValue.ToString();
                totalStock.MedUseValue = MedUseValue.ToString();
                totalStock.FinalStockValue = FinalStockValue; 
            return collection;
        }
        internal static string GetAddStockValue (string DecmasId,string proId, string AddStockAmount) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", DecmasId));
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("AddStockAmount", AddStockAmount));
            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[GetAddStockValue]", parameters);
            return table.Rows[0][0].ToString();
        }
        internal static void BuckleInventory( string proId, string BuckleAmount,string statusName,string foreign) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PROWAR_ID", '1'));
            parameters.Add(new SqlParameter("BuckleAmount", BuckleAmount));
            parameters.Add(new SqlParameter("STATUS_NAME", statusName));
            parameters.Add(new SqlParameter("PROINVREC_FOREIN", foreign));
            var table = dd.ExecuteProc("[HIS_POS_DB].[dbo].[BuckleInventory]", parameters);
        }
        internal static void RecoveryInventory(string decMasId,string proId, string RecoveryAmount) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", decMasId));
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PROWAR_ID", '1'));
            parameters.Add(new SqlParameter("RecoveryAmount", RecoveryAmount)); 
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[RecoveryInventory]", parameters);
        }
        internal static void InsertMedicine(string proId,string proName) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            parameters.Add(new SqlParameter("PRO_NAME", proName)); 
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertMedicine]", parameters);
        }

        internal static ObservableCollection<CooperativeEntryView.CopaymentEntry> GetCopayMentValue(CooperativeEntryView.CopaymentEntry totalStock, string startdate = null, string enddate = null)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = new DataTable();
            if (startdate == null)
                table = dd.ExecuteProc("[HIS_POS_DB].[CooperativeEntryView].[GetCopayMentValue]");
            else
            {
                
                   var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("STARTDATE", startdate));
                parameters.Add(new SqlParameter("ENDDATE", enddate));
                table = dd.ExecuteProc("[HIS_POS_DB].[CooperativeEntryView].[GetCopayMentValue]", parameters);
            }
            ObservableCollection<CooperativeEntryView.CopaymentEntry> collection = new ObservableCollection<CooperativeEntryView.CopaymentEntry>();
            double CopaymentValue = 0;
            double ClinicCopaymentValue = 0;
            double MedServiceValue = 0;
            double ClinicMedServiceValue = 0;
            double PaySelfValue = 0;
            double ClinicPaySelfValue = 0;
            double DepositValue = 0;
            foreach (DataRow row in table.Rows)
            {
                CooperativeEntryView.CopaymentEntry daily = new CooperativeEntryView.CopaymentEntry(row);
                CopaymentValue += Convert.ToDouble(daily.CopaymentValue);
                ClinicCopaymentValue += Convert.ToDouble(daily.ClinicCopaymentValue);
                MedServiceValue += Convert.ToDouble(daily.MedServiceValue);
                ClinicMedServiceValue += Convert.ToDouble(daily.ClinicMedServiceValue);
                PaySelfValue += Convert.ToDouble(daily.PaySelfValue);
                ClinicPaySelfValue += Convert.ToDouble(daily.ClinicPaySelfValue);
                DepositValue += Convert.ToDouble(daily.DepositValue);
                collection.Add(daily);
            }
            totalStock.CopaymentValue = CopaymentValue.ToString();
            totalStock.ClinicCopaymentValue = ClinicCopaymentValue.ToString();
            totalStock.MedServiceValue = MedServiceValue.ToString();
            totalStock.ClinicMedServiceValue = ClinicMedServiceValue.ToString();
            totalStock.PaySelfValue = PaySelfValue.ToString();
            totalStock.ClinicPaySelfValue = ClinicPaySelfValue.ToString();
            totalStock.DepositValue = DepositValue.ToString();

            return collection;
        }
        internal static ObservableCollection<CooperativeAdjustReportView.CooperativeAdjustMed> GetCooperativeAdjustMed(DateTime sDate, DateTime eDate)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@SDATE", sDate));
            parameters.Add(new SqlParameter("@EDATE", eDate));
            var table = dd.ExecuteProc("[HIS_POS_DB].[CooperativeAdjustReportView].[GetCooperativeAdjustMed]", parameters);
            ObservableCollection<CooperativeAdjustReportView.CooperativeAdjustMed> cooperativeAdjustMeds = new ObservableCollection<CooperativeAdjustReportView.CooperativeAdjustMed>();
            foreach (DataRow row in table.Rows) {
                cooperativeAdjustMeds.Add(new CooperativeAdjustReportView.CooperativeAdjustMed(row)); 
            }
            return cooperativeAdjustMeds;
        } 
    }
} 