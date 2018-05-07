using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Interface
{
    interface IProductTaking
    {
        string Category { get; set; }
        double Inventory { get; set; }
        string ValidDate { get; set; }
        string LastCheckDate { get; set; }
        string Location { get; set; }
    }
}
