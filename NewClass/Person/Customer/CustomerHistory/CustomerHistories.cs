using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.CustomerHistory;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistories:Collection<CustomerHistory>
    {
        public CustomerHistories()
        {

        }

        public CustomerHistories(int id)
        {
            var table = CustomerHistoryDb.GetData(id);
            for (var i = 0; i < 5; i++)
            {
                var cusHis = new CustomerHistory
                {
                    AdjustDate = DateTime.Today,
                    Status = true
                };
                if (ViewModelMainWindow.Institutions[i].Name.Length > 8)
                {
                    cusHis.Title = ViewModelMainWindow.Institutions[i].Name.Substring(0, 8) + "... " +
                                   ViewModelMainWindow.Divisions[i].Name;
                }
                else
                {
                    cusHis.Title = ViewModelMainWindow.Institutions[i].Name.Substring(0, 8) + " " +
                                   ViewModelMainWindow.Divisions[i].Name;
                }
                Add(cusHis);
            }
            //foreach (DataRow r in table.Rows)
            //{
                
            //}
        }
    }
}
