using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Interface
{
    public interface IProductReturn
    {
        string Note { get; set; }
        InStock Stock { get; set; }
        double ReturnAmount { get; set; }
        string BatchNumber { get; set; }
        double BatchLimit { get; set; }
        double ReturnPrice { get; set; }
        double ReturnTotalPrice { get; set; }
    }
}
