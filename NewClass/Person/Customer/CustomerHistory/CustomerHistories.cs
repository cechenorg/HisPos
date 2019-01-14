using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.Class.CustomerHistory;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistories:Collection<CustomerHistoryBase>
    {
        public CustomerHistories()
        {

        }

        public CustomerHistories(int id)
        {
            var table = CustomerHistoryDb.GetData(id);
            foreach (DataRow r in table.Rows)
            {
                switch ((HistoryType)int.Parse(r[""].ToString()))//switch by type 
                {
                    case HistoryType.Prescription:
                        Add(new PrescriptionHistory(r));
                        break;
                    case HistoryType.Prescribe:
                        Add(new PrescribeHistory());
                        break;
                }
            }
        }
    }
}
