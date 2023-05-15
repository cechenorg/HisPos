using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Pharmacy
{
    public class PharmacyDBService
    {

        public static IEnumerable<PharmacyInfo> GetPharmacyListByGroupServerName()
        {
            IEnumerable<PharmacyInfo> result = null;
             
            if(string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false)
            {
                result = SQLServerConnection.Query<PharmacyInfo>(
                    "[Get].[PharmacyListByGroupServerName]", new { groupServerName = ViewModelMainWindow.CurrentPharmacy.GroupServerName },
                    Properties.Settings.Default.SQL_global, ViewModelMainWindow.CurrentPharmacy.HISPOS_ServerName ); 
            } 
            return result;
        }
        /// <summary>
        /// 設定藥局有效期
        /// </summary>
        /// <returns></returns>
        public static void SetPharmacyValidityPeriod(string period)
        {
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[UpdatePharmacyValidityPeriod]",
                    param: new
                    {
                        period = period
                    }, commandType: CommandType.StoredProcedure);
            });
        }
    }

    public class PharmacyInfo
    {
        public string PHAMAS_ID { get; set; }
        public string PHAMAS_NAME { get; set; }
        public string PHAMAS_MEDICALNUM { get; set; }
        public string PHAMAS_TEL { get; set; }
        public string PHAMAS_ADDR { get; set; }
        public string PHAMAS_VALIDUSEDATE { get; set; }
        public string PHAMAS_VerifyKey { get; set; }
        public string PHAMAS_DbTarget { get; set; }
        public string PHAMAS_GroupServer { get; set; }
        public string PHAMAS_TAXNUM { get; set; }
    }
}
