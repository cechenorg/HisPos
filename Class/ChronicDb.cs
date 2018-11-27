﻿using His_Pos.H1_DECLARE.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using static His_Pos.H1_DECLARE.PrescriptionDec2.ChronicSendToServerWindow;

namespace His_Pos.Class
{
    public static class ChronicDb {
       
        internal static void DailyPredictChronic()
        { //檢查過領藥日的3-3 若整輪都沒領 則不預約 若有一次 則預約
            var dd = new DbConnection(Settings.Default.SQL_global);
            dd.ExecuteProc("[HIS_POS_DB].[Index].[DailyPredictChronic]");
        }
        internal static ObservableCollection<Chronic> GetChronicDeclareById(string cusId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            ObservableCollection<Chronic> chronics = new ObservableCollection<Chronic>();
            parameters.Add(new SqlParameter("CUS_ID", cusId));
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetChronicDeclareById]", parameters);
            foreach (DataRow row in dataTable.Rows) {
                chronics.Add(new Chronic(row));
            }
            return chronics;
        }
        internal static bool CheckChronicExistById(string cusId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            ObservableCollection<Chronic> chronics = new ObservableCollection<Chronic>();
            parameters.Add(new SqlParameter("CUS_ID", cusId));
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[CheckChronicExistById]", parameters); 
            if (dataTable.Rows[0][0].ToString() == "0")
                return false;
            else
                return true;
        }
        internal static void UpdateChronicData(string decMasId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            ObservableCollection<Chronic> chronics = new ObservableCollection<Chronic>();
            parameters.Add(new SqlParameter("DecMasId", decMasId));
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[UpdateChronicData]", parameters);
        }

        internal static string GetResidualAmountById(string proId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetResidualAmountById]", parameters);
            return dataTable.Rows[0][0].ToString();
        }
        internal static ObservableCollection<IndexView.IndexView.DailyTakeChronicList> DailyTakeChronic() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[Index].[DailyTakeChronic]");
            ObservableCollection<IndexView.IndexView.DailyTakeChronicList> collection = new ObservableCollection<IndexView.IndexView.DailyTakeChronicList>();
            foreach(DataRow row in dataTable.Rows)
            {
                collection.Add(new IndexView.IndexView.DailyTakeChronicList(row));
            }
            return collection;
        }
        internal static ObservableCollection<IndexView.IndexView.DailtChronicPhoneCall> DailyChronicPhoneCall()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[Index].[DailyChronicPhoneCall]");
            ObservableCollection<IndexView.IndexView.DailtChronicPhoneCall> collection = new ObservableCollection<IndexView.IndexView.DailtChronicPhoneCall>();
            foreach (DataRow row in dataTable.Rows)
            {
                collection.Add(new IndexView.IndexView.DailtChronicPhoneCall(row));
            }
            return collection;
        }
        internal static void UpdateDailyChronic() {
            var dd = new DbConnection(Settings.Default.SQL_global);
             dd.ExecuteProc("[HIS_POS_DB].[Index].[UpdateDailyChronic]");
        }
        internal static void UpdateChronicPhoneCall(string decMasId,string phoneCall) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>(); 
            parameters.Add(new SqlParameter("DecMasId", decMasId));
            parameters.Add(new SqlParameter("PHONECALL", phoneCall));
            dd.ExecuteProc("[HIS_POS_DB].[Index].[UpdateChronicPhoneCall]",parameters);
        }
        internal static void InsertChronicDetail(ObservableCollection<PrescriptionSendData> prescriptionSendDataCollection,string DecMasId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var chronicDetailTable = new DataTable();

            chronicDetailTable.Columns.Add("HISDECMAS_ID", typeof(string));
            chronicDetailTable.Columns.Add("PRO_ID", typeof(string)); 
            chronicDetailTable.Columns.Add("SEND_AMOUNT", typeof(string));
            DataRow row = null;
            foreach (PrescriptionSendData prescriptionSendData in prescriptionSendDataCollection) {
                row = chronicDetailTable.NewRow();
                row["HISDECMAS_ID"] = DecMasId;
                row["PRO_ID"] = prescriptionSendData.MedId;
                row["SEND_AMOUNT"] = prescriptionSendData.SendAmount;
                chronicDetailTable.Rows.Add(row); 
            }

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ChronicDetail", chronicDetailTable)); 
            dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertChronicDetail]", parameters);
        }

        internal static ObservableCollection<ChronicRegisterWindow.ChronicRegister> GetChronicGroupById(string DecMasId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global); 
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", DecMasId));
            DataTable table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetChronicGroupById]", parameters);
            ObservableCollection<ChronicRegisterWindow.ChronicRegister> chronicRegisters = new ObservableCollection<ChronicRegisterWindow.ChronicRegister>();
            foreach (DataRow row in table.Rows) {
                chronicRegisters.Add(new ChronicRegisterWindow.ChronicRegister(row));
            }
            return chronicRegisters;
        }
        internal static void UpdateRegisterStatus(string DecMasId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", DecMasId));
            dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[UpdateRegisterStatus]", parameters);
        }
         
    }
}
 
