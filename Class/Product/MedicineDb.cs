﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using His_Pos.Properties;
using His_Pos.Service;
using Microsoft.VisualBasic;

namespace His_Pos.Class.Product
{
    public class MedicineDb 
    {
        internal static DataTable GetInventoryMedicines()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            return dd.ExecuteProc("[HIS_POS_DB].[InventoryManagementView].[GetInventoryMedicine]");
        }

        internal static ObservableCollection<AbstractClass.Product> GetDeclareMedicine()
        {
            ObservableCollection<AbstractClass.Product> collection = new ObservableCollection<AbstractClass.Product>();

           var dd = new DbConnection(Settings.Default.SQL_local);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetMedicinesData]");

            foreach (DataRow row in table.Rows)
            {
                if(string.IsNullOrEmpty(row["HISMED_FORM"].ToString()))
                    collection.Add(new PrescriptionOTC(row));
                else
                    collection.Add(new DeclareMedicine(row,"Init"));
            }

            return collection;
        }

        internal static ObservableCollection<AbstractClass.Product> GetDeclareMedicineByMasId(string decmasId)
        {
            ObservableCollection<AbstractClass.Product> collection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAS_ID", decmasId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetDeclareMedicineByMasId]", parameters);

            foreach (DataRow row in table.Rows)
            {
                if (string.IsNullOrEmpty(row["HISMED_FORM"].ToString()))
                    collection.Add(new PrescriptionOTC(row));
                else
                    collection.Add(new DeclareMedicine(row, "Get"));
            }

            return collection;
        }

        internal static ObservableCollection<DeclareMedicine> GetDeclareFileMedicineData()
        {
            ObservableCollection<DeclareMedicine> collection = new ObservableCollection<DeclareMedicine>();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[DeclareFileExportView].[GetDeclareFileMedicineData]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new DeclareMedicine(row, "DeclareFile"));
            }
            return collection;
        }

        //更新藥品檔
        //讀檔xml
        //XmlSerializer serializer = new XmlSerializer(typeof(Class.Product.Table));
        //    using (FileStream fileStream = new FileStream(@"C:\Users\frank\Desktop\MedicineFileConverter\d30a75f36f3f9e27419c7afed56d18e0_export.xml", FileMode.Open))
        //{
        //    Class.Product.Table result = (Class.Product.Table)serializer.Deserialize(fileStream);
        //    MedicineDb.UpdateDeclareMedicines(result);
        //}
        internal static void UpdateDeclareMedicines(Table t)
        {
            var medicinesTable = new DataTable();
            medicinesTable.Columns.Add("HISMEDTEM_ID", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_ENG", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_CHI", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_QTY", typeof(float));
            medicinesTable.Columns.Add("HISMEDTEM_UNIT", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_SC", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_PRICE", typeof(double));
            medicinesTable.Columns.Add("HISMEDTEM_MANUFACTURER", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_FORM", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_INGREDIENT", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_ATC", typeof(string));
            medicinesTable.Columns.Add("HISMEDTEM_SDATE", typeof(DateTime));
            medicinesTable.Columns.Add("HISMEDTEM_EDATE", typeof(DateTime));
            foreach (var row in t.Row)
            {
                var r = medicinesTable.NewRow();
                r["HISMEDTEM_ID"] = row.Col2.Text;
                r["HISMEDTEM_ENG"] = Strings.StrConv(row.Col3.Text, VbStrConv.Narrow, 0);
                r["HISMEDTEM_CHI"] = Strings.StrConv(row.Col4.Text, VbStrConv.Narrow, 0);
                r["HISMEDTEM_QTY"] = float.Parse(row.Col5.Text);
                r["HISMEDTEM_UNIT"] = Strings.StrConv(row.Col6.Text, VbStrConv.Narrow, 0);
                r["HISMEDTEM_SC"] = row.Col7.Text.Substring(0, 1);
                r["HISMEDTEM_PRICE"] = TryParseDouble(row.Col8.Text);
                r["HISMEDTEM_MANUFACTURER"] = CheckDataNull(row.Col11.Text);
                r["HISMEDTEM_FORM"] = CheckDataNull(row.Col12.Text);
                r["HISMEDTEM_INGREDIENT"] = CheckDataNull(row.Col13.Text);
                r["HISMEDTEM_ATC"] = CheckDataNull(row.Col14.Text);
                r["HISMEDTEM_SDATE"] = ParseMedicineSdateEdateStr(row.Col9.Text);
                r["HISMEDTEM_EDATE"] = ParseMedicineSdateEdateStr(row.Col10.Text);
                medicinesTable.Rows.Add(r);
            }
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDICINE",medicinesTable)
            };
            dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[UpdateDeclareMedicine]", parameters);
        }
        //StringBuilder output = new StringBuilder();
        //    foreach (DataColumn col in table.Columns)
        //{
        //    output.AppendFormat("{0} ", r[col]);
        //}
        //output.AppendLine();
        //Console.WriteLine(output);
        private static DateTime ParseMedicineSdateEdateStr(string dateStr)
        {
            var myCultureInfo = new CultureInfo("ZH-TW");
            var calendar = new TaiwanCalendar();
            myCultureInfo.DateTimeFormat.Calendar = calendar;
            var year = int.Parse(dateStr.Substring(0, 3));
            var m = int.Parse(dateStr.Substring(3, 2));
            var d = int.Parse(dateStr.Substring(5, 2));
            return new DateTime(year, m, d, calendar).AddYears(-1911);
        }

        private static double TryParseDouble(string str)
        {
            double number;
            bool conversionSuccessful = double.TryParse(str, out number);
            if (conversionSuccessful)
            {
                return number;
            }
            else
            {
                return 0.00;
            }
        }

        private static string CheckDataNull(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "無";
            return Strings.StrConv(str, VbStrConv.Narrow, 0);
        }

        internal static void UpdateInventoryMedicineData(InventoryMedicine inventoryMedicine)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", inventoryMedicine.Id));
            parameters.Add(new SqlParameter("CHI_NAME", inventoryMedicine.ChiName));
            parameters.Add(new SqlParameter("ENG_NAME", inventoryMedicine.EngName));
            parameters.Add(new SqlParameter("STATUS", (inventoryMedicine.Status)? "1":"0"));
            parameters.Add(new SqlParameter("LOCATION", inventoryMedicine.Location));
            parameters.Add(new SqlParameter("BAR_CODE", inventoryMedicine.BarCode));
            parameters.Add(new SqlParameter("SAFEAMOUNT", Int32.Parse(inventoryMedicine.Stock.SafeAmount)));
            parameters.Add(new SqlParameter("BASICAMOUNT", Int32.Parse(inventoryMedicine.Stock.BasicAmount)));
            parameters.Add(new SqlParameter("COMMON", inventoryMedicine.Common));
            parameters.Add(new SqlParameter("SIDEEFFECT", inventoryMedicine.SideEffect));
            parameters.Add(new SqlParameter("INDICATION", inventoryMedicine.Indication));
            parameters.Add(new SqlParameter("WARNING", inventoryMedicine.Warnings));
            parameters.Add(new SqlParameter("MINORDER", inventoryMedicine.Stock.MinOrderAmount));
            parameters.Add(new SqlParameter("DESCRIPTION", inventoryMedicine.Note));

            dd.ExecuteProc("[HIS_POS_DB].[MedicineDetail].[UpdateInventoryMedicineDetail]", parameters);
        }

        internal static MedicineInformation GetMedicalInfoById(string id)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MedId", id));
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetMedInfoByMedId]", parameters);
            return new MedicineInformation(table.Rows[0]);
        }
    }
}
