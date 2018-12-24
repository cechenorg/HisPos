using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.Struct.Manufactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace His_Pos.Class.Manufactory
{
   public class ManufactoryDb
    {
        public static DataTable GetProductByManId(string id) {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", id));
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[MANBYID]", parameters); 
    }
        public static ObservableCollection<Manufactory> GetManufactoryData()
        {
            ObservableCollection<Manufactory> collection = new ObservableCollection<Manufactory>();

            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetManufactory]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new Manufactory(row));
            }

            return collection;
        }
        internal static void UpdateProductManufactory(string productId, ManufactoryChanged manufactoryChanged)
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productId));
            parameters.Add(new SqlParameter("MAN_ID", manufactoryChanged.ManufactoryId));
            parameters.Add(new SqlParameter("ORDER_ID", manufactoryChanged.changedOrderId));
            parameters.Add(new SqlParameter("PROCESSTYPE", manufactoryChanged.ProcessType.ToString()));
            dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProMan]", parameters);
        }

        internal static ObservableCollection<ManageManufactory> GetManageManufactory()
        {
            ObservableCollection<ManageManufactory> collection = new ObservableCollection<ManageManufactory>();

            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[GetManageManufactory]");

            foreach (DataRow row in table.Rows)
            {
                string parent = row["MAN_PARENT"].ToString();

                if (parent.Equals(String.Empty) )
                {
                    collection.Add(new ManageManufactory(row));
                }
                else
                {
                    ManageManufactory man = collection.Where(m => m.Id.Equals(parent)).ToList()[0];

                    man.ManufactoryPrincipals.Add(new ManufactoryPrincipal(row));
                }
            }

            return collection;
        }

        internal static ObservableCollection<ManufactoryPayOverview> GetManufactoryPayOverview(string manId)
        {
            ObservableCollection<ManufactoryPayOverview> collection = new ObservableCollection<ManufactoryPayOverview>();

            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[GetManufactoryPayOverview]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new ManufactoryPayOverview(row));
            }

            return collection;
        }

        internal static Collection<PurchasePrincipal> GetPrincipal(string manId)
        {
            Collection<PurchasePrincipal> collection = new Collection<PurchasePrincipal>();

            collection.Add(new PurchasePrincipal("無"));

            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetPrincipal]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new PurchasePrincipal(row));
            }

            collection.Add(new PurchasePrincipal("新增負責人"));

            return collection;
        }

        internal static ObservableCollection<ProductDetailManufactory> GetManufactoryCollection(string proId)
        {
            ObservableCollection<ProductDetailManufactory> manufactories = new ObservableCollection<ProductDetailManufactory>();

            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProMan]", parameters);
            
            foreach (DataRow m in table.Rows)
            {
                manufactories.Add(new ProductDetailManufactory(m, DataSource.PROMAN));
            }

            return manufactories;
        }

        internal static void AddNewOrderBasicSafe(StoreOrderProductType type, WareHouse wareHouse, Manufactory manufactory)
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", (type == StoreOrderProductType.BASIC) ? "BASIC" : "SAFE"));
            parameters.Add(new SqlParameter("ORDEMP_ID", MainWindow.CurrentUser.Id));
            parameters.Add(new SqlParameter("WARE_ID", wareHouse.Id));

            if (manufactory is null)
                parameters.Add(new SqlParameter("MAN_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("MAN_ID", manufactory.Id));

            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[AddNewOrderBySafeOrBasic]", parameters);
        }

        internal static ManageManufactory AddNewManageManufactory()
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[AddNewManageManufactory]");

            return new ManageManufactory(table.Rows[0]);
        }

        internal static void DeleteManageManufactory(string manId)
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manId));

            dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[DeleteManageManufactory]", parameters);
        }

        internal static void UpdateManageManufactory(ManageManufactory manufactory)
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", manufactory.Id));
            parameters.Add(new SqlParameter("NAME", manufactory.Name));
            parameters.Add(new SqlParameter("NICKNAME", manufactory.NickName));
            parameters.Add(new SqlParameter("ADDR", manufactory.Address));
            parameters.Add(new SqlParameter("TEL", manufactory.Telphone));
            parameters.Add(new SqlParameter("FAX", manufactory.Fax));
            parameters.Add(new SqlParameter("EIN", manufactory.UniformNumber));
            parameters.Add(new SqlParameter("EMAIL", manufactory.Email));
            parameters.Add(new SqlParameter("CONTROL", manufactory.ControlMedicineID));
            parameters.Add(new SqlParameter("MEDICAL", manufactory.MedicalID));
            parameters.Add(new SqlParameter("NOTE", manufactory.Note));
            parameters.Add(new SqlParameter("PARENT", DBNull.Value));
            parameters.Add(new SqlParameter("RESDEP", DBNull.Value));
            parameters.Add(new SqlParameter("LINE", DBNull.Value));
            parameters.Add(new SqlParameter("PAYCONDITION", DBNull.Value));
            parameters.Add(new SqlParameter("PAYTYPE", DBNull.Value));
            parameters.Add(new SqlParameter("ISENABLE", 1));
            parameters.Add(new SqlParameter("ISMAN", 1));

            dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[UpdateManageManufactory]", parameters);

            foreach (var principal in manufactory.ManufactoryPrincipals)
            {
                parameters.Clear();
                if( principal.Id.Equals("") )
                    parameters.Add(new SqlParameter("ID", DBNull.Value));
                else
                    parameters.Add(new SqlParameter("ID", principal.Id));
                parameters.Add(new SqlParameter("NAME", principal.Name));
                parameters.Add(new SqlParameter("NICKNAME", principal.NickName));
                parameters.Add(new SqlParameter("ADDR", DBNull.Value));
                parameters.Add(new SqlParameter("TEL", principal.Telephone));
                parameters.Add(new SqlParameter("FAX", principal.Fax));
                parameters.Add(new SqlParameter("EIN", DBNull.Value));
                parameters.Add(new SqlParameter("EMAIL", principal.Email));
                parameters.Add(new SqlParameter("CONTROL", DBNull.Value));
                parameters.Add(new SqlParameter("MEDICAL", DBNull.Value));
                parameters.Add(new SqlParameter("NOTE", DBNull.Value));
                parameters.Add(new SqlParameter("PARENT", manufactory.Id));
                parameters.Add(new SqlParameter("RESDEP", principal.ResponsibleDepartment));
                parameters.Add(new SqlParameter("LINE", principal.Line));
                parameters.Add(new SqlParameter("PAYCONDITION", principal.PayCondition));
                parameters.Add(new SqlParameter("PAYTYPE", principal.PayType));
                parameters.Add(new SqlParameter("ISENABLE", principal.IsEnable));
                parameters.Add(new SqlParameter("ISMAN", 1));

                dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[UpdateManageManufactory]", parameters);
            }
        }
    }
}
