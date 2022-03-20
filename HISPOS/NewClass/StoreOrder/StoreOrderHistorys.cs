using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrderHistorys : ObservableCollection<StoreOrderHistory>
    {
 

        public void getData(string ID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", ID));
            var historiesTable = MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderHistory]", parameters);
            historiesTable.Columns.Add("DoneDateString");
            foreach (DataRow r in historiesTable.Rows)
            {
                if (r["DoneDateTime"]==DBNull.Value)
                {
                    r["DoneDateString"] = "未結案";
                }
                else 
                {
                    r["DoneDateString"] = r["DoneDateTime"].ToString();
                }

                Add(new StoreOrderHistory(r));

            }
        }

        public  StoreOrderHistorys()
        {
           

        }
    }
}