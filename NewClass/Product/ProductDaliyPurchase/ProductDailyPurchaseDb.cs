using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductDaliyPurchase {
    public static class ProductDailyPurchaseDb {
        public static DataTable GetDailyPurchaseData() {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
           return MainWindow.ServerConnection.ExecuteProc("[Get].[DailyProductPurchase]");
        }
    }
}
