using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.OnTheWayDetail
{
    public struct OnTheWayDetailStruct
    {
        #region ----- Define Variables -----
        public string Status { get; set; }
        public string ID { get; set; }
        public string CusName { get; set; }
        public DateTime CreateDate { get; set; }
        public double Amount { get; set; }

        public string AmountHeader { get; set; }
        #endregion

        public OnTheWayDetailStruct(DataRow row)
        {
            Status = row.Field<string>("STATUS");
            ID = row.Field<string>("STOORD_ID");
            CusName = row.Field<string>("StoOrd_CustomerName");
            CreateDate = row.Field<DateTime>("StoOrd_CreateTime");
            Amount = row.Field<double>("StoOrdDet_OrderAmount");

            AmountHeader = "數量:";
        }
    }
}
