using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
   public class ManufactoryDb
    {

        public static DataTable GetManufactoryData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[MANUFACTORY]");
        }
        public static DataTable GetProManData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[PROMAN]");
        }
    }
}
