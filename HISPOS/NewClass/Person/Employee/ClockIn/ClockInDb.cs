using System.Data;
using His_Pos.Database;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.NewClass.Person.Employee.ClockIn
{
    public static class ClockInDb
    {

        public static DataTable EmployeeClockInLog(string year, string month, string day, string account, int type)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", year));
            parameterList.Add(new SqlParameter("WMonth", month));
            parameterList.Add(new SqlParameter("WDay", day));
            parameterList.Add(new SqlParameter("EmpNo", account));
            parameterList.Add(new SqlParameter("Type", type));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLog]", parameterList);
            return table;
        }
        public static DataTable ClockInLogByDate(string year, string month, string account)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", year));
            parameterList.Add(new SqlParameter("WMonth", month));
            parameterList.Add(new SqlParameter("EmpId", account));

            int IsDirectSale = 0;
            if (string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false)
                IsDirectSale = 1;
            parameterList.Add(new SqlParameter("IsDirectSale", IsDirectSale));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLogByDate]", parameterList);
            return table;
        }
        public static DataTable ClockInLogFotReport(string year, string month, string storeNo)
        {
            DataTable reDT = new DataTable();
            reDT.Columns.Add("Emp_ID", typeof(int));
            reDT.Columns.Add("CurPha_Name", typeof(string));
            reDT.Columns.Add("Emp_Account", typeof(string));
            reDT.Columns.Add("Emp_Name", typeof(string));
            reDT.Columns.Add("WDate", typeof(string));
            reDT.Columns.Add("WTime", typeof(string));
            reDT.Columns.Add("Type", typeof(string));
            reDT.Columns.Add("WTime2", typeof(string));
            reDT.Columns.Add("WMin", typeof(int));

            //先撈出該月有打卡"WMin"] = rowC的員工
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", year));
            parameterList.Add(new SqlParameter("WMonth", month));
            parameterList.Add(new SqlParameter("storeNo", storeNo));
            var tableEmp = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLogReportEmp]", parameterList);


            //在下迴圈把員工串起來
            foreach (DataRow row in tableEmp.Rows)
            {
                List<SqlParameter> parameterList2 = new List<SqlParameter>();
                parameterList2.Add(new SqlParameter("WYear", year));
                parameterList2.Add(new SqlParameter("WMonth", month));
                parameterList2.Add(new SqlParameter("EmpId", row["Emp_ID"]));
                //parameterList2.Add(new SqlParameter("storeNo", storeNo));

                var tableClock = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLogReport]", parameterList2);
                foreach (DataRow rowClock in tableClock.Rows)
                {
                    DataRow r = reDT.NewRow();
                    r["Emp_ID"] = rowClock["Emp_ID"];
                    r["Emp_Account"] = rowClock["Emp_Account"];
                    r["CurPha_Name"] = rowClock["CurPha_Name"];
                    r["Emp_Name"] = rowClock["Emp_Name"];
                    r["WDate"] = rowClock["WDate"];
                    r["WTime"] = rowClock["WTime"];
                    r["Type"] = rowClock["Type"];
                    r["WTime2"] = rowClock["WTime2"];
                    r["WMin"] = rowClock["WMin"];
                    reDT.Rows.Add(r);
                }
            }

            return reDT;
        }
        public static DataTable StoreByPermit(int Permit, string StoreNo)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Permit",Permit));
            parameterList.Add(new SqlParameter("StoreNo", StoreNo)); 
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[StoreByPermit]", parameterList);
            return table;
        }
    }
    //[Get].[StoreByPermit]
}