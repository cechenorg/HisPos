using System;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.Service
{
    class Localservice
    {
        public string ConnectionString = Properties.Settings.Default.SQL_local;

        public DataTable GetData(string sql) {
            DataTable table = new DataTable();
            try {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, ConnectionString);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
            }
            catch(Exception ex) {Log("localservice", "GetData",ex.Message); }
            return table;
        }//GetData()
        
        public void Log(string system,string functionName,string description)
        {
            string sql = @"Insert into TestDB.dbo.WebLog (COMNAME,SYSTEM,USER_ID,FUNCTION_NAME,SYSTIME,DESCRIPTION) 
                                                   Values(@comname,@system,@userId,@functionName,@systime,@description)";
            try
            {
                SqlConnection myConn = new SqlConnection(ConnectionString);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sql, myConn);
                myCommand.Parameters.AddWithValue("@comname", Environment.MachineName);
                myCommand.Parameters.AddWithValue("@system", system);
                myCommand.Parameters.AddWithValue("@userId", MainWindow.CurrentUser.UserId);
                myCommand.Parameters.AddWithValue("@functionName", functionName);
                myCommand.Parameters.AddWithValue("@systime",Convert.ToDateTime(DateTime.Now));
                myCommand.Parameters.AddWithValue("@description", description);
                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
            catch (Exception ex) {}
        }//Log()

        public void InsertHisData() {
            string line;
            string sql;

            String[] substrings;
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\user\Desktop\data1.txt");
            while ((line = file.ReadLine()) != null)
            {
                substrings = line.Split(',');
                if (substrings[9].Substring(0, 3) != "999") continue;
                if (substrings[6] == "單方") substrings[6] = "S";
                if (substrings[6] == "複方") substrings[6] = "C";

                for (int i = 1; i <= 13; i++)
                {
                    if (substrings[i].IndexOf("'") >= 0)
                    {
                        substrings[i] = substrings[i].Replace("'", "''");
                    }

                }
                sql = @"Insert into His_PosHIS_Test.dbo.medicine 
                        (MED_CODE, MED_ENG, MED_CHI, MED_QTY, MED_UNIT, MED_SC, MED_PRICE, MED_SDATE, MED_EDATE, MED_MANUFACTURER, MED_FORM, MED_INGREDIENT,MED_ATC) 
                        Values('" + substrings[1] + "','" +
                        substrings[2] + "','" +
                        substrings[3] + "'," +
                        substrings[4] + ",'" +
                        substrings[5] + "','" +
                        substrings[6] + "'," +
                        substrings[7] + "," +
                        "CONVERT(date,'" + Convert.ToString(19110000 + Convert.ToInt32(substrings[8])) + "')," +
                        "CONVERT(date,'" + Convert.ToString(19110000 + Convert.ToInt32(substrings[9])) + "'),'" +
                        substrings[10] + "','" +
                        substrings[11] + "','" +
                        substrings[12] + "','" +
                        substrings[13] + "')";
                try
                {
                    SqlConnection myConn = new SqlConnection("server=192.168.1.77,59086;database=His_PosHIS_Test;uid=singde;pwd=citymulti1234");
                    myConn.Open();
                    SqlCommand myCommand = new SqlCommand(sql, myConn);

                    myCommand.ExecuteNonQuery();
                    myConn.Close();
                }
                catch (Exception ex) { }
            }
            file.Close();
        }

        public void InsertProduct() {
            DataTable table = GetData(@"SELECT ProductNo,PublicBarcode,'' AS MEDICINECODE,0 AS QNAME,ProductName,'' AS ENGNMAE,CAST(MarketPrice AS smallmoney),CAST(MemberPrice AS smallmoney),CAST(EmployeePrice as smallmoney),'' AS POINT,0 As Profit,ControlProduct,0 As MAN_ID,TypeNo
                                        FROM[dbo].[ProductMaster]
                                        WHERE ProductNo > 20000001; ");
            string sql = string.Empty;
            foreach (DataRow row in table.Rows) {
                sql = "Insert into [HIS_POS_DB].[PRO].[MASTER] values(";
                sql += "'" + row["PublicBarcode"] + "',";
                sql += "'" + row["MEDICINECODE"] + "',";
                sql += "'" + row["QNAME"] + "',";
                sql += "'" + row["ProductName"] + "',";
                sql += "'" + row["ENGNMAE"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
                sql += "'" + row["ProductNo"] + "',";
            }
        }
    }
}
