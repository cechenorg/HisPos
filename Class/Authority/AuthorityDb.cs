﻿using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Properties;
using System.Collections.ObjectModel;
using static His_Pos.H4_BASIC_MANAGE.AuthenticationManage.AuthenticationManageView;
using System.Data;
using His_Pos.Class.Leave;

namespace His_Pos.Class.Authority
{
    public static class AuthorityDb
    {
        internal static void ChangeAuthLeaveStatus(bool status)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STATUS", status));

            dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[ChangeAuthLeaveStatus]", parameters);
        }

        internal static Collection<AuthStatus> GetAuthStatus()
        {
            Collection<AuthStatus> collection = new Collection<AuthStatus>();

            var dd = new DbConnection(Settings.Default.SQL_local);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[GetAuthStatus]");

            foreach(DataRow row in table.Rows)
            {
                collection.Add( new AuthStatus(row));
            }

            return collection;
        }

        internal static Collection<AuthLeaveRecord> GetLeaveRecord()
        {
            Collection<AuthLeaveRecord> collection = new Collection<AuthLeaveRecord>();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[GetAuthLeaveRecord]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new AuthLeaveRecord(row));
            }

            return collection;
        }

        internal static void AuthLeaveConfirm(List<AuthLeaveRecord> confirmList)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            DataTable details = new DataTable();
            details.Columns.Add("EMP_ID", typeof(string));
            details.Columns.Add("INSERTDATE", typeof(DateTime));
            foreach (var record in confirmList)
            {
                var newRow = details.NewRow();
                newRow["EMP_ID"] = record.Id;
                newRow["INSERTDATE"] = record.InsertTime;
                details.Rows.Add(newRow);
            }
            parameters.Add(new SqlParameter("LEAVE", details));
            dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[ConfirmLeaveRecord]", parameters);
        }
        internal static Collection<string> GetTabAuthByGroupId(string groupId) {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>(); 
            parameters.Add(new SqlParameter("GroupId", groupId));
           DataTable table = dd.ExecuteProc("[HIS_POS_DB].[LoginView].[GetTabAuthByGroupId]", parameters);
            Collection<string> collection = new Collection<string>();
            foreach (DataRow row in table.Rows) {
                collection.Add(row["AUTH_NAME"].ToString());
            }
            return collection;
        } 
    }
}
