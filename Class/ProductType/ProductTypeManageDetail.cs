using System;
using System.Data;

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

        public ProductTypeManageDetail(string parentId, string name, string engName) : base(parentId, name, engName)
        {
            StockValue = 0;
            Sales = 0;
            ItemCount = 0;
        }

        public double StockValue { get; set; }
        public double Sales { get; set; }
        public int ItemCount { get; set; }
    }
}
