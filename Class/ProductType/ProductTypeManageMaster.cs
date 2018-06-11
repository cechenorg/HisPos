using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.ProductType
{
    public class ProductTypeManageMaster : ProductType
    {
        public ProductTypeManageMaster(DataRow dataRow) : base(dataRow)
        {
            StockValue = Double.Parse(dataRow["STOCK"].ToString());
            Sales = Double.Parse(dataRow["TOTAL"].ToString());
            TypeCount = Int32.Parse(dataRow["COUNT"].ToString());
        }

        public double StockValue { get; set; }
        public double Sales { get; set; }
        public int TypeCount { get; set; }
    }
}
