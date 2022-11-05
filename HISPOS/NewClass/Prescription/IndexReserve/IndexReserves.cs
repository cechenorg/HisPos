using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
    public class IndexReserves : ObservableCollection<IndexReserve>
    {
        public IndexReserves()
        {
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            Clear();
            DataTable table = IndexReserveDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                IndexReserve indexReserve = new IndexReserve(r);
                Add(indexReserve);
            }
        }

        
    }
}