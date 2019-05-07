using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine
{
   public static class CustomerDetailPrescriptionMedicineDb
    {
        public static DataTable GetDataByCusID(int preID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "preID", preID);    
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerPrescriptionDetailMedicinesByPreID]", parameterList);
        }
    }
}
