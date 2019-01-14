using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public static class CopaymentDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[HISPOS_Develop].[Get].[Copayment]");
        }
    }
}
