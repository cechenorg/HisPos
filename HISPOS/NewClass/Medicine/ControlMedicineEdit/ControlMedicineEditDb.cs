using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Medicine.ControlMedicineEdit
{
    public class ControlMedicineEditDb
    {
        internal static DataTable GetData(string medID, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "medID", medID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineEditByMedID]", parameterList);
        }

        internal static void Update(string medID, string warID, ControlMedicineEdits controlMedicineEdits)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", medID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "Mediciines", SetControlMedicineEdit(controlMedicineEdits));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateControlMedicineEdit]", parameterList);
        }

        #region TableSet

        public static DataTable SetControlMedicineEdit(ControlMedicineEdits controlMedicineEdits)
        {
            DataTable controlMedicineEditTable = ControlMedicineEditTable();

            foreach (var c in controlMedicineEdits)
            {
                if (c.IsNew) continue;
                DataRow newRow = controlMedicineEditTable.NewRow();

                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_MedicineID", c.MedicineID);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_WareHouseID", c.WarID);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_Date", c.Date);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_Type", c.Type);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_ManufactoryID", c.Manufactory is null ? null : c.Manufactory.ID);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_Amount", c.Amount);
                DataBaseFunction.AddColumnValue(newRow, "ConMedEdit_BatchNumber", c.BatchNumber);

                controlMedicineEditTable.Rows.Add(newRow);
            }
            return controlMedicineEditTable;
        }

        public static DataTable ControlMedicineEditTable()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("ConMedEdit_MedicineID", typeof(string));
            detailTable.Columns.Add("ConMedEdit_WareHouseID", typeof(int));
            detailTable.Columns.Add("ConMedEdit_Date", typeof(DateTime));
            detailTable.Columns.Add("ConMedEdit_Type", typeof(string));
            detailTable.Columns.Add("ConMedEdit_ManufactoryID", typeof(int));
            detailTable.Columns.Add("ConMedEdit_Amount", typeof(int));
            detailTable.Columns.Add("ConMedEdit_BatchNumber", typeof(string));

            return detailTable;
        }

        #endregion TableSet
    }
}