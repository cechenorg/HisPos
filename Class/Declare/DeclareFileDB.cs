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
    public class DeclareFileDb
    {
        public static ObservableCollection<DeclareFile> GetDeclareFilesData()
        {
            ObservableCollection<DeclareFile> declareFiles = new ObservableCollection<DeclareFile>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[DeclareFileExportView].[GetDeclareFilesData]", dbConnection);
            foreach (DataRow declareFile in divisionTable.Rows)
            {
                declareFiles.Add(new DeclareFile(declareFile));
            }
            return declareFiles;
        }

        public static void SetDeclareFileByPharmacyId()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[UpdateDeclareFile]", dbConnection);
        }
    }
}
