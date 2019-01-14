using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public static class PaymentCategoryDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[HISPOS_Develop].[Get].[PaymentCategory]");
        }
    }
}
