using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.DiseaseCode
{
    class DiseaseCodeDb
    {
        public static ObservableCollection<DiseaseCode9To10> GetDiseaseCodeById(string id)
        {
            var diseaseCollection = new ObservableCollection<DiseaseCode9To10>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter> {new SqlParameter("ID", id)};

            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetDiseaseCode]", parameters);
            foreach (DataRow row in table.Rows)
            {
                diseaseCollection.Add(new DiseaseCode9To10(row));
            }
            return diseaseCollection;
        }
    }
}
