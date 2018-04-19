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
        string Note { get; set; }
        InStock Stock { get; set; }
        double LastPrice { get; set; }
        int FreeAmount { get; set; }
        string Invoice { get; set; }
        string ValidDate { get; set; }
        string BatchNumber { get; set; }
    }
}
