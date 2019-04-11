using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct HistoryPrice
    {
        public HistoryPrice(DataRow dataRow)
        {
            StartDate = dataRow.Field<string>("Med_StartDate");
            EndDate = dataRow.Field<string>("Med_EndDate");
            Price = dataRow.Field<double>("Med_Price");
        }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double Price { get; set; }
    }
}
