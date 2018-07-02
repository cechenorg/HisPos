using System.Collections.ObjectModel;
using System.Data;
using His_Pos.AbstractClass;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class Hospitals:ISelection
    {
        public readonly ObservableCollection<Hospital> HospitalsCollection = new ObservableCollection<Hospital>();
        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var institutionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetInstitution]", dbConnection);
            foreach (DataRow row in institutionTable.Rows)
            {
                HospitalsCollection.Add(new Hospital(row));
            }

            HospitalsCollection.Add(new Hospital("N","藥事居家照護/協助辦理門診戒菸計畫且直接交付指示用藥/提供「戒菸個案追蹤」/「戒菸衛教暨個案管理」"));
        }
    }
}
