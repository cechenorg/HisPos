using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Customer.CustomerPrescriptionChanged
{
    public static class CustomerPrescriptionChangedDb
    {
        public static DataTable GetDataByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "cusID", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerPrescriptionChangedByCusID]", parameterList);
        } 
    }
}
