using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
    public class IndexReserveDetails : ObservableCollection<IndexReserveDetail>
    {
        public IndexReserveDetails()
        {
        }

        public void GetDataById(int Id)
        {
            DataTable table = IndexReserveDetailDb.GetDataByDate(Id);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new IndexReserveDetail(r));
            }
        }

        public void GetSendDataById(int Id)
        {
            DataTable table = IndexReserveDetailDb.GetDataByDate(Id);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                IndexReserveDetail indexReserveDetail = new IndexReserveDetail(r);
                if (this.Count(ind => ind.ID == indexReserveDetail.ID) == 1)
                {
                    var temp = this.Single(ind => ind.ID == indexReserveDetail.ID);
                    indexReserveDetail.Amount += temp.Amount;
                    this.Remove(temp);
                }
                Add(indexReserveDetail);
            }
        }
    }
}