using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    interface IStockTakingRecord
    {
        string EmpId { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
    }
}
