using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public static class SpecialTreatDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[HISPOS_Develop].[Get].[SpecialTreatment]");
        }
    }
}
