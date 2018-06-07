using His_Pos.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class ProductTypeOTC : AbstractClass.Product, IProductType
    {

        public string TypeId { get; set; }
    }
}
