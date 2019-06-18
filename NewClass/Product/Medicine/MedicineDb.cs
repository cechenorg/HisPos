using System;
using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product.Medicine
{
    public static class MedicineDb
    {
        public static DataTable GetMedicinesBySearchId(string medicineID, string wareHouseID, DateTime? adjustDate)
        {
            if (string.IsNullOrEmpty(medicineID)) return new DataTable();
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            DataBaseFunction.AddSqlParameter(parameterList, "Pro_Id", medicineID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", wareHouseID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustDate", adjustDate ?? DateTime.Today);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineBySearchId]", parameterList);     
        }
        public static DataTable GetMedicinesBySearchIds(List<string> MedicineIds,string wareHouseID,DateTime? adjustDate)
        { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetPrescriptionDetail(MedicineIds));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", wareHouseID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustDate", adjustDate ?? DateTime.Today);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineBySearchIDs]", parameterList);
        }
        public static void InsertCooperativeMedicineOTC(string medicineID,string medicineName)
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
        public static DataTable GetDataByReserveId(string resId)
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
        public static DataTable SetPrescriptionDetail(List<string>MedicineIds)
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
        public static DataTable GetPrescriptionMedicineSumById(List<int> idList,string warID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetIDTable(idList));
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionMedicineSumById]", parameterList);    
        }
        private static DataTable IDTable() {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("ID", typeof(int));
            return idTable;
        }
        public static DataTable SetIDTable(List<int> IDList) {
            DataTable table = IDTable();
            foreach (int id in IDList)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", id);
                table.Rows.Add(newRow);
            }
            return table;
        }
    }
}
