using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryDetail
    {
        //public CustomerHistoryDetail()
        //{
        //}

        //public CustomerHistoryDetail(DataRow dataRow)
        //{
        //    SystemType = dataRow["SYSTEMTYPE"].ToString();
        //    HistoryDetailId = dataRow["CUSHISTORYDETAILID"].ToString();
        //}

        public string SystemType { get; set; }
        public string HistoryDetailId { get; set; }
    }
}