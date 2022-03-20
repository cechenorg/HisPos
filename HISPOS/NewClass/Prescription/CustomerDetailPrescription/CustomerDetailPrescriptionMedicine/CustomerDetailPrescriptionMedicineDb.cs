using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine
{
    public static class CustomerDetailPrescriptionMedicineDb
    {
        public static DataTable GetPrescriptionDataByCusID(int preID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "preID", preID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerPrescriptionDetailMedicinesByPreID]", parameterList);
        }

        public static DataTable GetReserveDataByCusID(int resID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ResID", resID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerReserveDetailMedicinesByResID]", parameterList);
        }
    }
}