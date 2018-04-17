using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Interface
{
    public interface IProductPurchase
    {
        InStock Stock { get; set; }
        double LastPrice { get; set; }
    }
}
