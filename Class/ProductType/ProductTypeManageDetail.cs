using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.ProductType
{
    public class ProductTypeManageDetail : ProductType
    {
        public ProductTypeManageDetail(DataRow dataRow) : base(dataRow)
        {
            StockValue = Double.Parse(dataRow["STOCK"].ToString());
            Sales = Double.Parse(dataRow["TOTAL"].ToString());
            ItemCount = Int32.Parse(dataRow["COUNT"].ToString());
        }

        public double StockValue { get; set; }
        public double Sales { get; set; }
        public int ItemCount { get; set; }
    }
}
