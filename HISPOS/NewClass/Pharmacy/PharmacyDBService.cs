using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
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
                result = SQLServerConnection.Query<PharmacyInfo>(Properties.Settings.Default.SQL_global, ViewModelMainWindow.CurrentPharmacy.HISPOS_ServerName,
                    "[Get].[PharmacyListByGroupServerName]", new { groupServerName = ViewModelMainWindow.CurrentPharmacy.GroupServerName }); 
            } 
            return result;
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
