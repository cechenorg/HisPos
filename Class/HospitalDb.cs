using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Class.Person;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public static class HospitalDb
    {
        internal static ObservableCollection<Hospital> GetData()
        {
            ObservableCollection<Hospital> HospitalsCollection = new ObservableCollection<Hospital>();
            var dbConnection = new DatabaseConnection(Settings.Default.SQL_local);
            var institutionTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetHospitalsData]");
            foreach (DataRow row in institutionTable.Rows)
            {
                HospitalsCollection.Add(new Hospital(row,DataSource.InitHospitalData));
            }
            return HospitalsCollection;
        }

        internal static void UpdateCommonHospitalById(Hospital hospital)
        {
            var dd = new DatabaseConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("Id",hospital.Id),
                new SqlParameter("Common", hospital.Common)
            };
            dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[UpdateCommonHospital]", parameters);
        }
    }
}
