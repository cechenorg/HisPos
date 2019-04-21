using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.MedicineSet
{
    public static class MedicineSetDb
    {
        public static DataTable GetData()
        { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineSet]");
        }
    }
}
