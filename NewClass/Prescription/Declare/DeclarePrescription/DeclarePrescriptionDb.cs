﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Database;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescriptionDb
    {
        public static DataTable GetDeclarePrescriptionsByMonthRange(DateTime start,DateTime end)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "start", start);
            DataBaseFunction.AddSqlParameter(parameterList, "end", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCount]", parameterList);
        }
    }
}
