using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    public interface ITrade
    {
        double Cost { get; set; }
        double TotalPrice { get; set; }
        double Amount { get; set; }
        double Price { get; set; }
    }
}
