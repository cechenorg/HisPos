﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class Divisions
    {
        public List<Division> DivisionsList { get; } = new List<Division>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[DIVISION]", dbConnection);
            foreach (DataRow division in divisionTable.Rows)
            {
                var d = new Division(division["HISDIV_ID"].ToString(), division["HISDIV_NAME"].ToString());
                DivisionsList.Add(d);
            }
        }
    }
}
