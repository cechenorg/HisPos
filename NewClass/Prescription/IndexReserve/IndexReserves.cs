using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
   public class IndexReserves : ObservableCollection<IndexReserve>
    {
        public IndexReserves() { }

        public void GetDataByDate(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = IndexReserveDb.GetDataByDate( sDate, eDate);
            foreach (DataRow r in table.Rows) {
                Add(new IndexReserve(r));
            }
        }
    }
}
