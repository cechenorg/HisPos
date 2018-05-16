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
        string Price { get; set; }

        string CountStatus { get; set; }
        string FocusColumn { get; set; }

        void CalculateData(string inputSource);
    }
}
