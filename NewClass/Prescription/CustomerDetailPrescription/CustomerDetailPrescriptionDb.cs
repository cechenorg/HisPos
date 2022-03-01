using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription
{
    public static class CustomerDetailPrescriptionDb
    {
        public static DataTable GetDataByCusID(int cusID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusID", cusID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerPrescriptionDetailByCusID]", parameterList);
        }
    }
}