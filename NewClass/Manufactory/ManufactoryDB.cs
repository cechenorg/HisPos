using System;
using System.Data;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;

namespace His_Pos.NewClass.Manufactory
{
    public class ManufactoryDB
    {
        internal static DataTable GetAllManufactories()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Manufactory]");
        }

        internal static DataTable DeleteManufactory(string iD)
        {
            throw new NotImplementedException();
        }

        internal static DataTable UpdateManufactoryDetail(ManufactoryManageDetail manufactoryManageDetail)
        {
            throw new NotImplementedException();
        }

        internal static DataTable GetManufactoryPrincipals(string manufactoryID)
        {
            throw new NotImplementedException();
        }
    }
}
