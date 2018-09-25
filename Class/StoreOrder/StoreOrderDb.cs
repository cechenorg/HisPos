using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;
using His_Pos.Class.Declare;
using His_Pos.Class.Product;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.Struct.StoreOrder;

namespace His_Pos.Class.StoreOrder
{
    public static class StoreOrderDb
    {
        public static ObservableCollection<StoreOrder> GetStoreOrderOverview(OrderType type)
        {
            ObservableCollection<StoreOrder> StoreOrderOverviewCollection = new ObservableCollection<StoreOrder>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", type.ToString()));
            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrder]", parameters);
            
            foreach (DataRow row in table.Rows)
            {
                StoreOrderOverviewCollection.Add(new StoreOrder(row));
            }

            return StoreOrderOverviewCollection;
        }
        internal static void DeleteOrder(string Id)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", Id));
            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[DeleteOrder]", parameters);
        }
        internal static void PurchaseAndReturn(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var product in storeOrder.Products) {
                parameters.Add(new SqlParameter("TYPE", storeOrder.Category.CategoryName.Substring(0, 1)));
                parameters.Add(new SqlParameter("PRO_ID",product.Id));
                //parameters.Add(new SqlParameter("PRO_INVENT", product.Amount));
                dd.ExecuteProc("[HIS_POS_DB].[SET].[PURCHASEANDRETURN]",parameters);
                parameters.Clear();
            }
        }

        internal static Collection<StoreOrderOverview> GetStoreOrderOverview()
        {
            Collection<StoreOrderOverview> collection = new Collection<StoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderOverview]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new StoreOrderOverview(row));
            }

            return collection;
        }

        internal static void SaveOrderDetail(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
            parameters.Add(new SqlParameter("ORD_EMP", storeOrder.OrdEmp));
            string type = string.Empty;
            switch (storeOrder.Type) {
                case OrderType.UNPROCESSING:
                    type = "P";
                    break;
                case OrderType.PROCESSING:
                    type = "G";
                    break;
                case OrderType.DONE:
                    type = "D";
                    break;
                case OrderType.WAITING:
                    type = "W";
                    break;
            }
            parameters.Add(new SqlParameter("STOORD_FLAG", type));

            if (String.IsNullOrEmpty(storeOrder.Category.CategoryName))
                parameters.Add(new SqlParameter("STOORD_TYPE",DBNull.Value));
            else
                parameters.Add(new SqlParameter("STOORD_TYPE", storeOrder.Category.CategoryName.Substring(0, 1)));

            if (String.IsNullOrEmpty(storeOrder.Manufactory.Id))
                parameters.Add(new SqlParameter("MAN_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("MAN_ID", storeOrder.Manufactory.Id));
            if (storeOrder.RecEmp == "")
                parameters.Add(new SqlParameter("REC_EMP",DBNull.Value ));
            else
                parameters.Add(new SqlParameter("REC_EMP", storeOrder.RecEmp));

            parameters.Add(new SqlParameter("PRINCIPAL_ID", storeOrder.Principal.Id));

            if (storeOrder.Warehouse is null)
                parameters.Add(new SqlParameter("WAREHOUSE_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("WAREHOUSE_ID", storeOrder.Warehouse.Id));
            
            parameters.Add(new SqlParameter("NOTE", storeOrder.Note));

            DataTable details = new DataTable();
            details.Columns.Add("PRO_ID", typeof(string));
            details.Columns.Add("ORDERQTY", typeof(int));
            details.Columns.Add("QTY", typeof(int));
            details.Columns.Add("PRICE", typeof(string));
            details.Columns.Add("DESCRIPTION", typeof(string));
            details.Columns.Add("VALIDDATE", typeof(string));
            details.Columns.Add("BATCHNUMBER", typeof(string));
            details.Columns.Add("FREEQTY", typeof(int));
            details.Columns.Add("INVOICE", typeof(string));
            details.Columns.Add("TOTAL", typeof(string));
            DateTime datetimevalue;
            foreach (var product in storeOrder.Products)
            {
                var newRow = details.NewRow();

                newRow["PRO_ID"] = product.Id;
                newRow["ORDERQTY"] = ((IProductPurchase)product).OrderAmount;
                newRow["QTY"] = ((ITrade)product).Amount;
                newRow["PRICE"] = ((ITrade) product).Price.ToString();
                newRow["DESCRIPTION"] = ((IProductPurchase)product).Note;
                newRow["VALIDDATE"] = ( DateTime.TryParse(((IProductPurchase)product).ValidDate, out datetimevalue) ) ? ((IProductPurchase)product).ValidDate:string.Empty ;
                newRow["BATCHNUMBER"] = ((IProductPurchase) product).BatchNumber;
                newRow["FREEQTY"] = ((IProductPurchase)product).FreeAmount;
                newRow["INVOICE"] = ((IProductPurchase)product).Invoice;
                newRow["TOTAL"] = ((ITrade)product).TotalPrice.ToString();

                details.Rows.Add(newRow);
            }

            parameters.Add(new SqlParameter("DETAILS", details));

            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[SaveStoreOrder]", parameters);
        }

        internal static string SaveOrderDeclareData(string declareId, ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData> declareMedicines)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DEC_ID", declareId));
            parameters.Add(new SqlParameter("ORD_EMP_ID", MainWindow.CurrentUser.Id));
            parameters.Add(new SqlParameter("WAREHOUSE_ID", '1'));

            DataTable details = new DataTable();
            details.Columns.Add("PRO_ID", typeof(string));
            details.Columns.Add("ORDERQTY", typeof(int));
            details.Columns.Add("QTY", typeof(int));
            details.Columns.Add("PRICE", typeof(string));
            details.Columns.Add("DESCRIPTION", typeof(string));
            details.Columns.Add("VALIDDATE", typeof(string));
            details.Columns.Add("BATCHNUMBER", typeof(string));
            details.Columns.Add("FREEQTY", typeof(int));
            details.Columns.Add("INVOICE", typeof(string));
            details.Columns.Add("TOTAL", typeof(string));

            foreach (var product in declareMedicines)
            {
                var newRow = details.NewRow();

                newRow["PRO_ID"] = product.MedId;
                newRow["ORDERQTY"] = Int32.Parse(product.SendAmount);
                newRow["QTY"] = 0;
                newRow["PRICE"] = "0";
                newRow["DESCRIPTION"] = "";
                newRow["VALIDDATE"] = "";
                newRow["BATCHNUMBER"] = "";
                newRow["FREEQTY"] = 0;
                newRow["INVOICE"] = "";
                newRow["TOTAL"] = "0";

                details.Rows.Add(newRow);
            }

            parameters.Add(new SqlParameter("DETAILS", details));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[AddDeclareOrder]", parameters);

            return table.Rows[0]["ID"].ToString();
        }

        

        internal static Collection<ReturnControl.BatchNumOverview> GetBatchNumOverview(string proId, string wareId)
        {
            Collection<ReturnControl.BatchNumOverview> collection = new BindingList<ReturnControl.BatchNumOverview>();

            return collection;
        }
        
        internal static Collection<ProductPurchaseView.SindeOrderDetail> GetOrderDetailFromSinde(string orderId)
        {
            ObservableCollection<ProductPurchaseView.SindeOrderDetail> collection = new ObservableCollection<ProductPurchaseView.SindeOrderDetail>();

            var dd = new DbConnection("Database=rx_center;Server=59.124.201.229;Port=3311;User Id=SD;Password=1234;SslMode=none", SqlConnectionType.NySql);

            DataTable dataTable = dd.MySqlQueryBySqlString($"call GetOrderDetail('{orderId.Substring(2, 10)}')");
            
            foreach (DataRow row in dataTable.Rows)
            {
                collection.Add(new ProductPurchaseView.SindeOrderDetail(row));
            }
            return collection;
        }

        internal static string GetNewOrderId(string OrdEmpId, string wareId, string manId, string orderType)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDEMP_ID", OrdEmpId));
            parameters.Add(new SqlParameter("WARE_ID", wareId));
            parameters.Add(new SqlParameter("MAN_ID", manId));
            parameters.Add(new SqlParameter("ORDERTYPE", orderType));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[AddNewStoreOrder]", parameters);

            return table.Rows[0]["STOORD_ID"].ToString();
        }

        internal static ObservableCollection<AbstractClass.Product> GetOrderReturnDetailById(string ordId)
        {
            ObservableCollection<AbstractClass.Product> StoreOrderCollection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", ordId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderReturnDetail]", parameters);
            
            foreach (DataRow row in table.Rows)
            {
                switch (row["PRO_TYPE"].ToString())
                {
                    case "M":
                        StoreOrderCollection.Add(new ProductReturnMedicine(row));
                        break;
                    case "O":
                        StoreOrderCollection.Add(new ProductReturnOTC(row));
                        break;
                }
            }
            return StoreOrderCollection;
        }

        public static ObservableCollection<AbstractClass.Product> GetOrderPurchaseDetailById(string ordId)
        {
            ObservableCollection<AbstractClass.Product> StoreOrderCollection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", ordId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderDetail]", parameters);

            string lastProductId = "";

            foreach (DataRow row in table.Rows)
            {
                AbstractClass.Product product = null;
                string currentProductId = row["PRO_ID"].ToString();

                switch (row["PRO_TYPE"].ToString())
                {
                    case "M":
                        product = new ProductPurchaseMedicine(row, DataSource.GetStoreOrderDetail);
                        break;
                    case "O":
                        product = new ProductPurchaseOtc(row, DataSource.GetStoreOrderDetail);
                        break;
                }

                if (lastProductId == currentProductId)
                    ((IProductPurchase)product).IsFirstBatch = false;

                lastProductId = currentProductId;
                StoreOrderCollection.Add(product);
            }
            return StoreOrderCollection;
        }

        internal static void SendOrderToSinde(StoreOrder storeOrderData)
        {
            string orderMedicines = "";

            foreach (var product in storeOrderData.Products)
            {
                orderMedicines += product.Id.PadRight(12, ' ');
                orderMedicines += ((IProductPurchase) product).OrderAmount.ToString().PadLeft(10, ' ');
                orderMedicines += ((IProductPurchase) product).Note;
                orderMedicines += "\r\n";
            }
            
            var dd = new DbConnection("Database=rx_center;Server=59.124.201.229;Port=3311;User Id=SD;Password=1234;SslMode=none", SqlConnectionType.NySql);
            
            dd.MySqlNonQueryBySqlString($"call InsertNewOrder('{storeOrderData.Id.Substring(2, 10)}', '{storeOrderData.Note}', '{orderMedicines}')");

        }

        internal static OrderType GetOrderStatusFromSinde(string orderId)
        {
            var dd = new DbConnection("Database=rx_center;Server=59.124.201.229;Port=3311;User Id=SD;Password=1234;SslMode=none", SqlConnectionType.NySql);

            DataTable dataTable = dd.MySqlQueryBySqlString($"call GetOrderStatus('{orderId.Substring(2, 10)}')");

            switch (dataTable.Rows[0]["FLAG"].ToString())
            {
                case "0":
                    return OrderType.WAITING;
                case "1":
                    return OrderType.PROCESSING;
                case "2":
                    return OrderType.SCRAP;
                default:
                    return OrderType.ERROR;
            }
        }
        internal static void SendDeclareOrderToSingde(string CurrentDecMasId, string storId, DeclareData declareData, DeclareTrade declareTrade, ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData> PrescriptionSendData) {
            string Rx_id = MainWindow.CurrentPharmacy.Id; //藥局機構代號 傳輸主KEY
            string Rx_order = declareData.Prescription.Treatment.AdjustDateStr.Replace("/",""); // 調劑日期(7)病歷號(9)
            string Pt_name = declareData.Prescription.Customer.Name; // 藥袋名稱(病患姓名)
            string Upload_data = DateTime.Now.ToString(" yyyy - MM - dd hh:mm:ss "); //更新時間( 2014 - 01 - 24 21:13:03 )
            string Upload_status = string.Empty; //	列印判斷
            string Prt_date = string.Empty; //列印日期
            string Inv_flag = "0"; //轉單處理確認0未處理 1已處理 2不處理
            string Batch_sht = storId; //出貨單號
            string Inv_chk = "0"; //  庫存確認 是1 否0
            string Inv_msg = ""; //庫存確認

            string empty = string.Empty;
            StringBuilder Dtl_data = new StringBuilder(); //  備註text  處方資訊

            //第一行
            Dtl_data.Append(CurrentDecMasId.PadRight(8,' ')); //藥局病例號
            Dtl_data.Append(declareData.Prescription.Customer.Name.PadRight(20,' ')); //病患姓名
            Dtl_data.Append(declareData.Prescription.Customer.IcCard.IcNumber.PadRight(10, ' ')); //身分證字號
            Dtl_data.Append(declareData.Prescription.Customer.Birthday.Replace("/","").PadRight(7, ' ')); //出生年月日
            string gender = declareData.Prescription.Customer.IcCard.IcNumber.Substring(1, 1) == "1" ? "1" : "2";
            Dtl_data.Append(gender.PadRight(1, ' ')); //性別判斷 1男 2女
            Dtl_data.Append(declareData.Prescription.Customer.ContactInfo.Tel.PadRight(20, ' ')); //電話
            Dtl_data.AppendLine();
            //第二行   
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.Hospital.Id.PadRight(10, ' ')); //院所代號
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.IcNumber.PadRight(10, ' ')); //診治醫師代號 (同院所代號)
            Dtl_data.Append(empty.PadRight(20, ' ')); //空
            Dtl_data.Append(MainWindow.CurrentUser.Id.PadRight(10, ' ')); //藥師代號
            Dtl_data.Append(MainWindow.CurrentUser.Name.PadRight(20, ' ')); //藥師姓名
            Dtl_data.AppendLine();
            //第三行
            Dtl_data.Append(declareData.Prescription.Treatment.TreatDateStr.Replace("/","").PadRight(7, ' ')); //處方日(就診日期)
            Dtl_data.Append(declareData.Prescription.Treatment.AdjustDateStr.Replace("/", "").PadRight(7, ' ')); //調劑日期
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id.PadRight(2, ' ')); //案件
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id.PadRight(2, ' ')); //科別
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.MainDiseaseCode.Id.PadRight(10, ' ')); //主診斷
            Dtl_data.Append(declareData.Prescription.Treatment.MedicalInfo.SecondDiseaseCode.Id.PadRight(40, ' ')); //次診斷
            Dtl_data.Append(declareData.Prescription.Customer.IcCard.MedicalNumber.PadRight(4, ' ')); //卡序 (0001、欠卡、自費)
            Dtl_data.Append("2".PadRight(1, ' ')); //1一般箋 2慢箋
            Dtl_data.Append(declareData.Prescription.ChronicTotal.PadRight(1, ' ')); //可調劑次數
            Dtl_data.Append(declareData.Prescription.ChronicSequence.PadRight(1, ' ')); //本次調劑次數
            Dtl_data.Append(declareTrade.PaySelf.PadRight(8, ' ')); //自費金額

            double medCost = 0;
            foreach (DeclareMedicine declareMedicine in declareData.Prescription.Medicines) {
                medCost += declareMedicine.TotalPrice;
            } 
            Dtl_data.Append(medCost.ToString().PadRight(8, ' ')); //藥品費

            string medicalPay = "0";
            if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) <= 13)
                medicalPay = "48";
            if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) >= 14 && Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) < 28)
                medicalPay = "59";
            if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) >= 28)
                medicalPay = "69";

            Dtl_data.Append(medicalPay.PadRight(4, ' ')); //藥事費
            Dtl_data.Append(declareTrade.CopayMent.PadRight(4, ' ')); //部分負擔
            Dtl_data.AppendLine();
            //第四行
            foreach (DeclareMedicine declareMedicine in declareData.Prescription.Medicines)
            {
                Dtl_data.Append(declareMedicine.Id.PadRight(13, ' ')); //健保碼
                Dtl_data.Append(declareMedicine.MedicalCategory.Dosage.PadRight(8, ' ')); //每次使用數量
                Dtl_data.Append(declareMedicine.Usage.Name.PadRight(9, ' ')); //使用頻率
                Dtl_data.Append(declareMedicine.Days.PadRight(10, ' ')); //使用天數
                Dtl_data.Append(declareMedicine.Amount.ToString().PadRight(8, ' ')); //使用總量
                Dtl_data.Append(declareMedicine.Position.PadRight(6, ' ')); //途徑 (詳見:途徑欄位說明)
                Dtl_data.Append(declareMedicine.PaySelf == true && declareMedicine.TotalPrice > 0 ? "Y" : "N".PadRight(1, ' ')); //自費判斷 Y自費收費 N自費不收費
                Dtl_data.Append(empty.PadRight(1, ' ')); //管藥判斷庫存是否充足 Y是 N 否
                string amount = string.Empty;
                foreach (ChronicSendToServerWindow.PrescriptionSendData row in PrescriptionSendData)
                { 
                    if (row.MedId == declareMedicine.Id) {
                        amount = row.SendAmount;
                        break;
                    }
                }
                Dtl_data.Append(amount.PadRight(8, ' ')); //訂購量
                Dtl_data.AppendLine();
            }
            
            var dd = new DbConnection("Database=rx_center;Server=59.124.201.229;Port=3311;User Id=SD;Password=1234;SslMode=none", SqlConnectionType.NySql);

            dd.MySqlNonQueryBySqlString($"call AddDeclareOrderToPreDrug('{Rx_id}', '{storId}', '{declareData.Prescription.Customer.Name}','{Dtl_data}')");
        }


        internal static OrderType GetDeclareOrderStatusFromSinde(string orderId)
        {
            var dd = new DbConnection("Database=rx_center;Server=59.124.201.229;Port=3311;User Id=SD;Password=1234;SslMode=none", SqlConnectionType.NySql);

            DataTable dataTable = dd.MySqlQueryBySqlString($"call GetDeclareOrderStatus('{orderId}')");

            switch (dataTable.Rows[0]["FLAG"].ToString())
            {
                case "0":
                    return OrderType.WAITING;
                case "1":
                    return OrderType.PROCESSING;
                default:
                    return OrderType.ERROR;
            }
        }
    }
}
