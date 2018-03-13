using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Declare
{
    public static class PrescriptionDB
    {
        public static ObservableCollection<PrescriptionOverview> GetPrescriptionOverviewBySearchCondition(DateTime sDate, DateTime eDate, string cusName, string id)
        {
            ObservableCollection<PrescriptionOverview> prescriptionOverviews = new ObservableCollection<PrescriptionOverview>();

            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SDATE", eDate));
            parameters.Add(new SqlParameter("EDATE", sDate));
            parameters.Add(new SqlParameter("CUSNAME", cusName));
            parameters.Add(new SqlParameter("HISCASCAT_ID", id));

            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREOVERVIEW]", parameters);

            foreach (var row in table.Rows)
            {
                
            }

            return prescriptionOverviews;
        }

        public static DeclareData GetDeclareDataById(string decMasId)
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", decMasId));

            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[GET].[]", parameters);
            return new DeclareData();
        }
    }
}
