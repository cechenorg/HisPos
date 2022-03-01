using His_Pos.Database;
using His_Pos.SYSTEM_TAB.INDEX;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
    public static class IndexReserveDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[IndexReserveByDate]", parameterList);
        }

        public static DataTable Save(int Id, string PhoneCallStatus, IndexPrepareMedType prepareMedStatus, string stoOrdID)
        {
            string PrepareStatus = "";
            switch (prepareMedStatus)
            {
                case IndexPrepareMedType.Prepare:
                    PrepareStatus = "D";
                    break;

                case IndexPrepareMedType.UnPrepare:
                    PrepareStatus = "F";
                    break;

                case IndexPrepareMedType.Unprocess:
                    PrepareStatus = "N";
                    break;
            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", Id);
            DataBaseFunction.AddSqlParameter(parameterList, "PhoneCallStatus", PhoneCallStatus);
            DataBaseFunction.AddSqlParameter(parameterList, "PrepareStatus", PrepareStatus);
            DataBaseFunction.AddSqlParameter(parameterList, "userID", ChromeTabViewModel.ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "stoOrdID", stoOrdID);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateIndexReserveStatus]", parameterList);
        }

        public static DataTable GetOrderIDByResMasID(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderIDByReserveID]", parameterList);
        }
    }
}