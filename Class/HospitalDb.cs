using System.Collections.ObjectModel;
using System.Data;
using His_Pos.AbstractClass;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public static class HospitalDb
    {
        internal static ObservableCollection<Hospital> GetData()
        {
            ObservableCollection<Hospital> HospitalsCollection = new ObservableCollection<Hospital>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var institutionTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetHospitalsData]");
            foreach (DataRow row in institutionTable.Rows)
            {
                HospitalsCollection.Add(new Hospital(row,DataSource.InitHospitalData));
            }
            return HospitalsCollection;
        }
    }
}
