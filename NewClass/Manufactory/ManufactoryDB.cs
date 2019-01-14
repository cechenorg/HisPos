using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
