using His_Pos.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class ProductTypeOTC : AbstractClass.Product, IProductType
    {
        public ProductTypeOTC(DataRow dataRow) : base(dataRow)
        {
            TypeId = dataRow["PROTYP_ID"].ToString();
        }

        public string TypeId { get; set; }
    }
}
