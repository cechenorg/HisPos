﻿using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public static class ChronicDb {
        internal static void CaculateChironic() { //假設病人1-3沒領  要幫他算出2-1~2-3
            var dd = new DbConnection(Settings.Default.SQL_global);
            dd.ExecuteProc("[HIS_POS_DB].[Index].[CaculateChironic]");
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
         
    }
}
