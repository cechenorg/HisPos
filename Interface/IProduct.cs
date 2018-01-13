using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    public interface IProduct
    {
        string Id { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        double Inventory { get; set; }
    }
}
