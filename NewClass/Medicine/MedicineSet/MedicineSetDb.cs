using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Medicine.MedicineSet
{
    public static class MedicineSetDb
    {
        public static DataTable GetMedicineSets()//取得所有藥品組合
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineSet]");
        }

        public static DataTable GetMedicineSetDetail(int setID)//取得藥品組合內容
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", setID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedSetDetailByID]", parameterList);
        }

        public static DataTable UpdateMedicineSet(MedicineSet set)//更新藥品組合
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", set.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Name", set.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Items", SetMedicineDetail(set.ID, set.MedicineSetItems));
            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateMedicineSetByID]", parameterList);
        }

        public static void DeleteMedicineSet(int setID)//刪除藥品組合
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", setID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteMedicineSetByID]", parameterList);
        }

        #region TableSet

        public static DataTable MedicineSetDetailTable()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("MedSetDet_ID", typeof(int));
            detailTable.Columns.Add("MedSetDet_MedicineID", typeof(String));
            detailTable.Columns.Add("MedSetDet_Dosage", typeof(double));
            detailTable.Columns.Add("MedSetDet_Usage", typeof(String));
            detailTable.Columns.Add("MedSetDet_Position", typeof(String));
            detailTable.Columns.Add("MedSetDet_MedicineDays", typeof(int));
            detailTable.Columns.Add("MedSetDet_TotalAmount", typeof(double));
            detailTable.Columns.Add("MedSetDet_IsPaySelf", typeof(bool));
            detailTable.Columns.Add("MedSetDet_PaySelfPrice", typeof(double));
            return detailTable;
        }

        public static DataTable SetMedicineDetail(int ID, MedicineSetItems medicineSetItems)
        {
            DataTable medicineSetDetailTable = MedicineSetDetailTable();

            foreach (var m in medicineSetItems)
            {
                DataRow newRow = medicineSetDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_ID", ID);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_MedicineID", m.ID);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_Dosage", m.Dosage);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_Usage", m.UsageName);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_Position", m.Position.ID);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_MedicineDays", m.Days);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_TotalAmount", m.Amount);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_IsPaySelf", m.PaySelf);
                DataBaseFunction.AddColumnValue(newRow, "MedSetDet_PaySelfPrice", m.Price);
                medicineSetDetailTable.Rows.Add(newRow);
            }
            return medicineSetDetailTable;
        }

        #endregion TableSet
    }
}