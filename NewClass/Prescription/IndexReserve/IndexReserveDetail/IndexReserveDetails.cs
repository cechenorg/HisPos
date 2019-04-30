using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
    public class IndexReserveDetails : ObservableCollection<IndexReserveDetail>
    {
        public IndexReserveDetails() { }
        public void GetDataById(int Id) {
            DataTable table = IndexReserveDetailDb.GetDataByDate(Id);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new IndexReserveDetail(r));
            }
        }
    }
}
