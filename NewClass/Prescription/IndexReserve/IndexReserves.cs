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

        public void GetDataByDate(DateTime sDate,DateTime eDate,bool IsShowUnPhoneCall,bool IsShowUnPrepareReserve) {
            Clear();
            DataTable table = IndexReserveDb.GetDataByDate( sDate, eDate);
            foreach (DataRow r in table.Rows) {
                IndexReserve indexReserve = new IndexReserve(r);
                if (indexReserve.PhoneCallStatus == "F") {
                    if (IsShowUnPhoneCall)
                        Add(indexReserve);
                    else
                        continue;
                } 
                if (indexReserve.PrepareStatus == "F") {
                    if (IsShowUnPrepareReserve)
                        Add(indexReserve);
                    else
                        continue;
                }
                Add(indexReserve);
            }
        }
    }
}
