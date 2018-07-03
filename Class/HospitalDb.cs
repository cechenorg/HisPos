using System.Collections.ObjectModel;
using System.Data;
using His_Pos.AbstractClass;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class HospitalDb:ISelection
    {
        public readonly ObservableCollection<Hospital> HospitalsCollection = new ObservableCollection<Hospital>();
        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var institutionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetHospitalsData]", dbConnection);
            foreach (DataRow row in institutionTable.Rows)
            {
                HospitalsCollection.Add(new Hospital(row));
            }
        }
    }
}
