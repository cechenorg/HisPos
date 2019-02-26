using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class ManufactoryDB
    {
        internal static DataTable GetAllManufactories()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Manufactory]");
        }
    }
}
