using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.CashFlow {
    public class CashFlows : ObservableCollection<CashFlow> {
        public CashFlows() { }

        public void GetCashFlowByDate(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = CashFlowDb.GetCashFlowByDate(sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new CashFlow(r));
            } 
        }
    }
}
