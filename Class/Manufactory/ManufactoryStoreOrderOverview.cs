using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
    public class ManufactoryStoreOrderOverview
    {
        public ManufactoryStoreOrderOverview(DataRow row)
        {
            OrderId = row["STOORD_ID"].ToString();
            OrdEmp = row["ORDEMP"].ToString();
            RecEmp = row["RECEMP"].ToString();
            OrdDate = row["STOORD_DATE"].ToString();
            RecDate = row["STOORD_RECDATE"].ToString();
            Total = row["TOTAL"].ToString();
        }

        public string OrderId { get; set; }
        public string OrdEmp { get; set; }
        public string RecEmp { get; set; }
        public string OrdDate { get; set; }
        public string RecDate { get; set; }
        public string Total { get; set; }
    }
}
