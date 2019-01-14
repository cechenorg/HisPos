using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public static class DivisionDb
    {
        public static DataTable GetData()
        { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Division]");
        }
    }
}
