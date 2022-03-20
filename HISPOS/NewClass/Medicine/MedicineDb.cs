using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Medicine
{
    public static class MedicineDb
    {
        public static DataTable GetQRcodeMedicine(List<string> MedicineIds, string wareHouseID, DateTime? adjustDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetQRcodePrescriptionDetail(MedicineIds));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", wareHouseID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustDate", adjustDate ?? DateTime.Today);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[QRcodeMedicine]", parameterList);
        }

        public static DataTable GetMedicinesBySearchIds(List<string> MedicineIds, string wareHouseID, DateTime? adjustDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetPrescriptionDetail(MedicineIds));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", wareHouseID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustDate", adjustDate ?? DateTime.Today);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineBySearchIDs]", parameterList);
        }

        public static void InsertCooperativeMedicineOTC(string medicineID, string medicineName)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "MedId", medicineID);
            DataBaseFunction.AddSqlParameter(parameterList, "Name", medicineName);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCooperativeMedicineOTC]", parameterList);
        }

        public static DataTable GetDataByPrescriptionId(int preId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMasId", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionDetailByPreMasId]", parameterList);
        }

        public static DataTable GetDataByReserveId(int resId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ResMasId", resId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReserveDetailByResMasId]", parameterList);
        }

        public static DataTable MedicineListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("MedicineID", typeof(string));
            return masterTable;
        }

        public static DataTable SetPrescriptionDetail(List<string> MedicineIds)
        { //一般藥費
            DataTable medicineListTable = MedicineListTable();
            foreach (string m in MedicineIds)
            {
                DataRow newRow = medicineListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m);
                medicineListTable.Rows.Add(newRow);
            }
            return medicineListTable;
        }

        public static DataTable SetQRcodePrescriptionDetail(List<string> MedicineIds)
        { //一般藥費
            DataTable medicineListTable = MedicineListTable();
            medicineListTable.Columns.Add("OrderID", typeof(int));
            int order = 0;
            foreach (string m in MedicineIds)
            {
                DataRow newRow = medicineListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m);
                DataBaseFunction.AddColumnValue(newRow, "OrderID", order);
                medicineListTable.Rows.Add(newRow);
                order++;
            }
            return medicineListTable;
        }

        internal static DataTable GetTagDataByID(string productID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PRO_ID", productID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineTagDataByID]", parameterList);
        }

        public static DataTable GetPrescriptionMedicineSumById(List<int> idList, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetIDTable(idList));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionMedicineSumById]", parameterList);
        }

        private static DataTable IDTable()
        {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("ID", typeof(int));
            return idTable;
        }

        public static DataTable SetIDTable(List<int> IDList)
        {
            DataTable table = IDTable();
            foreach (int id in IDList)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", id);
                table.Rows.Add(newRow);
            }
            return table;
        }

        public static DataTable GetInventoryByInvIDs(List<int> editInvIDList)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InvIDs", SetIDTable(editInvIDList));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineInventoryByInvIDs]", parameterList);
        }

        public static DataTable GetUsableAmountByPrescriptionID(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMas_ID", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CanUseAmountByPreMas_ID]", parameterList);
        }

        public static DataTable GetUsableAmountByReserveID(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ResMas_ID", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CanUseAmountByResMas_ID]", parameterList);
        }
    }
}