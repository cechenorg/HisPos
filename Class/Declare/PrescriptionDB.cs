using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        public static ObservableCollection<PrescriptionOverview> GetPrescriptionOverviewBySearchCondition(string sDate, string eDate, string cusName, string id,string docName,string insName)
        {
            ObservableCollection<PrescriptionOverview> prescriptionOverviews = new ObservableCollection<PrescriptionOverview>();

            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SDATE", sDate));
            parameters.Add(new SqlParameter("EDATE", eDate));
            parameters.Add(new SqlParameter("CUSNAME", cusName));
            parameters.Add(new SqlParameter("HISCASCAT_ID", id)); 
            parameters.Add(new SqlParameter("EMP_NAME", docName));
            parameters.Add(new SqlParameter("INS_NAME", insName));
            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[GetDeclareOverView]", parameters);

            foreach (DataRow row in table.Rows)
            {
                prescriptionOverviews.Add(new PrescriptionOverview(row));
            }
            return prescriptionOverviews;
        }

        public static DeclareData GetDeclareDataById(string decMasId)
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", decMasId));
            DeclareData declareData = null;
            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[GetDeclareDataByMasId]", parameters);
            foreach (DataRow row in table.Rows) {
                declareData = new DeclareData(row);
            }
            return declareData;
        }
    }
}
