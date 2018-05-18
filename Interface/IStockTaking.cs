using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Interface
{
    interface IStockTaking
    {
        string Category { get; set; }
        double Inventory { get; set; }
        double SafeAmount { get; set; }
        string ValidDate { get; set; }
        string LastCheckDate { get; set; }
        string Location { get; set; }
        string BatchNumber { get; set; }
        bool Status { get; set; }
        string TakingResult { get; set; }
        bool IsChecked { get; set; }
        bool IsEqual { get; set; }
    }
}
